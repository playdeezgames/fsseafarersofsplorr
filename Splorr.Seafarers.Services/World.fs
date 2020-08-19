namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TradeQuantity =
    | Maximum
    | Specific of uint64

type AvatarMessagePurger = string -> unit

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
            (nameSource             : TermSource)
            (worldSize              : Location) 
            (minimumIslandDistance  : float)
            (random                 : Random) 
            (maximumGenerationTries : uint32, 
             currentTry             : uint32) 
            (world                  : World) 
            : World =
        if currentTry>=maximumGenerationTries then
            world
            |> NameIslands nameSource random
        else
            let candidateLocation = (random.NextDouble() * (worldSize |> fst), random.NextDouble() * (worldSize |> snd))
            if world.Islands |> Map.exists(fun k _ ->(Location.DistanceTo candidateLocation k) < minimumIslandDistance) then
                GenerateIslands nameSource worldSize minimumIslandDistance random (maximumGenerationTries, currentTry+1u) world
            else
                GenerateIslands nameSource worldSize minimumIslandDistance random (maximumGenerationTries, 0u) {world with Islands = world.Islands |> Map.add candidateLocation (Island.Create())}

    let UpdateCharts 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (world    : World) 
            : World =
        let viewDistance = 
            vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        world.Avatars
        |> Map.tryFind world.AvatarId
        |> Option.fold 
            (fun w avatar -> 
                let avatarPosition = Avatar.GetPosition vesselSingleStatisticSource w.AvatarId |> Option.get
                w.Islands
                |> Map.filter
                    (fun location island -> 
                        (island.AvatarVisits.ContainsKey world.AvatarId |> not) && ((avatarPosition |> Location.DistanceTo location)<=viewDistance))
                |> Map.fold
                    (fun a location _ ->
                        a
                        |> TransformIsland location (Island.MakeSeen world.AvatarId >> Some)) w) world

    let Create 
            (nameSource                      : TermSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (rationItemSource                : RationItemSource)
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (random                          : Random) 
            (avatarId                        : string): World =
        let maximumGenerationRetries =
            WorldStatisticIdentifier.IslandGenerationRetries
            |> worldSingleStatisticSource 
            |> Statistic.GetCurrentValue
            |> uint
        let minimumIslandDistance = 
            WorldStatisticIdentifier.IslandDistance
            |> worldSingleStatisticSource 
            |> Statistic.GetCurrentValue
        let worldSize =
            (WorldStatisticIdentifier.PositionX
            |> worldSingleStatisticSource 
            |> Statistic.GetMaximumValue,
                WorldStatisticIdentifier.PositionY
                |> worldSingleStatisticSource 
                |> Statistic.GetMaximumValue)
        {
            AvatarId = avatarId
            Avatars = 
                Map.empty
                |> Map.add 
                    avatarId 
                    (Avatar.Create 
                        shipmateStatisticTemplateSource
                        shipmateSingleStatisticSink
                        rationItemSource
                        vesselStatisticTemplateSource
                        vesselStatisticSink
                        shipmateRationItemSink
                        avatarId)
            Islands = Map.empty
        }
        |> GenerateIslands 
            nameSource 
            worldSize 
            minimumIslandDistance
            random 
            (maximumGenerationRetries, 0u)
        |> UpdateCharts vesselSingleStatisticSource

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

    let ClearMessages 
            (avatarMessagePurger : AvatarMessagePurger) 
            (world               : World)
            : unit =
        avatarMessagePurger world.AvatarId

    let AddMessages
            (avatarMessageSink : AvatarMessageSink)
            (messages          : string list) 
            (world             : World) 
            : unit =
        Avatar.AddMessages avatarMessageSink messages world.AvatarId

    let SetSpeed 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarMessageSink           : AvatarMessageSink)
            (speed                       : float) 
            (world                       : World) 
            : unit = 
        world.AvatarId
        |> Avatar.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink speed 
        world.AvatarId
        |> Avatar.GetSpeed vesselSingleStatisticSource
        |> Option.iter
            (fun newSpeed ->
                world
                |> AddMessages avatarMessageSink [newSpeed |> sprintf "You set your speed to %.2f."])

    let SetHeading 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarMessageSink           : AvatarMessageSink)
            (heading                     : float) 
            (world                       : World) 
            : unit =
        world.AvatarId
        |> Avatar.SetHeading vesselSingleStatisticSource vesselSingleStatisticSink heading 
        world.AvatarId
        |> Avatar.GetHeading vesselSingleStatisticSource
        |> Option.iter
            (fun newHeading ->
                world
                |> AddMessages avatarMessageSink [newHeading |> Angle.ToDegrees |> Angle.ToString |> sprintf "You set your heading to %s." ])

    let IsAvatarAlive
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (world    : World) 
            : bool =
        (Shipmate.GetStatus 
            shipmateSingleStatisticSource
            world.AvatarId
            Primary) = Alive

    let rec Move
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (distance                      : uint32) 
            (world                         : World) 
            : World =
        let avatarId = world.AvatarId
        match distance, world.Avatars |> Map.tryFind avatarId with
        | x, Some _ when x > 0u ->
            world
            |> AddMessages avatarMessageSink [ "Steady as she goes." ]
            Avatar.Move 
                avatarInventorySink
                avatarInventorySource
                avatarShipmateSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateRationItemSource 
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
                vesselSingleStatisticSink 
                vesselSingleStatisticSource 
                avatarId 
            let steppedWorld = 
                world
                |> UpdateCharts vesselSingleStatisticSource
            if IsAvatarAlive 
                    shipmateSingleStatisticSource 
                    steppedWorld |> not then
                steppedWorld
                |> AddMessages avatarMessageSink [ "You die of old age!" ]
                steppedWorld
            else
                Move
                    avatarInventorySink
                    avatarInventorySource
                    avatarMessageSink 
                    avatarShipmateSource
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    shipmateRationItemSource 
                    shipmateSingleStatisticSink
                    shipmateSingleStatisticSource
                    vesselSingleStatisticSink 
                    vesselSingleStatisticSource 
                    (x-1u) 
                    steppedWorld
        | _ -> 
            world

    let GetNearbyLocations
            (from                        : Location) 
            (maximumDistance             : float) 
            (world                       : World) 
            : Location list =
        world.Islands
        |> Map.toList
        |> List.map fst
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)


    let private DoJobCompletion
            (avatarMessageSink             : AvatarMessageSink)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (location                      : Location) 
            (job                           : Job) 
            (world                         : World) 
            : World = 
        if location = job.Destination then
            world
            |> AddMessages avatarMessageSink [ "You complete your job." ]
            world
            |> TransformAvatar 
                (Avatar.CompleteJob 
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    shipmateSingleStatisticSink 
                    shipmateSingleStatisticSource 
                    world.AvatarId >> Some)
        else
            world

    let Dock 
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (termSources                   : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (commoditySource               : CommoditySource) 
            (itemSource                    : ItemSource) 
            (worldSingleStatisticSource    : WorldSingleStatisticSource)
            (islandMarketSource            : IslandMarketSource) 
            (islandMarketSink              : IslandMarketSink) 
            (islandItemSource              : IslandItemSource) 
            (islandItemSink                : IslandItemSink) 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarMessageSink             : AvatarMessageSink)
            (random                        : Random) 
            (location                      : Location) 
            (world                         : World) 
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
                |> Island.AddVisit (DateTimeOffset.Now.ToUnixTimeSeconds()) avatarId
                |> Island.GenerateJobs 
                    termSources 
                    worldSingleStatisticSource 
                    random 
                    destinations 
            Island.GenerateCommodities commoditySource islandMarketSource islandMarketSink random location
            Island.GenerateItems islandItemSource islandItemSink random itemSource location
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
            |> AddMessages (avatarMessageSink : AvatarMessageSink) [ "You dock." ]
            avatarId
            |> Avatar.AddMetric 
                avatarSingleMetricSink
                avatarSingleMetricSource
                Metric.VisitedIsland 
                (if newVisitCount > oldVisitCount then 1UL else 0UL)
            world
            |> TransformIsland location 
                (fun _ -> updatedIsland |> Some)
            |> Option.foldBack 
                (DoJobCompletion 
                    avatarMessageSink
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    shipmateSingleStatisticSink 
                    shipmateSingleStatisticSource 
                    location) avatar.Job
        | _, Some _ -> 
            world
            |> AddMessages (avatarMessageSink : AvatarMessageSink) [ "There is no place to dock there." ]
            world
        | _ ->
            world

    let DistanceTo 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarMessageSink           : AvatarMessageSink)
            (islandName                  : string) 
            (world                       : World) 
            : unit =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey world.AvatarId) && v.AvatarVisits.[world.AvatarId].VisitCount.IsSome then
                        Some k
                    else
                        None)
        match location, Avatar.GetPosition vesselSingleStatisticSource world.AvatarId with
        | Some l, Some avatarPosition ->
            world
            |> AddMessages avatarMessageSink [ (islandName, Location.DistanceTo l avatarPosition ) ||> sprintf "Distance to `%s` is %f." ]
        | _, Some _ ->
            world
            |> AddMessages avatarMessageSink [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    let HeadFor
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarMessageSink           : AvatarMessageSink)
            (islandName                  : string) 
            (world                       : World) 
            : unit =
        let location =
            world.Islands
            |> Map.tryPick 
                (fun k v -> 
                    if v.Name = islandName && (v.AvatarVisits.ContainsKey world.AvatarId) && v.AvatarVisits.[world.AvatarId].VisitCount.IsSome then
                        Some k
                    else
                        None)
        match location, Avatar.GetPosition vesselSingleStatisticSource world.AvatarId with
        | Some l, Some avatarPosition ->
            [
                AddMessages avatarMessageSink [ islandName |> sprintf "You head for `%s`." ]
                SetHeading vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink (Location.HeadingTo avatarPosition l |> Angle.ToDegrees)
            ]
            |> List.iter (fun f -> f world)
        | _, Some _ ->
            world
            |> AddMessages avatarMessageSink [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    let AcceptJob 
            (avatarMessageSink        : AvatarMessageSink)
            (avatarSingleMetricSink   : AvatarSingleMetricSink)
            (avatarSingleMetricSource : AvatarSingleMetricSource)
            (jobIndex                 : uint32) 
            (location                 : Location) 
            (world                    : World) 
            : World =
        let avatarId = world.AvatarId
        match jobIndex, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId, world.Avatars |> Map.tryFind avatarId |> Option.bind (fun a -> a.Job) with
        | 0u, _, _, _ ->
            world
            |> AddMessages avatarMessageSink [ "That job is currently unavailable." ]
            world
        | _, Some island, Some _,None ->
            match island |> Island.RemoveJob jobIndex with
            | isle, Some job ->
                world
                |> AddMessages avatarMessageSink [ "You accepted the job!" ]
                avatarId
                |> Avatar.AddMetric 
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    Metric.AcceptedJob 
                    1UL
                world
                |> SetIsland location (isle |> Some)
                |> TransformIsland job.Destination (Island.MakeKnown avatarId >> Some)
                |> TransformAvatar 
                    (Avatar.SetJob job 
                    >> Some)
            | _ ->
                world
                |> AddMessages avatarMessageSink [ "That job is currently unavailable." ]
                world
        | _, Some island, Some _, Some job ->
            world
            |> AddMessages avatarMessageSink [ "You must complete or abandon your current job before taking on a new one." ]
            world
        | _ -> 
            world

    let AbandonJob
            (avatarMessageSink             : AvatarMessageSink)
            (avatarSingleMetricSink   : AvatarSingleMetricSink)
            (avatarSingleMetricSource : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (world                         : World) 
            : World =
        let avatarId = world.AvatarId
        match world.Avatars |> Map.tryFind avatarId |> Option.bind (fun a -> a.Job) with
        | Some _ ->
            world
            |> AddMessages avatarMessageSink [ "You abandon your job." ]
            world
            |> TransformAvatar 
                (Avatar.AbandonJob 
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    shipmateSingleStatisticSink 
                    shipmateSingleStatisticSource 
                    world.AvatarId
                >> Some)
        | _ ->
            world
            |> AddMessages avatarMessageSink [ "You have no job to abandon." ]
            world
    
    //TODO: this function is in the wrong place!
    let private FindItemByName 
            (itemName : string) 
            (items    : Map<uint64, ItemDescriptor>) 
            : (uint64 * ItemDescriptor) option =
        items
        |> Map.tryPick (fun k v -> if v.ItemName = itemName then Some (k,v) else None)


    let BuyItems 
            (islandMarketSource            : IslandMarketSource)
            (islandSingleMarketSource      : IslandSingleMarketSource) 
            (islandSingleMarketSink        : Location->(uint64 * Market)->unit) 
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarInventorySink           : AvatarInventorySink)
            (avatarMessageSink             : AvatarMessageSink)
            (commoditySource               : CommoditySource) 
            (items                         : Map<uint64, ItemDescriptor>) 
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (world                         : World) 
            : World =
        let avatarId = world.AvatarId
        match items |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor) , Some island, Some avatar->
            let markets =
                islandMarketSource location
            let unitPrice = 
                Item.DetermineSalePrice (commoditySource()) markets descriptor 
            let availableTonnage = vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Tonnage |> Option.map (fun x->x.CurrentValue) |> Option.get
            let usedTonnage =
                avatarId
                |> Avatar.GetUsedTonnage
                    avatarInventorySource
                    items
            let quantity =
                match tradeQuantity with
                | Specific amount -> amount
                | Maximum -> min (floor(availableTonnage / descriptor.Tonnage)) (floor((world.AvatarId |> Avatar.GetMoney shipmateSingleStatisticSource) / unitPrice)) |> uint64
            let price = (quantity |> float) * unitPrice
            let tonnageNeeded = (quantity |> float) * descriptor.Tonnage
            if price > (world.AvatarId |> Avatar.GetMoney shipmateSingleStatisticSource) then
                world
                |> AddMessages avatarMessageSink ["You don't have enough money."]
                world
            elif usedTonnage + tonnageNeeded > availableTonnage then
                world
                |> AddMessages avatarMessageSink ["You don't have enough tonnage."]
                world
            elif quantity = 0UL then
                world
                |> AddMessages avatarMessageSink ["You don't have enough money to buy any of those."]
                world
            else
                Island.UpdateMarketForItemSale islandSingleMarketSource islandSingleMarketSink commoditySource descriptor quantity location
                world
                |> AddMessages avatarMessageSink [(quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s."]
                world
                |> TransformAvatar 
                    (fun a -> 
                        world.AvatarId
                        |> Avatar.SpendMoney 
                            shipmateSingleStatisticSink
                            shipmateSingleStatisticSource
                            price 
                        world.AvatarId
                        |> Avatar.AddInventory 
                            avatarInventorySink
                            avatarInventorySource
                            item 
                            quantity 
                        a
                        |> Some)//TODO: once this returns unit, this whole function returns unit
        | None, Some island, Some _ ->
            world
            |> AddMessages avatarMessageSink ["Round these parts, we don't sell things like that."]
            world
        | _ ->
            world
            |> AddMessages avatarMessageSink ["You cannot buy items here."]
            world

    let SellItems 
            (islandMarketSource            : Location -> Map<uint64, Market>) 
            (islandSingleMarketSource      : Location -> uint64            -> Market option) 
            (islandSingleMarketSink        : Location -> (uint64 * Market) -> unit) 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarInventorySink           : AvatarInventorySink)
            (avatarMessageSink             : AvatarMessageSink)
            (commoditySource               : CommoditySource) //TODO: this should move to top of parameter list
            (items                         : Map<uint64, ItemDescriptor>) //TODO: this should move to top and become source
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (world                         : World) 
            : World =
        let avatarId = world.AvatarId
        match items |> FindItemByName itemName, world.Islands |> Map.tryFind location, world.Avatars |> Map.tryFind avatarId with
        | Some (item, descriptor), Some _, Some avatar ->
            let quantity = 
                match tradeQuantity with
                | Specific q -> q
                | Maximum -> 
                    avatarId 
                    |> Avatar.GetItemCount avatarInventorySource item
            if quantity > (avatarId |> Avatar.GetItemCount avatarInventorySource item) then
                world
                |> AddMessages avatarMessageSink ["You don't have enough of those to sell."]
                world
            elif quantity = 0UL then
                world
                |> AddMessages avatarMessageSink ["You don't have any of those to sell."]
                world
            else
                let markets = islandMarketSource location
                let unitPrice = 
                    Item.DeterminePurchasePrice (commoditySource()) markets descriptor 
                let price = (quantity |> float) * unitPrice
                Island.UpdateMarketForItemPurchase islandSingleMarketSource islandSingleMarketSink commoditySource descriptor quantity location
                world
                |> AddMessages avatarMessageSink [(quantity, descriptor.ItemName) ||> sprintf "You complete the sale of %u %s."]
                world
                |> TransformAvatar 
                    (fun a -> 
                        Avatar.EarnMoney 
                            shipmateSingleStatisticSink
                            shipmateSingleStatisticSource
                            price 
                            world.AvatarId
                        world.AvatarId
                        |> Avatar.RemoveInventory 
                            avatarInventorySource
                            avatarInventorySink
                            item 
                            quantity 
                        a
                        |> Some)
        | None, Some island, Some _ ->
            world
            |> AddMessages avatarMessageSink ["Round these parts, we don't buy things like that."]
            world
        | _ ->
            world
            |> AddMessages avatarMessageSink ["You cannot sell items here."]
            world

    let CleanHull //TODO: this just passes everything along to avatar.CleanHull, so eliminate
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (side                          : Side) 
            (world                         : World) 
            : unit =
        world.AvatarId 
        |> Avatar.CleanHull 
            avatarShipmateSource
            avatarSingleMetricSink
            avatarSingleMetricSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSink 
            vesselSingleStatisticSource
            side 
            
