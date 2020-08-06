namespace Splorr.Seafarers.Services

open Splorr.Seafarers.Models
open System

module Angle =
    let private Tau = Math.PI * 2.0
    let private DegreesInACircle = 360.0
    let private RadiansPerDegree = Tau / DegreesInACircle
    let private DegreesPerRadian = DegreesInACircle / Tau

    let private Normalize 
            (radians : float) 
            : float =
        match Math.Atan2(Math.Sin(radians), Math.Cos(radians)) with
        | r when r < 0.0 -> r + Tau
        | r -> r

    let ToRadians =
        (*) RadiansPerDegree
        >> Normalize

    let ToString =
        sprintf "%.2f\u00b0"

    let ToDegrees =
        Normalize
        >> (*) DegreesPerRadian
