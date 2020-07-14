namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Vessel =
    let Create (tonnage:float) : Vessel =
        {
            Tonnage = tonnage
            Fouling = Statistic.Create (0.0, 0.5) 0.0
            FoulRate = 0.001 //TODO: dont hardcode, and base it on vessel type
        }
    
    let Befoul (vessel:Vessel) : Vessel =
        {vessel with
            Fouling = vessel.Fouling |> Statistic.ChangeBy vessel.FoulRate}

