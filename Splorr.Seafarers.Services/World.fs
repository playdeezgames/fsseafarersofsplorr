namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TradeQuantity =
    | Maximum
    | Specific of uint64

type AvatarMessagePurger = string -> unit
type IslandLocationByNameSource = string -> Location option
type IslandSource = unit -> Location list
type IslandFeatureGeneratorSource = unit -> Map<IslandFeatureIdentifier, IslandFeatureGenerator>
type IslandSingleFeatureSink = Location -> IslandFeatureIdentifier -> unit

type WorldUndockContext = 
    abstract member avatarMessageSink       : AvatarMessageSink
    abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink

type WorldGenerateIslandNamesContext =
    inherit UtilitySortListRandomlyContext

type WorldNameIslandsContext =
    inherit WorldGenerateIslandNamesContext
    abstract member islandSingleNameSink : IslandSingleNameSink
    abstract member islandSource         : IslandSource
    abstract member nameSource           : TermSource

type WorldPopulateIslandsContext =
    inherit IslandFeatureGeneratorGenerateContext   
    abstract member islandFeatureGeneratorSource : IslandFeatureGeneratorSource
    abstract member islandSingleFeatureSink      : IslandSingleFeatureSink
    abstract member islandSource                 : IslandSource

type WorldGenerateIslandsContext =
    inherit IslandCreateContext
    inherit WorldPopulateIslandsContext
    inherit WorldGenerateIslandNamesContext
    inherit WorldNameIslandsContext
    abstract member islandSingleNameSink          : IslandSingleNameSink
    abstract member termNameSource                : TermSource

type WorldCreateContext =
    inherit WorldGenerateIslandsContext
    abstract member avatarIslandSingleMetricSink    : AvatarIslandSingleMetricSink
    abstract member avatarJobSink                   : AvatarJobSink
    abstract member worldSingleStatisticSource      : WorldSingleStatisticSource
    abstract member shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource
    abstract member shipmateSingleStatisticSink     : ShipmateSingleStatisticSink
    abstract member rationItemSource                : RationItemSource
    abstract member vesselStatisticTemplateSource   : VesselStatisticTemplateSource
    abstract member vesselStatisticSink             : VesselStatisticSink
    abstract member vesselSingleStatisticSource     : VesselSingleStatisticSource
    abstract member shipmateRationItemSink          : ShipmateRationItemSink


type WorldDockContext =
    inherit IslandJobsGenerationContext
    abstract member avatarIslandFeatureSink        : AvatarIslandFeatureSink
    abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member avatarJobSink                  : AvatarJobSink
    abstract member avatarJobSource                : AvatarJobSource
    abstract member avatarMessageSink              : AvatarMessageSink
    abstract member avatarSingleMetricSink         : AvatarSingleMetricSink
    abstract member avatarSingleMetricSource       : AvatarSingleMetricSource
    abstract member commoditySource                : CommoditySource 
    abstract member islandItemSink                 : IslandItemSink 
    abstract member islandItemSource               : IslandItemSource 
    abstract member islandMarketSink               : IslandMarketSink 
    abstract member islandMarketSource             : IslandMarketSource 
    abstract member islandSource                   : IslandSource
    abstract member itemSource                     : ItemSource 
    abstract member shipmateSingleStatisticSink    : ShipmateSingleStatisticSink
    abstract member shipmateSingleStatisticSource  : ShipmateSingleStatisticSource

module World =
//TODO: top of "world generator" refactor
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
            (context : WorldGenerateIslandNamesContext) 
            (nameCount:int) 
            (names: Set<string>) 
            : List<string> =
        if names.Count>=nameCount then
            names
            |> Set.toList
            |> Utility.SortListRandomly context
            |> List.take nameCount
        else
            names
            |> Set.add (GenerateIslandName context.random)
            |> GenerateIslandNames context nameCount

    let private NameIslands  //TODO: move to world generator?
            (context: WorldNameIslandsContext)
            : unit =
        let locations = 
            context.islandSource()
        GenerateIslandNames 
            context 
            (locations.Length) 
            (context.nameSource() |> Set.ofList)
        |> Utility.SortListRandomly context
        |> List.zip (locations)
        |> List.iter
            (fun (l,n) -> 
                context.islandSingleNameSink l (Some n))

    let private PopulateIslands
            (context : WorldPopulateIslandsContext)
            : unit =
        let generators = context.islandFeatureGeneratorSource()
        context.islandSource()
        |> List.iter
            (fun location -> 
                generators
                |> Map.iter
                    (fun identifier generator ->
                        if IslandFeatureGenerator.Generate context generator then
                            context.islandSingleFeatureSink location identifier))

    let rec private GenerateIslands  //TODO: move to world generator?
            (context                : WorldGenerateIslandsContext)
            (worldSize              : Location) 
            (minimumIslandDistance  : float)
            (maximumGenerationTries : uint32, 
             currentTry             : uint32) 
            : unit =
        if currentTry>=maximumGenerationTries then
            NameIslands 
                context
            PopulateIslands
                context
                
        else
            let locations = context.islandSource()
            let candidateLocation = (context.random.NextDouble() * (worldSize |> fst), context.random.NextDouble() * (worldSize |> snd))
            if locations |> List.exists(fun k ->(Location.DistanceTo candidateLocation k) < minimumIslandDistance) then
                GenerateIslands 
                    context 
                    worldSize 
                    minimumIslandDistance 
                    (maximumGenerationTries, currentTry+1u) 
            else
                Island.Create
                    context
                    candidateLocation
                GenerateIslands 
                    context 
                    worldSize 
                    minimumIslandDistance 
                    (maximumGenerationTries, 0u) 
