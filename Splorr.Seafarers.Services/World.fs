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

type WorldGenerateIslandNameContext =
    inherit OperatingContext
    abstract member random : Random

type WorldAddMessagesContext = 
    inherit OperatingContext
    abstract member avatarMessageSink : AvatarMessageSink

type WorldUndockContext = 
    inherit OperatingContext
    abstract member avatarMessageSink       : AvatarMessageSink
    abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink

type WorldClearMessagesContext =
    inherit OperatingContext
    abstract member avatarMessagePurger : AvatarMessagePurger

type WorldGenerateIslandNamesContext =
    inherit OperatingContext

type WorldNameIslandsContext =
    inherit OperatingContext
    abstract member islandSingleNameSink : IslandSingleNameSink
    abstract member islandSource         : IslandSource
    abstract member nameSource           : TermSource

type WorldPopulateIslandsContext =
    inherit OperatingContext
    abstract member islandFeatureGeneratorSource : IslandFeatureGeneratorSource
    abstract member islandSingleFeatureSink      : IslandSingleFeatureSink
    abstract member islandSource                 : IslandSource

type WorldGenerateIslandsContext =
    inherit OperatingContext
    abstract member islandSingleNameSink          : IslandSingleNameSink
    abstract member termNameSource                : TermSource
    abstract member islandSource : IslandSource
    abstract member random : Random

type WorldUpdateChartsContext = 
    inherit OperatingContext
    abstract member avatarIslandSingleMetricSink : AvatarIslandSingleMetricSink
    abstract member islandSource                 : IslandSource
    abstract member vesselSingleStatisticSource  : VesselSingleStatisticSource

type WorldCreateContext =
    inherit OperatingContext
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

type WorldCleanHullContext =
    inherit OperatingContext

type WorldIsAvatarAliveContext = 
    inherit OperatingContext

type WorldGetNearbyLocationsContext =
    inherit OperatingContext
    abstract member islandSource : IslandSource

type WorldDoJobCompletionContext =
    inherit OperatingContext

type WorldDockContext =
    inherit OperatingContext
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

type WorldAcceptJobContext =
    inherit OperatingContext
    abstract member avatarJobSink         : AvatarJobSink
    abstract member avatarJobSource       : AvatarJobSource
    abstract member islandJobPurger       : IslandJobPurger
    abstract member islandSingleJobSource : IslandSingleJobSource
    abstract member islandSource          : IslandSource

type WorldBuyItemsContext =
    inherit OperatingContext
    abstract member islandSource                  : IslandSource
    abstract member itemSource                    : ItemSource
    abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource

type WorldSellItemsContext =
    inherit OperatingContext
    abstract member islandSource                  : IslandSource
    abstract member itemSource                    : ItemSource

type WorldAbandonJobContext =
    inherit OperatingContext
    abstract member avatarJobSource  : AvatarJobSource

type WorldSetSpeedContext =
    inherit OperatingContext

type WorldSetHeadingContext =
    inherit OperatingContext

type WorldMoveContext =
    inherit OperatingContext

type WorldDistanceToContext =
    inherit OperatingContext
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member islandLocationByNameSource     : IslandLocationByNameSource

type WorldHeadForContext =
    inherit OperatingContext
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member islandLocationByNameSource     : IslandLocationByNameSource

type WorldHasDarkAlleyMinimumStakesContext =
    inherit OperatingContext
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    abstract member islandSingleStatisticSource : IslandSingleStatisticSource 
    abstract member avatarIslandFeatureSource : AvatarIslandFeatureSource

