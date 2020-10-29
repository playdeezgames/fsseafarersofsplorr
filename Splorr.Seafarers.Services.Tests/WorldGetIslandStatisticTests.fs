module WorldGetIslandStatisticTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandStatistic.It gets the given statistic for the island at the given location.`` () =
    let  calledGetIslandStatistic = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetStatisticContext).islandSingleStatisticSource := 
        Spies.Source(calledGetIslandStatistic, Some {MaximumValue = 100.0; MinimumValue = 0.0; CurrentValue=50.0})
    let actual = 
        World.GetIslandStatistic
            context
            IslandStatisticIdentifier.CareenDistance
            Dummies.ValidIslandLocation
    Assert.AreEqual(Some {MaximumValue = 100.0; MinimumValue = 0.0; CurrentValue=50.0}, actual)
    Assert.IsTrue(calledGetIslandStatistic.Value)

