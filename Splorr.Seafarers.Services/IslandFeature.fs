namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

module IslandFeature =
    let Create 
            (context   : ServiceContext) 
            (generator : IslandFeatureGenerator)
            : bool = 
        Map.empty
        |> Map.add true generator.FeatureWeight
        |> Map.add false generator.FeaturelessWeight
        |> Utility.WeightedGenerator context
        |> Option.get
