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
        