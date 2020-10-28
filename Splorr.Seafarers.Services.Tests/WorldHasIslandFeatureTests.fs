module WorldHasIslandFeatureTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``HasIslandFeature.It gets whether or not an island feature is present at a given island.`` () =
    let calledGetIslandHandFeature = ref false
    let context = Contexts.TestContext()
    (context :> Island.HasFeatureContext).islandSingleFeatureSource := Spies.Source(calledGetIslandHandFeature, true)
    let actual = 
        World.HasIslandFeature
            context
            IslandFeatureIdentifier.DarkAlley
            Dummies.ValidIslandLocation
    Assert.AreEqual(true, actual)
    Assert.IsTrue(calledGetIslandHandFeature.Value)

