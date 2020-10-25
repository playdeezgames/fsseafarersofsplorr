module WorldGetIslandFeaturesTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandFeatures.It retrieves the island features for a given island.`` () =
    let calledGetIslandFeatures = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetFeaturesContext).islandFeatureSource := 
        Spies.Source(calledGetIslandFeatures, [ IslandFeatureIdentifier.Dock ])
    let actual = 
        World.GetIslandFeatures
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual([ IslandFeatureIdentifier.Dock ], actual)
    Assert.IsTrue(calledGetIslandFeatures.Value)


