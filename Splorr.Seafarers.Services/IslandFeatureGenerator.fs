namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

module IslandFeatureGenerator =
    type GenerateContext =
        inherit OperatingContext
        abstract member random : Random
    
    let Generate 
            (context   : OperatingContext) 
            (generator : IslandFeatureGenerator)
            : bool = 
        let context = context :?> GenerateContext
        let total = 
            generator.FeaturelessWeight + generator.FeatureWeight

        let generated = 
            context.random.NextDouble() * total 

        generated < generator.FeatureWeight
