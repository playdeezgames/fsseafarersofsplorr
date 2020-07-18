namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Avatar =
    let SetStatistic (identifier:StatisticIdentifier) (statistic:Statistic option) (avatar:Avatar) : Avatar =
        match statistic with
        | Some stat ->
            {avatar with Statistics = avatar.Statistics |> Map.add identifier stat}
        | None -> 
            {avatar with Statistics = avatar.Statistics |> Map.remove identifier}

    let GetStatistic (identifier:StatisticIdentifier) (avatar:Avatar) : Statistic option =
        avatar.Statistics
        |> Map.tryFind identifier   

    let TransformStatistic (identifier:StatisticIdentifier) (transform:Statistic -> Statistic option) (avatar:Avatar) : Avatar =
        avatar
        |> GetStatistic identifier
        |> Option.fold
            (fun a s -> 
                a 
                |> SetStatistic identifier (s |> transform) ) avatar

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
            RationItem = 1UL
            Metrics = Map.empty
            Vessel = Vessel.Create 100.0
            Statistics = Map.empty
        }
        |> SetStatistic StatisticIdentifier.Satiety (Statistic.Create (0.0, 100.0) 100.0 |> Some)
        |> SetStatistic StatisticIdentifier.Health (Statistic.Create (0.0, 100.0) 100.0 |> Some)
        |> SetStatistic StatisticIdentifier.Turn (Statistic.Create (0.0, 50000.0) 0.0 |> Some)


    let SetSpeed (speed:float) (avatar:Avatar) : Avatar = //TODO: make speed a statistic
        let clampedSpeed = 
            match speed with
            | x when x < 0.0 -> 0.0
            | x when x > 1.0 -> 1.0
            | x -> x
        {avatar with Speed = clampedSpeed}

    let SetHeading (heading:Dms) (avatar:Avatar) : Avatar =
        {avatar with Heading = heading |> Dms.ToFloat}

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

    let private TransformSatiety (transform:Statistic -> Statistic) =
        TransformStatistic StatisticIdentifier.Satiety (transform >> Some)

    let private TransformTurn (transform:Statistic -> Statistic) =
        TransformStatistic StatisticIdentifier.Turn (transform >> Some)

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
            |> TransformSatiety (Statistic.ChangeCurrentBy satietyIncrease)
        | _ ->
            if avatar.Statistics.[StatisticIdentifier.Satiety].CurrentValue > avatar.Statistics.[StatisticIdentifier.Satiety].MinimumValue then
                avatar
                |> TransformSatiety (Statistic.ChangeCurrentBy (satietyDecrease))
            else
                avatar
                |> TransformTurn (Statistic.ChangeMaximumBy (satietyDecrease))

    let GetEffectiveSpeed (avatar:Avatar) : float =
        let currentValue =
            avatar.Vessel.Fouling
            |> Map.fold (fun a _ v->a+v.CurrentValue) 0.0
        (avatar.Speed * (1.0 - currentValue))

    let Move(avatar: Avatar) : Avatar =
        let actualSpeed = avatar |> GetEffectiveSpeed
        let newPosition = ((avatar.Position |> fst) + System.Math.Cos(avatar.Heading) * actualSpeed, (avatar.Position |> snd) + System.Math.Sin(avatar.Heading) * actualSpeed)
        {
            avatar with 
                Position = newPosition
                Vessel = avatar.Vessel |> Vessel.Befoul
        }
        |> TransformStatistic StatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some)
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
        if avatar.Statistics.[StatisticIdentifier.Health].CurrentValue <= avatar.Statistics.[StatisticIdentifier.Health].MinimumValue then
            ZERO_HEALTH    
        elif avatar.Statistics.[StatisticIdentifier.Turn].CurrentValue >= avatar.Statistics.[StatisticIdentifier.Turn].MaximumValue then
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
        |> TransformStatistic StatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some)
        |> IncrementMetric Metric.CleanedHull
