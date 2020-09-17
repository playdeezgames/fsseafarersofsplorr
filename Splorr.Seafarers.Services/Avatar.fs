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
    inherit VesselCreateContext
    inherit ShipmateCreateContext
    abstract member avatarJobSink : AvatarJobSink

type AvatarAddMetricContext =
    abstract member avatarSingleMetricSink   : AvatarSingleMetricSink
    abstract member avatarSingleMetricSource : AvatarSingleMetricSource


type AvatarEatContext =
    inherit ShipmateEatContext
    inherit AvatarAddMetricContext
    abstract member avatarInventorySink           : AvatarInventorySink
    abstract member avatarInventorySource         : AvatarInventorySource
    abstract member avatarShipmateSource          : AvatarShipmateSource

type AvatarSetPrimaryStatisticContext = 
    inherit ShipmateTransformStatisticContext

type AvatarSetMoneyContext =
    inherit AvatarSetPrimaryStatisticContext

type AvatarSetReputationContext = 
    inherit AvatarSetPrimaryStatisticContext

type AvatarGetSpeedContext =
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarGetHeadingContext =
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarSetPositionContext =
    abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarSetSpeedContext =
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink

type AvatarGetCurrentFoulingContext =
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarGetMaximumFoulingContext =
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarGetEffectiveSpeedContext =
    inherit AvatarGetCurrentFoulingContext
    inherit AvatarGetSpeedContext


type AvatarSetHeadingContext =
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink

type AvatarRemoveInventoryContext =
    abstract member avatarInventorySource : AvatarInventorySource
    abstract member avatarInventorySink   : AvatarInventorySink

type AvatarIncrementMetricContext =
    inherit AvatarAddMetricContext

type AvatarGetPositionContext = 
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type AvatarGetPrimaryStatisticContext =
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource

type AvatarAbandonJobContext =
    inherit AvatarSetReputationContext
    inherit AvatarGetPrimaryStatisticContext
    inherit AvatarIncrementMetricContext
    abstract member avatarJobSink : AvatarJobSink
    abstract member avatarJobSource : AvatarJobSource

type AvatarGetItemCountContext =
    abstract member avatarInventorySource : AvatarInventorySource

type AvatarAddMessagesContext =
    abstract member avatarMessageSink : AvatarMessageSink

type AvatarGetUsedTonnageContext =
    abstract member avatarInventorySource : AvatarInventorySource

type AvatarCompleteJobContext =
    inherit AvatarSetReputationContext
    inherit ShipmateTransformStatisticContext
    inherit AvatarGetPrimaryStatisticContext
    inherit AvatarAddMetricContext
    abstract member avatarJobSink : AvatarJobSink
    abstract member avatarJobSource : AvatarJobSource

type AvatarEarnMoneyContext =
    inherit AvatarSetMoneyContext
    inherit AvatarGetPrimaryStatisticContext

type AvatarSpendMoneyContext =
    inherit AvatarSetMoneyContext
    inherit AvatarGetPrimaryStatisticContext

type AvatarAddInventoryContext =
    inherit AvatarGetItemCountContext
    abstract member avatarInventorySink   : AvatarInventorySink
    abstract member avatarInventorySource : AvatarInventorySource

type AvatarTransformShipmatesContext =
    abstract avatarShipmateSource : AvatarShipmateSource

type AvatarMoveContext =
    inherit VesselBefoulContext
    inherit ShipmateTransformStatisticContext
    inherit AvatarEatContext
    inherit AvatarGetEffectiveSpeedContext
    inherit AvatarSetPositionContext
    inherit AvatarGetPositionContext
    inherit AvatarTransformShipmatesContext
    abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource

type AvatarCleanHullContext =
    inherit VesselTransformFoulingContext
    inherit ShipmateTransformStatisticContext
    inherit AvatarIncrementMetricContext
    inherit AvatarTransformShipmatesContext
    abstract member avatarShipmateSource          : AvatarShipmateSource

type AvatarGetGamblingHandContext =
    abstract member avatarGamblingHandSource : AvatarGamblingHandSource

type AvatarDealGamblingHandContext =
    abstract member avatarGamblingHandSink : AvatarGamblingHandSink
    abstract member random : Random

type AvatarFoldGamblingHand =
    abstract member avatarGamblingHandSink : AvatarGamblingHandSink
(*
interface AvatarFoldGamblingHand with
    member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink
*)

type AvatarEnterIslandFeatureContext =
    abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink
    abstract member islandSingleFeatureSource : IslandSingleFeatureSource

