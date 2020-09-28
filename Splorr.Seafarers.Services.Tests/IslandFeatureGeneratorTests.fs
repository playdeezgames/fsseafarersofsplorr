module IslandFeatureGeneratorTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TestIslandFeatureGeneratorGenerateContext(random) =
    interface Utility.RandomContext with
        member _.random : Random = random

[<Test>]
let ``Generate.It generates the presence of a feature based on the weights of the feature generator.`` () =
    [
        (  1.0,   1.0,     10, true)
        ( 10.0,   1.0,    100, true)
        (  1.0,  10.0,   1000, false)
        (100.0,   1.0,  10000, true)
        (  1.0, 100.0, 100000, false)
    ]
    |> List.iter
        (fun (feature, featureless, seed, expected)->
            let givenGenerator:IslandFeatureGenerator =
                {
                    FeatureWeight = feature
                    FeaturelessWeight = featureless
                }
            let context = TestIslandFeatureGeneratorGenerateContext(Random(seed))

            let actual = IslandFeature.Create context givenGenerator
            Assert.AreEqual(expected, actual))


