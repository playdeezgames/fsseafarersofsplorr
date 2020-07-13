namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Vessel =
    let Create (tonnage:float) : Vessel =
        {Tonnage = tonnage}

