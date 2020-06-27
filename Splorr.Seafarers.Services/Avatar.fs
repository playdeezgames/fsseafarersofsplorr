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