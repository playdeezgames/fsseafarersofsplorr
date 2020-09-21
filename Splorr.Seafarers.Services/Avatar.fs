namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models
open Tarot

type MessagePurger = string -> unit
type AvatarMessageSink = string -> string -> unit
type AvatarShipmateSource = string -> ShipmateIdentifier list
type AvatarInventorySource = string -> AvatarInventory
type AvatarInventorySink = string -> AvatarInventory -> unit
type AvatarSingleMetricSource = string -> Metric -> uint64
type AvatarSingleMetricSink = string -> Metric * uint64 -> unit
type AvatarJobSource = string -> Job option
type AvatarJobSink = string -> Job option -> unit
type AvatarIslandFeatureSink = AvatarIslandFeature option * string -> unit
type AvatarIslandFeatureSource = string -> AvatarIslandFeature option
type AvatarGamblingHandSource = string -> AvatarGamblingHand option
type AvatarGamblingHandSink = string -> AvatarGamblingHand option -> unit

type AvatarCreateContext =
    inherit OperatingContext
    abstract member avatarJobSink : AvatarJobSink

type AvatarAddMetricContext =
    inherit OperatingContext
    abstract member avatarSingleMetricSink   : AvatarSingleMetricSink
    abstract member avatarSingleMetricSource : AvatarSingleMetricSource

type AvatarEatContext =
    inherit OperatingContext
    abstract member avatarInventorySink           : AvatarInventorySink
    abstract member avatarInventorySource         : AvatarInventorySource
    abstract member avatarShipmateSource          : AvatarShipmateSource

type AvatarSetPrimaryStatisticContext = 
    inherit OperatingContext

type AvatarSetMoneyContext =
    inherit OperatingContext

type AvatarSetReputationContext = 
    inherit OperatingContext

type AvatarGetSpeedContext =
    inherit OperatingContext
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarGetHeadingContext =
    inherit OperatingContext
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarSetPositionContext =
    inherit OperatingContext
    abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarSetSpeedContext =
    inherit OperatingContext
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink

type AvatarGetCurrentFoulingContext =
    inherit OperatingContext
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarGetMaximumFoulingContext =
    inherit OperatingContext
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarGetEffectiveSpeedContext =
    inherit OperatingContext

type AvatarSetHeadingContext =
    inherit OperatingContext
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink

type AvatarRemoveInventoryContext =
    inherit OperatingContext
    abstract member avatarInventorySource : AvatarInventorySource
    abstract member avatarInventorySink   : AvatarInventorySink

type AvatarIncrementMetricContext =
    inherit OperatingContext

type AvatarGetPositionContext = 
    inherit OperatingContext
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarGetPrimaryStatisticContext =
    inherit OperatingContext
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource

type AvatarAbandonJobContext =
    inherit OperatingContext
    abstract member avatarJobSink : AvatarJobSink
    abstract member avatarJobSource : AvatarJobSource

type AvatarGetItemCountContext =
    inherit OperatingContext
    abstract member avatarInventorySource : AvatarInventorySource

type AvatarAddMessagesContext =
    inherit OperatingContext
    abstract member avatarMessageSink : AvatarMessageSink

type AvatarGetUsedTonnageContext =
    inherit OperatingContext
    abstract member avatarInventorySource : AvatarInventorySource

type AvatarCompleteJobContext =
    inherit OperatingContext
    abstract member avatarJobSink : AvatarJobSink
    abstract member avatarJobSource : AvatarJobSource

type AvatarEarnMoneyContext =
    inherit OperatingContext

type AvatarSpendMoneyContext =
    inherit OperatingContext

type AvatarAddInventoryContext =
    inherit OperatingContext
    abstract member avatarInventorySink   : AvatarInventorySink
    abstract member avatarInventorySource : AvatarInventorySource

type AvatarTransformShipmatesContext =
    inherit OperatingContext
    abstract avatarShipmateSource : AvatarShipmateSource

type AvatarMoveContext =
    inherit OperatingContext
    abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource

type AvatarCleanHullContext =
    inherit OperatingContext
    abstract member avatarShipmateSource          : AvatarShipmateSource

type AvatarGetGamblingHandContext =
    inherit OperatingContext
    abstract member avatarGamblingHandSource : AvatarGamblingHandSource

type AvatarDealGamblingHandContext =
    inherit OperatingContext
    abstract member avatarGamblingHandSink : AvatarGamblingHandSink
    abstract member random : Random

