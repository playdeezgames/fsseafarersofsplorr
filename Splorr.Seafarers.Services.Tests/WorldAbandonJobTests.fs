module WorldAbandonJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

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
    input
    |> World.AbandonJob
        avatarJobSink
        avatarJobSource
        (avatarExpectedMessageSink expectedMessage)
        (assertAvatarSingleMetricSink [Metric.AcceptedJob, 1UL])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource

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
    input
    |> World.AbandonJob
        avatarJobSink
        avatarJobSource
        (avatarExpectedMessageSink expectedMessage)
        (assertAvatarSingleMetricSink [Metric.AbandonedJob, 1UL])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource



