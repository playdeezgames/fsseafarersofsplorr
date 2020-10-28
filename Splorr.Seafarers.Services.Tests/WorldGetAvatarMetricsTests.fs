module WorldGetAvatarMetricsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarMetrics.It gets avatar metrics.`` () =
    let calledGetAvatarMetric = ref false
    let context = Contexts.TestContext()
    (context :> AvatarMetric.GetContext).avatarMetricSource := Spies.Source(calledGetAvatarMetric, Map.empty)
    let actual = 
        World.GetAvatarMetrics
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(Map.empty, actual)
    Assert.IsTrue(calledGetAvatarMetric.Value)


