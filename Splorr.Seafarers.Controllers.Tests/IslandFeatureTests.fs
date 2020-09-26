﻿module IslandFeatureTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Tarot

module private Mock =
    let AvatarIslandFeatureSource (location:Location, feature:IslandFeatureIdentifier) (_) =
        {
            featureId = feature
            location = location
        }
        |> Some


type TestIslandFeatureRunContext
        (avatarGamblingHandSink,
        avatarGamblingHandSource,
        avatarIslandFeatureSink,
        avatarIslandFeatureSource,
        avatarMessageSink,
        avatarMessageSource,
        islandSingleFeatureSource,
        islandSingleNameSource,
        islandSingleStatisticSource,
        shipmateSingleStatisticSource) =
    interface AvatarMessages.GetContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
    interface Island.GetNameContext with
        member _.islandSingleNameSource : IslandSingleNameSource = islandSingleNameSource
    interface Island.HasFeatureContext with
        member _.islandSingleFeatureSource : IslandSingleFeatureSource = islandSingleFeatureSource
    interface AvatarMessages.AddContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface World.AddMessagesContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface AvatarGamblingHand.GetContext with
        member _.avatarGamblingHandSource : AvatarGamblingHandSource = avatarGamblingHandSource
    interface AvatarGamblingHand.DealContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink
        member _.random : Random = Fixtures.Common.Dummy.Random
    interface Avatar.EnterIslandFeatureContext with
        member this.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink
        member this.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
    interface AvatarGamblingHand.FoldContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink
    interface World.HasDarkAlleyMinimumStakesContext with
        member _.shipmateSingleStatisticSource : ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        member _.islandSingleStatisticSource : IslandSingleStatisticSource = islandSingleStatisticSource
        member _.avatarIslandFeatureSource : AvatarIslandFeatureSource = avatarIslandFeatureSource

[<Test>]
let ``Run.It should return InPlay when the given island does not exist.`` () =
    let givenLocation = invalidLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected=
        givenAvatarId
        |> Gamestate.InPlay
        |> Some
    let avatarGamblingHandSource (_) =
        Assert.Fail "avatarGamblingHandSource"
        None
    let avatarGamblingHandSink (_) (_) =
        Assert.Fail("avatarGamblingHandSink")
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Fixtures.Common.Fake.AvatarIslandFeatureSource,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSourceStub,
            islandSingleNameSourceStub,
            islandSingleStatisticSourceStub,
            shipmateSingleStatisticSourceStub) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            Fixtures.Common.Fake.CommandSource
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It should return InPlay state when the given island exists but does not have the given feature.`` () =
    let givenLocation = noDarkAlleyIslandLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected=
        givenAvatarId
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) = Some ""
    let avatarGamblingHandSource (_) =
        Assert.Fail "avatarGamblingHandSource"
        None
    let avatarGamblingHandSink (_) (_) =
        Assert.Fail("avatarGamblingHandSink")
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Fixtures.Common.Fake.AvatarIslandFeatureSource,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSourceStub,
            islandSingleNameSource,
            islandSingleStatisticSourceStub,
            shipmateSingleStatisticSourceStub) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            Fixtures.Common.Fake.CommandSource
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It should return InPlay state when dark alley exists and the player is gambling and gives an invalid command.`` () =
    let givenLocation = Fixtures.Common.Dummy.IslandLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let givenHand = (Minor (Wands, Rank.Ace),Minor (Wands, Rank.Deuce),Minor (Wands, Rank.Three)) |> Some
    let expected=
        ("Maybe try 'help'?", givenAvatarId
            |> Gamestate.InPlay)
        |> Gamestate.ErrorMessage
        |> Some
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleNameSource (_) = Some ""
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0, 5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (5.0, 5.0) 5.0 |> Some
    let avatarGamblingHandSource (_) =
        givenHand
    let avatarGamblingHandSink (_) (_) =
        Assert.Fail("avatarGamblingHandSink")
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Fixtures.Common.Fake.AvatarIslandFeatureSource,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Gamble))
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It should quit the gambling game when dark alley exists and the player is gambling and gives no bet command.`` () =
    let givenLocation = Fixtures.Common.Dummy.IslandLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let givenHand = (Minor (Wands, Rank.Ace),Minor (Wands, Rank.Deuce),Minor (Wands, Rank.Three)) |> Some
    let expected=
        givenAvatarId
        |> Gamestate.InPlay
        |> Some
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleNameSource (_) = Some ""
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0, 5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (5.0, 5.0) 5.0 |> Some
    let avatarGamblingHandSource (_) =
        givenHand
    let mutable called = false
    let avatarGamblingHandSink (_) (hand:AvatarGamblingHand option) =
        called <- true
        Assert.AreEqual(None, hand)
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Fixtures.Common.Fake.AvatarIslandFeatureSource,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (None |> Command.Bet |> Some))
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.True(called)


