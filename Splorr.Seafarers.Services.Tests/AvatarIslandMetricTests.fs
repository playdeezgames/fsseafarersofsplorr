module AvatarIslandMetricTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TestAvatarIslandMetricGetContext(avatarIslandSingleMetricSource) =
    interface ServiceContext
    interface AvatarIslandMetric.GetContext with
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
[<Test>]
let ``Get.It calls AvatarIslandSingleMetricSource on the context.`` () =
    let mutable gotMetric = false
    let avatarIslandSingleMetricSource (_) (_) (_) =
        gotMetric <- true
        None
    let context = TestAvatarIslandMetricGetContext(avatarIslandSingleMetricSource)
    let expected = None
    let actual = 
        AvatarIslandMetric.Get
            context
            Fixtures.Common.Dummy.AvatarId
            Fixtures.Common.Dummy.IslandLocation
            AvatarIslandMetricIdentifier.LastVisit
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(gotMetric)

type TestAvatarIslandMetricPutContext(avatarIslandSingleMetricSink) =
    interface ServiceContext
    interface AvatarIslandMetric.PutContext with
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
[<Test>]
let ``Put.It calls AvatarIslandSingleMetricSink on the context.`` () =
    let mutable putMetric = false
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        putMetric<-true
    let context = TestAvatarIslandMetricPutContext(avatarIslandSingleMetricSink)
    AvatarIslandMetric.Put
        context
        Fixtures.Common.Dummy.AvatarId
        Fixtures.Common.Dummy.IslandLocation
        AvatarIslandMetricIdentifier.LastVisit
        1UL
    Assert.IsTrue(putMetric)



