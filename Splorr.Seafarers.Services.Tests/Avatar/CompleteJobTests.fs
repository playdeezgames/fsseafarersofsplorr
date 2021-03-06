module AvatarCompleteJobTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestAvatarCompleteJobContext 
        (avatarJobSink,
        avatarJobSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource) =
    interface AvatarJob.CompleteContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
    interface AvatarShipmates.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarMetric.AddContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface ShipmateStatistic.PutContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
    interface ShipmateStatistic.GetContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``CompleteJob.It does nothing when the given avatar has no job.`` () =
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let context = 
        TestAvatarCompleteJobContext 
            (avatarJobSink,
            avatarJobSource,
            Fixtures.Common.Fake.AvatarSingleMetricSink,
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> AvatarJob.CompleteContext
    AvatarJob.Complete
        context
        Fixtures.Common.Dummy.AvatarId

[<Test>]
let ``CompleteJob.It sets job to None, adds reward money, adds reputation and metrics when the given avatar has a job.`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) (0.0) |> Some
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (-100.0, 100.0) (0.0) |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.CompletedJob ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let inputJob = 
        {
            Reward = 10.0
            FlavorText=""
            Destination=(0.0, 0.0)
        }
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(inputJob.Reward, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (job: Job option) =
        let expected : Job option = None
        Assert.AreEqual(expected, job)
    let avatarJobSource (_) =
        inputJob 
        |> Some
    let context = 
        TestAvatarCompleteJobContext
            (avatarJobSink,
            avatarJobSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.CompletedJob, 1UL)]),
            avatarSingleMetricSource,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> AvatarJob.CompleteContext
    AvatarJob.Complete
        context
        Fixtures.Common.Dummy.AvatarId