type AvatarFoldGamblingHand =
    inherit OperatingContext
    abstract member avatarGamblingHandSink : AvatarGamblingHandSink

type AvatarEnterIslandFeatureContext =
    inherit OperatingContext
    abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink
    abstract member islandSingleFeatureSource : IslandSingleFeatureSource

module Avatar =
    let Create 
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> AvatarCreateContext
        Vessel.Create 
            context
            avatarId
        Shipmate.Create 
            context
            avatarId 
            Primary
        context.avatarJobSink avatarId None
    
    let GetPosition
            (context  : OperatingContext)
            (avatarId : string)
            : Location option =
        let context = context :?> AvatarGetPositionContext
        let positionX =
            VesselStatisticIdentifier.PositionX
            |> context.vesselSingleStatisticSource avatarId
            |> Option.map Statistic.GetCurrentValue
        let positionY = 
            VesselStatisticIdentifier.PositionY
            |> context.vesselSingleStatisticSource avatarId
            |> Option.map Statistic.GetCurrentValue
        match positionX, positionY with
        | Some x, Some y ->
            (x,y) |> Some
        | _ ->
            None

    let GetSpeed
            (context  : OperatingContext)
            (avatarId : string)
            : float option =
        let context = context :?> AvatarGetSpeedContext
        VesselStatisticIdentifier.Speed
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    let GetHeading
            (context  : OperatingContext)
            (avatarId : string)
            : float option =
        let context = context :?> AvatarGetHeadingContext
        VesselStatisticIdentifier.Heading
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    let SetPosition 
            (context  : OperatingContext)
            (position : Location) 
            (avatarId : string) 
            : unit =
        let context = context :?> AvatarSetPositionContext
        match 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PositionX, 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PositionY 
            with
        | Some x, Some _ ->
            context.vesselSingleStatisticSink 
                avatarId 
                (VesselStatisticIdentifier.PositionX, 
                    x 
                    |> Statistic.SetCurrentValue 
                        (position 
                        |> fst))
            context.vesselSingleStatisticSink 
                avatarId 
                (VesselStatisticIdentifier.PositionY, 
                    x 
                    |> Statistic.SetCurrentValue 
                        (position 
                        |> snd))
        | _ -> ()

    let SetSpeed 
            (context  : OperatingContext)
            (speed    : float) 
            (avatarId : string) 
            : unit =
        let context = context :?> AvatarSetSpeedContext
        context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Speed
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Speed, 
                    statistic
                    |> Statistic.SetCurrentValue speed)
                |> context.vesselSingleStatisticSink avatarId)

    let SetHeading 
            (context  : OperatingContext)
            (heading  : float) 
            (avatarId : string) 
            : unit =
        let context = context :?> AvatarSetHeadingContext
        context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Heading, 
                    statistic
                    |> Statistic.SetCurrentValue (heading |> Angle.ToRadians))
                |> context.vesselSingleStatisticSink avatarId)

    let RemoveInventory 
            (context  : OperatingContext)
            (item     : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let context = context :?> AvatarRemoveInventoryContext
        let inventory = 
            avatarId 
            |>  context.avatarInventorySource
        match inventory.TryFind item with
        | Some count ->
            if count > quantity then
                inventory
                |> Map.add item (count-quantity)
            else
                inventory 
                |> Map.remove item
        | _ ->
            inventory
        |> context.avatarInventorySink avatarId

    let AddMetric 
            (context  : OperatingContext)
            (metric   : Metric) 
            (amount   : uint64) 
            (avatarId : string)
            : unit =
        let context = context :?> AvatarAddMetricContext
        context.avatarSingleMetricSink avatarId (metric, (context.avatarSingleMetricSource avatarId metric) + amount)

    let private IncrementMetric 
            (context  : OperatingContext)
            (metric   : Metric) 
            (avatarId : string) 
            : unit =
        let context = context :?> AvatarIncrementMetricContext
        let rateOfIncrement = 1UL
        avatarId
        |> AddMetric 
            context
            metric 
            rateOfIncrement

    let private Eat
            (context : OperatingContext)
            (avatarId                      : string)
            : unit =
        let context = context :?> AvatarEatContext
        let inventory, eaten, starved =
            ((context.avatarInventorySource avatarId, 0UL, 0UL), context.avatarShipmateSource avatarId)
            ||> List.fold 
                (fun (inventory,eatMetric, starveMetric) identifier -> 
                    let updateInventory, ate, starved =
                        Shipmate.Eat
                            context
                            inventory 
                            avatarId 
                            identifier
                    (updateInventory, 
                        (if ate then eatMetric+1UL else eatMetric), 
                            (if starved then starveMetric+1UL else starveMetric))) 
        inventory
        |> context.avatarInventorySink avatarId
        if eaten > 0UL then
            avatarId
            |> AddMetric 
                context
                Metric.Ate 
                eaten
        if starved > 0UL then
            avatarId
            |> AddMetric 
                context
                Metric.Starved 
                starved

    let private GetFouling
            //TODO: context me
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (getter : Statistic -> float)
            (avatarId                    : string)
            :float =
        [
            VesselStatisticIdentifier.PortFouling
            VesselStatisticIdentifier.StarboardFouling
        ]
        |> List.map
            (vesselSingleStatisticSource avatarId
                >> Option.map getter
                >> Option.defaultValue 0.0)
        |> List.reduce (+)
    
    let GetCurrentFouling
            (context : OperatingContext) =
        let context = context :?> AvatarGetCurrentFoulingContext
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetCurrentValue
    
    let GetMaximumFouling
            (context : OperatingContext) =
        let context = context :?> AvatarGetMaximumFoulingContext
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetMaximumValue 

    let GetEffectiveSpeed 
            (context  : OperatingContext)
            (avatarId : string)
            : float =
        let context = context :?> AvatarGetEffectiveSpeedContext
        let currentValue = GetCurrentFouling context avatarId
        let currentSpeed = GetSpeed context avatarId |> Option.get
        (currentSpeed * (1.0 - currentValue))

    let TransformShipmates 
            (context   : OperatingContext)
            (transform : ShipmateIdentifier -> unit) 
            (avatarId  : string) 
            : unit =
        let context = context :?> AvatarTransformShipmatesContext
        avatarId
        |> context.avatarShipmateSource
        |> List.iter transform

    let Move
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> AvatarMoveContext
        let actualSpeed = 
            avatarId 
            |> GetEffectiveSpeed context
        let actualHeading = 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading 
            |> Option.map Statistic.GetCurrentValue 
            |> Option.get
        Vessel.Befoul 
            context
            avatarId
        let avatarPosition = 
            GetPosition 
                context 
                avatarId 
            |> Option.get
        let newPosition = ((avatarPosition |> fst) + System.Math.Cos(actualHeading) * actualSpeed, (avatarPosition |> snd) + System.Math.Sin(actualHeading) * actualSpeed)
        SetPosition context newPosition avatarId
        TransformShipmates
            context
            (fun identifier -> 
                Shipmate.TransformStatistic 
                    context
                    ShipmateStatisticIdentifier.Turn 
                    (Statistic.ChangeCurrentBy 1.0 >> Some)
                    avatarId
                    identifier)
            avatarId
        avatarId
        |> AddMetric 
            context
            Metric.Moved 
            1UL
        avatarId
        |> Eat 
            context

    let private SetPrimaryStatistic
            (context    : OperatingContext)
            (identifier : ShipmateStatisticIdentifier)
            (amount     : float) 
            (avatarId   : string)
            : unit =
        let context = context :?> AvatarSetPrimaryStatisticContext
        Shipmate.TransformStatistic 
            context
            identifier 
            (Statistic.SetCurrentValue amount >> Some) 
            avatarId
            Primary

    let SetMoney (context : OperatingContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Money 

    let SetReputation (context : OperatingContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation 

    let private GetPrimaryStatistic 
            (context : OperatingContext)
            (identifier : ShipmateStatisticIdentifier) 
            (avatarId     : string) 
            : float =
        let context = context :?> AvatarGetPrimaryStatisticContext
        context.shipmateSingleStatisticSource 
            avatarId 
            Primary 
            identifier
        |> Option.map (fun statistic -> statistic.CurrentValue)
        |> Option.defaultValue 0.0

    let GetMoney (context:OperatingContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Money

    let GetReputation (context:OperatingContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation
    
    let AbandonJob 
            (context : OperatingContext)
            (avatarId: string)
            : unit =
        let context = context :?> AvatarAbandonJobContext
        let reputationCostForAbandoningAJob = -1.0
        avatarId
        |> context.avatarJobSource
        |> Option.iter
            (fun _ -> 
                avatarId
                |> SetReputation 
                    context
                    ((GetReputation 
                        context
                        avatarId) + 
                            reputationCostForAbandoningAJob) 
                    
                avatarId
                |> IncrementMetric 
                    context
                    Metric.AbandonedJob
                context.avatarJobSink avatarId None)

    let CompleteJob
            (context : OperatingContext)
            (avatarId:string)
            : unit =
        let context = context :?> AvatarCompleteJobContext
        match avatarId |> context.avatarJobSource with
        | Some job ->
            SetReputation 
                context
                ((GetReputation 
                    context
                    avatarId) + 
                        1.0)
                avatarId
            Shipmate.TransformStatistic 
                context
                ShipmateStatisticIdentifier.Money 
                (Statistic.ChangeCurrentBy job.Reward >> Some)
                avatarId
                Primary
            avatarId
            |> AddMetric 
                context
                Metric.CompletedJob 
                1UL
            None
            |> context.avatarJobSink avatarId
        | _ -> ()

    let EarnMoney 
            (context : OperatingContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        let context = context :?> AvatarEarnMoneyContext
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) + amount)
                avatarId

    let SpendMoney 
            (context : OperatingContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        let context = context :?> AvatarSpendMoneyContext
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) - amount)
                avatarId

    let GetItemCount 
            (context : OperatingContext)
            (item                  : uint64) 
            (avatarId              : string) 
            : uint64 =
        let context = context :?> AvatarGetItemCountContext
        match avatarId |> context.avatarInventorySource |> Map.tryFind item with
        | Some x -> x
        | None -> 0UL

    let AddInventory 
            (context : OperatingContext)
            (item : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let context = context :?> AvatarAddInventoryContext
        let newQuantity = (avatarId |> GetItemCount context item) + quantity
        avatarId
        |> context.avatarInventorySource
        |> Map.add item newQuantity
        |> context.avatarInventorySink avatarId

    let AddMessages 
            (context : OperatingContext)
            (messages : string list) 
            (avatarId : string) 
            : unit =
        let context = context :?> AvatarAddMessagesContext
        messages
        |> List.iter (context.avatarMessageSink avatarId)


    let GetUsedTonnage
            (context : OperatingContext)
            (items : Map<uint64, ItemDescriptor>) //TODO: to source
            (avatarId : string) 
            : float =
        let context = context :?> AvatarGetUsedTonnageContext
        (0.0, avatarId |> context.avatarInventorySource)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    let CleanHull 
            (context : OperatingContext)
            (side : Side) 
            (avatarId : string)
            : unit =
        let context = context :?> AvatarCleanHullContext
        Vessel.TransformFouling 
            context
            avatarId 
            side 
            (fun x-> 
                {x with 
                    CurrentValue = x.MinimumValue})
        TransformShipmates 
            context
            (Shipmate.TransformStatistic
                context
                ShipmateStatisticIdentifier.Turn 
                (Statistic.ChangeCurrentBy 1.0 >> Some)
                avatarId)
            avatarId
        avatarId
        |> IncrementMetric
            context
            Metric.CleanedHull

    let GetGamblingHand
            (context : OperatingContext)
            (avatarId: string)
            : AvatarGamblingHand option =
        let context = context :?> AvatarGetGamblingHandContext
        context.avatarGamblingHandSource avatarId
        
    let DealGamblingHand
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> AvatarDealGamblingHandContext
        match Deck.Create()
            |> List.sortBy (fun _ -> context.random.Next()) with
        | first :: second :: third :: _ ->
            (first, second, third)
            |> Some
            |> context.avatarGamblingHandSink avatarId
        | _ ->
            raise (NotImplementedException "DealGamblingHand did not have at least three cards")

    let FoldGamblingHand
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> AvatarFoldGamblingHand
        context.avatarGamblingHandSink avatarId None

    let EnterIslandFeature
            (context  : OperatingContext)
            (avatarId : string)
            (location : Location)
            (feature  : IslandFeatureIdentifier)
            : unit =
        let context = context :?> AvatarEnterIslandFeatureContext
        if context.islandSingleFeatureSource location feature then
            context.avatarIslandFeatureSink 
                ({featureId = feature; location = location} |> Some, 
                    avatarId)

    type GetIslandFeatureContext =
        inherit OperatingContext
        abstract member avatarIslandFeatureSource : AvatarIslandFeatureSource
    let GetIslandFeature
            (context : OperatingContext)
            (avatarId: string)
            : AvatarIslandFeature option =
        let context = context :?> GetIslandFeatureContext
        context.avatarIslandFeatureSource avatarId
