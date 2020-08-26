namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TradeQuantity =
    | Maximum
    | Specific of uint64

type AvatarMessagePurger = string -> unit
type IslandLocationByNameSource = string -> Location option
type IslandSource = unit -> Location list

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

    let private NameIslands  //TODO: move to world generator?
            (islandSingleNameSink : IslandSingleNameSink)
            (islandSource         : IslandSource)
            (nameSource           : TermSource)
            (random               : Random) 
            : unit =
        let locations = 
            islandSource()
        GenerateIslandNames 
            random 
            (locations.Length) 
            (nameSource() |> Set.ofList)
        |> Utility.SortListRandomly random
        |> List.zip (locations)
        |> List.iter
            (fun (l,n) -> 
                islandSingleNameSink l (Some n))

    let rec private GenerateIslands  //TODO: move to world generator?
            (islandSingleNameSink          : IslandSingleNameSink)
            (islandSingleStatisticSink     : IslandSingleStatisticSink)
            (islandSource                  : IslandSource)
            (islandStatisticTemplateSource : IslandStatisticTemplateSource)
            (nameSource                    : TermSource)
            (worldSize                     : Location) 
            (minimumIslandDistance         : float)
            (random                        : Random) 
            (maximumGenerationTries        : uint32, 
             currentTry                    : uint32) 
            : unit =
        if currentTry>=maximumGenerationTries then
            NameIslands 
                islandSingleNameSink 
                islandSource
                nameSource 
                random
        else
            let locations = islandSource()
            let candidateLocation = (random.NextDouble() * (worldSize |> fst), random.NextDouble() * (worldSize |> snd))
            if locations |> List.exists(fun k ->(Location.DistanceTo candidateLocation k) < minimumIslandDistance) then
                GenerateIslands 
                    islandSingleNameSink 
                    islandSingleStatisticSink
                    islandSource
                    islandStatisticTemplateSource
                    nameSource 
                    worldSize 
                    minimumIslandDistance 
                    random 
                    (maximumGenerationTries, currentTry+1u) 
            else
                Island.Create
                    islandSingleStatisticSink
                    islandStatisticTemplateSource
                    candidateLocation
                GenerateIslands 
                    islandSingleNameSink 
                    islandSingleStatisticSink
                    islandSource
                    islandStatisticTemplateSource
                    nameSource 
                    worldSize 
                    minimumIslandDistance 
                    random 
                    (maximumGenerationTries, 0u) 

    let UpdateCharts 
            (avatarIslandSingleMetricSink : AvatarIslandSingleMetricSink)
            (islandSource                 : IslandSource)
            (vesselSingleStatisticSource  : VesselSingleStatisticSource)
            (world                        : World) 
            : unit =
        let viewDistance = 
            vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition = 
            world.AvatarId 
            |> Avatar.GetPosition 
                vesselSingleStatisticSource 
            |> Option.get
        islandSource()
        |> List.filter
            (fun location -> 
                ((avatarPosition |> Location.DistanceTo location)<=viewDistance))
        |> List.iter
            (fun location ->
                avatarIslandSingleMetricSink world.AvatarId location AvatarIslandMetricIdentifier.Seen 1UL)

    let Create 
            (avatarIslandSingleMetricSink    : AvatarIslandSingleMetricSink)
            (avatarJobSink                   : AvatarJobSink)
            (islandSingleNameSink            : IslandSingleNameSink)
            (islandSingleStatisticSink       : IslandSingleStatisticSink)
            (islandSource                    : IslandSource)
            (islandStatisticTemplateSource   : IslandStatisticTemplateSource)
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
        Avatar.Create 
            avatarJobSink
            rationItemSource
            shipmateRationItemSink
            shipmateSingleStatisticSink
            shipmateStatisticTemplateSource
            vesselStatisticSink
            vesselStatisticTemplateSource
            avatarId
        GenerateIslands 
            islandSingleNameSink
            islandSingleStatisticSink
            islandSource
            islandStatisticTemplateSource
            nameSource 
            worldSize 
            minimumIslandDistance
            random 
            (maximumGenerationRetries, 0u)
        let world =
            {
                AvatarId = avatarId
            }
        world
        |> UpdateCharts 
            avatarIslandSingleMetricSink
            islandSource
            vesselSingleStatisticSource
        world

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
            (avatarIslandSingleMetricSink  : AvatarIslandSingleMetricSink)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (islandSource                  : IslandSource)
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (distance                      : uint32) 
            (world                         : World) 
            : World =
        let avatarId = world.AvatarId
        match distance with
        | x when x > 0u ->
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
            world
            |> UpdateCharts 
                avatarIslandSingleMetricSink
                islandSource
                vesselSingleStatisticSource
            if IsAvatarAlive 
                    shipmateSingleStatisticSource 
                    world |> not then
                world
                |> AddMessages avatarMessageSink [ "You die of old age!" ]
                world
            else
                Move
                    avatarInventorySink
                    avatarInventorySource
                    avatarIslandSingleMetricSink
                    avatarMessageSink 
                    avatarShipmateSource
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    islandSource
                    shipmateRationItemSource 
                    shipmateSingleStatisticSink
                    shipmateSingleStatisticSource
                    vesselSingleStatisticSink 
                    vesselSingleStatisticSource 
                    (x-1u) 
                    world
        | _ -> 
            world

    let GetNearbyLocations
            (islandSource                : IslandSource)
            (from                        : Location) 
            (maximumDistance             : float) 
            : Location list =
        islandSource()
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)


    let private DoJobCompletion
            (avatarJobSink                 : AvatarJobSink)
            (avatarJobSource               : AvatarJobSource)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (location                      : Location) 
            (job                           : Job) 
            (world                         : World) 
            : unit = 
        if location = job.Destination then
            Avatar.CompleteJob 
                avatarJobSink
                avatarJobSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateSingleStatisticSink 
                shipmateSingleStatisticSource 
                world.AvatarId
            world
            |> AddMessages avatarMessageSink [ "You complete your job." ]

    let Dock
            (avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarJobSink                  : AvatarJobSink)
            (avatarJobSource                : AvatarJobSource)
            (avatarMessageSink              : AvatarMessageSink)
            (avatarSingleMetricSink         : AvatarSingleMetricSink)
            (avatarSingleMetricSource       : AvatarSingleMetricSource)
            (commoditySource                : CommoditySource) 
            (islandItemSink                 : IslandItemSink) 
            (islandItemSource               : IslandItemSource) 
            (islandJobSink                  : IslandJobSink)
            (islandJobSource                : IslandJobSource)
            (islandMarketSink               : IslandMarketSink) 
            (islandMarketSource             : IslandMarketSource) 
            (islandSource                   : IslandSource)
            (itemSource                     : ItemSource) 
            (shipmateSingleStatisticSink    : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource  : ShipmateSingleStatisticSource)
            (termSources                    : TermSources)
            (worldSingleStatisticSource     : WorldSingleStatisticSource)
            (random                         : Random) 
            (location                       : Location) 
            (world                          : World) 
            : unit =
        let avatarId = world.AvatarId
        let locations = islandSource()
        match locations |> List.tryFind (fun x -> x = location) with
        | Some l ->
            let destinations =
                locations
                |> Set.ofList
                |> Set.remove location
            let oldVisitCount =
                avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
                |> Option.defaultValue 0UL
            Island.AddVisit
                avatarIslandSingleMetricSink
                avatarIslandSingleMetricSource
                (DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64)
                avatarId
                location
            let newVisitCount =
                avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
                |> Option.defaultValue 0UL
            l
            |> Island.GenerateJobs 
                islandJobSink
                islandJobSource
                termSources 
                worldSingleStatisticSource 
                random 
                destinations 
            Island.GenerateCommodities commoditySource islandMarketSource islandMarketSink random location
            Island.GenerateItems islandItemSource islandItemSink random itemSource location
            world
            |> AddMessages (avatarMessageSink : AvatarMessageSink) [ "You dock." ]
            avatarId
            |> Avatar.AddMetric 
                avatarSingleMetricSink
                avatarSingleMetricSource
                Metric.VisitedIsland 
                (if newVisitCount > oldVisitCount then 1UL else 0UL)
            world
            |> Option.foldBack 
                (fun job w ->
                    DoJobCompletion
                        avatarJobSink
                        avatarJobSource
                        avatarMessageSink
                        avatarSingleMetricSink
                        avatarSingleMetricSource
                        shipmateSingleStatisticSink 
                        shipmateSingleStatisticSource 
                        location
                        job
                        w
                    w) (avatarJobSource avatarId)
            |> ignore
        | _ -> 
            world
            |> AddMessages (avatarMessageSink : AvatarMessageSink) [ "There is no place to dock there." ]

    let DistanceTo 
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarMessageSink              : AvatarMessageSink)
            (islandLocationByNameSource     : IslandLocationByNameSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (islandName                     : string) 
            (world                          : World) 
            : unit =
        let location =
            islandLocationByNameSource islandName
            |> Option.bind
                (fun l ->
                    if (avatarIslandSingleMetricSource world.AvatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
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
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarMessageSink              : AvatarMessageSink)
            (islandLocationByNameSource     : IslandLocationByNameSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (vesselSingleStatisticSink      : VesselSingleStatisticSink)
            (islandName                     : string) 
            (world                          : World) 
            : unit =
        let location =
            islandLocationByNameSource islandName
            |> Option.bind
                (fun l ->
                    if (avatarIslandSingleMetricSource world.AvatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
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
            (avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarJobSink                  : AvatarJobSink)
            (avatarJobSource                : AvatarJobSource)
            (avatarMessageSink              : AvatarMessageSink)
            (avatarSingleMetricSink         : AvatarSingleMetricSink)
            (avatarSingleMetricSource       : AvatarSingleMetricSource)
            (islandJobPurger                : IslandJobPurger)
            (islandSingleJobSource          : IslandSingleJobSource)
            (islandSource                   : IslandSource)
            (jobIndex                       : uint32) 
            (location                       : Location) 
            (world                          : World) 
            : unit =
        let avatarId = world.AvatarId
        let locations = islandSource()
        match jobIndex, locations |> List.tryFind (fun x -> x = location), avatarJobSource avatarId with
        | 0u, _, _ ->
            world
            |> AddMessages avatarMessageSink [ "That job is currently unavailable." ]
        | _, Some location, None ->
            match islandSingleJobSource location jobIndex with
            | Some job ->
                world
                |> AddMessages avatarMessageSink [ "You accepted the job!" ]
                avatarId
                |> Avatar.AddMetric 
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    Metric.AcceptedJob 
                    1UL
                avatarJobSink avatarId (job|>Some)
                Island.MakeKnown
                    avatarIslandSingleMetricSink
                    avatarIslandSingleMetricSource
                    avatarId
                    job.Destination
                islandJobPurger location jobIndex
            | _ ->
                world
                |> AddMessages avatarMessageSink [ "That job is currently unavailable." ]
        | _, Some island, Some job ->
            world
            |> AddMessages avatarMessageSink [ "You must complete or abandon your current job before taking on a new one." ]
        | _ -> 
            ()

    let AbandonJob
            (avatarJobSink                 : AvatarJobSink)
            (avatarJobSource               : AvatarJobSource)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (world                         : World) 
            : World =
        let avatarId = world.AvatarId
        match avatarJobSource avatarId with
        | Some _ ->
            world
            |> AddMessages avatarMessageSink [ "You abandon your job." ]
            Avatar.AbandonJob 
                avatarJobSink
                avatarJobSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateSingleStatisticSink 
                shipmateSingleStatisticSource 
                world.AvatarId
            world
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
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarMessageSink             : AvatarMessageSink)
            (commoditySource               : CommoditySource) 
            (islandMarketSource            : IslandMarketSource)
            (islandSingleMarketSink        : IslandSingleMarketSink) 
            (islandSingleMarketSource      : IslandSingleMarketSource) 
            (islandSource                  : IslandSource)
            (itemSource                    : ItemSource) 
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (world                         : World) 
            : unit =
        let avatarId = world.AvatarId
        let items = itemSource()
        match items |> FindItemByName itemName, islandSource() |> List.tryFind (fun x-> x = location) with
        | Some (item, descriptor) , Some _ ->
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
            elif usedTonnage + tonnageNeeded > availableTonnage then
                world
                |> AddMessages avatarMessageSink ["You don't have enough tonnage."]
            elif quantity = 0UL then
                world
                |> AddMessages avatarMessageSink ["You don't have enough money to buy any of those."]
            else
                Island.UpdateMarketForItemSale islandSingleMarketSource islandSingleMarketSink commoditySource descriptor quantity location
                world
                |> AddMessages avatarMessageSink [(quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s."]
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
        | None, Some _ ->
            world
            |> AddMessages avatarMessageSink ["Round these parts, we don't sell things like that."]
        | _ ->
            world
            |> AddMessages avatarMessageSink ["You cannot buy items here."]

    let SellItems 
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarMessageSink             : AvatarMessageSink)
            (commoditySource               : CommoditySource)
            (islandMarketSource            : IslandMarketSource) 
            (islandSingleMarketSink        : IslandSingleMarketSink) 
            (islandSingleMarketSource      : IslandSingleMarketSource) 
            (islandSource                  : IslandSource)
            (itemSource                    : ItemSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (world                         : World) 
            : World =
        let avatarId = world.AvatarId
        let items = itemSource()
        match items |> FindItemByName itemName, islandSource()|> List.tryFind ((=)location) with
        | Some (item, descriptor), Some _ ->
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
                world
        | None, Some island ->
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
            
