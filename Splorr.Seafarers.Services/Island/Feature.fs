namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module IslandFeature =
    let internal Create 
            (context   : CommonContext) 
            (generator : IslandFeatureGenerator)
            : bool = 
        Map.empty
        |> Map.add true generator.FeatureWeight
        |> Map.add false generator.FeaturelessWeight
        |> Utility.GenerateFromWeightedValues context
        |> Option.get
