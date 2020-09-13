module WorldAbandonJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

type TestWorldAbandonJobContext
        (avatarJobSink,
        avatarJobSource,
        avatarMessageSink,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource) =
    interface WorldAbandonJobContext with
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
    interface AvatarAbandonJobContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface WorldAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface AvatarAddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
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
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
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
            (avatarExpectedMessageSink expectedMessage),
            (assertAvatarSingleMetricSink [Metric.AcceptedJob, 1UL]),
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> WorldAbandonJobContext
    input
    |> World.AbandonJob
        context

[<Test>]
let ``AbandonJob.It adds a messages and abandons the job when the avatar has a a job`` () =
    let input = avatarId
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
    let context = 
        TestWorldAbandonJobContext
            (avatarJobSink,
            avatarJobSource,
            (avatarExpectedMessageSink expectedMessage),
            (assertAvatarSingleMetricSink [Metric.AbandonedJob, 1UL]),
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> WorldAbandonJobContext
    input
    |> World.AbandonJob
        context



