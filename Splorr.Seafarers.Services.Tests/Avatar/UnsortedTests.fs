module AvatarTests

open System
open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open AvatarTestFixtures
open Tarot

let private inputAvatarId = "avatar"

type TestAvatarAddMessagesContext(avatarMessageSink) =
    interface AvatarAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink

type TestAvatarAddMetricContext
        (avatarSingleMetricSink,
        avatarSingleMetricSource) =
    interface AvatarAddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource

(*HERE BE THE END OF THE TEST CONTEXTS!*)

[<Test>]
let ``AddMessages.It adds messages to a given avatar.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let firstMessage = "Here's a message!"
    let secondMessage = "And another one!"
    let inputMessages = [firstMessage; secondMessage]
    let avatarMessageSink (_) (message:string) =
        match message with
        | x when x = firstMessage || x = secondMessage ->
            ()
        | _ ->
            Assert.Fail("Got an unexpected message.")
    let context = TestAvatarAddMessagesContext(avatarMessageSink) :> AvatarAddMessagesContext
    input
    |> Avatar.AddMessages context inputMessages


[<Test>]
let ``AddMetric.It creates a metric value when there is no previously existing metric value in the avatar's table.`` () = 
    let input = Fixtures.Common.Dummy.AvatarId
    let inputMetric = Metric.Moved
    let inputValue = 1UL
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.Moved
        | Metric.Ate
        | Metric.Starved ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarAddMetricContext
            ((Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.Moved, 1UL)]),
            avatarSingleMetricSource) :> AvatarAddMetricContext
    input
    |> Avatar.AddMetric
        context
        inputMetric 
        inputValue

[<Test>]
let ``AddMetric.It adds to a metric value when there is a previously existing metric value in the avatar's table.`` () = 
    let input = Fixtures.Common.Dummy.AvatarId
    let inputMetric = Metric.Moved
    let inputValue = 1UL
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.Moved
        | Metric.Ate
        | Metric.Starved ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarAddMetricContext
            ((Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.Moved, 1UL)]),
            avatarSingleMetricSource) :> AvatarAddMetricContext
    input
    |> Avatar.AddMetric 
        context
        inputMetric 
        inputValue

type TestAvatarGetIslandFeatureContext(avatarIslandFeatureSource) =
    interface Avatar.GetIslandFeatureContext with
        member this.avatarIslandFeatureSource: AvatarIslandFeatureSource = avatarIslandFeatureSource

[<Test>]
let ``GetIslandFeature.It retrieves none when the avatar is at sea.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let mutable called = false
    let avatarIslandFeatureSource (_) =
        called <- true
        None
    let context = TestAvatarGetIslandFeatureContext(avatarIslandFeatureSource) :> Avatar.GetIslandFeatureContext
    let expected : AvatarIslandFeature option = None
    let actual = Avatar.GetIslandFeature context givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
    


    