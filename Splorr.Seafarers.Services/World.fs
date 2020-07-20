namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

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
        GenerateIslandNames random (world.Islands.Count) (Set.empty) //TODO: make antwerp an island name
        |> List.sortBy (fun _ -> random.Next())
        |> List.zip (world.Islands |> Map.toList |> List.map fst)
        |> List.fold
            (fun w (l,n) -> w |> TransformIsland l (Island.SetName n >> Some)) world

    let rec private GenerateIslands (configuration:WorldConfiguration) (random:System.Random) (currentTry:uint32) (world: World) : World =
        if currentTry>=configuration.MaximumGenerationTries then
            world
        else
            let candidateLocation = (random.NextDouble() * (configuration.WorldSize |> fst), random.NextDouble() * (configuration.WorldSize |> snd))
            if world.Islands |> Map.exists(fun k _ ->(Location.DistanceTo candidateLocation k) < configuration.MinimumIslandDistance) then
                GenerateIslands configuration random (currentTry+1u) world
            else
                GenerateIslands configuration random 0u {world with Islands = world.Islands |> Map.add candidateLocation (Island.Create())}

    let Create (configuration:WorldConfiguration) (random:System.Random) (avatarId:string): World =
        {
            AvatarId = avatarId
            Avatars = [avatarId,Avatar.Create configuration.StatisticDescriptors configuration.RationItems (configuration.WorldSize |> Location.ScaleBy 0.5)] |> Map.ofList
            Islands = Map.empty
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

    //a bool is not sufficient!
    //an avatar may be dead because of health
    //an avatar may be dead because of turns
    let IsAvatarAlive (avatarId:string) (world:World) =
        match world.Avatars |> Map.tryFind avatarId with
        | Some avatar -> 
            match avatar with
            | Avatar.ALIVE -> true
            | Avatar.ZERO_HEALTH -> false
            | Avatar.OLD_AGE -> false
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
                |> AddMessages avatarId [ "You die of old age!" ]
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

    let Dock (random:System.Random) (rewardRange:float*float) (commodities:Map<uint64, CommodityDescriptor>) (items:Map<uint64, ItemDescriptor>) (location: Location) (avatarId:string) (world:World) : World =
        match world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some island, Some avatar ->
            let destinations =
                world.Islands
                |> Map.toList
                |> List.map fst
                |> Set.ofList
                |> Set.remove location
            let updatedIsland = 
                island
                |> Island.AddVisit world.Avatars.[avatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].CurrentValue avatarId//only when this counts as a new visit...
                |> Island.GenerateJobs random rewardRange destinations 
                |> Island.GenerateCommodities random commodities
                |> Island.GenerateItems random items
            let oldVisitCount =
                island.AvatarVisits
                |> Map.tryFind avatarId
                |> Option.map (fun x -> x.VisitCount)
                |> Option.defaultValue 0u
            let newVisitCount =
                updatedIsland.AvatarVisits
                |> Map.tryFind avatarId
                |> Option.map (fun x -> x.VisitCount)
                |> Option.defaultValue 0u
            world
            |> TransformIsland location 
                (fun _ -> updatedIsland |> Some)
            |> Option.foldBack (DoJobCompletion location avatarId) avatar.Job
            |> TransformAvatar avatarId (Avatar.AddMetric Metric.VisitedIsland (if newVisitCount > oldVisitCount then 1u else 0u) >> Some)//...should this add to the metric
            |> AddMessages avatarId [ "You dock." ]
        | _, Some _ -> 
            world
            |> AddMessages avatarId [ "There is no place to dock there." ]
        | _ ->
            world

    let DistanceTo (islandName: string) (world:World) : World =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey world.AvatarId) then
                        Some k
                    else
                        None)
        match location, world.Avatars |> Map.tryFind world.AvatarId with
        | Some l, Some avatar ->
            world
            |> AddMessages world.AvatarId [ (islandName, Location.DistanceTo l avatar.Position ) ||> sprintf "Distance to `%s` is %f." ]
        | _, Some _ ->
            world
            |> AddMessages world.AvatarId [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            world

    let HeadFor (islandName: string) (world:World) : World =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey world.AvatarId) then
                        Some k
                    else
                        None)
        match location, world.Avatars |> Map.tryFind world.AvatarId with
        | Some l, Some avatar ->
            world
            |> SetHeading (Location.HeadingTo avatar.Position l |> Dms.ToDms) world.AvatarId
            |> AddMessages world.AvatarId [ islandName |> sprintf "You head for `%s`." ]
        | _, Some _ ->
            world
            |> AddMessages world.AvatarId [ islandName |> sprintf "I don't know how to get to `%s`." ]
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
                |> TransformAvatar avatarId 
                    (Avatar.SetJob job 
                    >> Avatar.AddMetric Metric.AcceptedJob 1u
                    >> Some)
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
    
    //TODO: this function is in the wrong place!
    let private FindItemByName (itemName:string) (items:Map<uint64, ItemDescriptor>) : (uint64 * ItemDescriptor) option =
        items
        |> Map.tryPick (fun k v -> if v.ItemName = itemName then Some (k,v) else None)


    let BuyItems (commodities:Map<uint64, CommodityDescriptor>) (items:Map<uint64, ItemDescriptor>) (location:Location) (tradeQuantity:TradeQuantity) (itemName:string) (avatarId:string) (world:World) : World =
        match items |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor) , Some island, Some avatar->
            let unitPrice = 
                Item.DetermineSalePrice commodities island.Markets descriptor 
            let availableTonnage = avatar.Vessel.Tonnage
            let usedTonnage =
                avatar
                |> Avatar.GetUsedTonnage items
            let quantity =
                match tradeQuantity with
                | Specific amount -> amount
                | Maximum -> min (floor(availableTonnage / descriptor.Tonnage)) (floor(avatar.Money / unitPrice)) |> uint32
            let price = (quantity |> float) * unitPrice
            let tonnageNeeded = (quantity |> float) * descriptor.Tonnage
            if price > avatar.Money then
                world
                |> AddMessages avatarId ["You don't have enough money."]
            elif usedTonnage + tonnageNeeded > availableTonnage then
                world
                |> AddMessages avatarId ["You don't have enough tonnage."]
            elif quantity = 0u then
                world
                |> AddMessages avatarId ["You don't have enough money to buy any of those."]
            else
                world
                |> AddMessages avatarId [(quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s."]
                |> TransformAvatar avatarId (Avatar.SpendMoney price >> Some)
                |> TransformAvatar avatarId (Avatar.AddInventory item quantity >> Some)
                |> TransformIsland location (Island.UpdateMarketForItemSale commodities descriptor quantity >> Some)
        | None, Some island, Some _ ->
            world
            |> AddMessages avatarId ["Round these parts, we don't sell things like that."]
        | _ ->
            world
            |> AddMessages avatarId ["You cannot buy items here."]

    let SellItems (commodities:Map<uint64, CommodityDescriptor>) (items:Map<uint64, ItemDescriptor>) (location:Location) (tradeQuantity:TradeQuantity) (itemName:string) (avatarId:string) (world:World) : World =
        match items |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
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
                    Item.DeterminePurchasePrice commodities island.Markets descriptor 
                let price = (quantity |> float) * unitPrice
                world
                |> AddMessages avatarId [(quantity, descriptor.ItemName) ||> sprintf "You complete the sale of %u %s."]
                |> TransformAvatar avatarId (Avatar.EarnMoney price >> Some)
                |> TransformAvatar avatarId (Avatar.RemoveInventory item quantity >> Some)
                |> TransformIsland location (Island.UpdateMarketForItemPurchase commodities descriptor quantity >> Some)
        | None, Some island, Some _ ->
            world
            |> AddMessages avatarId ["Round these parts, we don't buy things like that."]
        | _ ->
            world
            |> AddMessages avatarId ["You cannot sell items here."]

    let CleanHull (avatarId:string) (side:Side) (world:World) : World =
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.fold
            (fun w _ -> 
                w
                |> TransformAvatar avatarId (Avatar.CleanHull side >> Some)) world
