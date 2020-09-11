﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

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
//type AvatarIslandFeatureSource = string -> IslandFeatureIdentifier option

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
    interface
    end

type AvatarGetMaximumFoulingContext =
    interface
    end

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

type AvatarMoveContext =
    inherit VesselBefoulContext
    inherit ShipmateTransformStatisticContext
    inherit AvatarEatContext
    inherit AvatarGetEffectiveSpeedContext
    inherit AvatarSetPositionContext
    inherit AvatarGetPositionContext

type AvatarGetPrimaryStatisticContext =
    interface
    end

type AvatarAbandonJobContext =
    inherit AvatarSetReputationContext
    inherit AvatarGetPrimaryStatisticContext
    inherit AvatarIncrementMetricContext

type AvatarGetItemCountContext =
    interface
    end

type AvatarAddMessagesContext =
    interface
    end

type AvatarGetUsedTonnageContext =
    interface
    end

type AvatarCompleteJobContext =
    inherit AvatarSetReputationContext
    inherit ShipmateTransformStatisticContext
    inherit AvatarGetPrimaryStatisticContext
    inherit AvatarAddMetricContext

type AvatarEarnMoneyContext =
    inherit AvatarSetMoneyContext
    inherit AvatarGetPrimaryStatisticContext

type AvatarSpendMoneyContext =
    inherit AvatarSetMoneyContext
    inherit AvatarGetPrimaryStatisticContext

type AvatarAddInventoryContext =
    inherit AvatarGetItemCountContext

type AvatarCleanHullContext =
    inherit VesselTransformFoulingContext
    inherit ShipmateTransformStatisticContext
    inherit AvatarIncrementMetricContext

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
            (context : AvatarGetHeadingContext)
            (avatarId                    : string)
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
        | Some x, Some y ->
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
            (context : AvatarSetHeadingContext)
            (heading : float) 
            (avatarId  : string) 
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
            (context : AvatarAddMetricContext)
            (metric                   : Metric) 
            (amount                   : uint64) 
            (avatarId                 : string)
            : unit =
        context.avatarSingleMetricSink avatarId (metric, (context.avatarSingleMetricSource avatarId metric) + amount)

    let private IncrementMetric 
            (context : AvatarIncrementMetricContext)
            (metric                   : Metric) 
            (avatarId                 : string) 
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

    
    let GetCurrentFouling
            (context : AvatarGetCurrentFoulingContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            :float =
        let portFouling = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PortFouling
            |> Option.map (fun x -> x.CurrentValue)
            |> Option.defaultValue 0.0
        let starboardFouling = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.StarboardFouling
            |> Option.map (fun x -> x.CurrentValue)
            |> Option.defaultValue 0.0
        portFouling + starboardFouling
    
    let GetMaximumFouling
            (context : AvatarGetMaximumFoulingContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            :float =
        let portFouling = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PortFouling
            |> Option.map (fun x -> x.MaximumValue)
            |> Option.defaultValue 0.0
        let starboardFouling = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.StarboardFouling
            |> Option.map (fun x -> x.MaximumValue)
            |> Option.defaultValue 0.0
        portFouling + starboardFouling

    let GetEffectiveSpeed 
            (context : AvatarGetEffectiveSpeedContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            : float =
        let currentValue = GetCurrentFouling context vesselSingleStatisticSource avatarId
        let currentSpeed = GetSpeed context avatarId |> Option.get
        (currentSpeed * (1.0 - currentValue))

    let TransformShipmates 
            (avatarShipmateSource : AvatarShipmateSource)
            (transform            : ShipmateIdentifier -> unit) 
            (avatarId             : string) 
            : unit =
        avatarId
        |> avatarShipmateSource
        |> List.iter transform

    let Move
            (context : AvatarMoveContext)
            (avatarShipmateSource          : AvatarShipmateSource)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (avatarId                      : string)
            : unit =
        let actualSpeed = 
            avatarId 
            |> GetEffectiveSpeed context vesselSingleStatisticSource
        let actualHeading = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading 
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
            avatarShipmateSource
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
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (avatarId     : string) 
            : float =
        shipmateSingleStatisticSource 
            avatarId 
            Primary 
            identifier
        |> Option.map (fun statistic -> statistic.CurrentValue)
        |> Option.defaultValue 0.0

    let GetMoney context = GetPrimaryStatistic context ShipmateStatisticIdentifier.Money

    let GetReputation context = GetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation
    
    let AbandonJob 
            (context : AvatarAbandonJobContext)
            (avatarJobSink                 : AvatarJobSink)
            (avatarJobSource               : AvatarJobSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (avatarId: string)
            : unit =
        let reputationCostForAbandoningAJob = -1.0
        avatarId
        |> avatarJobSource
        |> Option.iter
            (fun _ -> 
                avatarId
                |> SetReputation 
                    context
                    ((GetReputation 
                        context
                        shipmateSingleStatisticSource 
                        avatarId) + 
                            reputationCostForAbandoningAJob) 
                    
                avatarId
                |> IncrementMetric 
                    context
                    Metric.AbandonedJob
                avatarJobSink avatarId None)

    let CompleteJob
            (context : AvatarCompleteJobContext)
            (avatarJobSink                 : AvatarJobSink)
            (avatarJobSource               : AvatarJobSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (avatarId:string)
            : unit =
        match avatarId |> avatarJobSource with
        | Some job ->
            SetReputation 
                context
                ((GetReputation 
                    context
                    shipmateSingleStatisticSource 
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
            |> avatarJobSink avatarId
        | _ -> ()

    let EarnMoney 
            (context : AvatarEarnMoneyContext)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context shipmateSingleStatisticSource avatarId) + amount)
                avatarId

    let SpendMoney 
            (context : AvatarSpendMoneyContext)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context shipmateSingleStatisticSource avatarId) - amount)
                avatarId

    let GetItemCount 
            (context : AvatarGetItemCountContext)
            (avatarInventorySource : AvatarInventorySource)
            (item                  : uint64) 
            (avatarId              : string) 
            : uint64 =
        match avatarId |> avatarInventorySource |> Map.tryFind item with
        | Some x -> x
        | None -> 0UL

    let AddInventory 
            (context : AvatarAddInventoryContext)
            (avatarInventorySink   : AvatarInventorySink)
            (avatarInventorySource : AvatarInventorySource)
            (item                  : uint64) 
            (quantity              : uint64) 
            (avatarId              : string) 
            : unit =
        let newQuantity = (avatarId |> GetItemCount context avatarInventorySource item) + quantity
        avatarId
        |> avatarInventorySource
        |> Map.add item newQuantity
        |> avatarInventorySink avatarId

    let AddMessages 
            (context : AvatarAddMessagesContext)
            (avatarMessageSink : AvatarMessageSink)
            (messages          : string list) 
            (avatarId          : string) 
            : unit =
        messages
        |> List.iter (avatarMessageSink avatarId)


    let GetUsedTonnage
            (context : AvatarGetUsedTonnageContext)
            (avatarInventorySource : AvatarInventorySource)
            (items                 : Map<uint64, ItemDescriptor>) //TODO: to source
            (avatarId              : string) 
            : float =
        (0.0, avatarId |> avatarInventorySource)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    let CleanHull 
            (context : AvatarCleanHullContext)
            (avatarShipmateSource          : AvatarShipmateSource)
            (side                          : Side) 
            (avatarId                      : string)
            : unit =
        Vessel.TransformFouling 
            context
            avatarId 
            side 
            (fun x-> {x with CurrentValue = x.MinimumValue})
        TransformShipmates 
            avatarShipmateSource 
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
