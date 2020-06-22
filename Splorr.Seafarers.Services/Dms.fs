namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models
open System

module Dms =
    let private Normalize(radians:float) : float =
        match Math.Atan2(Math.Sin(radians), Math.Cos(radians)) with
        | r when r<0.0 -> r + Math.PI*2.0
        | r -> r

    let ToFloat (dms: Dms) : float =
        (((((dms.Seconds/60.0)+(dms.Minutes|>float))/60.0)+(dms.Degrees|>float))*Math.PI/180.0)
        |> Normalize

    let ToString (dms:Dms) : string =
        sprintf "%d\u00b0%d'%f\"" dms.Degrees dms.Minutes dms.Seconds

    let ToDms(radians: float) : Dms =
        let radians = radians |> Normalize
        let degrees: float = radians * 180.0 / Math.PI
        let minutes: float = (degrees-Math.Floor(degrees)) * 60.0
        let seconds: float = (minutes-Math.Floor(minutes)) * 60.0
        {
            Degrees = (degrees |> int)
            Minutes = (minutes |> int)
            Seconds = seconds
        }
