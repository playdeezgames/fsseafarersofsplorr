﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Avatar =
    let Create(position:Location): Avatar =
        {
            Position = position
            Speed = 1.0
            Heading = 0.0
            ViewDistance = 10.0
            DockDistance = 1.0
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
        