namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Location =
    let DistanceTo 
            (from   : Location) 
            (``to`` : Location) 
            : float =
        let deltaX = (``to`` |> fst) - (from |> fst)
        let deltaY = (``to`` |> snd) - (from |> snd)
        sqrt((deltaX ** 2.0) + (deltaY ** 2.0))

    let ScaleBy 
            (scale    : float) 
            (location : Location) 
            : Location =
        ((location |> fst) * scale,(location |> snd) * scale)

    let HeadingTo 
            (from   : Location) 
            (``to`` : Location) 
            : float =
        let deltaX = (``to`` |> fst) - (from |> fst)
        let deltaY = (``to`` |> snd) - (from |> snd)
        atan2 deltaY deltaX