module Avatar =
    let Create 
            (context  : AvatarCreateContext)
            (avatarId : string)
            : unit =
        Vessel.Create 
            context
            avatarId
        Shipmate.Create 
            context
            avatarId 
            Primary
        context.avatarJobSink avatarId None

    let GetPosition
            (context  : AvatarGetPositionContext)
            (avatarId : string)
            : Location option =
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
            (context  : AvatarGetSpeedContext)
            (avatarId : string)
            : float option =
        VesselStatisticIdentifier.Speed
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    let GetHeading
            (context  : AvatarGetHeadingContext)
            (avatarId : string)
            : float option =
        VesselStatisticIdentifier.Heading
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    let SetPosition 
            (context  : AvatarSetPositionContext)
            (position : Location) 
            (avatarId : string) 
            : unit =
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
            (context  : AvatarSetSpeedContext)
            (speed    : float) 
            (avatarId : string) 
            : unit =
        context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Speed
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Speed, 
                    statistic
                    |> Statistic.SetCurrentValue speed)
                |> context.vesselSingleStatisticSink avatarId)

    let SetHeading 
            (context  : AvatarSetHeadingContext)
            (heading  : float) 
            (avatarId : string) 
            : unit =
        context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Heading, 
                    statistic
                    |> Statistic.SetCurrentValue (heading |> Angle.ToRadians))
                |> context.vesselSingleStatisticSink avatarId)

    let RemoveInventory 
            (context  : AvatarRemoveInventoryContext)
            (item     : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
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
            (context  : AvatarAddMetricContext)
            (metric   : Metric) 
            (amount   : uint64) 
            (avatarId : string)
            : unit =
        context.avatarSingleMetricSink avatarId (metric, (context.avatarSingleMetricSource avatarId metric) + amount)

    let private IncrementMetric 
            (context  : AvatarIncrementMetricContext)
            (metric   : Metric) 
            (avatarId : string) 
            : unit =
        let rateOfIncrement = 1UL
        avatarId
        |> AddMetric 
            context
            metric 
            rateOfIncrement

    let private Eat
            (context : AvatarEatContext)
            (avatarId                      : string)
            : unit =
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
            (context : AvatarGetCurrentFoulingContext) =
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetCurrentValue
    
    let GetMaximumFouling
            (context : AvatarGetMaximumFoulingContext) =
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetMaximumValue 

    let GetEffectiveSpeed 
            (context  : AvatarGetEffectiveSpeedContext)
            (avatarId : string)
            : float =
        let currentValue = GetCurrentFouling context avatarId
        let currentSpeed = GetSpeed context avatarId |> Option.get
        (currentSpeed * (1.0 - currentValue))

    let TransformShipmates 
            (context   : AvatarTransformShipmatesContext)
            (transform : ShipmateIdentifier -> unit) 
            (avatarId  : string) 
            : unit =
        avatarId
        |> context.avatarShipmateSource
        |> List.iter transform

    let Move
            (context  : AvatarMoveContext)
            (avatarId : string)
            : unit =
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
            (context    : AvatarSetPrimaryStatisticContext)
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

    let SetMoney (context : AvatarSetMoneyContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Money 

    let SetReputation (context : AvatarSetReputationContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation 

    let private GetPrimaryStatistic 
            (context : AvatarGetPrimaryStatisticContext)
            (identifier : ShipmateStatisticIdentifier) 
            (avatarId     : string) 
            : float =
        context.shipmateSingleStatisticSource 
            avatarId 
            Primary 
            identifier
        |> Option.map (fun statistic -> statistic.CurrentValue)
        |> Option.defaultValue 0.0

    let GetMoney context = GetPrimaryStatistic context ShipmateStatisticIdentifier.Money

    let GetReputation context = GetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation
    
    let AbandonJob 
            (context : AvatarAbandonJobContext)
            (avatarId: string)
            : unit =
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
            (context : AvatarCompleteJobContext)
            (avatarId:string)
            : unit =
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
            (context : AvatarEarnMoneyContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) + amount)
                avatarId

    let SpendMoney 
            (context : AvatarSpendMoneyContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) - amount)
                avatarId

    let GetItemCount 
            (context : AvatarGetItemCountContext)
            (item                  : uint64) 
            (avatarId              : string) 
            : uint64 =
        match avatarId |> context.avatarInventorySource |> Map.tryFind item with
        | Some x -> x
        | None -> 0UL

    let AddInventory 
            (context : AvatarAddInventoryContext)
            (item : uint64) 
            (quantity : uint64) 
            (avatarId : string) 
            : unit =
        let newQuantity = (avatarId |> GetItemCount context item) + quantity
        avatarId
        |> context.avatarInventorySource
        |> Map.add item newQuantity
        |> context.avatarInventorySink avatarId

    let AddMessages 
            (context : AvatarAddMessagesContext)
            (messages : string list) 
            (avatarId : string) 
            : unit =
        messages
        |> List.iter (context.avatarMessageSink avatarId)


    let GetUsedTonnage
            (context : AvatarGetUsedTonnageContext)
            (items : Map<uint64, ItemDescriptor>) //TODO: to source
            (avatarId : string) 
            : float =
        (0.0, avatarId |> context.avatarInventorySource)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    let CleanHull 
            (context : AvatarCleanHullContext)
            (side : Side) 
            (avatarId : string)
            : unit =
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
            (context : AvatarGetGamblingHandContext)
            (avatarId: string)
            : AvatarGamblingHand option =
        context.avatarGamblingHandSource avatarId
        
    let DealGamblingHand
            (context  : AvatarDealGamblingHandContext)
            (avatarId : string)
            : unit =
        match Deck.Create()
            |> List.sortBy (fun _ -> context.random.Next()) with
        | first :: second :: third :: _ ->
            (first, second, third)
            |> Some
            |> context.avatarGamblingHandSink avatarId
        | _ ->
            raise (NotImplementedException "DealGamblingHand did not have at least three cards")

    let FoldGamblingHand
            (context  : AvatarFoldGamblingHand)
            (avatarId : string)
            : unit =
        context.avatarGamblingHandSink avatarId None

    let EnterIslandFeature
            (context  : AvatarEnterIslandFeatureContext)
            (avatarId : string)
            (location : Location)
            (feature  : IslandFeatureIdentifier)
            : unit =
        if context.islandSingleFeatureSource location feature then
            context.avatarIslandFeatureSink 
                ({featureId = feature; location = location} |> Some, 
                    avatarId)

    type GetIslandFeatureContext =
        abstract member avatarIslandFeatureSource : AvatarIslandFeatureSource

    let GetIslandFeature
            (context : GetIslandFeatureContext)
            (avatarId: string)
            : AvatarIslandFeature option =
        context.avatarIslandFeatureSource avatarId
