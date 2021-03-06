namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TradeQuantity =
    | Maximum
    | Specific of uint64

type AvatarMessagePurger = string -> unit
type IslandLocationByNameSource = string -> Location option
type IslandFeatureGeneratorSource = unit -> Map<IslandFeatureIdentifier, IslandFeatureGenerator>
type IslandSingleFeatureSink = Location -> IslandFeatureIdentifier -> unit
type WorldSingleStatisticSource = WorldStatisticIdentifier -> Statistic
type GameDataSink = string -> string option

module World =
    type GetStatisticContext =
        inherit ServiceContext
        abstract member worldSingleStatisticSource : WorldSingleStatisticSource
    let GetStatistic
            (context : ServiceContext)
            (identifier : WorldStatisticIdentifier)
            : Statistic =
        (context :?> GetStatisticContext).worldSingleStatisticSource identifier

    let private GenerateIslandName
            (context: ServiceContext)
            : string =
        let consonants = [ "h"; "k"; "l"; "m"; "p" ]
        let vowels = [ "a"; "e"; "i"; "o"; "u" ]
        let vowel = Utility.PickRandomly context [true; false]
        let nameLength =
            Map.empty
            |> Map.add 3 1.0
            |> Map.add 4 3.0
            |> Map.add 5 6.0
            |> Map.add 6 7.0
            |> Map.add 7 6.0
            |> Map.add 8 3.0
            |> Map.add 9 1.0
            |> Utility.WeightedGenerator context
            |> Option.get
        [1..(nameLength)]
        |> List.map 
            (fun i -> i % 2 = (if vowel then 1 else 0))
        |> List.map
            (fun v -> 
                if v then
                    Utility.PickRandomly context vowels
                else
                    Utility.PickRandomly context consonants)
        |> List.reduce (+)

    let rec private GenerateIslandNames  //TODO: move to world generator?
            (context : ServiceContext) 
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
            |> Set.add (GenerateIslandName context)
            |> GenerateIslandNames context nameCount

    type NameIslandsContext =
        inherit ServiceContext
        abstract member islandSingleNameSink : IslandSingleNameSink
        abstract member islandSource         : IslandSource
        abstract member nameSource           : TermSource
    let private NameIslands
            (context: ServiceContext)
            : unit =
        let context = context :?> NameIslandsContext
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

    type PopulateIslandsContext =
        inherit ServiceContext
        abstract member islandFeatureGeneratorSource : IslandFeatureGeneratorSource
        abstract member islandSingleFeatureSink      : IslandSingleFeatureSink
        abstract member islandSource                 : IslandSource
    let private PopulateIslands
            (context : ServiceContext)
            : unit =
        let context = context :?> PopulateIslandsContext
        let generators = context.islandFeatureGeneratorSource()
        context.islandSource()
        |> List.iter
            (fun location -> 
                generators
                |> Map.iter
                    (fun identifier generator ->
                        if IslandFeature.Create context generator then
                            context.islandSingleFeatureSink location identifier))

    type GenerateIslandsContext =
        inherit ServiceContext
        abstract member islandSingleNameSink          : IslandSingleNameSink
        abstract member termNameSource                : TermSource
        abstract member islandSource : IslandSource
    let rec private GenerateIslands  //TODO: move to world generator?
            (context                : ServiceContext)
            (worldSize              : Location) 
            (minimumIslandDistance  : float)
            (maximumGenerationTries : uint32, 
             currentTry             : uint32) 
            : unit =
        let context = context :?> GenerateIslandsContext
        if currentTry>=maximumGenerationTries then
            NameIslands 
                context
            PopulateIslands
                context
        else
            let locations = context.islandSource()
            let candidateLocation = 
                (Utility.RangeGenerator 
                    context 
                    (0.0, (worldSize |> fst)), 
                        Utility.RangeGenerator 
                            context 
                            (0.0,(worldSize |> snd)))
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

    type UpdateChartsContext = 
        inherit ServiceContext
        abstract member avatarIslandSingleMetricSink : AvatarIslandSingleMetricSink
        abstract member islandSource                 : IslandSource
        abstract member vesselSingleStatisticSource  : VesselSingleStatisticSource
    let UpdateCharts 
            (context : ServiceContext)
            (avatarId : string) 
            : unit =
        let context = context :?> UpdateChartsContext
        let viewDistance = 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition = 
            avatarId 
            |> Vessel.GetPosition 
                context
            |> Option.get
        context.islandSource()
        |> List.filter
            (fun location -> 
                ((avatarPosition |> Location.DistanceTo location)<=viewDistance))
        |> List.iter
            (fun location ->
                context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.Seen 1UL)

    type CreateContext =
        inherit ServiceContext
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
    let Create 
            (context  : ServiceContext)
            (avatarId : string)
            : unit =
        let context = context :?> CreateContext
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

    type ClearMessagesContext =
        inherit ServiceContext
        abstract member avatarMessagePurger : AvatarMessagePurger
    let ClearMessages 
            (context  : ServiceContext)
            (avatarId : string)
            : unit =
        let context = context :?> ClearMessagesContext
        context.avatarMessagePurger avatarId

    type AddMessagesContext = 
        inherit ServiceContext
        abstract member avatarMessageSink : AvatarMessageSink
    let AddMessages
            (context : ServiceContext)
            (messages          : string list) 
            (avatarId          : string) 
            : unit =
        let context = context :?> AddMessagesContext
        AvatarMessages.Add
            context
            messages 
            avatarId

    let SetSpeed 
            (context  : ServiceContext)
            (speed    : float) 
            (avatarId : string) 
            : unit = 
        avatarId
        |> Vessel.SetSpeed 
            context
            speed 
        avatarId
        |> Vessel.GetSpeed 
            context
        |> Option.iter
            (fun newSpeed ->
                avatarId
                |> AddMessages context [newSpeed |> sprintf "You set your speed to %.2f."])

    let SetHeading 
            (context  : ServiceContext)
            (heading  : float) 
            (avatarId : string) 
            : unit =
        avatarId
        |> Vessel.SetHeading 
            context
            heading 
        avatarId
        |> Vessel.GetHeading 
            context
        |> Option.iter
            (fun newHeading ->
                avatarId
                |> AddMessages context [newHeading |> Angle.ToDegrees |> Angle.ToString |> sprintf "You set your heading to %s." ])

    let IsAvatarAlive
            (context  : ServiceContext)
            (avatarId : string) 
            : bool =
        (Shipmate.GetStatus 
            context
            avatarId
            Primary) = Alive

    let rec Move
            (context  : ServiceContext)
            (distance : uint32) 
            (avatarId : string) 
            : unit =
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

    type GetNearbyLocationsContext =
        inherit ServiceContext
        abstract member islandSource : IslandSource
    let GetNearbyLocations
            (context : ServiceContext)
            (from                        : Location) 
            (maximumDistance             : float) 
            : Location list =
        let context = context :?> GetNearbyLocationsContext
        context.islandSource()
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)


    let private DoJobCompletion
            (context  : ServiceContext)
            (location : Location) 
            (job      : Job) 
            (avatarId : string) 
            : unit = 
        if location = job.Destination then
            AvatarJob.Complete
                context
                avatarId
            avatarId
            |> AddMessages context  [ "You complete your job." ]

    type DockContext =
        inherit ServiceContext
        abstract member avatarIslandFeatureSink        : AvatarIslandFeatureSink
        abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
        abstract member avatarJobSink                  : AvatarJobSink
        abstract member avatarJobSource                : AvatarJobSource
        abstract member avatarMessageSink              : AvatarMessageSink
        abstract member avatarSingleMetricSink         : AvatarSingleMetricSink
        abstract member avatarSingleMetricSource       : AvatarSingleMetricSource
        abstract member islandItemSink                 : IslandItemSink 
        abstract member islandItemSource               : IslandItemSource 
        abstract member islandMarketSink               : IslandMarketSink 
        abstract member islandMarketSource             : IslandMarketSource 
        abstract member islandSource                   : IslandSource
        abstract member itemSource                     : ItemSource 
        abstract member shipmateSingleStatisticSink    : ShipmateSingleStatisticSink
        abstract member shipmateSingleStatisticSource  : ShipmateSingleStatisticSource
    let Dock
            (context  : ServiceContext)
            (location : Location) 
            (avatarId : string) 
            : unit =
        let context = context :?> DockContext
        let locations = context.islandSource()
        match locations |> List.tryFind (fun x -> x = location) with
        | Some l ->
            let destinations =
                locations
                |> Set.ofList
                |> Set.remove location
            let oldVisitCount =
                AvatarIslandMetric.Get  context avatarId location AvatarIslandMetricIdentifier.VisitCount
                |> Option.defaultValue 0UL
            IslandVisit.Add
                context
                avatarId
                location
            let newVisitCount =
                AvatarIslandMetric.Get context avatarId location AvatarIslandMetricIdentifier.VisitCount
                |> Option.defaultValue 0UL
            l
            |> IslandJob.Generate 
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
            |> AvatarMetric.Add 
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
    type DistanceToContext =
        inherit ServiceContext
        abstract member islandLocationByNameSource     : IslandLocationByNameSource
    let DistanceTo 
            (context    : ServiceContext)
            (islandName : string) 
            (avatarId   : string) 
            : unit =
        let context = context :?> DistanceToContext
        let location =
            context.islandLocationByNameSource islandName
            |> Option.bind
                (fun l ->
                    if (AvatarIslandMetric.Get context avatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
                    else
                        None)
        match location, Vessel.GetPosition context avatarId with
        | Some l, Some avatarPosition ->
            avatarId
            |> AddMessages context [ (islandName, Location.DistanceTo l avatarPosition ) ||> sprintf "Distance to `%s` is %f." ]
        | _, Some _ ->
            avatarId
            |> AddMessages context [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    type HeadForContext =
        inherit ServiceContext
        abstract member islandLocationByNameSource     : IslandLocationByNameSource
    let HeadFor
            (context : ServiceContext)
            (islandName                     : string) 
            (avatarId                       : string) 
            : unit =
        let context = context :?> HeadForContext
        let location =
            context.islandLocationByNameSource islandName
            |> Option.bind
                (fun l ->
                    if (AvatarIslandMetric.Get context avatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
                    else
                        None)
        match location, Vessel.GetPosition context avatarId with
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

    type AcceptJobContext =
        inherit ServiceContext
        abstract member avatarJobSink         : AvatarJobSink
        abstract member avatarJobSource       : AvatarJobSource
        abstract member islandSingleJobSource : IslandSingleJobSource
        abstract member islandSource          : IslandSource
    let AcceptJob 
            (context  : ServiceContext)
            (jobIndex : uint32) 
            (location : Location) 
            (avatarId : string) 
            : unit =
        let context = context :?> AcceptJobContext
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
                |> AvatarMetric.Add 
                    context
                    Metric.AcceptedJob 
                    1UL
                context.avatarJobSink avatarId (job|>Some)
                IslandVisit.MakeKnown
                    context
                    avatarId
                    job.Destination
                IslandJob.Purge context location jobIndex
            | _ ->
                avatarId
                |> AddMessages context [ "That job is currently unavailable." ]
        | _, Some _, Some _ ->
            avatarId
            |> AddMessages context [ "You must complete or abandon your current job before taking on a new one." ]
        | _ -> 
            ()

    type AbandonJobContext =
        inherit ServiceContext
        abstract member avatarJobSource  : AvatarJobSource
    let AbandonJob
            (context  : ServiceContext)
            (avatarId : string) 
            : unit =
        let context = context :?> AbandonJobContext
        match context.avatarJobSource avatarId with
        | Some _ ->
            avatarId
            |> AddMessages context [ "You abandon your job." ]
            avatarId
            |> AvatarJob.Abandon
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

    type BuyItemsContext =
        inherit ServiceContext
        abstract member islandSource                  : IslandSource
        abstract member itemSource                    : ItemSource
        abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource
    let BuyItems 
            (context                       : ServiceContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        let context = context :?> BuyItemsContext
        let items = context.itemSource()
        match items |> FindItemByName itemName, context.islandSource() |> List.tryFind (fun x-> x = location) with
        | Some (item, descriptor) , Some _ ->
            let unitPrice = 
                IslandMarket.DetermineSalePrice 
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
                |> AvatarInventory.GetUsedTonnage
                    context
                    items
            let quantity =
                match tradeQuantity with
                | Specific amount -> amount
                | Maximum -> min (floor(availableTonnage / descriptor.Tonnage)) (floor((avatarId |> AvatarShipmates.GetMoney context) / unitPrice)) |> uint64
            let price = (quantity |> float) * unitPrice
            let tonnageNeeded = (quantity |> float) * descriptor.Tonnage
            if price > (avatarId |> AvatarShipmates.GetMoney context) then
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
                |> AvatarShipmates.SpendMoney 
                    context
                    price 
                avatarId
                |> AvatarInventory.AddInventory 
                    context
                    item 
                    quantity
        | None, Some _ ->
            avatarId
            |> AddMessages context ["Round these parts, we don't sell things like that."]
        | _ ->
            avatarId
            |> AddMessages context ["You cannot buy items here."]

    type SellItemsContext =
        inherit ServiceContext
        abstract member islandSource                  : IslandSource
        abstract member itemSource                    : ItemSource
    let SellItems 
            (context : ServiceContext)
            (location                      : Location) 
            (tradeQuantity                 : TradeQuantity) 
            (itemName                      : string) 
            (avatarId                      : string) 
            : unit =
        let context = context :?> SellItemsContext
        let items = context.itemSource()
        match items |> FindItemByName itemName, context.islandSource()|> List.tryFind ((=)location) with
        | Some (item, descriptor), Some _ ->
            let quantity = 
                match tradeQuantity with
                | Specific q -> q
                | Maximum -> 
                    avatarId 
                    |> AvatarInventory.GetItemCount context item
            if quantity > (avatarId |> AvatarInventory.GetItemCount context item) then
                avatarId
                |> AddMessages context ["You don't have enough of those to sell."]
            elif quantity = 0UL then
                avatarId
                |> AddMessages context ["You don't have any of those to sell."]
            else
                let unitPrice = 
                    IslandMarket.DeterminePurchasePrice 
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
                AvatarShipmates.EarnMoney 
                    context
                    price 
                    avatarId
                avatarId
                |> AvatarInventory.RemoveInventory 
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
            (context : ServiceContext)
            (side                          : Side) 
            (avatarId                      : string) 
            : unit =
        avatarId 
        |> Avatar.CleanHull 
            context
            side 

    type UndockContext = 
        inherit ServiceContext
        abstract member avatarMessageSink       : AvatarMessageSink
        abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink
    let Undock
            (context : ServiceContext)
            (avatarId : string)
            : unit =
        let context = context :?> UndockContext
        avatarId
        |> AddMessages context [ "You undock." ]
        context.avatarIslandFeatureSink (None, avatarId)
    
    let HasDarkAlleyMinimumStakes
            (context : ServiceContext)
            (avatarId : string)
            : bool =
        match AvatarIslandFeature.Get context avatarId with
        | Some feature when feature.featureId = (IslandFeatureIdentifier.DarkAlley) ->
            let minimumBet = 
                Island.GetStatistic
                    context
                    IslandStatisticIdentifier.MinimumGamblingStakes
                    feature.location
                |> Option.get
                |> Statistic.GetCurrentValue
            let money =
                AvatarShipmates.GetMoney
                    context
                    avatarId
            money >= minimumBet
        | _ -> 
            false

    type CanPlaceBetContext =
        inherit ServiceContext
        inherit AvatarShipmates.GetPrimaryStatisticContext

    let CanPlaceBet
            (context : ServiceContext)
            (amount : float)
            (avatarId : string)
            : bool =
        let context = context :?> CanPlaceBetContext
        (AvatarShipmates.GetMoney context avatarId) >= amount
     
    type ResolveHandContext =
        inherit ServiceContext

    let ResolveHand
            (context: ServiceContext)
            (amount : float)
            (avatarId: string)
            : unit =
        let context = context :?> ResolveHandContext
        match AvatarIslandFeature.Get context avatarId with
        | Some feature when feature.featureId = IslandFeatureIdentifier.DarkAlley ->
            match AvatarGamblingHand.Get context avatarId with
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

    type SaveContext =
        abstract member gameDataSink : GameDataSink
    let Save
            (context : ServiceContext)
            (filename : string)
            (avatarId: string)
            : unit =
        match (context :?> SaveContext).gameDataSink filename with
        | Some s ->
            AddMessages context [ s |> sprintf "Saved game to '%s'." ] avatarId
        | _ ->
            AddMessages context [ "Could not save game!" ] avatarId
        

            
    let FoldGamblingHand
            (context  : ServiceContext)
            (avatarId : string)
            : unit =
        avatarId
        |> AvatarMessages.Add 
            context 
            ["Doesn't seem like a good wager to you!"] 
        avatarId
        |> AvatarGamblingHand.Fold
            context 

    let BetOnGamblingHand
            (context : ServiceContext)
            (amount : float)
            (avatarId : string) 
            : bool =
        match AvatarGamblingHand.Get context avatarId with
        | None ->
            AvatarMessages.Add context ["You aren't currently gambling!"] avatarId
            false
        | Some hand ->
            match AvatarIslandFeature.Get context avatarId with
            | Some feature when feature.featureId = IslandFeatureIdentifier.DarkAlley ->
                let minimumWager = 
                    Island.GetStatistic 
                        context 
                        IslandStatisticIdentifier.MinimumGamblingStakes 
                        feature.location 
                    |> Option.get
                    |> Statistic.GetCurrentValue
                if amount<minimumWager then
                    AvatarMessages.Add 
                        context 
                        [sprintf "Minimum stakes are %0.2f!" minimumWager] 
                        avatarId
                    false
                else
                    let money =
                        AvatarShipmates.GetMoney 
                            context 
                            avatarId
                    if money < amount then
                        AvatarMessages.Add 
                            context 
                            [ sprintf "You cannot bet more than you have!" ] 
                            avatarId
                        false
                    elif AvatarGamblingHand.IsWinner hand then
                        AvatarMessages.Add 
                            context 
                            [ sprintf "You win!" ] 
                            avatarId
                        AvatarShipmates.EarnMoney 
                            context 
                            amount 
                            avatarId
                        AvatarGamblingHand.Fold
                            context
                            avatarId
                        true
                    else
                        AvatarMessages.Add 
                            context 
                            [ sprintf "You lose!" ] 
                            avatarId
                        AvatarShipmates.SpendMoney 
                            context 
                            amount 
                            avatarId
                        AvatarGamblingHand.Fold
                            context
                            avatarId
                        true
            | _ -> 
                AvatarMessages.Add context ["You aren't currently gambling!"] avatarId
                false
