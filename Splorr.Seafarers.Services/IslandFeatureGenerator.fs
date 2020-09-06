namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type IslandFeatureGeneratorGenerateContext =
    abstract member random : Random

module IslandFeatureGenerator =
    let Generate 
            (context    : IslandFeatureGeneratorGenerateContext) 
            (generator : IslandFeatureGenerator)
            : bool = 
        let total = 
            generator.FeaturelessWeight + generator.FeatureWeight

        let generated = 
            context.random.NextDouble() * total 

        generated < generator.FeatureWeight
