namespace Splorr.Seafarers.Services
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
    interface
    end

type AvatarEatContext =
    inherit ShipmateEatContext
    inherit AvatarAddMetricContext

type AvatarSetPrimaryStatisticContext = 
    inherit ShipmateTransformStatisticContext

type AvatarGetSpeedContext =
    interface
    end

type AvatarGetHeadingContext =
    interface
    end

type AvatarSetPositionContext =
    interface
    end

type AvatarSetSpeedContext =
    interface
    end

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
    interface
    end

type AvatarRemoveInventoryContext =
    interface
    end

type AvatarIncrementMetricContext =
    inherit AvatarAddMetricContext

type AvatarGetPositionContext = 
    interface
    end

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
    inherit AvatarSetPrimaryStatisticContext
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
    inherit AvatarSetPrimaryStatisticContext
    inherit ShipmateTransformStatisticContext
    inherit AvatarGetPrimaryStatisticContext
    inherit AvatarAddMetricContext

type AvatarEarnMoneyContext =
    inherit AvatarSetPrimaryStatisticContext
    inherit AvatarGetPrimaryStatisticContext

type AvatarSpendMoneyContext =
    inherit AvatarSetPrimaryStatisticContext
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
            (context : AvatarGetPositionContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            : Location option =
        let positionX =
            VesselStatisticIdentifier.PositionX
            |> vesselSingleStatisticSource avatarId
            |> Option.map Statistic.GetCurrentValue
        let positionY = 
            VesselStatisticIdentifier.PositionY
            |> vesselSingleStatisticSource avatarId
            |> Option.map Statistic.GetCurrentValue
        match positionX, positionY with
        | Some x, Some y ->
            (x,y) |> Some
        | _ ->
            None

    let GetSpeed
            (context : AvatarGetSpeedContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            : float option =
        VesselStatisticIdentifier.Speed
        |> vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    let GetHeading
            (context : AvatarGetHeadingContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            : float option =
        VesselStatisticIdentifier.Heading
        |> vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    let SetPosition 
            (context: AvatarSetPositionContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (position                    : Location) 
            (avatarId                    : string) 
            : unit =
        match 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PositionX, 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PositionY 
            with
        | Some x, Some y ->
            vesselSingleStatisticSink 
                avatarId 
                (VesselStatisticIdentifier.PositionX, 
                    x 
                    |> Statistic.SetCurrentValue 
                        (position 
                        |> fst))
            vesselSingleStatisticSink 
                avatarId 
                (VesselStatisticIdentifier.PositionY, 
                    x 
                    |> Statistic.SetCurrentValue 
                        (position 
                        |> snd))
        | _ -> ()

    let SetSpeed 
            (context : AvatarSetSpeedContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (speed                       : float) 
            (avatarId                    : string) 
            : unit =
        vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Speed
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Speed, 
                    statistic
                    |> Statistic.SetCurrentValue speed)
                |> vesselSingleStatisticSink avatarId)

    let SetHeading 
            (context : AvatarSetHeadingContext)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (heading : float) 
            (avatarId  : string) 
            : unit =
        vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Heading, 
                    statistic
                    |> Statistic.SetCurrentValue (heading |> Angle.ToRadians))
                |> vesselSingleStatisticSink avatarId)

    let RemoveInventory 
            (context : AvatarRemoveInventoryContext)
            (avatarInventorySource : AvatarInventorySource)
            (avatarInventorySink   : AvatarInventorySink)
            (item                  : uint64) 
            (quantity              : uint64) 
            (avatarId              : string) 
            : unit =
        let inventory = 
            avatarId 
            |>  avatarInventorySource
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
        |> avatarInventorySink avatarId

    let AddMetric 
            (context : AvatarAddMetricContext)
            (avatarSingleMetricSink   : AvatarSingleMetricSink)
            (avatarSingleMetricSource : AvatarSingleMetricSource)
            (metric                   : Metric) 
            (amount                   : uint64) 
            (avatarId                 : string)
            : unit =
        avatarSingleMetricSink avatarId (metric, (avatarSingleMetricSource avatarId metric) + amount)

    let private IncrementMetric 
            (context : AvatarIncrementMetricContext)
            (avatarSingleMetricSink   : AvatarSingleMetricSink)
            (avatarSingleMetricSource : AvatarSingleMetricSource)
            (metric                   : Metric) 
            (avatarId                 : string) 
            : unit =
        let rateOfIncrement = 1UL
        avatarId
        |> AddMetric 
            context
            avatarSingleMetricSink
            avatarSingleMetricSource
            metric 
            rateOfIncrement

    let private Eat
            (context : AvatarEatContext)
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (avatarId                      : string)
            : unit =
        let inventory, eaten, starved =
            ((avatarInventorySource avatarId, 0UL, 0UL), avatarShipmateSource avatarId)
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
        |> avatarInventorySink avatarId
        if eaten > 0UL then
            avatarId
            |> AddMetric 
                context
                avatarSingleMetricSink 
                avatarSingleMetricSource 
                Metric.Ate 
                eaten
        if starved > 0UL then
            avatarId
            |> AddMetric 
                context
                avatarSingleMetricSink 
                avatarSingleMetricSource 
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
        let currentSpeed = GetSpeed context vesselSingleStatisticSource avatarId |> Option.get
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
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
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
        let avatarPosition = GetPosition context vesselSingleStatisticSource avatarId |> Option.get
        let newPosition = ((avatarPosition |> fst) + System.Math.Cos(actualHeading) * actualSpeed, (avatarPosition |> snd) + System.Math.Sin(actualHeading) * actualSpeed)
        SetPosition context vesselSingleStatisticSource vesselSingleStatisticSink newPosition avatarId
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
            avatarSingleMetricSink
            avatarSingleMetricSource
            Metric.Moved 
            1UL
        avatarId
        |> Eat 
            context
            avatarInventorySink
            avatarInventorySource
            avatarShipmateSource
            avatarSingleMetricSink
            avatarSingleMetricSource

    let private SetPrimaryStatistic
            (context : AvatarSetPrimaryStatisticContext)
            (identifier                    : ShipmateStatisticIdentifier)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        Shipmate.TransformStatistic 
            context
            identifier 
            (Statistic.SetCurrentValue amount >> Some) 
            avatarId
            Primary

    let SetMoney (context : AvatarSetPrimaryStatisticContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Money 

    let SetReputation (context : AvatarSetPrimaryStatisticContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation 

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
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
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
                    shipmateSingleStatisticSource 
                    shipmateSingleStatisticSink 
                    ((GetReputation 
                        context
                        shipmateSingleStatisticSource 
                        avatarId) + 
                            reputationCostForAbandoningAJob) 
                    
                avatarId
                |> IncrementMetric 
                    context
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    Metric.AbandonedJob
                avatarJobSink avatarId None)

    let CompleteJob
            (context : AvatarCompleteJobContext)
            (avatarJobSink                 : AvatarJobSink)
            (avatarJobSource               : AvatarJobSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (avatarId:string)
            : unit =
        match avatarId |> avatarJobSource with
        | Some job ->
            SetReputation 
                context
                shipmateSingleStatisticSource 
                shipmateSingleStatisticSink 
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
                avatarSingleMetricSink
                avatarSingleMetricSource
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
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
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
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
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
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
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
            avatarSingleMetricSink
            avatarSingleMetricSource
            Metric.CleanedHull
