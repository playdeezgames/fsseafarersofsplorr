namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models
open System

module Dms =
    let private Normalize(radians:float) : float =
        match Math.Atan2(Math.Sin(radians), Math.Cos(radians)) with
        | r when r<0.0 -> r + Math.PI*2.0
        | r -> r

    let ToRadians (degrees: float) : float =
        degrees*Math.PI/180.0
        |> Normalize

    let ToString (degrees:float) : string =
        sprintf "%.2f\u00b0" degrees

    let ToDegrees(radians: float) : float =
        let radians = radians |> Normalize
        radians * 180.0 / Math.PI