[<Test>]
let ``Run.It should return InPlay state when dark alley exists but the player does not have minimum gambling stakes.`` () =
    let givenLocation = Fixtures.Common.Dummy.IslandLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected=
        givenAvatarId
        |> Gamestate.InPlay
        |> Some
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleNameSource (_) = Some ""
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0, 5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (4.0, 4.0) 4.0 |> Some
    let avatarGamblingHandSource (_) =
        None
    let avatarGamblingHandSink (_) (_) =
        Assert.Fail("avatarGamblingHandSink")
    let avatarIslandFeatureSink (f:AvatarIslandFeature option,_) =
        match f with
        | Some feature ->
            Assert.AreEqual(IslandFeatureIdentifier.Dock, feature.featureId)
            Assert.AreEqual(givenLocation, feature.location)
        | _ ->
            Assert.Fail("avatarIslandFeatureSink")
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSink,
            Mock.AvatarIslandFeatureSource (givenLocation, IslandFeatureIdentifier.DarkAlley),
            avatarMessageSinkExpected ["Come back when you've got more money!"],
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            Fixtures.Common.Fake.CommandSource
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.When in the dark alley, the leave command will take the player back to the dock.`` () =
    let givenLocation = Fixtures.Common.Dummy.IslandLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleStatisticSource (_) (_) = 
        Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = 
        Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        Fixtures.Common.Dummy.AvatarId
        |> Gamestate.InPlay
        |> Some
    let avatarGamblingHandSource (_) =
        None
    let avatarGamblingHandSink (_) (_) =
        Assert.Fail("avatarGamblingHandSink")
    let avatarIslandFeatureSink (f:AvatarIslandFeature option,_) =
        match f with
        | Some feature ->
            Assert.AreEqual(IslandFeatureIdentifier.Dock, feature.featureId)
            Assert.AreEqual(givenLocation, feature.location)
        | _ ->
            Assert.Fail("avatarIslandFeatureSink")

    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSink,
            Mock.AvatarIslandFeatureSource (givenLocation, IslandFeatureIdentifier.DarkAlley),
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Leave))
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.When in the dark alley, the gamble command will deal to the player some cards and enter the gambling minigame.`` () =
    let givenLocation = Fixtures.Common.Dummy.IslandLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleStatisticSource (_) (_) = 
        Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = 
        Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        Fixtures.Common.Dummy.AvatarId
        |> Gamestate.InPlay
        |> Some
    let avatarGamblingHandSource (_) =
        None
    let mutable handDealt : bool = false
    let avatarGamblingHandSink (_) (hand:AvatarGamblingHand option) =
        handDealt <- true
        Assert.IsTrue(hand.IsSome)
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Mock.AvatarIslandFeatureSource (givenLocation, IslandFeatureIdentifier.DarkAlley),
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Gamble))
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(handDealt)

[<Test>]
let ``Run.When in the dark alley, the help command will take the player to the help state for the dark alley.`` () =
    let givenLocation = Fixtures.Common.Dummy.IslandLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        Fixtures.Common.Dummy.AvatarId
        |> Gamestate.InPlay
        |> Gamestate.Help
        |> Some
    let avatarGamblingHandSource (_) =
        None
    let avatarGamblingHandSink (_) (_) =
        Assert.Fail("avatarGamblingHandSink")
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Mock.AvatarIslandFeatureSource (givenLocation, IslandFeatureIdentifier.DarkAlley),
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Help))
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.When in the dark alley but not gambling, the an invalid command gives you an error message.`` () =
    let givenLocation = Fixtures.Common.Dummy.IslandLocation
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        ("Maybe try 'help'?",
            Fixtures.Common.Dummy.AvatarId
            |> Gamestate.InPlay)
        |> Gamestate.ErrorMessage
        |> Some
    let avatarGamblingHandSource (_) =
        None
    let avatarGamblingHandSink (_) (_) =
        Assert.Fail("avatarGamblingHandSink")
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Mock.AvatarIslandFeatureSource (givenLocation, IslandFeatureIdentifier.DarkAlley),
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) 
        :> ServiceContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake None)
            sinkDummy
            givenLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    