namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type MessagePurger = string -> unit

type AvatarMessageSink = string -> string -> unit

module Avatar =
    let TransformShipmate 
            (transform : Shipmate->Shipmate) 
            (index     : ShipmateIdentifier) 
            (avatar    : Avatar) 
            : Avatar =
        {avatar with
            Shipmates = 
                avatar.Shipmates
                |> Map.map
                    (fun idx item -> 
                        if idx = index then
                            item |> transform
                        else
                            item)}

    let Create 
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (rationItemSource                : RationItemSource)
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (avatarId                        : string)
            : Avatar =
        Vessel.Create vesselStatisticTemplateSource vesselStatisticSink avatarId
        {
            Job = None
            Inventory = Map.empty
            Metrics = Map.empty
            Shipmates =
                Map.empty
                |> Map.add Primary (Shipmate.Create shipmateStatisticTemplateSource rationItemSource shipmateRationItemSink avatarId Primary)
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

    let private PurgeInventory (avatar:Avatar) : Avatar =
        {avatar with Inventory = avatar.Inventory |> Map.filter (fun _ v -> v>0u)}

    let RemoveInventory 
            (item     : uint64) 
            (quantity : uint32) 
            (avatar   : Avatar) 
            : Avatar =
        match avatar.Inventory.TryFind item with
        | Some count ->
            if count > quantity then
                {avatar with 
                    Inventory = 
                        avatar.Inventory 
                        |> Map.add item (count-quantity)}
            else
                {avatar with 
                    Inventory = 
                        avatar.Inventory 
                        |> Map.remove item}
        | None ->
            avatar
        |> PurgeInventory

    let AddMetric 
            (metric : Metric) 
            (amount : uint) 
            (avatar : Avatar) 
            : Avatar =
        let newValue =
            avatar.Metrics
            |> Map.tryFind metric
            |> Option.defaultValue 0u
            |> (+) amount
        {avatar with
            Metrics = 
                avatar.Metrics 
                |> Map.add metric newValue
                |> Map.filter (fun _ v -> v>0u)}

    let private IncrementMetric 
            (metric : Metric) 
            (avatar : Avatar) 
            : Avatar =
        let rateOfIncrement = 1u
        avatar
        |> AddMetric metric rateOfIncrement

    let private Eat
            (shipmateRationItemSource : ShipmateRationItemSource)
            (avatarId                 : string)
            (avatar                   : Avatar) 
            : Avatar =
        let shipmates, inventory, eaten =
            ((Map.empty, avatar.Inventory, 0u), avatar.Shipmates)
            ||> Map.fold 
                (fun (shipmates:Map<ShipmateIdentifier,Shipmate>,inventory,metric) identifier shipmate -> 
                    let mate, updateInventory, ate =
                        Shipmate.Eat shipmateRationItemSource inventory avatarId identifier shipmate
                    (shipmates |> Map.add identifier mate,updateInventory, if ate then metric+1u else metric)) 
        {avatar with 
            Shipmates = shipmates
            Inventory = inventory}
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
            (transform : Shipmate -> Shipmate) 
            (avatar    : Avatar) 
            : Avatar =
        avatar.Shipmates
        |> Map.fold
            (fun a i _ ->
                a |> TransformShipmate transform i) avatar

    let Move
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (shipmateRationItemSource    : ShipmateRationItemSource)
            (avatarId                    : string)
            (avatar                      : Avatar) 
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
        avatar
        |> TransformShipmates (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some))
        |> AddMetric Metric.Moved 1u
        |> Eat shipmateRationItemSource avatarId

    let SetJob 
            (job    : Job) 
            (avatar : Avatar) 
            : Avatar =
        {avatar with Job = job |> Some}

    let private SetPrimaryStatistic
            (identifier: ShipmateStatisticIdentifier)
            (amount : float) 
            (avatar : Avatar) 
            : Avatar =
        avatar
        |> TransformShipmate 
            (Shipmate.TransformStatistic 
                identifier 
                (Statistic.SetCurrentValue amount >> Some)) Primary

    let SetMoney = SetPrimaryStatistic ShipmateStatisticIdentifier.Money 

    let SetReputation = SetPrimaryStatistic ShipmateStatisticIdentifier.Reputation 


    let private GetPrimaryStatistic 
            (identifier : ShipmateStatisticIdentifier) 
            (avatar     : Avatar) 
            : float =
        avatar.Shipmates
        |> Map.tryFind Primary
        |> Option.map 
            (fun shipmate ->
                shipmate.Statistics
                |> Map.tryFind identifier
                |> Option.map (fun statistic -> statistic.CurrentValue)
                |> Option.defaultValue 0.0)
        |> Option.defaultValue 0.0

    let GetMoney = GetPrimaryStatistic ShipmateStatisticIdentifier.Money

    let GetReputation = GetPrimaryStatistic ShipmateStatisticIdentifier.Reputation
    
    let AbandonJob 
            (avatar : Avatar) 
            : Avatar =
        let reputationCostForAbandoningAJob = -1.0
        avatar.Job
        |> Option.fold 
            (fun a _ -> 
                {
                    a with 
                        Job = None
                }
                |> SetReputation ((a |> GetReputation) + reputationCostForAbandoningAJob)
                |> IncrementMetric Metric.AbandonedJob) avatar

    let CompleteJob 
            (avatar : Avatar) 
            : Avatar =
        match avatar.Job with
        | Some job ->
            {avatar with 
                Job = None
            }
            |> SetReputation ((avatar |> GetReputation) + 1.0)
            |> TransformShipmate 
                (Shipmate.TransformStatistic 
                    ShipmateStatisticIdentifier.Money 
                    (Statistic.ChangeCurrentBy job.Reward >> Some)) Primary
            |> AddMetric Metric.CompletedJob 1u
        | _ -> avatar

    let EarnMoney 
            (amount : float) 
            (avatar : Avatar) 
            : Avatar =
        if amount <= 0.0 then
            avatar
        else
            avatar |> SetMoney ((avatar |> GetMoney) + amount)

    let SpendMoney 
            (amount : float) 
            (avatar : Avatar) 
            : Avatar =
        if amount < 0.0 then
            avatar
        else
            avatar |> SetMoney ((avatar |> GetMoney) - amount)

    let GetItemCount 
            (item   : uint64) 
            (avatar : Avatar) 
            : uint32 =
        match avatar.Inventory.TryFind item with
        | Some x -> x
        | None -> 0u

    let AddInventory 
            (item     : uint64) 
            (quantity : uint32) 
            (avatar   : Avatar) 
            : Avatar =
        let newQuantity = (avatar |> GetItemCount item) + quantity
        {avatar with Inventory = avatar.Inventory |> Map.add item newQuantity}

    let AddMessages 
            (avatarMessageSink : AvatarMessageSink)
            (messages          : string list) 
            (avatarId          : string) 
            : unit =
        messages
        |> List.iter (avatarMessageSink avatarId)


    let GetUsedTonnage 
            (items  : Map<uint64, ItemDescriptor>) //TODO: to source
            (avatar : Avatar) 
            : float =
        (0.0, avatar.Inventory)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    let CleanHull 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarId                    : string)
            (side                        : Side) 
            (avatar                      : Avatar) : Avatar =
        Vessel.TransformFouling vesselSingleStatisticSource vesselSingleStatisticSink avatarId side (fun x-> {x with CurrentValue = x.MinimumValue})
        avatar
        |> TransformShipmates (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some))
        |> IncrementMetric Metric.CleanedHull
