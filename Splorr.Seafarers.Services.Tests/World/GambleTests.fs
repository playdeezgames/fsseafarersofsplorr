module GambleTests

open NUnit.Framework
open System
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type private WorldCanPlaceBetContext(shipmateSingleStatisticSource) =
    interface Avatar.GetPrimaryStatisticContext with
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
    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Avatar.GetIslandFeatureContext with
        member this.avatarIslandFeatureSource: AvatarIslandFeatureSource = avatarIslandFeatureSource
    interface Avatar.GetGamblingHandContext with
        member this.avatarGamblingHandSource: AvatarGamblingHandSource = avatarGamblingHandSource
    interface World.CanPlaceBetContext
    interface Avatar.AddMessagesContext with
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

