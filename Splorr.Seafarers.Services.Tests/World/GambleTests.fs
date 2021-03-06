module GambleTests

open NUnit.Framework
open System
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type private WorldCanPlaceBetContext(shipmateSingleStatisticSource) =
    interface AvatarShipmates.GetPrimaryStatisticContext with
        member _.shipmateSingleStatisticSource : ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface World.CanPlaceBetContext

module private Mock =
    let ShipmateSingleStatisticSource (mockValue:Statistic option) (_) (_) (_) =
        mockValue

[<Test>]
let ``CanPlaceBet.It returns false when the avatar does not have enough money.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenAmount = 1.0
    let givenStatistic = Statistic.Create (0.0, 1000000.0) 0.0 |> Some
    let expected = false
    let context = 
        WorldCanPlaceBetContext
            (Mock.ShipmateSingleStatisticSource givenStatistic) :> World.CanPlaceBetContext 
    let actual =
        World.CanPlaceBet context givenAmount givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CanPlaceBet.It returns true when the avatar has enough money.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenAmount = 1.0
    let givenStatistic = Statistic.Create (0.0, 1000000.0) 1.0 |> Some
    let expected = true
    let context = 
        WorldCanPlaceBetContext
            (Mock.ShipmateSingleStatisticSource givenStatistic) :> World.CanPlaceBetContext 
    let actual =
        World.CanPlaceBet context givenAmount givenAvatarId
    Assert.AreEqual(expected, actual)

type TestWorldResolveHandContext
        (avatarGamblingHandSource,
        avatarIslandFeatureSource,
        avatarMessageSink,
        islandSingleStatisticSource,
        shipmateSingleStatisticSource) =
    interface AvatarShipmates.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarIslandFeature.GetContext with
        member this.avatarIslandFeatureSource: AvatarIslandFeatureSource = avatarIslandFeatureSource
    interface AvatarGamblingHand.GetContext with
        member this.avatarGamblingHandSource: AvatarGamblingHandSource = avatarGamblingHandSource
    interface World.CanPlaceBetContext
    interface AvatarMessages.AddContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface World.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface Island.GetStatisticContext with
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
    interface World.ResolveHandContext

[<Test>]
let ``ResolveHand.It will do nothing if the avatar is not in the dark alley.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenAmount = 5.0
    let avatarMessageSink (_) (_) =
        Assert.Fail("avatarMessageSink")
    let avatarGamblingHandSource (_) =
        Assert.Fail("avatarGamblingHandSource")
        None
    let mutable called = false
    let avatarIslandFeatureSource (_) =
        called <- true
        None
    let islandSingleStatisticSource (_) (_) =
        Assert.Fail("islandSingleStatisticSource")
        None
    let shipmateSingleStatisticSource (_) (_) (_) =
        Assert.Fail("shipmateSingleStatisticSource")
        None
    let context = 
        TestWorldResolveHandContext
            (avatarGamblingHandSource,
            avatarIslandFeatureSource,
            avatarMessageSink,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) :> World.ResolveHandContext
    World.ResolveHand
        context
        givenAmount
        givenAvatarId
    Assert.IsTrue(called)

[<Test>]
let ``ResolveHand.It will do nothing if the avatar is in the dark alley but has not been dealt a hand.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenAmount = 5.0
    let avatarMessageSink (_) (_) =
        Assert.Fail("avatarMessageSink")
    let mutable called = false
    let avatarGamblingHandSource (_) =
        called <- true
        None
    let islandSingleStatisticSource (_) (_) =
        Assert.Fail("islandSingleStatisticSource")
        None
    let shipmateSingleStatisticSource (_) (_) (_) =
        Assert.Fail("shipmateSingleStatisticSource")
        None
    let context = 
        TestWorldResolveHandContext
            (avatarGamblingHandSource,
            Fixtures.Common.Mock.AvatarIslandFeatureSource (IslandFeatureIdentifier.DarkAlley, Fixtures.Common.Dummy.IslandLocation),
            avatarMessageSink,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) :> World.ResolveHandContext
    World.ResolveHand
        context
        givenAmount
        givenAvatarId
    Assert.IsTrue(called)

[<Test>]
let ``ResolveHand.It will give an error message if the avatar does not have enough to place the given bet.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenAmount = 5.0
    let avatarGamblingHandSource (_) =
        Fixtures.Common.Dummy.GamblingHand
        |> Some
    let islandSingleStatisticSource (_) (_) =
        Assert.Fail("islandSingleStatisticSource")
        None
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000000.0) (0.0)
            |> Some
        | _ ->
            Assert.Fail("shipmateSingleStatisticSource")
            None
    let context = 
        TestWorldResolveHandContext
            (avatarGamblingHandSource,
            Fixtures.Common.Mock.AvatarIslandFeatureSource (IslandFeatureIdentifier.DarkAlley, Fixtures.Common.Dummy.IslandLocation),
            Fixtures.Common.Mock.AvatarMessageSink "You don't have enough money.",
            islandSingleStatisticSource,
            shipmateSingleStatisticSource) :> World.ResolveHandContext
    World.ResolveHand
        context
        givenAmount
        givenAvatarId

type TestHasDarkAlleyMinimumStakesContext
        (avatarIslandFeatureSource,
        islandSingleStatisticSource,
        shipmateSingleStatisticSource) =
    interface ServiceContext
    interface AvatarIslandFeature.GetContext with
        member this.avatarIslandFeatureSource: AvatarIslandFeatureSource = avatarIslandFeatureSource
    interface Island.GetStatisticContext with
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
    interface AvatarShipmates.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
[<Test>]
let ``HasDarkAlleyMinimumStakes.It returns false when the avatar is not at an island.`` () =
    let avatarIslandFeatureSource (_) =
        None
    let context = 
        TestHasDarkAlleyMinimumStakesContext
            (avatarIslandFeatureSource,
            Fixtures.Common.Fake.IslandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource)
    let actual =
        World.HasDarkAlleyMinimumStakes
            context
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(false, actual)

[<Test>]
let ``HasDarkAlleyMinimumStakes.It returns false when the avatar is at an island but not in the dark alley.`` () =
    let avatarIslandFeatureSource (_) =
        {
            featureId = IslandFeatureIdentifier.Dock
            location = Fixtures.Common.Dummy.IslandLocation
        }
        |> Some
    let context = 
        TestHasDarkAlleyMinimumStakesContext
            (avatarIslandFeatureSource,
            Fixtures.Common.Fake.IslandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource)
    let actual =
        World.HasDarkAlleyMinimumStakes
            context
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(false, actual)

[<Test>]
let ``HasDarkAlleyMinimumStakes.It returns false when the avatar is at an island and in the dark alley but does not have enough money.`` () =
    let avatarIslandFeatureSource (_) =
        {
            featureId = IslandFeatureIdentifier.DarkAlley
            location = Fixtures.Common.Dummy.IslandLocation
        }
        |> Some
    let islandSingleStatisticSource (_) (_) =
        Statistic.Create (5.0, 5.0) 5.0
        |> Some
    let shipmateSingleStatisticSource (_) (_) (_) =
        Statistic.Create (0.0, 100.0) 0.0
        |> Some
    let context = 
        TestHasDarkAlleyMinimumStakesContext
            (avatarIslandFeatureSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSource)
    let actual =
        World.HasDarkAlleyMinimumStakes
            context
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(false, actual)
    