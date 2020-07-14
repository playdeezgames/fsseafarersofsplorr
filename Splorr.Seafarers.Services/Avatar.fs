namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Avatar =
    let Create(position:Location): Avatar =
        {
            Messages = []
            Position = position
            Speed = 1.0
            Heading = 0.0
            ViewDistance = 10.0
            DockDistance = 1.0
            Money = 0.0
            Reputation = 0.0
            Job = None
            Inventory = Map.empty
            Satiety = Statistic.Create (0.0, 100.0) 100.0
            Health = Statistic.Create (0.0, 100.0) 100.0
            Turn = Statistic.Create (0.0, 15000.0) 0.0
            RationItem = 1u
            Metrics = Map.empty
            Vessel = Vessel.Create 100.0
        }

    let SetSpeed (speed:float) (avatar:Avatar) : Avatar = //TODO: make speed a statistic
        let clampedSpeed = 
            match speed with
            | x when x < 0.0 -> 0.0
            | x when x > 1.0 -> 1.0
            | x -> x
        {avatar with Speed = clampedSpeed}

    let SetHeading (heading:Dms) (avatar:Avatar) : Avatar =
        {avatar with Heading = heading |> Dms.ToFloat}

    let RemoveInventory (item:uint) (quantity:uint32) (avatar:Avatar) : Avatar =
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

    let private TransformSatiety (transform:Statistic -> Statistic) (avatar:Avatar) : Avatar =
        {avatar with Satiety = avatar.Satiety |> transform}

    let private TransformHealth (transform:Statistic -> Statistic) (avatar:Avatar) : Avatar =
        {avatar with Health = avatar.Health |> transform}

    let AddMetric (metric:Metric) (amount:uint) (avatar:Avatar) : Avatar =
        let newValue =
            avatar.Metrics
            |> Map.tryFind metric
            |> Option.defaultValue 0u
            |> (+) amount
        {avatar with
            Metrics = avatar.Metrics |> Map.add metric newValue}

    let private IncrementMetric (metric:Metric) (avatar:Avatar) : Avatar =
        let rateOfIncrement = 1u
        avatar
        |> AddMetric metric rateOfIncrement

    let private Eat (avatar:Avatar) : Avatar =
        let satietyDecrease = -1.0
        let satietyIncrease = 1.0
        let rationConsumptionRate = 1u
        match avatar.Inventory.TryFind avatar.RationItem with
        | Some count when count > 0u ->
            avatar
            |> IncrementMetric Metric.Ate
            |> RemoveInventory avatar.RationItem rationConsumptionRate
            |> TransformSatiety (Statistic.ChangeBy satietyIncrease)
        | _ ->
            if avatar.Satiety.CurrentValue > avatar.Satiety.MinimumValue then
                avatar
                |> TransformSatiety (Statistic.ChangeBy (satietyDecrease))
            else
                avatar
                |> TransformHealth (Statistic.ChangeBy (satietyDecrease))

    let GetEffectiveSpeed (avatar:Avatar) : float =
        (avatar.Speed * (1.0 - avatar.Vessel.Fouling.CurrentValue))

    let Move(avatar: Avatar) : Avatar =
        let actualSpeed = avatar |> GetEffectiveSpeed
        let newPosition = ((avatar.Position |> fst) + System.Math.Cos(avatar.Heading) * actualSpeed, (avatar.Position |> snd) + System.Math.Sin(avatar.Heading) * actualSpeed)
        {
            avatar with 
                Position = newPosition
                Turn = avatar.Turn |> Statistic.ChangeBy 1.0
                Vessel = avatar.Vessel |> Vessel.Befoul
        }
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

    let GetItemCount (item:uint) (avatar:Avatar) : uint32 =
        match avatar.Inventory.TryFind item with
        | Some x -> x
        | None -> 0u

    let AddInventory (item:uint) (quantity:uint32) (avatar:Avatar) : Avatar =
        let newQuantity = (avatar |> GetItemCount item) + quantity
        {avatar with Inventory = avatar.Inventory |> Map.add item newQuantity}

    let (|ALIVE|DEAD|) (avatar:Avatar) =
        if avatar.Health.CurrentValue <= avatar.Health.MinimumValue then
            DEAD    
        else
            ALIVE

    let ClearMessages (avatar:Avatar) : Avatar =
        {avatar with Messages = []}

    let AddMessages (messages:string list) (avatar:Avatar) : Avatar =
        {avatar with Messages = List.append avatar.Messages messages}

    let GetUsedTonnage (items:Map<uint, ItemDescriptor>) (avatar:Avatar) : float =
        (0.0, avatar.Inventory)
        ||> Map.fold
            (fun result item quantity -> 
                let d = items.[item]
                result + (quantity |> float) * d.Tonnage)

    let CleanHull (avatar:Avatar) : Avatar =
        {avatar with 
            Vessel = 
                {avatar.Vessel with Fouling = {avatar.Vessel.Fouling with CurrentValue = 0.0}}
            Turn = avatar.Turn |> Statistic.ChangeBy 1.0}
