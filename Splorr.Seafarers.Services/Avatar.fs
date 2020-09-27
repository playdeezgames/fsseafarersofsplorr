namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type AvatarShipmateSource = string -> ShipmateIdentifier list
type AvatarInventorySource = string -> AvatarInventory
type AvatarInventorySink = string -> AvatarInventory -> unit
type AvatarJobSink = string -> Job option -> unit
type AvatarIslandFeatureSink = AvatarIslandFeature option * string -> unit
type AvatarIslandFeatureSource = string -> AvatarIslandFeature option

module Avatar =
    type CreateContext =
        inherit ServiceContext
        abstract member avatarJobSink : AvatarJobSink
    let Create 
            (context  : ServiceContext)
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
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetPosition
            (context  : ServiceContext)
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
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetSpeed
            (context  : ServiceContext)
            (avatarId : string)
            : float option =
        let context = context :?> GetSpeedContext
        VesselStatisticIdentifier.Speed
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    type GetHeadingContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetHeading
            (context  : ServiceContext)
            (avatarId : string)
            : float option =
        let context = context :?> GetHeadingContext
        VesselStatisticIdentifier.Heading
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue
    
    type SetPositionContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let SetPosition 
            (context  : ServiceContext)
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
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    let SetSpeed 
            (context  : ServiceContext)
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
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    let SetHeading 
            (context  : ServiceContext)
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
        inherit ServiceContext
        abstract member avatarInventorySource : AvatarInventorySource
        abstract member avatarInventorySink   : AvatarInventorySink
    let RemoveInventory 
            (context  : ServiceContext)
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
    
    let internal IncrementMetric 
            (context  : ServiceContext)
            (metric   : Metric) 
            (avatarId : string) 
            : unit =
        let rateOfIncrement = 1UL
        avatarId
        |> AvatarMetric.Add 
            context
            metric 
            rateOfIncrement

    type EatContext =
        inherit ServiceContext
        abstract member avatarInventorySink           : AvatarInventorySink
        abstract member avatarInventorySource         : AvatarInventorySource
        abstract member avatarShipmateSource          : AvatarShipmateSource
    let private Eat
            (context : ServiceContext)
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
            |> AvatarMetric.Add 
                context
                Metric.Ate 
                eaten
        if starved > 0UL then
            avatarId
            |> AvatarMetric.Add 
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
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetCurrentFouling
            (context : ServiceContext) =
        let context = context :?> GetCurrentFoulingContext
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetCurrentValue
    
    type GetMaximumFoulingContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetMaximumFouling
            (context : ServiceContext) =
        let context = context :?> GetMaximumFoulingContext
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetMaximumValue 

    let GetEffectiveSpeed 
            (context  : ServiceContext)
            (avatarId : string)
            : float =
        let currentValue = GetCurrentFouling context avatarId
        let currentSpeed = GetSpeed context avatarId |> Option.get
        (currentSpeed * (1.0 - currentValue))

    type TransformShipmatesContext =
        inherit ServiceContext
        abstract avatarShipmateSource : AvatarShipmateSource
    let TransformShipmates 
            (context   : ServiceContext)
            (transform : ShipmateIdentifier -> unit) 
            (avatarId  : string) 
            : unit =
        let context = context :?> TransformShipmatesContext
        avatarId
        |> context.avatarShipmateSource
        |> List.iter transform


    type MoveContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource
    let Move
            (context  : ServiceContext)
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
        |> AvatarMetric.Add 
            context
            Metric.Moved 
            1UL
        avatarId
        |> Eat 
            context

    let private SetPrimaryStatistic
            (context    : ServiceContext)
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

    let SetMoney (context : ServiceContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Money 

    let SetReputation (context : ServiceContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation 

    type GetPrimaryStatisticContext =
        inherit ServiceContext
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    let private GetPrimaryStatistic 
            (context : ServiceContext)
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

    let GetMoney (context:ServiceContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Money

    let GetReputation (context:ServiceContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation

    let EarnMoney 
            (context : ServiceContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) + amount)
                avatarId

    let SpendMoney 
            (context : ServiceContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) - amount)
                avatarId

    type GetItemCountContext =
        inherit ServiceContext
        abstract member avatarInventorySource : AvatarInventorySource
    let GetItemCount 
            (context : ServiceContext)
            (item                  : uint64) 
            (avatarId              : string) 
            : uint64 =
        let context = context :?> GetItemCountContext
        match avatarId |> context.avatarInventorySource |> Map.tryFind item with
        | Some x -> x
        | None -> 0UL

    type AddInventoryContext =
        inherit ServiceContext
        abstract member avatarInventorySink   : AvatarInventorySink
        abstract member avatarInventorySource : AvatarInventorySource
    let AddInventory 
            (context : ServiceContext)
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

    type GetUsedTonnageContext =
        inherit ServiceContext
        abstract member avatarInventorySource : AvatarInventorySource
    let GetUsedTonnage
            (context : ServiceContext)
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
        inherit ServiceContext
        abstract member avatarShipmateSource          : AvatarShipmateSource
    let CleanHull 
            (context : ServiceContext)
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



    type EnterIslandFeatureContext =
        inherit ServiceContext
        abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink
        abstract member islandSingleFeatureSource : IslandSingleFeatureSource
    let EnterIslandFeature
            (context  : ServiceContext)
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
        inherit ServiceContext
        abstract member avatarIslandFeatureSource : AvatarIslandFeatureSource
    let GetIslandFeature
            (context : ServiceContext)
            (avatarId: string)
            : AvatarIslandFeature option =
        let context = context :?> GetIslandFeatureContext
        context.avatarIslandFeatureSource avatarId

    type GetInventoryContext =
        inherit ServiceContext
        abstract member avatarInventorySource : AvatarInventorySource
    let GetInventory
            (context : ServiceContext)
            (avatarId : string)
            : AvatarInventory =
        (context :?> GetInventoryContext).avatarInventorySource avatarId


