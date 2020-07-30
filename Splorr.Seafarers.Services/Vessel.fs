namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Vessel =
    let Create 
            (tonnage:float) 
            : Vessel =
        {
            Tonnage = tonnage
            Fouling = 
                Map.empty
                |> Map.add Port (Statistic.Create (0.0, 0.25) 0.0)
                |> Map.add Starboard (Statistic.Create (0.0, 0.25) 0.0)
            FoulRate = 0.001 //TODO: dont hardcode, and base it on vessel type
        }

    let TransformFouling 
            (side      : Side) 
            (transform : Statistic -> Statistic) 
            (vessel    : Vessel) : Vessel =
        let fouling = vessel.Fouling.[side] |> transform
        {vessel with
            Fouling =
                vessel.Fouling
                |> Map.add side fouling}
    
    let Befoul 
            (vessel:Vessel) 
            : Vessel =
        vessel
        |> TransformFouling Port (Statistic.ChangeCurrentBy (vessel.FoulRate/2.0))
        |> TransformFouling Starboard (Statistic.ChangeCurrentBy (vessel.FoulRate/2.0))

