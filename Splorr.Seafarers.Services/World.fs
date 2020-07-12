namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type WorldGenerationConfiguration =
    {
        WorldSize: Location
        MinimumIslandDistance: float
        MaximumGenerationTries: uint32
        RewardRange: float * float
        Commodities: Map<uint, CommodityDescriptor>
        Items: Map<uint, ItemDescriptor>
    }

type TradeQuantity =
    | Maximum
    | Specific of uint32

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
            Avatars = ["",Avatar.Create(configuration.WorldSize |> Location.ScaleBy 0.5)] |> Map.ofList
            Islands = Map.empty
            RewardRange = configuration.RewardRange
            Commodities = configuration.Commodities
            Items = configuration.Items
        }
        |> GenerateIslands configuration random 0u
        |> NameIslands random

    let TransformAvatar (avatarId:string) (transform:Avatar -> Avatar option) (world:World) : World =
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.bind transform
        |> Option.fold
            (fun w avatar -> 
                {w with Avatars = w.Avatars|> Map.add avatarId avatar}) {world with Avatars = world.Avatars |> Map.remove avatarId}

    let ClearMessages(avatarId:string) =
        TransformAvatar avatarId (Avatar.ClearMessages >> Some)

    let AddMessages(avatarId:string) (messages: string list) =
        TransformAvatar avatarId (Avatar.AddMessages messages >> Some)

    let SetSpeed (speed:float) (avatarId:string) (world:World) : World = 
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.bind (Avatar.SetSpeed speed >> Some)
        |> Option.fold 
            (fun w a ->
                {w with Avatars = w.Avatars |> Map.add avatarId a}
                |> AddMessages avatarId [a.Speed |> sprintf "You set your speed to %f."]) world

    let SetHeading (heading:Dms) (avatarId:string) (world:World) : World =
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.bind (Avatar.SetHeading heading >> Some)
        |> Option.fold 
            (fun w a ->
                {w with Avatars = w.Avatars |> Map.add avatarId a}
                |> AddMessages avatarId [a.Heading |> Dms.ToDms |> Dms.ToString |> sprintf "You set your heading to %s." ]) world

    let IsAvatarAlive (avatarId:string) (world:World) =
        match world.Avatars |> Map.tryFind avatarId with
        | Some avatar -> 
            match avatar with
            | Avatar.ALIVE -> true
            | Avatar.DEAD -> false
        | _ -> false
        

    let rec Move (distance:uint32) (avatarId:string) (world:World) :World =
        match distance, world.Avatars |> Map.tryFind avatarId with
        | x, Some _ when x > 0u ->
            let steppedWorld = 
                world
                |> TransformAvatar avatarId (Avatar.Move >> Some)
                |> AddMessages avatarId [ "Steady as she goes." ]
            if IsAvatarAlive avatarId steppedWorld |> not then
                steppedWorld
                |> AddMessages avatarId [ "You starve to death!" ]
            else
                Move (x-1u) avatarId steppedWorld
        | _ -> world

    let GetNearbyLocations (from:Location) (maximumDistance:float) (world:World) : Location list =
        world.Islands
        |> Map.toList
        |> List.map fst
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)


    let private DoJobCompletion (location:Location) (avatarId:string) (job:Job) (world:World) : World = 
        if location = job.Destination then
            world
            |> AddMessages avatarId [ "You complete your job." ]
            |> TransformAvatar avatarId (Avatar.CompleteJob >> Some)
        else
            world

    let Dock (random:System.Random) (location: Location) (avatarId:string) (world:World) : World =
        match world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some _, Some avatar ->
            let destinations =
                world.Islands
                |> Map.toList
                |> List.map fst
                |> Set.ofList
                |> Set.remove location
            world
            |> TransformIsland location 
                (Island.AddVisit world.Avatars.[avatarId].Turn.CurrentValue avatarId
                >> Island.GenerateJobs random world.RewardRange destinations 
                >> Island.GenerateCommodities random world.Commodities
                >> Island.GenerateItems random world.Items
                >> Some)
            |> Option.foldBack (DoJobCompletion location avatarId) avatar.Job
            |> AddMessages avatarId [ "You dock." ]
        | _, Some _ -> 
            world
            |> AddMessages avatarId [ "There is no place to dock there." ]
        | _ ->
            world

    let HeadFor (islandName: string) (avatarId:string) (world:World) : World =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey avatarId) then
                        Some k
                    else
                        None)
        match location, world.Avatars |> Map.tryFind avatarId with
        | Some l, Some avatar ->
            world
            |> SetHeading (Location.HeadingTo avatar.Position l |> Dms.ToDms) avatarId
            |> AddMessages avatarId [ islandName |> sprintf "You head for `%s`." ]
        | _, Some _ ->
            world
            |> AddMessages avatarId [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            world

    let AcceptJob (jobIndex:uint32) (location:Location) (avatarId:string) (world:World) : World =
        match jobIndex, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId, world.Avatars |> Map.tryFind avatarId |> Option.bind (fun a -> a.Job) with
        | 0u, _, _, _ ->
            world
            |> AddMessages avatarId [ "That job is currently unavailable." ]
        | _, Some island, Some _,None ->
            match island |> Island.RemoveJob jobIndex with
            | isle, Some job ->
                world
                |> SetIsland location (isle |> Some)
                |> TransformIsland job.Destination (Island.MakeKnown avatarId >> Some)
                |> TransformAvatar avatarId (Avatar.SetJob job >> Some)
                |> AddMessages avatarId [ "You accepted the job!" ]
            | _ ->
                world
                |> AddMessages avatarId [ "That job is currently unavailable." ]
        | _, Some island, Some _, Some job ->
            world
            |> AddMessages avatarId [ "You must complete or abandon your current job before taking on a new one." ]
        | _ -> 
            world

    let AbandonJob (avatarId: string) (world:World) : World =
        match world.Avatars |> Map.tryFind avatarId |> Option.bind (fun a -> a.Job) with
        | Some _ ->
            world
            |> AddMessages avatarId [ "You abandon your job." ]
            |> TransformAvatar avatarId (Avatar.AbandonJob >> Some)
        | _ ->
            world
            |> AddMessages avatarId [ "You have no job to abandon." ]

    let private FindItemByName (itemName:string) (world:World) : (uint * ItemDescriptor) option =
        world.Items
        |> Map.tryPick (fun k v -> if v.DisplayName = itemName then Some (k,v) else None)


    let BuyItems (location:Location) (tradeQuantity:TradeQuantity) (itemName:string) (avatarId:string) (world:World) : World =
        match world |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor) , Some island, Some avatar->
            let unitPrice = 
                Item.DetermineSalePrice world.Commodities island.Markets descriptor 
            let quantity =
                match tradeQuantity with
                | Specific amount -> amount
                | Maximum -> floor(avatar.Money / unitPrice) |> uint32
            let price = (quantity |> float) * unitPrice
            if price > avatar.Money then
                world
                |> AddMessages avatarId ["You don't have enough money."]
            elif quantity = 0u then
                world
                |> AddMessages avatarId ["You don't have enough money to buy any of those."]
            else
                world
                |> AddMessages avatarId [(quantity, descriptor.DisplayName) ||> sprintf "You complete the purchase of %u %s."]
                |> TransformAvatar avatarId (Avatar.SpendMoney price >> Some)
                |> TransformAvatar avatarId (Avatar.AddInventory item quantity >> Some)
                |> TransformIsland location (Island.UpdateMarketForItemSale world.Commodities descriptor quantity >> Some)
        | None, Some island, Some _ ->
            world
            |> AddMessages avatarId ["Round these parts, we don't sell things like that."]
        | _ ->
            world
            |> AddMessages avatarId ["You cannot buy items here."]

    let SellItems (location:Location) (tradeQuantity:TradeQuantity) (itemName:string) (avatarId:string) (world:World) : World =
        match world |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor), Some island, Some avatar ->
            let quantity = 
                match tradeQuantity with
                | Specific q -> q
                | Maximum -> 
                    avatar 
                    |> Avatar.GetItemCount item
            if quantity > (avatar |> Avatar.GetItemCount item) then
                world
                |> AddMessages avatarId ["You don't have enough of those to sell."]
            elif quantity = 0u then
                world
                |> AddMessages avatarId ["You don't have any of those to sell."]
            else
                let unitPrice = 
                    Item.DeterminePurchasePrice world.Commodities island.Markets descriptor 
                let price = (quantity |> float) * unitPrice
                world
                |> AddMessages avatarId [(quantity, descriptor.DisplayName) ||> sprintf "You complete the sale of %u %s."]
                |> TransformAvatar avatarId (Avatar.EarnMoney price >> Some)
                |> TransformAvatar avatarId (Avatar.RemoveInventory item quantity >> Some)
                |> TransformIsland location (Island.UpdateMarketForItemPurchase world.Commodities descriptor quantity >> Some)
        | None, Some island, Some _ ->
            world
            |> AddMessages avatarId ["Round these parts, we don't buy things like that."]
        | _ ->
            world
            |> AddMessages avatarId ["You cannot sell items here."]
