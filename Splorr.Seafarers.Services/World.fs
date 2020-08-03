namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TradeQuantity =
    | Maximum
    | Specific of uint32

module World =
    let private GenerateIslandName //TODO: move to world generator?
            (random:Random) 
            : string =
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

    let rec private GenerateIslandNames  //TODO: move to world generator?
            (random:Random) 
            (nameCount:int) 
            (names: Set<string>) 
            : List<string> =
        if names.Count>=nameCount then
            names
            |> Set.toList
            |> Utility.SortListRandomly random
            |> List.take nameCount
        else
            names
            |> Set.add (GenerateIslandName random)
            |> GenerateIslandNames random nameCount

    let SetIsland 
            (location:Location) 
            (island:Island option) 
            (world:World) 
            : World =
        match island with 
        | Some i ->
            {world with Islands = world.Islands |> Map.add location i}
        | None ->
            {world with Islands = world.Islands |> Map.remove location}
            

    let TransformIsland 
            (location  : Location) 
            (transform : Island->Island option) 
            (world     : World) 
            : World =
        (world.Islands
        |> Map.tryFind location
        |> Option.bind transform
        |> SetIsland location) world

    let private NameIslands  //TODO: move to world generator?
            (nameSource : TermSource)
            (random     : Random) 
            (world      : World) 
            : World =
        GenerateIslandNames 
            random 
            (world.Islands.Count) 
            (nameSource() |> Set.ofList)
        |> Utility.SortListRandomly random
        |> List.zip (world.Islands |> Map.toList |> List.map fst)
        |> List.fold
            (fun w (l,n) -> w |> TransformIsland l (Island.SetName n >> Some)) world

    let rec private GenerateIslands  //TODO: move to world generator?
            (nameSource    : TermSource)
            (configuration : WorldConfiguration) 
            (random        : Random) 
            (currentTry    : uint32) 
            (world         : World) 
            : World =
        if currentTry>=configuration.MaximumGenerationTries then
            world
            |> NameIslands nameSource random
        else
            let candidateLocation = (random.NextDouble() * (configuration.WorldSize |> fst), random.NextDouble() * (configuration.WorldSize |> snd))
            if world.Islands |> Map.exists(fun k _ ->(Location.DistanceTo candidateLocation k) < configuration.MinimumIslandDistance) then
                GenerateIslands nameSource configuration random (currentTry+1u) world
            else
                GenerateIslands nameSource configuration random 0u {world with Islands = world.Islands |> Map.add candidateLocation (Island.Create())}

    let UpdateCharts 
            (world    : World) 
            : World =
        world.Avatars
        |> Map.tryFind world.AvatarId
        |> Option.fold 
            (fun w avatar -> 
                w.Islands
                |> Map.filter
                    (fun location island -> 
                        (island.AvatarVisits.ContainsKey world.AvatarId |> not) && ((avatar.Position |> Location.DistanceTo location)<=avatar.ViewDistance))
                |> Map.fold
                    (fun a location _ ->
                        a
                        |> TransformIsland location (Island.MakeSeen world.AvatarId >> Some)) w) world

    let Create 
            (nameSource                    : TermSource)
            (vesselStatisticTemplateSource : unit -> Map<VesselStatisticIdentifier, VesselStatisticTemplate>)
            (vesselStatisticSink           : string -> Map<VesselStatisticIdentifier, Statistic> -> unit)
            (configuration                 : WorldConfiguration) 
            (random                        : Random) 
            (avatarId                      : string): World =
        {
            AvatarId = avatarId
            Avatars = 
                Map.empty
                |> Map.add 
                    avatarId 
                    (Avatar.Create 
                        vesselStatisticTemplateSource
                        vesselStatisticSink
                        avatarId
                        configuration.AvatarDistances 
                        configuration.StatisticDescriptors 
                        configuration.RationItems 
                        (configuration.WorldSize 
                        |> Location.ScaleBy 0.5))
            Islands = Map.empty
        }
        |> GenerateIslands nameSource configuration random 0u
        |> UpdateCharts

    let TransformAvatar 
            (transform : Avatar -> Avatar option) 
            (world     : World) : World =
        let avatarId = world.AvatarId
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.bind transform
        |> Option.fold
            (fun w avatar -> 
                {w with Avatars = w.Avatars|> Map.add avatarId avatar}) {world with Avatars = world.Avatars |> Map.remove avatarId}

    let ClearMessages =
        TransformAvatar (Avatar.ClearMessages >> Some)

    let AddMessages
            (messages : string list) =
        TransformAvatar (Avatar.AddMessages messages >> Some)

    let SetSpeed 
            (speed    : float) 
            (world    : World) : World = 
        let avatarId = world.AvatarId
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.bind (Avatar.SetSpeed speed >> Some)
        |> Option.fold 
            (fun w a ->
                {w with Avatars = w.Avatars |> Map.add avatarId a}
                |> AddMessages [a.Speed |> sprintf "You set your speed to %f."]) world

    let SetHeading 
            (heading  : float) 
            (world    : World) 
            : World =
        let avatarId = world.AvatarId
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.bind (Avatar.SetHeading heading >> Some)
        |> Option.fold 
            (fun w a ->
                {w with Avatars = w.Avatars |> Map.add avatarId a}
                |> AddMessages [a.Heading |> Angle.ToDegrees |> Angle.ToString |> sprintf "You set your heading to %s." ]) world

    //a bool is not sufficient!
    //an avatar may be dead because of health
    //an avatar may be dead because of turns
    let IsAvatarAlive 
            (world    : World) 
            : bool =
        match world.Avatars |> Map.tryFind world.AvatarId with
        | Some avatar -> 
            match avatar with
            | Avatar.ALIVE -> true
            | Avatar.ZERO_HEALTH -> false
            | Avatar.OLD_AGE -> false
        | _ -> false
        

    let rec Move 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink   : string->VesselStatisticIdentifier*Statistic->unit)
            (distance                    : uint32) 
            (world                       : World) 
            : World =
        let avatarId = world.AvatarId
        match distance, world.Avatars |> Map.tryFind avatarId with
        | x, Some _ when x > 0u ->
            let steppedWorld = 
                world
                |> TransformAvatar (Avatar.Move vesselSingleStatisticSource vesselSingleStatisticSink avatarId >> Some)
                |> AddMessages [ "Steady as she goes." ]
                |> UpdateCharts
            if IsAvatarAlive steppedWorld |> not then
                steppedWorld
                |> AddMessages [ "You die of old age!" ]
            else
                Move vesselSingleStatisticSource vesselSingleStatisticSink (x-1u) steppedWorld
        | _ -> 
            world

    let GetNearbyLocations 
            (from            : Location) 
            (maximumDistance : float) 
            (world           : World) 
            : Location list =
        world.Islands
        |> Map.toList
        |> List.map fst
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)


    let private DoJobCompletion 
            (location : Location) 
            (job      : Job) 
            (world    : World) : World = 
        if location = job.Destination then
            let avatarId = world.AvatarId
            world
            |> AddMessages [ "You complete your job." ]
            |> TransformAvatar (Avatar.CompleteJob >> Some)
        else
            world

    let Dock 
            (termSources        : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (commoditySource    : unit ->Map<uint64, CommodityDescriptor>) 
            (itemSource         : unit->Map<uint64, ItemDescriptor>) 
            (islandMarketSource : Location->Map<uint64, Market>) 
            (islandMarketSink   : Location->Map<uint64, Market>->unit) 
            (islandItemSource   : Location->Set<uint64>) 
            (islandItemSink     : Location->Set<uint64>->unit) 
            (random             : Random) 
            (rewardRange        : float*float) 
            (location           : Location) 
            (world              : World) 
            : World =
        let avatarId = world.AvatarId
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
                |> Island.GenerateJobs termSources random rewardRange destinations 
            let items = itemSource()
            Island.GenerateCommodities commoditySource islandMarketSource islandMarketSink random location
            Island.GenerateItems islandItemSource islandItemSink random items location
            let oldVisitCount =
                island.AvatarVisits
                |> Map.tryFind avatarId
                |> Option.bind (fun x -> x.VisitCount)
                |> Option.defaultValue 0u
            let newVisitCount =
                updatedIsland.AvatarVisits
                |> Map.tryFind avatarId
                |> Option.bind (fun x -> x.VisitCount)
                |> Option.defaultValue 0u
            world
            |> TransformIsland location 
                (fun _ -> updatedIsland |> Some)
            |> Option.foldBack (DoJobCompletion location) avatar.Job
            |> TransformAvatar (Avatar.AddMetric Metric.VisitedIsland (if newVisitCount > oldVisitCount then 1u else 0u) >> Some)//...should this add to the metric
            |> AddMessages [ "You dock." ]
        | _, Some _ -> 
            world
            |> AddMessages [ "There is no place to dock there." ]
        | _ ->
            world

    let DistanceTo 
            (islandName : string) 
            (world      : World) 
            : World =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey world.AvatarId) && v.AvatarVisits.[world.AvatarId].VisitCount.IsSome then
                        Some k
                    else
                        None)
        match location, world.Avatars |> Map.tryFind world.AvatarId with
        | Some l, Some avatar ->
            world
            |> AddMessages [ (islandName, Location.DistanceTo l avatar.Position ) ||> sprintf "Distance to `%s` is %f." ]
        | _, Some _ ->
            world
            |> AddMessages [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            world

    let HeadFor 
            (islandName : string) 
            (world      : World) 
            : World =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey world.AvatarId) && v.AvatarVisits.[world.AvatarId].VisitCount.IsSome then
                        Some k
                    else
                        None)
        match location, world.Avatars |> Map.tryFind world.AvatarId with
        | Some l, Some avatar ->
            world
            |> SetHeading (Location.HeadingTo avatar.Position l |> Angle.ToDegrees)
            |> AddMessages [ islandName |> sprintf "You head for `%s`." ]
        | _, Some _ ->
            world
            |> AddMessages [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            world

    let AcceptJob 
            (jobIndex : uint32) 
            (location : Location) 
            (world    : World) 
            : World =
        let avatarId = world.AvatarId
        match jobIndex, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId, world.Avatars |> Map.tryFind avatarId |> Option.bind (fun a -> a.Job) with
        | 0u, _, _, _ ->
            world
            |> AddMessages [ "That job is currently unavailable." ]
        | _, Some island, Some _,None ->
            match island |> Island.RemoveJob jobIndex with
            | isle, Some job ->
                world
                |> SetIsland location (isle |> Some)
                |> TransformIsland job.Destination (Island.MakeKnown avatarId >> Some)
                |> TransformAvatar 
                    (Avatar.SetJob job 
                    >> Avatar.AddMetric Metric.AcceptedJob 1u
                    >> Some)
                |> AddMessages [ "You accepted the job!" ]
            | _ ->
                world
                |> AddMessages [ "That job is currently unavailable." ]
        | _, Some island, Some _, Some job ->
            world
            |> AddMessages [ "You must complete or abandon your current job before taking on a new one." ]
        | _ -> 
            world

    let AbandonJob 
            (world    : World) 
            : World =
        let avatarId = world.AvatarId
        match world.Avatars |> Map.tryFind avatarId |> Option.bind (fun a -> a.Job) with
        | Some _ ->
            world
            |> AddMessages [ "You abandon your job." ]
            |> TransformAvatar (Avatar.AbandonJob >> Some)
        | _ ->
            world
            |> AddMessages [ "You have no job to abandon." ]
    
    //TODO: this function is in the wrong place!
    let private FindItemByName 
            (itemName : string) 
            (items    : Map<uint64, ItemDescriptor>) 
            : (uint64 * ItemDescriptor) option =
        items
        |> Map.tryPick (fun k v -> if v.ItemName = itemName then Some (k,v) else None)


    let BuyItems 
            (islandMarketSource     : Location->Map<uint64, Market>) 
            (islandSingleMarketSink : Location->(uint64 * Market)->unit) 
            (vesselSingleStatisticSource:string->VesselStatisticIdentifier->Statistic option)
            (commodities            : Map<uint64, CommodityDescriptor>) 
            (items                  : Map<uint64, ItemDescriptor>) 
            (location               : Location) 
            (tradeQuantity          : TradeQuantity) 
            (itemName               : string) 
            (world                  : World) 
            : World =
        let avatarId = world.AvatarId
        match items |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor) , Some island, Some avatar->
            let markets =
                islandMarketSource location
            let unitPrice = 
                Item.DetermineSalePrice commodities markets descriptor 
            let availableTonnage = vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Tonnage |> Option.map (fun x->x.CurrentValue) |> Option.get
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
                |> AddMessages ["You don't have enough money."]
            elif usedTonnage + tonnageNeeded > availableTonnage then
                world
                |> AddMessages ["You don't have enough tonnage."]
            elif quantity = 0u then
                world
                |> AddMessages ["You don't have enough money to buy any of those."]
            else
                Island.UpdateMarketForItemSale islandMarketSource islandSingleMarketSink commodities descriptor quantity location
                world
                |> AddMessages [(quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s."]
                |> TransformAvatar (Avatar.SpendMoney price >> Avatar.AddInventory item quantity >> Some)
        | None, Some island, Some _ ->
            world
            |> AddMessages ["Round these parts, we don't sell things like that."]
        | _ ->
            world
            |> AddMessages ["You cannot buy items here."]

    let SellItems 
            (islandMarketSource       : Location -> Map<uint64, Market>) 
            (islandSingleMarketSource : Location -> uint64            -> Market option) 
            (islandSingleMarketSink   : Location -> (uint64 * Market) -> unit) 
            (commodities              : Map<uint64, CommodityDescriptor>) 
            (items                    : Map<uint64, ItemDescriptor>) 
            (location                 : Location) 
            (tradeQuantity            : TradeQuantity) 
            (itemName                 : string) 
            (world                    : World) 
            : World =
        let avatarId = world.AvatarId
        match items |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor), Some _, Some avatar ->
            let quantity = 
                match tradeQuantity with
                | Specific q -> q
                | Maximum -> 
                    avatar 
                    |> Avatar.GetItemCount item
            if quantity > (avatar |> Avatar.GetItemCount item) then
                world
                |> AddMessages ["You don't have enough of those to sell."]
            elif quantity = 0u then
                world
                |> AddMessages ["You don't have any of those to sell."]
            else
                let markets = islandMarketSource location
                let unitPrice = 
                    Item.DeterminePurchasePrice commodities markets descriptor 
                let price = (quantity |> float) * unitPrice
                Island.UpdateMarketForItemPurchase islandSingleMarketSource islandSingleMarketSink commodities descriptor quantity location
                world
                |> AddMessages [(quantity, descriptor.ItemName) ||> sprintf "You complete the sale of %u %s."]
                |> TransformAvatar (Avatar.EarnMoney price >> Avatar.RemoveInventory item quantity >> Some)
        | None, Some island, Some _ ->
            world
            |> AddMessages ["Round these parts, we don't buy things like that."]
        | _ ->
            world
            |> AddMessages ["You cannot sell items here."]

    let CleanHull 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink   : string->VesselStatisticIdentifier*Statistic->unit)
            (side     : Side) 
            (world    : World) 
            : World =
        let avatarId = world.AvatarId
        world.Avatars
        |> Map.tryFind avatarId
        |> Option.fold
            (fun w _ -> 
                w
                |> TransformAvatar (Avatar.CleanHull vesselSingleStatisticSource vesselSingleStatisticSink avatarId side >> Some)) world
