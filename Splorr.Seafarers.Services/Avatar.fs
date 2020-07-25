namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Avatar =
    let TransformShipmate (transform:Shipmate->Shipmate) (index:uint) (avatar:Avatar) : Avatar =
        {avatar with
            Shipmates = 
                avatar.Shipmates
                |> Array.mapi 
                    (fun idx item -> 
                        if (idx |> uint) = index then
                            item |> transform
                        else
                            item)}

    let Create (viewDistance:float, dockDistance:float) (statisticDescriptors:AvatarStatisticTemplate list) (rationItems:uint64 list) (position:Location): Avatar =
        {
            Messages = []
            Position = position
            Speed = 1.0
            Heading = 0.0
            ViewDistance = viewDistance
            DockDistance = dockDistance
            Money = 0.0
            Reputation = 0.0
            Job = None
            Inventory = Map.empty
            Metrics = Map.empty
            Vessel = Vessel.Create 100.0
            Shipmates =
                [| Shipmate.Create rationItems statisticDescriptors |]
        }


    let SetSpeed (speed:float) (avatar:Avatar) : Avatar = //TODO: make speed a statistic
        let clampedSpeed = 
            match speed with
            | x when x < 0.0 -> 0.0
            | x when x > 1.0 -> 1.0
            | x -> x
        {avatar with Speed = clampedSpeed}

    let SetHeading (heading:float) (avatar:Avatar) : Avatar =
        {avatar with Heading = heading |> Dms.ToRadians}

    let RemoveInventory (item:uint64) (quantity:uint32) (avatar:Avatar) : Avatar =
        if quantity>0u then
            match avatar.Inventory.TryFind item with
            | Some count ->
                if count > quantity then
                    {avatar with Inventory = avatar.Inventory |> Map.add item (count-quantity)}
                else
                    {avatar with Inventory = avatar.Inventory |> Map.remove item}
            | None ->
                avatar
        else
            avatar

    let AddMetric (metric:Metric) (amount:uint) (avatar:Avatar) : Avatar =
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

    let private IncrementMetric (metric:Metric) (avatar:Avatar) : Avatar =
        let rateOfIncrement = 1u
        avatar
        |> AddMetric metric rateOfIncrement

    let private Eat (avatar:Avatar) : Avatar =
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

    let GetEffectiveSpeed (avatar:Avatar) : float =
        let currentValue =
            avatar.Vessel.Fouling
            |> Map.fold (fun a _ v->a+v.CurrentValue) 0.0
        (avatar.Speed * (1.0 - currentValue))

    let TransformShipmates (transform:Shipmate -> Shipmate) (avatar:Avatar) : Avatar =
        [0u..(avatar.Shipmates.Length-1) |> uint]
        |> List.fold
            (fun a i ->
                a |> TransformShipmate transform i) avatar

    let Move(avatar: Avatar) : Avatar =
        let actualSpeed = avatar |> GetEffectiveSpeed
        let newPosition = ((avatar.Position |> fst) + System.Math.Cos(avatar.Heading) * actualSpeed, (avatar.Position |> snd) + System.Math.Sin(avatar.Heading) * actualSpeed)
        {
            avatar with 
                Position = newPosition
                Vessel = avatar.Vessel |> Vessel.Befoul
        }
        |> TransformShipmates (Shipmate.TransformStatistic AvatarStatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some))
        |> AddMetric Metric.Moved 1u
        |> Eat

    let SetJob (job: Job) (avatar:Avatar) : Avatar =
        {avatar with Job = job |> Some}

    let AbandonJob (avatar:Avatar) : Avatar =
        let reputationCostForAbandoningAJob = -1.0
        avatar.Job
        |> Option.fold 
            (fun a _ -> 
                {a with Job = None; Reputation = a.Reputation + reputationCostForAbandoningAJob}
                |> IncrementMetric Metric.AbandonedJob) avatar

    let CompleteJob (avatar:Avatar) : Avatar =
        match avatar.Job with
        | Some job ->
            {avatar with 
                Job = None
                Money = avatar.Money + job.Reward
                Reputation = avatar.Reputation + 1.0}
            |> AddMetric Metric.CompletedJob 1u
        | _ -> avatar

    let private SetMoney (amount:float) (avatar:Avatar) : Avatar =
        {avatar with Money = if amount < 0.0 then 0.0 else amount}

    let EarnMoney (amount:float) (avatar:Avatar) : Avatar =
        if amount <= 0.0 then
            avatar
        else
            avatar |> SetMoney (avatar.Money + amount)

    let SpendMoney (amount:float) (avatar:Avatar) : Avatar =
        if amount < 0.0 then
            avatar
        else
            avatar |> SetMoney (avatar.Money - amount)

    let GetItemCount (item:uint64) (avatar:Avatar) : uint32 =
        match avatar.Inventory.TryFind item with
        | Some x -> x
        | None -> 0u

    let AddInventory (item:uint64) (quantity:uint32) (avatar:Avatar) : Avatar =
        let newQuantity = (avatar |> GetItemCount item) + quantity
        {avatar with Inventory = avatar.Inventory |> Map.add item newQuantity}

    let (|ALIVE|ZERO_HEALTH|OLD_AGE|) (avatar:Avatar) =
        if avatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Health].CurrentValue <= avatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Health].MinimumValue then
            ZERO_HEALTH    
        elif avatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].CurrentValue >= avatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].MaximumValue then
            OLD_AGE
        else
            ALIVE

    let ClearMessages (avatar:Avatar) : Avatar =
        {avatar with Messages = []}

    let AddMessages (messages:string list) (avatar:Avatar) : Avatar =
        {avatar with Messages = List.append avatar.Messages messages}

    let GetUsedTonnage (items:Map<uint64, ItemDescriptor>) (avatar:Avatar) : float =
        (0.0, avatar.Inventory)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    let CleanHull (side:Side) (avatar:Avatar) : Avatar =
        {avatar with 
            Vessel = 
                avatar.Vessel
                |> Vessel.TransformFouling side (fun x-> {x with CurrentValue = x.MinimumValue})}
        |> TransformShipmates (Shipmate.TransformStatistic AvatarStatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some))
        |> IncrementMetric Metric.CleanedHull
