namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

module IslandFeature =
    type CreateContext =
        inherit ServiceContext
        abstract member random : Random
    
    let Create 
            (context   : ServiceContext) 
            (generator : IslandFeatureGenerator)
            : bool = 
        let context = context :?> CreateContext
        let total = 
            generator.FeaturelessWeight + generator.FeatureWeight

        let generated = 
            context.random.NextDouble() * total 

        generated < generator.FeatureWeight
