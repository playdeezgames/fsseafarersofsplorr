module AvatarAbandonJobTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestAvatarAbandonJobContext 
        (avatarJobSink,
        avatarJobSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource) =
    interface Avatar.AbandonJobContext with
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Avatar.AddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``AbandonJob.It does nothing when the given avatar has no job.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let context = 
        TestAvatarAbandonJobContext 
            (avatarJobSink,
            avatarJobSource,
            Fixtures.Common.Fake.AvatarSingleMetricSink,
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink, 
            Fixtures.Common.Fake.ShipmateSingleStatisticSource) :> Avatar.AbandonJobContext
    input
    |> Avatar.AbandonJob
        context

[<Test>]
let ``AbandonJob.It set job to None when the given avatar has a job.`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (-100.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier,statistic:Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(-1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (job:Job option) =
        let expected : Job option = None
        Assert.AreEqual(expected, job)
    let avatarJobSource (_) =
        {
            FlavorText  = ""
            Reward      = 0.0
            Destination = (0.0, 0.0)
        }
        |> Some
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.AbandonedJob ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarAbandonJobContext
            (avatarJobSink,
            avatarJobSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.AbandonedJob, 1UL)]),
            avatarSingleMetricSource,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> Avatar.AbandonJobContext
    Avatar.AbandonJob
        context
        Fixtures.Common.Dummy.AvatarId

