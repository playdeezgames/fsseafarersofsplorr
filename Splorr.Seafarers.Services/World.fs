namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type WorldGenerationConfiguration =
    {
        WorldSize: Location
        MinimumIslandDistance: float
        MaximumGenerationTries: uint32
        RewardRange: float * float
        Commodities: Map<Commodity, CommodityDescriptor>
        Items: Map<Item, ItemDescriptor>
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
            RewardRange = configuration.RewardRange
            Commodities = configuration.Commodities
            Items = configuration.Items
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

    let (|AVATAR_ALIVE|AVATAR_DEAD|) (world:World) =
        match world.Avatar with
        | Avatar.ALIVE -> AVATAR_ALIVE
        | _ -> AVATAR_DEAD

    let rec Move (distance:uint32) (world:World) :World =
        match distance with
        | 0u -> world
        | x ->
            let steppedWorld = 
                {
                    world with 
                        Avatar = world.Avatar |> Avatar.Move
                        Turn = world.Turn + 1u
                }
                |> AddMessages [ "Steady as she goes." ]
            match steppedWorld with
            | AVATAR_DEAD ->
                steppedWorld
                |> AddMessages [ "You starve to death!" ]
            | _ ->
                Move (x-1u) steppedWorld

    let GetNearbyLocations (from:Location) (maximumDistance:float) (world:World) : Location list =
        world.Islands
        |> Map.toList
        |> List.map fst
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)

    let TransformAvatar (transform:Avatar -> Avatar) (world:World) : World =
        {world with Avatar = world.Avatar |> transform}


    let private DoJobCompletion (location:Location) (job:Job) (world:World) : World = 
        if location = job.Destination then
            world
            |> AddMessages [ "You complete your job." ]
            |> TransformAvatar Avatar.CompleteJob
        else
            world

    let Dock (random:System.Random) (location: Location) (world:World) : World =
        match world.Islands |> Map.tryFind location with
        | Some island ->
            let destinations =
                world.Islands
                |> Map.toList
                |> List.map fst
                |> Set.ofList
                |> Set.remove location
            world
            |> TransformIsland location 
                (Island.AddVisit world.Turn 
                >> Island.GenerateJobs random world.RewardRange destinations 
                >> Island.GenerateCommodities random world.Commodities
                >> Island.GenerateItems random world.Items
                >> Some)
            |> Option.foldBack (DoJobCompletion location) world.Avatar.Job
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

    let AcceptJob (jobIndex:uint32) (location:Location) (world:World) : World =
        match jobIndex, world.Islands |> Map.tryFind location, world.Avatar.Job with
        | 0u, _, _ ->
            world
            |> AddMessages [ "That job is currently unavailable." ]
        | _, Some island, None ->
            match island |> Island.RemoveJob jobIndex with
            | isle, Some job ->
                world
                |> SetIsland location (isle |> Some)
                |> TransformIsland job.Destination (Island.MakeKnown >> Some)
                |> TransformAvatar (Avatar.SetJob job)
                |> AddMessages [ "You accepted the job!" ]
            | _ ->
                world
                |> AddMessages [ "That job is currently unavailable." ]
        | _, Some island, Some job ->
            world
            |> AddMessages [ "You must complete or abandon your current job before taking on a new one." ]
        | _ -> 
            world

    let AbandonJob (world:World) : World =
        match world.Avatar.Job with
        | Some _ ->
            world
            |> AddMessages [ "You abandon your job." ]
            |> TransformAvatar (Avatar.AbandonJob)
        | _ ->
            world
            |> AddMessages [ "You have no job to abandon." ]

    let private FindItemByName (itemName:string) (world:World) : (Item * ItemDescriptor) option =
        world.Items
        |> Map.tryPick (fun k v -> if v.DisplayName = itemName then Some (k,v) else None)

    let BuyItems (location:Location) (quantity:uint32) (itemName:string) (world:World) : World =
        match world |> FindItemByName itemName, world.Islands |> Map.tryFind location with
        | Some (item, descriptor) , Some island->
            let unitPrice = 
                Item.DetermineSalePrice world.Commodities island.Markets descriptor 
            let price = (quantity |> float) * unitPrice
            if price > world.Avatar.Money then
                world
                |> AddMessages ["You don't have enough money to buy those."]
            else
                world
                |> AddMessages ["You complete the purchase."]
                |> TransformAvatar (Avatar.SpendMoney price)
                |> TransformAvatar (Avatar.AddInventory item quantity)
                |> TransformIsland location (Island.UpdateMarketForItemSale world.Commodities descriptor quantity >> Some)
        | None, Some island ->
            world
            |> AddMessages ["Round these parts, we don't sell things like that."]
        | _ ->
            world
            |> AddMessages ["You cannot buy items here."]

    let SellItems (location:Location) (quantity:uint32) (itemName:string) (world:World) : World =
        match world |> FindItemByName itemName, world.Islands |> Map.tryFind location with
        | Some (item, descriptor), Some island ->
            if quantity > (world.Avatar |> Avatar.GetItemCount item) then
                world
                |> AddMessages ["You don't have enough of those to sell."]
            else
                let unitPrice = 
                    Item.DeterminePurchasePrice world.Commodities island.Markets descriptor 
                let price = (quantity |> float) * unitPrice
                world
                |> AddMessages ["You complete the sale."]
                |> TransformAvatar (Avatar.EarnMoney price)
                |> TransformAvatar (Avatar.RemoveInventory item quantity)
                |> TransformIsland location (Island.UpdateMarketForItemPurchase world.Commodities descriptor quantity >> Some)
        | None, Some island ->
            world
            |> AddMessages ["Round these parts, we don't buy things like that."]
        | _ ->
            world
            |> AddMessages ["You cannot sell items here."]
