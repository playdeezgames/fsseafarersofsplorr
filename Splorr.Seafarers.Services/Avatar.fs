namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Avatar =
    let TransformShipmate 
            (transform : Shipmate->Shipmate) 
            (index     : uint) 
            (avatar    : Avatar) 
            : Avatar =
        {avatar with
            Shipmates = 
                avatar.Shipmates
                |> Array.mapi 
                    (fun idx item -> 
                        if (idx |> uint) = index then
                            item |> transform
                        else
                            item)}

    let Create 
            (vesselStatisticTemplateSource : VesselStatisticTemplateSource)
            (vesselStatisticSink           : VesselStatisticSink)
            (avatarId                      : string)
            (statisticDescriptors          : ShipmateStatisticTemplate list) //TODO: from source?
            (rationItems                   : uint64 list) 
            (position                      : Location)
            : Avatar =
        Vessel.Create vesselStatisticTemplateSource vesselStatisticSink avatarId
        {
            Messages = []
            Position = position
            Heading = 0.0
            Money = 0.0
            Reputation = 0.0
            Job = None
            Inventory = Map.empty
            Metrics = Map.empty
            Shipmates =
                [| Shipmate.Create rationItems statisticDescriptors |]
        }

    let GetSpeed
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string)
            : float option =
        VesselStatisticIdentifier.Speed
        |> vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

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
            (heading : float) 
            (avatar  : Avatar) 
            : Avatar = //TODO: make heading a vessel statistic
        {avatar with Heading = heading |> Angle.ToRadians}

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
                {avatar with Inventory = avatar.Inventory |> Map.add item (count-quantity)}
            else
                {avatar with Inventory = avatar.Inventory |> Map.remove item}
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
            (avatar : Avatar) 
            : Avatar =
        let shipmates, inventory, eaten =
            avatar.Shipmates
            |> Array.fold 
                (fun (s:Shipmate array,i,m) v -> 
                    let mate, inv, ate =
                        Shipmate.Eat i v
                    ([| mate |] |> Array.append s,inv, if ate then m+1u else m)) ([||], avatar.Inventory, 0u)
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
        [0u..(avatar.Shipmates.Length-1) |> uint]
        |> List.fold
            (fun a i ->
                a |> TransformShipmate transform i) avatar

    let Move
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarId                    : string)
            (avatar                      : Avatar) 
            : Avatar =
        let actualSpeed = avatarId |> GetEffectiveSpeed vesselSingleStatisticSource
        Vessel.Befoul vesselSingleStatisticSource vesselSingleStatisticSink avatarId
        let newPosition = ((avatar.Position |> fst) + System.Math.Cos(avatar.Heading) * actualSpeed, (avatar.Position |> snd) + System.Math.Sin(avatar.Heading) * actualSpeed)
        {
            avatar with 
                Position = newPosition
        }
        |> TransformShipmates (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some))
        |> AddMetric Metric.Moved 1u
        |> Eat

    let SetJob 
            (job    : Job) 
            (avatar : Avatar) 
            : Avatar =
        {avatar with Job = job |> Some}

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
                        Reputation = a.Reputation + reputationCostForAbandoningAJob
                }
                |> IncrementMetric Metric.AbandonedJob) avatar

    let CompleteJob 
            (avatar : Avatar) 
            : Avatar =
        match avatar.Job with
        | Some job ->
            {avatar with 
                Job = None
                Money = avatar.Money + job.Reward
                Reputation = avatar.Reputation + 1.0
            }
            |> AddMetric Metric.CompletedJob 1u
        | _ -> avatar

    let private SetMoney //TODO: should money be a statistic? yes
            (amount : float) 
            (avatar : Avatar) 
            : Avatar =
        {avatar with 
            Money = if amount < 0.0 then 0.0 else amount}

    let EarnMoney 
            (amount : float) 
            (avatar : Avatar) 
            : Avatar =
        if amount <= 0.0 then
            avatar
        else
            avatar |> SetMoney (avatar.Money + amount)

    let SpendMoney 
            (amount : float) 
            (avatar : Avatar) 
            : Avatar =
        if amount < 0.0 then
            avatar
        else
            avatar |> SetMoney (avatar.Money - amount)

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

    let (|ALIVE|ZERO_HEALTH|OLD_AGE|)  //TODO: active patterns, while neat... just no. get rid of it and make a function instead.
            (avatar : Avatar) =
        if avatar.Shipmates.[0].Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue <= avatar.Shipmates.[0].Statistics.[ShipmateStatisticIdentifier.Health].MinimumValue then
            ZERO_HEALTH    
        elif avatar.Shipmates.[0].Statistics.[ShipmateStatisticIdentifier.Turn].CurrentValue >= avatar.Shipmates.[0].Statistics.[ShipmateStatisticIdentifier.Turn].MaximumValue then
            OLD_AGE
        else
            ALIVE

    let ClearMessages 
            (avatar : Avatar) 
            : Avatar =
        {avatar with Messages = []}

    let AddMessages 
            (messages : string list) 
            (avatar   : Avatar) 
            : Avatar =
        {avatar with Messages = List.append avatar.Messages messages}

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
