namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

module IslandFeature =
    let Create 
            (context   : ServiceContext) 
            (generator : IslandFeatureGenerator)
            : bool = 
        let context = context :?> Utility.RandomContext
        let total = 
            generator.FeaturelessWeight + generator.FeatureWeight

        let generated = 
            context.random.NextDouble() * total 

        generated < generator.FeatureWeight
