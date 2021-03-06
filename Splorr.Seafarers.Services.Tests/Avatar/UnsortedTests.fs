module AvatarTests

open System
open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open AvatarTestFixtures
open Tarot

let private inputAvatarId = "avatar"

type TestAvatarAddMessagesContext(avatarMessageSink) =
    interface AvatarMessages.AddContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink

type TestAvatarAddMetricContext
        (avatarSingleMetricSink,
        avatarSingleMetricSource) =
    interface AvatarMetric.AddContext with
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
    let context = TestAvatarAddMessagesContext(avatarMessageSink) :> AvatarMessages.AddContext
    input
    |> AvatarMessages.Add context inputMessages


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
            avatarSingleMetricSource) :> AvatarMetric.AddContext
    input
    |> AvatarMetric.Add
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
            avatarSingleMetricSource) :> AvatarMetric.AddContext
    input
    |> AvatarMetric.Add 
        context
        inputMetric 
        inputValue

type TestAvatarGetIslandFeatureContext(avatarIslandFeatureSource) =
    interface AvatarIslandFeature.GetContext with
        member this.avatarIslandFeatureSource: AvatarIslandFeatureSource = avatarIslandFeatureSource

[<Test>]
let ``GetIslandFeature.It retrieves none when the avatar is at sea.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let mutable called = false
    let avatarIslandFeatureSource (_) =
        called <- true
        None
    let context = TestAvatarGetIslandFeatureContext(avatarIslandFeatureSource) :> AvatarIslandFeature.GetContext
    let expected : AvatarIslandFeature option = None
    let actual = AvatarIslandFeature.Get context givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
 
type TestAvatarGetMetricsContext
        (avatarMetricSource) =
    interface ServiceContext
    interface AvatarMetric.GetContext with
        member this.avatarMetricSource: AvatarMetricSource = avatarMetricSource
[<Test>]
let ``GetMetrics.It calls the AvatarMetricSource in the context.`` () =
    let mutable called = false
    let avatarMetricSource (_) =
        called<-true
        Map.empty
    let context = 
        TestAvatarGetMetricsContext
            (avatarMetricSource) :> ServiceContext
    let expected = Map.empty
    let actual =
        AvatarMetric.Get
            context
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)

type TestAvatarGetMessagesContext
       (avatarMessageSource) =
   interface ServiceContext
   interface AvatarMessages.GetContext with
       member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
[<Test>]
let ``GetMessages.It calls the AvatarMetricSource in the context.`` () =
   let mutable called = false
   let avatarMessageSource (_) =
       called<-true
       []
   let context = 
       TestAvatarGetMessagesContext
           (avatarMessageSource) :> ServiceContext
   let expected = []
   let actual =
       AvatarMessages.Get
           context
           Fixtures.Common.Dummy.AvatarId
   Assert.AreEqual(expected, actual)
   Assert.IsTrue(called)

type TestAvatarGetInventoryContext
       (avatarInventorySource) =
   interface ServiceContext
   interface AvatarInventory.GetInventoryContext with
       member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
[<Test>]
let ``GetInventory.It calls the AvatarInventorySource in the context.`` () =
   let mutable called = false
   let avatarInventorySource (_) =
       called<-true
       Map.empty
   let context = 
       TestAvatarGetInventoryContext
           (avatarInventorySource) :> ServiceContext
   let expected = Map.empty
   let actual =
       AvatarInventory.GetInventory
           context
           Fixtures.Common.Dummy.AvatarId
   Assert.AreEqual(expected, actual)
   Assert.IsTrue(called)

    