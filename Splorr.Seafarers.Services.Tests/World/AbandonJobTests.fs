module WorldAbandonJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldAbandonJobContext
        (avatarJobSink,
        avatarJobSource,
        avatarMessageSink,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource) =
    interface World.AbandonJobContext with
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
    interface AvatarJob.AbandonContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
    interface AvatarShipmates.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarMessages.AddContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface World.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface AvatarMetric.AddContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface ShipmateStatistic.PutContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
    interface ShipmateStatistic.GetContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource


[<Test>]
let ``AbandonJob.It adds a message when the avatar has no job.`` () =
    let input = ""
    let expectedMessage = "You have no job to abandon."
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, _: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let context = 
        TestWorldAbandonJobContext
            (avatarJobSink,
            avatarJobSource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            (Fixtures.Common.Mock.AvatarSingleMetricSink [Metric.AcceptedJob, 1UL]),
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> World.AbandonJobContext
    input
    |> World.AbandonJob
        context

[<Test>]
let ``AbandonJob.It adds a messages and abandons the job when the avatar has a a job`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let expectedMessage = "You abandon your job."
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create(-100.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(-1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (actual) =
        Assert.AreEqual(None, actual)
    let avatarJobSource (_) =
        {
            FlavorText  = ""
            Reward      = 0.0
            Destination = (0.0,0.0)
        } 
        |> Some
    let avatarSingleMetricSource (_) (metric:Metric) : uint64 =
        match metric with 
        | Metric.AbandonedJob ->
            0UL
        | _ ->
            Assert.Fail(metric.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestWorldAbandonJobContext
            (avatarJobSink,
            avatarJobSource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            (Fixtures.Common.Mock.AvatarSingleMetricSink [Metric.AbandonedJob, 1UL]),
            avatarSingleMetricSource,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> World.AbandonJobContext
    input
    |> World.AbandonJob
        context