//end of "world generator"
    let UpdateCharts 
            (avatarIslandSingleMetricSink : AvatarIslandSingleMetricSink)
            (islandSource                 : IslandSource)
            (vesselSingleStatisticSource  : VesselSingleStatisticSource)
            (avatarId                     : string) 
            : unit =
        let viewDistance = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition = 
            avatarId 
            |> Avatar.GetPosition 
                vesselSingleStatisticSource 
            |> Option.get
        islandSource()
        |> List.filter
            (fun location -> 
                ((avatarPosition |> Location.DistanceTo location)<=viewDistance))
        |> List.iter
            (fun location ->
                avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.Seen 1UL)

    let Create 
            (context  : WorldCreateContext)
            (random   : Random) 
            (avatarId : string)
            : unit =
        let maximumGenerationRetries =
            WorldStatisticIdentifier.IslandGenerationRetries
            |> context.worldSingleStatisticSource 
            |> Statistic.GetCurrentValue
            |> uint
        let minimumIslandDistance = 
            WorldStatisticIdentifier.IslandDistance
            |> context.worldSingleStatisticSource 
            |> Statistic.GetCurrentValue
        let worldSize =
            (WorldStatisticIdentifier.PositionX
            |> context.worldSingleStatisticSource 
            |> Statistic.GetMaximumValue,
                WorldStatisticIdentifier.PositionY
                |> context.worldSingleStatisticSource 
                |> Statistic.GetMaximumValue)
        Avatar.Create 
            context.avatarJobSink
            context.rationItemSource
            context.shipmateRationItemSink
            context.shipmateSingleStatisticSink
            context.shipmateStatisticTemplateSource
            context.vesselStatisticSink
            context.vesselStatisticTemplateSource
            avatarId
        GenerateIslands 
            context 
            worldSize 
            minimumIslandDistance
            (maximumGenerationRetries, 0u)
        avatarId
        |> UpdateCharts 
            context.avatarIslandSingleMetricSink
            context.islandSource
            context.vesselSingleStatisticSource

    let ClearMessages 
            (avatarMessagePurger : AvatarMessagePurger) 
            (avatarId            : string)
            : unit =
        avatarMessagePurger avatarId

    let AddMessages
            (avatarMessageSink : AvatarMessageSink)
            (messages          : string list) 
            (avatarId          : string) 
            : unit =
        Avatar.AddMessages avatarMessageSink messages avatarId

    let SetSpeed 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarMessageSink           : AvatarMessageSink)
            (speed                       : float) 
            (avatarId                    : string) 
            : unit = 
        avatarId
        |> Avatar.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink speed 
        avatarId
        |> Avatar.GetSpeed vesselSingleStatisticSource
        |> Option.iter
            (fun newSpeed ->
                avatarId
                |> AddMessages avatarMessageSink [newSpeed |> sprintf "You set your speed to %.2f."])

    let SetHeading 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarMessageSink           : AvatarMessageSink)
            (heading                     : float) 
            (avatarId                    : string) 
            : unit =
        avatarId
        |> Avatar.SetHeading vesselSingleStatisticSource vesselSingleStatisticSink heading 
        avatarId
        |> Avatar.GetHeading vesselSingleStatisticSource
        |> Option.iter
            (fun newHeading ->
                avatarId
                |> AddMessages avatarMessageSink [newHeading |> Angle.ToDegrees |> Angle.ToString |> sprintf "You set your heading to %s." ])

    let IsAvatarAlive
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (avatarId                      : string) 
            : bool =
        (Shipmate.GetStatus 
            shipmateSingleStatisticSource
            avatarId
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
            (avatarId                      : string) 
            : unit =
        match distance with
        | x when x > 0u ->
            avatarId
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
            avatarId
            |> UpdateCharts 
                avatarIslandSingleMetricSink
                islandSource
                vesselSingleStatisticSource
            if IsAvatarAlive 
                    shipmateSingleStatisticSource 
                    avatarId |> not then
                avatarId
                |> AddMessages avatarMessageSink [ "You die of old age!" ]
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
                    avatarId
        | _ -> 
            ()

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
            (avatarId                      : string) 
            : unit = 
        if location = job.Destination then
            Avatar.CompleteJob 
                avatarJobSink
                avatarJobSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateSingleStatisticSink 
                shipmateSingleStatisticSource 
                avatarId
            avatarId
            |> AddMessages avatarMessageSink [ "You complete your job." ]

    let Dock
            (context  : WorldDockContext)
            (random   : Random) 
            (location : Location) 
            (avatarId : string) 
            : unit =
        let locations = context.islandSource()
        match locations |> List.tryFind (fun x -> x = location) with
        | Some l ->
            let destinations =
                locations
                |> Set.ofList
                |> Set.remove location
            let oldVisitCount =
                context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
                |> Option.defaultValue 0UL
            Island.AddVisit
                context.avatarIslandSingleMetricSink
                context.avatarIslandSingleMetricSource
                (DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64)
                avatarId
                location
            let newVisitCount =
                context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
                |> Option.defaultValue 0UL
            l
            |> Island.GenerateJobs 
                context 
                destinations 
            Island.GenerateCommodities 
                context.commoditySource 
                context.islandMarketSource 
                context.islandMarketSink 
                random 
                location
            Island.GenerateItems 
                context.islandItemSource 
                context.islandItemSink 
                random 
                context.itemSource location
            avatarId
            |> AddMessages 
                context.avatarMessageSink 
                [ 
                    "You dock." 
                ]
            avatarId
            |> Avatar.AddMetric 
                context.avatarSingleMetricSink
                context.avatarSingleMetricSource
                Metric.VisitedIsland 
                (if newVisitCount > oldVisitCount then 1UL else 0UL)
            avatarId
            |> Option.foldBack 
                (fun job w ->
                    DoJobCompletion
                        context.avatarJobSink
                        context.avatarJobSource
                        context.avatarMessageSink
                        context.avatarSingleMetricSink
                        context.avatarSingleMetricSource
                        context.shipmateSingleStatisticSink 
                        context.shipmateSingleStatisticSource 
                        location
                        job
                        w
                    w) (context.avatarJobSource avatarId)
            |> ignore
            context.avatarIslandFeatureSink (IslandFeatureIdentifier.Dock |> Some, avatarId)
        | _ -> 
            avatarId
            |> AddMessages 
                context.avatarMessageSink 
                [ 
                    "There is no place to dock there." 
                ]

    let DistanceTo 
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarMessageSink              : AvatarMessageSink)
            (islandLocationByNameSource     : IslandLocationByNameSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (islandName                     : string) 
            (avatarId                       : string) 
            : unit =
        let location =
            islandLocationByNameSource islandName
            |> Option.bind
                (fun l ->
                    if (avatarIslandSingleMetricSource avatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
                    else
                        None)
        match location, Avatar.GetPosition vesselSingleStatisticSource avatarId with
        | Some l, Some avatarPosition ->
            avatarId
            |> AddMessages avatarMessageSink [ (islandName, Location.DistanceTo l avatarPosition ) ||> sprintf "Distance to `%s` is %f." ]
        | _, Some _ ->
            avatarId
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
            (avatarId                       : string) 
            : unit =
        let location =
            islandLocationByNameSource islandName
            |> Option.bind
                (fun l ->
                    if (avatarIslandSingleMetricSource avatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
                    else
                        None)
        match location, Avatar.GetPosition vesselSingleStatisticSource avatarId with
        | Some l, Some avatarPosition ->
            [
                AddMessages avatarMessageSink [ islandName |> sprintf "You head for `%s`." ]
                SetHeading vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink (Location.HeadingTo avatarPosition l |> Angle.ToDegrees)
            ]
            |> List.iter (fun f -> f avatarId)
        | _, Some _ ->
            avatarId
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
            (avatarId                       : string) 
            : unit =
        let locations = islandSource()
        match jobIndex, locations |> List.tryFind (fun x -> x = location), avatarJobSource avatarId with
        | 0u, _, _ ->
            avatarId
            |> AddMessages avatarMessageSink [ "That job is currently unavailable." ]
        | _, Some location, None ->
            match islandSingleJobSource location jobIndex with
            | Some job ->
                avatarId
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
                avatarId
                |> AddMessages avatarMessageSink [ "That job is currently unavailable." ]
        | _, Some island, Some job ->
            avatarId
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
            (avatarId                      : string) 
            : unit =
        match avatarJobSource avatarId with
        | Some _ ->
            avatarId
            |> AddMessages avatarMessageSink [ "You abandon your job." ]
            Avatar.AbandonJob 
                avatarJobSink
                avatarJobSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateSingleStatisticSink 
                shipmateSingleStatisticSource 
                avatarId
        | _ ->
            avatarId
            |> AddMessages avatarMessageSink [ "You have no job to abandon." ]
    
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
            (avatarId                      : string) 
            : unit =
        let items = itemSource()
        match items |> FindItemByName itemName, islandSource() |> List.tryFind (fun x-> x = location) with
        | Some (item, descriptor) , Some _ ->
            let markets =
                islandMarketSource location
            let unitPrice = 
                Item.DetermineSalePrice commoditySource markets descriptor 
            let availableTonnage = 
                vesselSingleStatisticSource 
                    avatarId 
                    VesselStatisticIdentifier.Tonnage 
                |> Option.map 
                    Statistic.GetCurrentValue 
                |> Option.get
            let usedTonnage =
                avatarId
                |> Avatar.GetUsedTonnage
                    avatarInventorySource
                    items
            let quantity =
                match tradeQuantity with
                | Specific amount -> amount
                | Maximum -> min (floor(availableTonnage / descriptor.Tonnage)) (floor((avatarId |> Avatar.GetMoney shipmateSingleStatisticSource) / unitPrice)) |> uint64
            let price = (quantity |> float) * unitPrice
            let tonnageNeeded = (quantity |> float) * descriptor.Tonnage
            if price > (avatarId |> Avatar.GetMoney shipmateSingleStatisticSource) then
                avatarId
                |> AddMessages avatarMessageSink ["You don't have enough money."]
            elif usedTonnage + tonnageNeeded > availableTonnage then
                avatarId
                |> AddMessages avatarMessageSink ["You don't have enough tonnage."]
            elif quantity = 0UL then
                avatarId
                |> AddMessages avatarMessageSink ["You don't have enough money to buy any of those."]
            else
                Island.UpdateMarketForItemSale islandSingleMarketSource islandSingleMarketSink commoditySource descriptor quantity location
                avatarId
                |> AddMessages avatarMessageSink [(quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s."]
                avatarId
                |> Avatar.SpendMoney 
                    shipmateSingleStatisticSink
                    shipmateSingleStatisticSource
                    price 
                avatarId
                |> Avatar.AddInventory 
                    avatarInventorySink
                    avatarInventorySource
                    item 
                    quantity
        | None, Some _ ->
            avatarId
            |> AddMessages avatarMessageSink ["Round these parts, we don't sell things like that."]
        | _ ->
            avatarId
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
            (avatarId                      : string) 
            : unit =
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
                avatarId
                |> AddMessages avatarMessageSink ["You don't have enough of those to sell."]
            elif quantity = 0UL then
                avatarId
                |> AddMessages avatarMessageSink ["You don't have any of those to sell."]
            else
                let markets = islandMarketSource location
                let unitPrice = 
                    Item.DeterminePurchasePrice commoditySource markets descriptor 
                let price = (quantity |> float) * unitPrice
                Island.UpdateMarketForItemPurchase islandSingleMarketSource islandSingleMarketSink commoditySource descriptor quantity location
                avatarId
                |> AddMessages avatarMessageSink [(quantity, descriptor.ItemName) ||> sprintf "You complete the sale of %u %s."]
                Avatar.EarnMoney 
                    shipmateSingleStatisticSink
                    shipmateSingleStatisticSource
                    price 
                    avatarId
                avatarId
                |> Avatar.RemoveInventory 
                    avatarInventorySource
                    avatarInventorySink
                    item 
                    quantity 
        | None, Some island ->
            avatarId
            |> AddMessages avatarMessageSink ["Round these parts, we don't buy things like that."]
        | _ ->
            avatarId
            |> AddMessages avatarMessageSink ["You cannot sell items here."]

    let CleanHull //TODO: this just passes everything along to avatar.CleanHull, so eliminate
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (side                          : Side) 
            (avatarId                      : string) 
            : unit =
        avatarId 
        |> Avatar.CleanHull 
            avatarShipmateSource
            avatarSingleMetricSink
            avatarSingleMetricSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSink 
            vesselSingleStatisticSource
            side 

    let Undock
            (context : WorldUndockContext)
            (avatarId : string)
            : unit =
        avatarId
        |> AddMessages  context.avatarMessageSink [ "You undock." ]
        context.avatarIslandFeatureSink (None, avatarId)
        
            
