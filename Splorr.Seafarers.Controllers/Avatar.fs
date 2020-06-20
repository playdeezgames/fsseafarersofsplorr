namespace Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

module Avatar =
    let Create(): Avatar =
        {
            X = 0.0
            Y = 0.0
            Speed = 1.0
            Heading = 0.0
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
                X = avatar.X + System.Math.Cos(avatar.Heading) * avatar.Speed
                Y = avatar.Y + System.Math.Sin(avatar.Heading) * avatar.Speed
        }
        