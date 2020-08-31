namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

module IslandFeatureGenerator =
    let Generate 
            (random    : Random) 
            (generator : IslandFeatureGenerator)
            : bool = 
        let total = 
            generator.FeaturelessWeight + generator.FeatureWeight

        let generated = 
            random.NextDouble() * total 

        generated < generator.FeatureWeight
