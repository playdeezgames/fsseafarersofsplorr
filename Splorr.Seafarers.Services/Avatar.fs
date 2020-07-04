namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Avatar =
    let Create(position:Location): Avatar =
        {
            Position = position
            Speed = 1.0
            Heading = 0.0
            ViewDistance = 10.0
            DockDistance = 1.0
            Money = 0.0
            Reputation = 0.0
            Job = None
            Inventory = Map.empty
        }

    let SetSpeed (speed:float) (avatar:Avatar) : Avatar =
        let clampedSpeed = 
            match speed with
            | x when x < 0.0 -> 0.0
            | x when x > 1.0 -> 1.0
            | x -> x
        {avatar with Speed = clampedSpeed}

    let SetHeading (heading:Dms) (avatar:Avatar) : Avatar =
        {avatar with Heading = heading |> Dms.ToFloat}

    let Move(avatar: Avatar) : Avatar =
        {
            avatar with 
                Position = ((avatar.Position |> fst) + System.Math.Cos(avatar.Heading) * avatar.Speed, (avatar.Position |> snd) + System.Math.Sin(avatar.Heading) * avatar.Speed)
        }

    let SetJob (job: Job) (avatar:Avatar) : Avatar =
        {avatar with Job = job |> Some}

    let AbandonJob (avatar:Avatar) : Avatar =
        avatar.Job
        |> Option.fold 
            (fun a _ -> {a with Job = None; Reputation = a.Reputation - 1.0}) avatar

    let CompleteJob (avatar:Avatar) : Avatar =
        match avatar.Job with
        | Some job ->
            {avatar with 
                Job = None
                Money = avatar.Money + job.Reward
                Reputation = avatar.Reputation + 1.0}
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

    let GetItemCount (item:Item) (avatar:Avatar) : uint32 =
        match avatar.Inventory.TryFind item with
        | Some x -> x
        | None -> 0u

    let AddInventory (item:Item) (quantity:uint32) (avatar:Avatar) : Avatar =
        let newQuantity = (avatar |> GetItemCount item) + quantity
        {avatar with Inventory = avatar.Inventory |> Map.add item newQuantity}

    let RemoveInventory (item:Item) (quantity:uint32) (avatar:Avatar) : Avatar =
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