module World =
    let private GenerateIslandName
            (context: OperatingContext)
            : string =
        let context = context :?> WorldGenerateIslandNameContext
        let consonants = [| "h"; "k"; "l"; "m"; "p" |]
        let vowels = [| "a"; "e"; "i"; "o"; "u" |]
        let vowel = context.random.Next(2)>0
        let nameLength = context.random.Next(3) + context.random.Next(3) + context.random.Next(3) + 3
        [1..(nameLength)]
        |> List.map 
            (fun i -> i % 2 = (if vowel then 1 else 0))
        |> List.map
            (fun v -> 
                if v then
                    vowels.[context.random.Next(vowels.Length)]
                else
                    consonants.[context.random.Next(consonants.Length)])
        |> List.reduce (+)

    let rec private GenerateIslandNames  //TODO: move to world generator?
            (context : OperatingContext) 
            (nameCount:int) 
            (names: Set<string>) 
            : List<string> =
        let context = context :?> WorldGenerateIslandNamesContext
        if names.Count>=nameCount then
            names
            |> Set.toList
            |> Utility.SortListRandomly context
            |> List.take nameCount
        else
            names
            |> Set.add (GenerateIslandName context)
            |> GenerateIslandNames context nameCount

    let private NameIslands
            (context: OperatingContext)
            : unit =
        let context = context :?> WorldNameIslandsContext
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
            (context : OperatingContext)
            : unit =
        let context = context :?> WorldPopulateIslandsContext
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
            (context                : OperatingContext)
            (worldSize              : Location) 
            (minimumIslandDistance  : float)
            (maximumGenerationTries : uint32, 
             currentTry             : uint32) 
            : unit =
        let context = context :?> WorldGenerateIslandsContext
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

    let UpdateCharts 
            (context : OperatingContext)
            (avatarId : string) 
            : unit =
        let context = context :?> WorldUpdateChartsContext
        let viewDistance = 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition = 
            avatarId 
            |> Avatar.GetPosition 
                context
            |> Option.get
        context.islandSource()
        |> List.filter
            (fun location -> 
                ((avatarPosition |> Location.DistanceTo location)<=viewDistance))
        |> List.iter
            (fun location ->
                context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.Seen 1UL)

    let Create 
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> WorldCreateContext
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
            context
            avatarId
        GenerateIslands 
            context 
            worldSize 
            minimumIslandDistance
            (maximumGenerationRetries, 0u)
        avatarId
        |> UpdateCharts 
            context

    let ClearMessages 
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> WorldClearMessagesContext
        context.avatarMessagePurger avatarId

    let AddMessages
            (context : OperatingContext)
            (messages          : string list) 
            (avatarId          : string) 
            : unit =
        let context = context :?> WorldAddMessagesContext
        Avatar.AddMessages
            context
            messages 
            avatarId

    let SetSpeed 
            (context  : OperatingContext)
            (speed    : float) 
            (avatarId : string) 
            : unit = 
        let context = context :?> WorldSetSpeedContext
        avatarId
        |> Avatar.SetSpeed 
            context
            speed 
        avatarId
        |> Avatar.GetSpeed 
            context
        |> Option.iter
            (fun newSpeed ->
                avatarId
                |> AddMessages context [newSpeed |> sprintf "You set your speed to %.2f."])

    let SetHeading 
            (context  : OperatingContext)
            (heading  : float) 
            (avatarId : string) 
            : unit =
        let context = context :?> WorldSetHeadingContext
        avatarId
        |> Avatar.SetHeading 
            context
            heading 
        avatarId
        |> Avatar.GetHeading 
            context
        |> Option.iter
            (fun newHeading ->
                avatarId
                |> AddMessages context [newHeading |> Angle.ToDegrees |> Angle.ToString |> sprintf "You set your heading to %s." ])

    let IsAvatarAlive
            (context  : OperatingContext)
            (avatarId : string) 
            : bool =
        let context = context :?> WorldIsAvatarAliveContext
        (Shipmate.GetStatus 
            context
            avatarId
            Primary) = Alive

    let rec Move
            (context  : OperatingContext)
            (distance : uint32) 
            (avatarId : string) 
            : unit =
        let context = context :?> WorldMoveContext
        match distance with
        | x when x > 0u ->
            avatarId
            |> AddMessages context [ "Steady as she goes." ]
            Avatar.Move 
                context
                avatarId 
            avatarId
            |> UpdateCharts 
                context
            if IsAvatarAlive 
                    context
                    avatarId |> not then
                avatarId
                |> AddMessages context [ "You die of old age!" ]
            else
                Move
                    context
                    (x-1u) 
                    avatarId
        | _ -> 
            ()

    let GetNearbyLocations
            (context : OperatingContext)
            (from                        : Location) 
            (maximumDistance             : float) 
            : Location list =
        let context = context :?> WorldGetNearbyLocationsContext
        context.islandSource()
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)


    let private DoJobCompletion
            (context  : OperatingContext)
            (location : Location) 
            (job      : Job) 
            (avatarId : string) 
            : unit = 
        let context = context :?> WorldDoJobCompletionContext
        if location = job.Destination then
            Avatar.CompleteJob 
                context
                avatarId
            avatarId
            |> AddMessages context  [ "You complete your job." ]

    let Dock
            (context  : OperatingContext)
            (location : Location) 
            (avatarId : string) 
            : unit =
        let context = context :?> WorldDockContext
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
                context
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
                context
                location
            Island.GenerateItems 
                context
                location
            avatarId
            |> AddMessages 
                context
                [ 
                    "You dock." 
                ]
            avatarId
            |> Avatar.AddMetric 
                context
                Metric.VisitedIsland 
                (if newVisitCount > oldVisitCount then 1UL else 0UL)
            avatarId
            |> Option.foldBack 
                (fun job w ->
                    DoJobCompletion
                        context
                        location
                        job
                        w
                    w) (context.avatarJobSource avatarId)
            |> ignore
            context.avatarIslandFeatureSink 
                ({
                    featureId = IslandFeatureIdentifier.Dock
                    location = location
                } 
                |> Some, 
                    avatarId)
        | _ -> 
            avatarId
            |> AddMessages 
                context
                [ 
                    "There is no place to dock there." 
                ]

    let DistanceTo 
            (context    : OperatingContext)
            (islandName : string) 
            (avatarId   : string) 
            : unit =
        let context = context :?> WorldDistanceToContext
        let location =
            context.islandLocationByNameSource islandName
            |> Option.bind
                (fun l ->
                    if (context.avatarIslandSingleMetricSource avatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
                    else
                        None)
        match location, Avatar.GetPosition context avatarId with
        | Some l, Some avatarPosition ->
            avatarId
            |> AddMessages context [ (islandName, Location.DistanceTo l avatarPosition ) ||> sprintf "Distance to `%s` is %f." ]
        | _, Some _ ->
            avatarId
            |> AddMessages context [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    let HeadFor
            (context : OperatingContext)
            (islandName                     : string) 
            (avatarId                       : string) 
            : unit =
        let context = context :?> WorldHeadForContext
        let location =
            context.islandLocationByNameSource islandName
            |> Option.bind
                (fun l ->
                    if (context.avatarIslandSingleMetricSource avatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
                    else
                        None)
        match location, Avatar.GetPosition context avatarId with
        | Some l, Some avatarPosition ->
            [
                AddMessages
                    context
                    [ islandName |> sprintf "You head for `%s`." ]
                SetHeading
                    context
                    (Location.HeadingTo avatarPosition l |> Angle.ToDegrees)
            ]
            |> List.iter (fun f -> f avatarId)
        | _, Some _ ->
            avatarId
            |> AddMessages context [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    let AcceptJob 
            (context  : OperatingContext)
            (jobIndex : uint32) 
            (location : Location) 
            (avatarId : string) 
            : unit =
        let context = context :?> WorldAcceptJobContext
        let locations = context.islandSource()
        match jobIndex, locations |> List.tryFind (fun x -> x = location), context.avatarJobSource avatarId with
        | 0u, _, _ ->
            avatarId
            |> AddMessages context [ "That job is currently unavailable." ]
        | _, Some location, None ->
            match context.islandSingleJobSource location jobIndex with
            | Some job ->
                avatarId
                |> AddMessages context [ "You accepted the job!" ]
                avatarId
                |> Avatar.AddMetric 
                    context
                    Metric.AcceptedJob 
                    1UL
                context.avatarJobSink avatarId (job|>Some)
                Island.MakeKnown
                    context
                    avatarId
                    job.Destination
                context.islandJobPurger location jobIndex
            | _ ->
                avatarId
                |> AddMessages context [ "That job is currently unavailable." ]
        | _, Some _, Some _ ->
            avatarId
            |> AddMessages context [ "You must complete or abandon your current job before taking on a new one." ]
        | _ -> 
            ()

    let AbandonJob
            (context  : OperatingContext)
            (avatarId : string) 
            : unit =
        let context = context :?> WorldAbandonJobContext
        match context.avatarJobSource avatarId with
        | Some _ ->
            avatarId
            |> AddMessages context [ "You abandon your job." ]
            avatarId
            |> Avatar.AbandonJob 
                context
        | _ ->
            avatarId
            |> AddMessages context [ "You have no job to abandon." ]
    
    //TODO: this function is in the wrong place!
    let private FindItemByName 
            (itemName : string) 
            (items    : Map<uint64, ItemDescriptor>) 
            : (uint64 * ItemDescriptor) option =
        items
        |> Map.tryPick
            (fun itemId descriptor ->
                if descriptor.ItemName = itemName then
                    Some (itemId,descriptor)
                else
                    None)

    let BuyItems 
            (context                       : OperatingContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        let context = context :?> WorldBuyItemsContext
        let items = context.itemSource()
        match items |> FindItemByName itemName, context.islandSource() |> List.tryFind (fun x-> x = location) with
        | Some (item, descriptor) , Some _ ->
            let unitPrice = 
                Item.DetermineSalePrice 
                    context
                    item 
                    location
            let availableTonnage = 
                context.vesselSingleStatisticSource 
                    avatarId 
                    VesselStatisticIdentifier.Tonnage 
                |> Option.map 
                    Statistic.GetCurrentValue 
                |> Option.get
            let usedTonnage =
                avatarId
                |> Avatar.GetUsedTonnage
                    context
                    items
            let quantity =
                match tradeQuantity with
                | Specific amount -> amount
                | Maximum -> min (floor(availableTonnage / descriptor.Tonnage)) (floor((avatarId |> Avatar.GetMoney context) / unitPrice)) |> uint64
            let price = (quantity |> float) * unitPrice
            let tonnageNeeded = (quantity |> float) * descriptor.Tonnage
            if price > (avatarId |> Avatar.GetMoney context) then
                avatarId
                |> AddMessages context ["You don't have enough money."]
            elif usedTonnage + tonnageNeeded > availableTonnage then
                avatarId
                |> AddMessages context ["You don't have enough tonnage."]
            elif quantity = 0UL then
                avatarId
                |> AddMessages context ["You don't have enough money to buy any of those."]
            else
                Island.UpdateMarketForItemSale 
                    context
                    descriptor 
                    quantity 
                    location
                avatarId
                |> AddMessages context [(quantity, descriptor.ItemName) ||> sprintf "You complete the purchase of %u %s."]
                avatarId
                |> Avatar.SpendMoney 
                    context
                    price 
                avatarId
                |> Avatar.AddInventory 
                    context
                    item 
                    quantity
        | None, Some _ ->
            avatarId
            |> AddMessages context ["Round these parts, we don't sell things like that."]
        | _ ->
            avatarId
            |> AddMessages context ["You cannot buy items here."]

    let SellItems 
            (context : OperatingContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        let context = context :?> WorldSellItemsContext
        let items = context.itemSource()
        match items |> FindItemByName itemName, context.islandSource()|> List.tryFind ((=)location) with
        | Some (item, descriptor), Some _ ->
            let quantity = 
                match tradeQuantity with
                | Specific q -> q
                | Maximum -> 
                    avatarId 
                    |> Avatar.GetItemCount context item
            if quantity > (avatarId |> Avatar.GetItemCount context item) then
                avatarId
                |> AddMessages context ["You don't have enough of those to sell."]
            elif quantity = 0UL then
                avatarId
                |> AddMessages context ["You don't have any of those to sell."]
            else
                let unitPrice = 
                    Item.DeterminePurchasePrice 
                        context
                        item 
                        location
                let price = (quantity |> float) * unitPrice
                Island.UpdateMarketForItemPurchase 
                    context
                    descriptor 
                    quantity 
                    location
                avatarId
                |> AddMessages context [(quantity, descriptor.ItemName) ||> sprintf "You complete the sale of %u %s."]
                Avatar.EarnMoney 
                    context
                    price 
                    avatarId
                avatarId
                |> Avatar.RemoveInventory 
                    context
                    item 
                    quantity 
        | None, Some _ ->
            avatarId
            |> AddMessages context ["Round these parts, we don't buy things like that."]
        | _ ->
            avatarId
            |> AddMessages context ["You cannot sell items here."]

    let CleanHull //TODO: this just passes everything along to avatar.CleanHull, so eliminate
            (context : OperatingContext)
            (side                          : Side) 
            (avatarId                      : string) 
            : unit =
        let context = context :?> WorldCleanHullContext
        avatarId 
        |> Avatar.CleanHull 
            context
            side 

    let Undock
            (context : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> WorldUndockContext
        avatarId
        |> AddMessages context [ "You undock." ]
        context.avatarIslandFeatureSink (None, avatarId)
     
    let HasDarkAlleyMinimumStakes
            (context : OperatingContext)
            (location : Location)
            (avatarId : string)
            : bool option =
        let context = context :?> WorldHasDarkAlleyMinimumStakesContext
        match context.avatarIslandFeatureSource avatarId with
        | Some feature when feature.featureId = IslandFeatureIdentifier.DarkAlley ->
            let minimumBet = 
                context.islandSingleStatisticSource 
                    location 
                    IslandStatisticIdentifier.MinimumGamblingStakes
                |> Option.get
                |> Statistic.GetCurrentValue
            let money =
                context.shipmateSingleStatisticSource 
                    avatarId 
                    ShipmateIdentifier.Primary 
                    ShipmateStatisticIdentifier.Money
                |> Option.get
                |> Statistic.GetCurrentValue
            if money >= minimumBet then
                Some true
            else
                Some false
        | _ -> 
            None

    type CanPlaceBetContext =
        inherit OperatingContext
        inherit Avatar.GetPrimaryStatisticContext

    let CanPlaceBet
            (context : OperatingContext)
            (amount : float)
            (avatarId : string)
            : bool =
        let context = context :?> CanPlaceBetContext
        (Avatar.GetMoney context avatarId) >= amount
     
    type ResolveHandContext =
        inherit OperatingContext

    let ResolveHand
            (context: OperatingContext)
            (amount : float)
            (avatarId: string)
            : unit =
        let context = context :?> ResolveHandContext
        match Avatar.GetIslandFeature context avatarId with
        | Some feature when feature.featureId = IslandFeatureIdentifier.DarkAlley ->
            match Avatar.GetGamblingHand context avatarId with
            | Some (first, second, third) ->
                if CanPlaceBet context amount avatarId then
                    let minimumStakes =
                        Island.GetStatistic 
                            context 
                            IslandStatisticIdentifier.MinimumGamblingStakes 
                            feature.location
                        |> Option.get
                        |> Statistic.GetCurrentValue
                    if amount >= minimumStakes then
                        ()
                    else
                        AddMessages context [sprintf "You have to be at least %.2f." minimumStakes] avatarId
                else
                    AddMessages context ["You don't have enough money."] avatarId
            | _ ->
                ()
        | _ ->
            ()
            
