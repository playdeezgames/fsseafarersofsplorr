module WorldGetAvatarIslandMetricTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarIslandMetric.It retrieves an avatar island metric.`` () =
    let calledGetAvatarIslandMetric = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource := Spies.Source(calledGetAvatarIslandMetric, Some 99UL)
    let actual =
        World.GetAvatarIslandMetric
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
            AvatarIslandMetricIdentifier.LastVisit
    Assert.AreEqual(Some 99UL, actual)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)

