﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type MessagePurger = string -> unit

type AvatarMessageSink = string -> string -> unit
type AvatarShipmateSource = string -> ShipmateIdentifier list
type AvatarInventorySource = string -> AvatarInventory
type AvatarInventorySink = string -> AvatarInventory -> unit

module Avatar =
    let Create 
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (rationItemSource                : RationItemSource)
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (avatarId                        : string)
            : Avatar =
        Vessel.Create vesselStatisticTemplateSource vesselStatisticSink avatarId
        Shipmate.Create 
            shipmateStatisticTemplateSource 
            shipmateSingleStatisticSink
            rationItemSource 
            shipmateRationItemSink 
            avatarId 
            Primary
        {
            Job = None
            Metrics = Map.empty
        }

    let GetPosition
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
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            : float option =
        VesselStatisticIdentifier.Speed
        |> vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    let GetHeading
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            : float option =
        VesselStatisticIdentifier.Heading
        |> vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    let SetPosition 
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
            (metric : Metric) 
            (amount : uint64) 
            (avatar : Avatar) 
            : Avatar =
        let newValue =
            avatar.Metrics
            |> Map.tryFind metric
            |> Option.defaultValue 0UL
            |> (+) amount
        {avatar with
            Metrics = 
                avatar.Metrics 
                |> Map.add metric newValue
                |> Map.filter (fun _ v -> v>0UL)}

    let private IncrementMetric 
            (metric : Metric) 
            (avatar : Avatar) 
            : Avatar =
        let rateOfIncrement = 1UL
        avatar
        |> AddMetric metric rateOfIncrement

    let private Eat
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarInventorySink           : AvatarInventorySink)
            (avatarId                      : string)
            (avatar                        : Avatar) 
            : Avatar =
        let inventory, eaten =
            ((avatarInventorySource avatarId, 0UL), avatarShipmateSource avatarId)
            ||> List.fold 
                (fun (inventory,metric) identifier -> 
                    let updateInventory, ate =
                        Shipmate.Eat
                            shipmateRationItemSource 
                            shipmateSingleStatisticSource
                            shipmateSingleStatisticSink
                            inventory 
                            avatarId 
                            identifier
                    (updateInventory, if ate then metric+1UL else metric)) 
        inventory
        |> avatarInventorySink avatarId
        avatar
        |> AddMetric Metric.Ate eaten

    
    let GetCurrentFouling
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
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            : float =
        let currentValue = GetCurrentFouling vesselSingleStatisticSource avatarId
        let currentSpeed = GetSpeed vesselSingleStatisticSource avatarId |> Option.get
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
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarInventorySink           : AvatarInventorySink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (avatarId                      : string)
            (avatar                        : Avatar) 
            : Avatar =
        let actualSpeed = 
            avatarId 
            |> GetEffectiveSpeed vesselSingleStatisticSource
        let actualHeading = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading 
            |> Option.map Statistic.GetCurrentValue 
            |> Option.get
        Vessel.Befoul vesselSingleStatisticSource vesselSingleStatisticSink avatarId
        let avatarPosition = GetPosition vesselSingleStatisticSource avatarId |> Option.get
        let newPosition = ((avatarPosition |> fst) + System.Math.Cos(actualHeading) * actualSpeed, (avatarPosition |> snd) + System.Math.Sin(actualHeading) * actualSpeed)
        SetPosition vesselSingleStatisticSource vesselSingleStatisticSink newPosition avatarId
        TransformShipmates
            avatarShipmateSource
            (fun identifier -> 
                Shipmate.TransformStatistic 
                    shipmateSingleStatisticSource
                    shipmateSingleStatisticSink
                    ShipmateStatisticIdentifier.Turn 
                    (Statistic.ChangeCurrentBy 1.0 >> Some)
                    avatarId
                    identifier)
            avatarId
        avatar
        |> AddMetric Metric.Moved 1UL
        |> Eat 
            shipmateRationItemSource 
            shipmateSingleStatisticSource
            shipmateSingleStatisticSink
            avatarShipmateSource
            avatarInventorySource
            avatarInventorySink
            avatarId

    let SetJob 
            (job    : Job) 
            (avatar : Avatar) 
            : Avatar =
        {avatar with Job = job |> Some}

    let private SetPrimaryStatistic
            (identifier                    : ShipmateStatisticIdentifier)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        Shipmate.TransformStatistic 
            shipmateSingleStatisticSource
            shipmateSingleStatisticSink
            identifier 
            (Statistic.SetCurrentValue amount >> Some) 
            avatarId
            Primary

    let SetMoney = SetPrimaryStatistic ShipmateStatisticIdentifier.Money 

    let SetReputation = SetPrimaryStatistic ShipmateStatisticIdentifier.Reputation 


    let private GetPrimaryStatistic 
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

    let GetMoney = GetPrimaryStatistic ShipmateStatisticIdentifier.Money

    let GetReputation = GetPrimaryStatistic ShipmateStatisticIdentifier.Reputation
    
    let AbandonJob 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarId: string)
            (avatar : Avatar) 
            : Avatar =
        let reputationCostForAbandoningAJob = -1.0
        avatar.Job
        |> Option.fold 
            (fun a _ -> 
                SetReputation 
                    shipmateSingleStatisticSource 
                    shipmateSingleStatisticSink 
                    ((GetReputation 
                        shipmateSingleStatisticSource 
                        avatarId) + 
                            reputationCostForAbandoningAJob) 
                    avatarId
                {
                    a with 
                        Job = None
                }
                |> IncrementMetric Metric.AbandonedJob) avatar

    let CompleteJob 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarId:string)
            (avatar : Avatar) 
            : Avatar =
        match avatar.Job with
        | Some job ->
            SetReputation 
                shipmateSingleStatisticSource 
                shipmateSingleStatisticSink 
                ((GetReputation 
                    shipmateSingleStatisticSource 
                    avatarId) + 
                        1.0)
                avatarId
            Shipmate.TransformStatistic 
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                ShipmateStatisticIdentifier.Money 
                (Statistic.ChangeCurrentBy job.Reward >> Some)
                avatarId
                Primary
            {avatar with 
                Job = None
            }
            |> AddMetric Metric.CompletedJob 1UL
        | _ -> avatar

    let EarnMoney 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                ((GetMoney shipmateSingleStatisticSource avatarId) + amount)
                avatarId

    let SpendMoney 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                ((GetMoney shipmateSingleStatisticSource avatarId) - amount)
                avatarId

    let GetItemCount 
            (avatarInventorySource : AvatarInventorySource)
            (item                  : uint64) 
            (avatarId              : string) 
            : uint64 =
        match avatarId |> avatarInventorySource |> Map.tryFind item with
        | Some x -> x
        | None -> 0UL

    let AddInventory 
            (avatarInventorySource : AvatarInventorySource)
            (avatarInventorySink   : AvatarInventorySink)
            (item                  : uint64) 
            (quantity              : uint64) 
            (avatarId              : string) 
            : unit =
        let newQuantity = (avatarId |> GetItemCount avatarInventorySource item) + quantity
        avatarId
        |> avatarInventorySource
        |> Map.add item newQuantity
        |> avatarInventorySink avatarId

    let AddMessages 
            (avatarMessageSink : AvatarMessageSink)
            (messages          : string list) 
            (avatarId          : string) 
            : unit =
        messages
        |> List.iter (avatarMessageSink avatarId)


    let GetUsedTonnage
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
            (avatarShipmateSource          : AvatarShipmateSource)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (avatarId                      : string)
            (side                          : Side) 
            (avatar                        : Avatar) : Avatar =
        Vessel.TransformFouling vesselSingleStatisticSource vesselSingleStatisticSink avatarId side (fun x-> {x with CurrentValue = x.MinimumValue})
        TransformShipmates 
            avatarShipmateSource 
            (Shipmate.TransformStatistic
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                ShipmateStatisticIdentifier.Turn 
                (Statistic.ChangeCurrentBy 1.0 >> Some)
                avatarId)
            avatarId
        avatar
        |> IncrementMetric Metric.CleanedHull
