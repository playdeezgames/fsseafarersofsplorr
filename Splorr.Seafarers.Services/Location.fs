namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Location =
    let DistanceTo (from:Location) (``to``:Location) : float =
        sqrt(((``to``|>fst)-(from|>fst))*((``to``|>fst)-(from|>fst))+((``to``|>snd)-(from|>snd))*((``to``|>snd)-(from|>snd)))

    let ScaleBy (scale:float) (location:Location) : Location =
        ((location|>fst) * scale,(location|>snd) * scale)

    let HeadingTo (from:Location) (``to``:Location) : float =
        System.Math.Atan2((``to`` |> snd)-(from |> snd),(``to`` |> fst)-(from |> fst))
