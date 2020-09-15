module AvatarGamblingTests

open Splorr.Seafarers.Services
open System
open NUnit.Framework
open Tarot
open Splorr.Seafarers.Models

type TestAvatarGetGamblingHandContext (avatarGamblingHandSource) =
    interface AvatarGetGamblingHandContext with
        member _.avatarGamblingHandSource : AvatarGamblingHandSource = avatarGamblingHandSource

type TestAvatarDealGamblingHandContext(avatarGamblingHandSink, random) =
    interface AvatarDealGamblingHandContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink
        member _.random : Random = random

type TestAvatarEnterIslandFeatureContext
        (avatarIslandFeatureSink, 
        islandSingleFeatureSource,
        vesselSingleStatisticSource) =
    interface AvatarGetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarEnterIslandFeatureContext with
        member this.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
        member this.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink


[<Test>]
let ``GetGamblingHand.It retrieves a gambling hand for a given avatar.`` () =
    let expected =
        (Minor (Wands, Rank.Ace),Minor (Wands, Rank.Deuce),Minor (Wands, Rank.Three)) |> Some
    let mutable called : bool = false
    let avatarGamblingHandSource (_) =
        called <- true
        expected
    let context = TestAvatarGetGamblingHandContext (avatarGamblingHandSource) :> AvatarGetGamblingHandContext
    let actual =
        Avatar.GetGamblingHand
            context
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.True(called)
    
[<Test>]
let ``DealGamblingHand.It deals a new hand to the given avatar.`` () =
    let expectedHand : AvatarGamblingHand option =
        (Major Arcana.Empress,Major Arcana.WheelOfFortune,Minor (Pentacles, Rank.Four)) |> Some
    let mutable called : bool = false
    let avatarGamblingHandSink (_) (hand:AvatarGamblingHand option) =
        called <- true
        Assert.AreEqual(expectedHand, hand)
    let random : Random = Random(1000)
    let context = TestAvatarDealGamblingHandContext(avatarGamblingHandSink, random) :> AvatarDealGamblingHandContext 
    Avatar.DealGamblingHand
        context
        Fixtures.Common.Dummy.AvatarId
    Assert.True(called)

[<Test>]
let ``EnterIslandFeature.It does not enter the dark alley when one is not present.`` () =
    let inputLocation = (1.0, 2.0)
    let vesselSingleStatisticSource (_) (id:VesselStatisticIdentifier) =
        match id with
        | VesselStatisticIdentifier.PositionX ->
            Statistic.Create (0.0, 100.0) (inputLocation |> fst) |> Some
        | VesselStatisticIdentifier.PositionY ->
            Statistic.Create (0.0, 100.0) (inputLocation |> snd) |> Some
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "vesselSingleStatisticSource - %s")
            None
    let avatarIslandFeatureSink (_) =
        Assert.Fail("avatarIslandFeatureSink")
    let islandSingleFeatureSource (_) (_) =
        false
    let context = 
        TestAvatarEnterIslandFeatureContext
            (avatarIslandFeatureSink,
            islandSingleFeatureSource,
            vesselSingleStatisticSource) 
        :> AvatarEnterIslandFeatureContext
    Avatar.EnterIslandFeature
        context
        Fixtures.Common.Dummy.AvatarId
        inputLocation
        IslandFeatureIdentifier.DarkAlley


[<Test>]
let ``EnterIslandFeature.It enters the dark alley when one is present.`` () =
    let inputLocation = (1.0, 2.0)
    let vesselSingleStatisticSource (_) (id:VesselStatisticIdentifier) =
        match id with
        | VesselStatisticIdentifier.PositionX ->
            Statistic.Create (0.0, 100.0) (inputLocation |> fst) |> Some
        | VesselStatisticIdentifier.PositionY ->
            Statistic.Create (0.0, 100.0) (inputLocation |> snd) |> Some
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "vesselSingleStatisticSource - %s")
            None
    let mutable called = false
    let avatarIslandFeatureSink (feature: AvatarIslandFeature option, _) =
        match feature with 
        | Some f ->
            called <- true
            Assert.AreEqual(IslandFeatureIdentifier.DarkAlley, f.featureId)
            Assert.AreEqual(inputLocation, f.location)
        | _ -> 
            Assert.Fail("avatarIslandFeatureSink")
    let islandSingleFeatureSource (_) (_) =
        true
    let context = 
        TestAvatarEnterIslandFeatureContext
            (avatarIslandFeatureSink,
            islandSingleFeatureSource,
            vesselSingleStatisticSource) 
        :> AvatarEnterIslandFeatureContext
    Avatar.EnterIslandFeature
        context
        Fixtures.Common.Dummy.AvatarId
        inputLocation
        IslandFeatureIdentifier.DarkAlley
    Assert.True(called)


