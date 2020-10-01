module WorldBetOnGamblingHandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Tarot


type TestWorldBetOnGamblingHandContext
        (avatarGamblingHandSink,
        avatarGamblingHandSource,
        avatarIslandFeatureSource,
        avatarMessageSink,
        islandSingleStatisticSource,
        shipmateSingleStatisticSink,
        shipmateSingleStatisticSource) =
    interface ServiceContext
    interface AvatarGamblingHand.GetContext with
        member this.avatarGamblingHandSource: AvatarGamblingHandSource = avatarGamblingHandSource
    interface AvatarMessages.AddContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface AvatarIslandFeature.GetContext with
        member this.avatarIslandFeatureSource: AvatarIslandFeatureSource = avatarIslandFeatureSource
    interface Island.GetStatisticContext with
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
    interface AvatarShipmates.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface ShipmateStatistic.PutContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
    interface ShipmateStatistic.GetContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarGamblingHand.FoldContext with
        member this.avatarGamblingHandSink: AvatarGamblingHandSink = avatarGamblingHandSink
            

[<Test>]
let ``BetOnGamblingHand.It returns false and adds a message when the player does not have an active gambling hand.`` () =
    let mutable gotGamblingHand = false
    let avatarGamblingHandSource (_) =
        gotGamblingHand <- true
        None
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let context = 
        TestWorldBetOnGamblingHandContext
            (Fixtures.Common.Fake.AvatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSource,
            avatarMessageSink,
            Fixtures.Common.Fake.IslandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource)
    let amount = 1.0
    let expected = false
    let actual = World.BetOnGamblingHand context amount Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(gotGamblingHand)
    Assert.IsTrue(addedMessages)
    
[<Test>]
let ``BetOnGamblingHand.It returns false and adds a message when the player is not in a dark alley.`` () =
    let mutable gotGamblingHand = false
    let avatarGamblingHandSource (_) =
        gotGamblingHand <- true
        (Minor (Wands, Rank.Ace), Minor (Wands, Rank.Ace), Minor (Wands, Rank.Ace))
        |> Some
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let avatarIslandFeatureSource (_) =
        None
    let context = 
        TestWorldBetOnGamblingHandContext
            (Fixtures.Common.Fake.AvatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSource,
            avatarMessageSink,
            Fixtures.Common.Fake.IslandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource)
    let amount = -1.0
    let expected = false
    let actual = World.BetOnGamblingHand context amount Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(gotGamblingHand)
    Assert.IsTrue(addedMessages)

[<Test>]
let ``BetOnGamblingHand.It returns false and adds a message when the player bets less than the minimum wager.`` () =
    let mutable gotGamblingHand = false
    let avatarGamblingHandSource (_) =
        gotGamblingHand <- true
        (Minor (Wands, Rank.Ace), Minor (Wands, Rank.Ace), Minor (Wands, Rank.Ace))
        |> Some
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let avatarIslandFeatureSource (_) =
        {
            featureId = IslandFeatureIdentifier.DarkAlley
            location = Fixtures.Common.Dummy.IslandLocation
        }
        |> Some
    let mutable gotMinimumWager = false
    let islandSingleStatisticSource (_) (_) =
        gotMinimumWager <- true
        5.0
        |> Statistic.Create (5.0, 5.0) 
        |> Some
    let context = 
        TestWorldBetOnGamblingHandContext
            (Fixtures.Common.Fake.AvatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSource,
            avatarMessageSink,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource)
    let amount = -1.0
    let expected = false
    let actual = 
        World.BetOnGamblingHand 
            context 
            amount 
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(gotGamblingHand)
    Assert.IsTrue(addedMessages)
    Assert.IsTrue(gotMinimumWager)

[<Test>]
let ``BetOnGamblingHand.It returns false and adds a message when the player bets more than he has.`` () =
    let mutable gotGamblingHand = false
    let avatarGamblingHandSource (_) =
        gotGamblingHand <- true
        (Minor (Wands, Rank.Ace), Minor (Wands, Rank.Ace), Minor (Wands, Rank.Ace))
        |> Some
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let avatarIslandFeatureSource (_) =
        {
            featureId = IslandFeatureIdentifier.DarkAlley
            location = Fixtures.Common.Dummy.IslandLocation
        }
        |> Some
    let mutable gotMinimumWager = false
    let islandSingleStatisticSource (_) (_) =
        gotMinimumWager <- true
        5.0
        |> Statistic.Create (5.0, 5.0) 
        |> Some
    let mutable gotMoney = false
    let shipmateSingleStatisticSource (_) (_) (_) =
        gotMoney <- true
        None
    let context = 
        TestWorldBetOnGamblingHandContext
            (Fixtures.Common.Fake.AvatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSource,
            avatarMessageSink,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource)
    let amount = 5.0
    let expected = false
    let actual = 
        World.BetOnGamblingHand 
            context 
            amount 
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(gotGamblingHand)
    Assert.IsTrue(addedMessages)
    Assert.IsTrue(gotMoney)
    Assert.IsTrue(gotMinimumWager)

