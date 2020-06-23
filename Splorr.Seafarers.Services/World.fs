namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type WorldGenerationConfiguration =
    {
        WorldSize: Location
        MinimumIslandDistance: float
        MaximumGenerationTries: uint32
    }

module World =
    let private GenerateIslandName (random:System.Random) : string =
        let consonants = [| "h"; "k"; "l"; "m"; "p" |]
        let vowels = [| "a"; "e"; "i"; "o"; "u" |]
        let vowel = random.Next(2)>0
        let nameLength = random.Next(3) + random.Next(3) + random.Next(3) + 3
        [1..(nameLength)]
        |> List.map 
            (fun i -> i % 2 = (if vowel then 1 else 0))
        |> List.map
            (fun v -> 
                if v then
                    vowels.[random.Next(vowels.Length)]
                else
                    consonants.[random.Next(consonants.Length)])
        |> List.reduce (+)

    let rec private GenerateIslandNames (random:System.Random) (nameCount:int) (names: Set<string>) : List<string> =
        if names.Count>=nameCount then
            names
            |> Set.toList
        else
            names
            |> Set.add (GenerateIslandName random)
            |> GenerateIslandNames random nameCount

    let SetIsland (location:Location) (island:Island option) (world:World) : World =
        match island with 
        | Some i ->
            {world with Islands = world.Islands |> Map.add location i}
        | None ->
            {world with Islands = world.Islands |> Map.remove location}
            

    let TransformIsland (location: Location) (transform: Island->Island option) (world:World) : World =
        (world.Islands
        |> Map.tryFind location
        |> Option.bind transform
        |> SetIsland location) world

    let private NameIslands (random:System.Random) (world:World) : World =
        GenerateIslandNames random (world.Islands.Count) (Set.empty)
        |> List.sortBy (fun _ -> random.Next())
        |> List.zip (world.Islands |> Map.toList |> List.map fst)
        |> List.fold
            (fun w (l,n) -> w |> TransformIsland l (Island.SetName n >> Some)) world

    let rec private GenerateIslands (configuration:WorldGenerationConfiguration) (random:System.Random) (currentTry:uint32) (world: World) : World =
        if currentTry>=configuration.MaximumGenerationTries then
            world
        else
            let candidateLocation = (random.NextDouble() * (configuration.WorldSize |> fst), random.NextDouble() * (configuration.WorldSize |> snd))
            if world.Islands |> Map.exists(fun k _ ->(Location.DistanceTo candidateLocation k) < configuration.MinimumIslandDistance) then
                GenerateIslands configuration random (currentTry+1u) world
            else
                GenerateIslands configuration random 0u {world with Islands = world.Islands |> Map.add candidateLocation (Island.Create())}

    let Create (configuration:WorldGenerationConfiguration) (random:System.Random) : World =
        {
            Turn = 0u
            Messages = []
            Avatar = Avatar.Create(configuration.WorldSize |> Location.ScaleBy 0.5)
            Islands = Map.empty
        }
        |> GenerateIslands configuration random 0u
        |> NameIslands random

    let ClearMessages(world:World) : World =
        {world with Messages=[]}

    let AddMessages(messages: string list) (world:World) : World =
        {world with Messages = List.append world.Messages messages}

    let SetSpeed (speed:float) (world:World) : World = 
        let updatedAvatar =
            world.Avatar
            |> Avatar.SetSpeed speed
        let message = updatedAvatar.Speed |> sprintf "You set your speed to %f."
        {world with Avatar = updatedAvatar}
        |> AddMessages [ message ]

    let SetHeading (heading:Dms) (world:World) : World =
        {world with Avatar = world.Avatar |> Avatar.SetHeading heading}
        |> AddMessages [ heading |> Dms.ToString |> sprintf "You set your heading to %s." ]

    let Move(world:World) :World =
        {
            world with 
                Avatar = world.Avatar |> Avatar.Move
                Turn = world.Turn + 1u
        }
        |> AddMessages [ "Steady as she goes." ]

    let GetNearbyLocations (from:Location) (maximumDistance:float) (world:World) : Location list =
        world.Islands
        |> Map.toList
        |> List.map fst
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)

    let Dock (location: Location) (world:World) : World =
        match world.Islands |> Map.tryFind location with
        | Some island ->
            world
            |> TransformIsland location (Island.AddVisit world.Turn >> Some)
            |> AddMessages [ "You dock." ]
        | _ -> 
            world
            |> AddMessages [ "There is no place to dock there." ]

    let HeadFor (islandName: string) (world:World) : World =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && v.VisitCount.IsSome then
                        Some k
                    else
                        None)
        match location with
        | Some l ->
            world
            |> SetHeading (Location.HeadingTo world.Avatar.Position l |> Dms.ToDms)
            |> AddMessages [ islandName |> sprintf "You head for `%s`." ]
        | _ ->
            world
            |> AddMessages [ islandName |> sprintf "I don't know how to get to `%s`." ]
