module WorldGetStatisticTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetStatistic.It returns a world statistic.`` () =
    let called = ref false
    let context = Contexts.TestContext()
    (context :> WorldStatistic.GetStatisticContext).worldSingleStatisticSource := Spies.Source(called, {MaximumValue = 1.0; MinimumValue=0.0; CurrentValue=0.5})
    let actual =
        World.GetStatistic
            context
            WorldStatisticIdentifier.JobReward
    Assert.AreEqual(1.0, actual.MaximumValue)
    Assert.AreEqual(0.0, actual.MinimumValue)
    Assert.AreEqual(0.5, actual.CurrentValue)
    Assert.IsTrue(called.Value)