[<Test>]
let ``BetOnGamblingHand.It returns true and adds a message and resolves the hand and deducts the money from the avatar when there is a losing hand.`` () =
    let mutable gotGamblingHand = false
    let avatarGamblingHandSource (_) =
        gotGamblingHand <- true
        (Minor (Wands, Rank.Ace), Minor (Wands, Rank.Deuce), Minor (Wands, Rank.Three))
        |> Some
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let avatarIslandFeatureSource (_) =
        {
            featureId = IslandFeatureIdentifier.DarkAlley
            location = Fixtures.Common.Dummy.IslandLocation
        }
        |> Some
    let mutable gotMinimumWager = false
    let islandSingleStatisticSource (_) (_) =
        gotMinimumWager <- true
        5.0
        |> Statistic.Create (5.0, 5.0) 
        |> Some
    let mutable gotMoney = false
    let shipmateSingleStatisticSource (_) (_) (_) =
        gotMoney <- true
        10.0
        |> Statistic.Create (0.0, 1000000.0) 
        |> Some
    let mutable setMoney = false
    let shipmateSingleStatisticSink (_) (_) (statisticId : ShipmateStatisticIdentifier, statistic : Statistic option)=
        Assert.AreEqual(ShipmateStatisticIdentifier.Money, statisticId)
        Assert.AreEqual(5.0, statistic.Value.CurrentValue)
        setMoney <- true
    let mutable foldedHand = false
    let avatarGamblingHandSink (_) (_)=
        foldedHand <- true
    let context = 
        TestWorldBetOnGamblingHandContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSource,
            avatarMessageSink,
            islandSingleStatisticSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource)
    let amount = 5.0
    let expected = true
    let actual = 
        World.BetOnGamblingHand 
            context 
            amount 
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(gotGamblingHand)
    Assert.IsTrue(addedMessages)
    Assert.IsTrue(gotMoney)
    Assert.IsTrue(gotMinimumWager)
    Assert.IsTrue(setMoney)


[<Test>]
let ``BetOnGamblingHand.It returns true and adds a message and resolves the hand and adds the money from the avatar when there is a winning hand.`` () =
    let mutable gotGamblingHand = false
    let avatarGamblingHandSource (_) =
        gotGamblingHand <- true
        (Minor (Wands, Rank.Ace), Minor (Wands, Rank.Three), Minor (Wands, Rank.Deuce))
        |> Some
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let avatarIslandFeatureSource (_) =
        {
            featureId = IslandFeatureIdentifier.DarkAlley
            location = Fixtures.Common.Dummy.IslandLocation
        }
        |> Some
    let mutable gotMinimumWager = false
    let islandSingleStatisticSource (_) (_) =
        gotMinimumWager <- true
        5.0
        |> Statistic.Create (5.0, 5.0) 
        |> Some
    let mutable gotMoney = false
    let shipmateSingleStatisticSource (_) (_) (_) =
        gotMoney <- true
        10.0
        |> Statistic.Create (0.0, 1000000.0) 
        |> Some
    let mutable setMoney = false
    let shipmateSingleStatisticSink (_) (_) (statisticId : ShipmateStatisticIdentifier, statistic : Statistic option)=
        Assert.AreEqual(ShipmateStatisticIdentifier.Money, statisticId)
        Assert.AreEqual(15.0, statistic.Value.CurrentValue)
        setMoney <- true
    let mutable foldedHand = false
    let avatarGamblingHandSink (_) (_)=
        foldedHand <- true
    let context = 
        TestWorldBetOnGamblingHandContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSource,
            avatarMessageSink,
            islandSingleStatisticSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource)
    let amount = 5.0
    let expected = true
    let actual = 
        World.BetOnGamblingHand 
            context 
            amount 
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(gotGamblingHand)
    Assert.IsTrue(addedMessages)
    Assert.IsTrue(gotMoney)
    Assert.IsTrue(gotMinimumWager)
    Assert.IsTrue(setMoney)
