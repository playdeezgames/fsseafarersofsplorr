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

module Avatar =
    type CreateContext =
        inherit OperatingContext
        abstract member avatarJobSink : AvatarJobSink
    let Create 
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> CreateContext
        Vessel.Create 
            context
            avatarId
        Shipmate.Create 
            context
            avatarId 
            Primary
        context.avatarJobSink avatarId None

    type GetPositionContext = 
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetPosition
            (context  : OperatingContext)
            (avatarId : string)
            : Location option =
        let context = context :?> GetPositionContext
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

    type GetSpeedContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetSpeed
            (context  : OperatingContext)
            (avatarId : string)
            : float option =
        let context = context :?> GetSpeedContext
        VesselStatisticIdentifier.Speed
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    type GetHeadingContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetHeading
            (context  : OperatingContext)
            (avatarId : string)
            : float option =
        let context = context :?> GetHeadingContext
        VesselStatisticIdentifier.Heading
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue
    
    type SetPositionContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let SetPosition 
            (context  : OperatingContext)
            (position : Location) 
            (avatarId : string) 
            : unit =
        let context = context :?> SetPositionContext
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

    type SetSpeedContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    let SetSpeed 
            (context  : OperatingContext)
            (speed    : float) 
            (avatarId : string) 
            : unit =
        let context = context :?> SetSpeedContext
        context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Speed
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Speed, 
                    statistic
                    |> Statistic.SetCurrentValue speed)
                |> context.vesselSingleStatisticSink avatarId)

    type SetHeadingContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    let SetHeading 
            (context  : OperatingContext)
            (heading  : float) 
            (avatarId : string) 
            : unit =
        let context = context :?> SetHeadingContext
        context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Heading, 
                    statistic
                    |> Statistic.SetCurrentValue (heading |> Angle.ToRadians))
                |> context.vesselSingleStatisticSink avatarId)

    type RemoveInventoryContext =
        inherit OperatingContext
        abstract member avatarInventorySource : AvatarInventorySource
        abstract member avatarInventorySink   : AvatarInventorySink
    let RemoveInventory 
            (context  : OperatingContext)
            (item     : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let context = context :?> RemoveInventoryContext
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
    
    type AddMetricContext =
        inherit OperatingContext
        abstract member avatarSingleMetricSink   : AvatarSingleMetricSink
        abstract member avatarSingleMetricSource : AvatarSingleMetricSource
    let AddMetric 
            (context  : OperatingContext)
            (metric   : Metric) 
            (amount   : uint64) 
            (avatarId : string)
            : unit =
        let context = context :?> AddMetricContext
        context.avatarSingleMetricSink avatarId (metric, (context.avatarSingleMetricSource avatarId metric) + amount)

    let private IncrementMetric 
            (context  : OperatingContext)
            (metric   : Metric) 
            (avatarId : string) 
            : unit =
        let rateOfIncrement = 1UL
        avatarId
        |> AddMetric 
            context
            metric 
            rateOfIncrement

    type EatContext =
        inherit OperatingContext
        abstract member avatarInventorySink           : AvatarInventorySink
        abstract member avatarInventorySource         : AvatarInventorySource
        abstract member avatarShipmateSource          : AvatarShipmateSource
    let private Eat
            (context : OperatingContext)
            (avatarId                      : string)
            : unit =
        let context = context :?> EatContext
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

    type GetCurrentFoulingContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetCurrentFouling
            (context : OperatingContext) =
        let context = context :?> GetCurrentFoulingContext
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetCurrentValue
    
    type GetMaximumFoulingContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetMaximumFouling
            (context : OperatingContext) =
        let context = context :?> GetMaximumFoulingContext
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetMaximumValue 

    let GetEffectiveSpeed 
            (context  : OperatingContext)
            (avatarId : string)
            : float =
        let currentValue = GetCurrentFouling context avatarId
        let currentSpeed = GetSpeed context avatarId |> Option.get
        (currentSpeed * (1.0 - currentValue))

    type TransformShipmatesContext =
        inherit OperatingContext
        abstract avatarShipmateSource : AvatarShipmateSource
    let TransformShipmates 
            (context   : OperatingContext)
            (transform : ShipmateIdentifier -> unit) 
            (avatarId  : string) 
            : unit =
        let context = context :?> TransformShipmatesContext
        avatarId
        |> context.avatarShipmateSource
        |> List.iter transform


    type MoveContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource
    let Move
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> MoveContext
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
        Shipmate.TransformStatistic 
            context
            identifier 
            (Statistic.SetCurrentValue amount >> Some) 
            avatarId
            Primary

    let SetMoney (context : OperatingContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Money 

    let SetReputation (context : OperatingContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation 

    type GetPrimaryStatisticContext =
        inherit OperatingContext
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    let private GetPrimaryStatistic 
            (context : OperatingContext)
            (identifier : ShipmateStatisticIdentifier) 
            (avatarId     : string) 
            : float =
        let context = context :?> GetPrimaryStatisticContext
        context.shipmateSingleStatisticSource 
            avatarId 
            Primary 
            identifier
        |> Option.map (fun statistic -> statistic.CurrentValue)
        |> Option.defaultValue 0.0

    let GetMoney (context:OperatingContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Money

    let GetReputation (context:OperatingContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation
    

    type AbandonJobContext =
        inherit OperatingContext
        abstract member avatarJobSink : AvatarJobSink
        abstract member avatarJobSource : AvatarJobSource
    let AbandonJob 
            (context : OperatingContext)
            (avatarId: string)
            : unit =
        let context = context :?> AbandonJobContext
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

    type CompleteJobContext =
        inherit OperatingContext
        abstract member avatarJobSink : AvatarJobSink
        abstract member avatarJobSource : AvatarJobSource
    let CompleteJob
            (context : OperatingContext)
            (avatarId:string)
            : unit =
        let context = context :?> CompleteJobContext
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
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) - amount)
                avatarId

    type GetItemCountContext =
        inherit OperatingContext
        abstract member avatarInventorySource : AvatarInventorySource
    let GetItemCount 
            (context : OperatingContext)
            (item                  : uint64) 
            (avatarId              : string) 
            : uint64 =
        let context = context :?> GetItemCountContext
        match avatarId |> context.avatarInventorySource |> Map.tryFind item with
        | Some x -> x
        | None -> 0UL

    type AddInventoryContext =
        inherit OperatingContext
        abstract member avatarInventorySink   : AvatarInventorySink
        abstract member avatarInventorySource : AvatarInventorySource
    let AddInventory 
            (context : OperatingContext)
            (item : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let context = context :?> AddInventoryContext
        let newQuantity = (avatarId |> GetItemCount context item) + quantity
        avatarId
        |> context.avatarInventorySource
        |> Map.add item newQuantity
        |> context.avatarInventorySink avatarId

    type AddMessagesContext =
        inherit OperatingContext
        abstract member avatarMessageSink : AvatarMessageSink
    let AddMessages 
            (context : OperatingContext)
            (messages : string list) 
            (avatarId : string) 
            : unit =
        let context = context :?> AddMessagesContext
        messages
        |> List.iter (context.avatarMessageSink avatarId)

    type GetUsedTonnageContext =
        inherit OperatingContext
        abstract member avatarInventorySource : AvatarInventorySource
    let GetUsedTonnage
            (context : OperatingContext)
            (items : Map<uint64, ItemDescriptor>) //TODO: to source
            (avatarId : string) 
            : float =
        let context = context :?> GetUsedTonnageContext
        (0.0, avatarId |> context.avatarInventorySource)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    type CleanHullContext =
        inherit OperatingContext
        abstract member avatarShipmateSource          : AvatarShipmateSource
    let CleanHull 
            (context : OperatingContext)
            (side : Side) 
            (avatarId : string)
            : unit =
        let context = context :?> CleanHullContext
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

    type GetGamblingHandContext =
        inherit OperatingContext
        abstract member avatarGamblingHandSource : AvatarGamblingHandSource
    let GetGamblingHand
            (context  : OperatingContext)
            (avatarId : string)
            : AvatarGamblingHand option =
        let context = context :?> GetGamblingHandContext
        context.avatarGamblingHandSource avatarId
      
      
    type DealGamblingHandContext =
        inherit OperatingContext
        abstract member avatarGamblingHandSink : AvatarGamblingHandSink
        abstract member random : Random
    let DealGamblingHand
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> DealGamblingHandContext
        match Deck.Create()
            |> List.sortBy (fun _ -> context.random.Next()) with
        | first :: second :: third :: _ ->
            (first, second, third)
            |> Some
            |> context.avatarGamblingHandSink avatarId
        | _ ->
            raise (NotImplementedException "DealGamblingHand did not have at least three cards")

    type FoldGamblingHandContext =
        inherit OperatingContext
        abstract member avatarGamblingHandSink : AvatarGamblingHandSink
    let FoldGamblingHand
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> FoldGamblingHandContext
        context.avatarGamblingHandSink avatarId None


    type EnterIslandFeatureContext =
        inherit OperatingContext
        abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink
        abstract member islandSingleFeatureSource : IslandSingleFeatureSource
    let EnterIslandFeature
            (context  : OperatingContext)
            (avatarId : string)
            (location : Location)
            (feature  : IslandFeatureIdentifier)
            : unit =
        let context = context :?> EnterIslandFeatureContext
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
