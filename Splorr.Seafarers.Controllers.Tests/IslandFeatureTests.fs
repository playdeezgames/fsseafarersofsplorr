module IslandFeatureTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Tarot

module private Dummies =
    let GivenValidLocation = Fixtures.Common.Dummy.IslandLocation

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
        avatarMessagePurger,
        avatarMessageSink,
        avatarMessageSource,
        islandSingleFeatureSource,
        islandSingleNameSource,
        islandSingleStatisticSource,
        shipmateSingleStatisticSink,
        shipmateSingleStatisticSource) =
    interface CommonContext
    interface Utility.RandomContext with
        member this.random: Random = Random()
    interface ShipmateStatistic.PutContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
    interface ShipmateStatistic.GetContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarShipmates.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Island.GetStatisticContext with
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
    interface AvatarIslandFeature.GetContext with
        member this.avatarIslandFeatureSource: AvatarIslandFeatureSource = avatarIslandFeatureSource
    interface World.ClearMessagesContext with
        member this.avatarMessagePurger: AvatarMessagePurger = avatarMessagePurger
    interface AvatarMessages.GetContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
    interface IslandName.GetNameContext with
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
    interface AvatarIslandFeature.EnterContext with
        member this.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink
        member this.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
    interface AvatarGamblingHand.FoldContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink

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
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSourceStub,
            islandSingleNameSourceStub,
            islandSingleStatisticSourceStub,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSourceStub) 
        :> CommonContext
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
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSourceStub,
            islandSingleNameSource,
            islandSingleStatisticSourceStub,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSourceStub) 
        :> CommonContext
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
    let mutable purgedMessages = false
    let avatarMessagePurger (_) =
        purgedMessages <- true
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Fixtures.Common.Fake.AvatarIslandFeatureSource,
            avatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Gamble))
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(purgedMessages)

[<Test>]
let ``Run.It should quit the gambling game when dark alley exists and the player is gambling and gives no bet command.`` () =
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
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let mutable purgedMessages = false
    let avatarMessagePurger (_) =
        purgedMessages <- true
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Fixtures.Common.Fake.AvatarIslandFeatureSource,
            avatarMessagePurger,
            avatarMessageSink,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (None |> Command.Bet |> Some))
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
    Assert.IsTrue(addedMessages)
    Assert.IsTrue(purgedMessages)


[<Test>]
let ``Run.It should resolve the gambling game when dark alley exists and the player is gambling and gives a bet command.`` () =
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
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (0.0, 1000000.0) 5.0 |> Some
    let avatarGamblingHandSource (_) =
        givenHand
    let mutable called = false
    let avatarGamblingHandSink (_) (hand:AvatarGamblingHand option) =
        called <- true
        Assert.AreEqual(None, hand)
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let mutable purgedMessages = false
    let avatarMessagePurger (_) =
        purgedMessages <- true
    let avatarIslandFeatureSource (_) =
        {
            featureId = IslandFeatureIdentifier.DarkAlley
            location = Dummies.GivenValidLocation
        }
        |> Some
    let shipmateSingleStatisticSink (_) 
            (shipmateId:ShipmateIdentifier) 
            (statisticId:ShipmateStatisticIdentifier,statistic:Statistic option) =
        Assert.AreEqual(Primary, shipmateId)
        Assert.AreEqual(ShipmateStatisticIdentifier.Money, statisticId)
        Assert.AreEqual(0.0, statistic.Value.CurrentValue)
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            avatarIslandFeatureSource,
            avatarMessagePurger,
            avatarMessageSink,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (5.0 |> Some |> Command.Bet |> Some))
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
    Assert.IsTrue(addedMessages)
    Assert.IsTrue(purgedMessages)

[<Test>]
let ``Run.It should enter the confirm quit state when dark alley exists and the player is gambling and gives quit command.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let givenHand = (Minor (Wands, Rank.Ace),Minor (Wands, Rank.Deuce),Minor (Wands, Rank.Three)) |> Some
    let expected=
        givenAvatarId
        |> Gamestate.InPlay
        |> Gamestate.ConfirmQuit
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
    let mutable addedMessages = false
    let avatarMessageSink (_) (_) =
        addedMessages <- true
    let mutable purgedMessages = false
    let avatarMessagePurger (_) =
        purgedMessages <- true
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            Fixtures.Common.Fake.AvatarIslandFeatureSink,
            Fixtures.Common.Fake.AvatarIslandFeatureSource,
            avatarMessagePurger,
            avatarMessageSink,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Command.Quit |> Some))
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsFalse(called)
    Assert.IsFalse(addedMessages)
    Assert.IsTrue(purgedMessages)


[<Test>]
let ``Run.It should return InPlay state when dark alley exists but the player does not have minimum gambling stakes.`` () =
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
            Assert.AreEqual(Dummies.GivenValidLocation, feature.location)
        | _ ->
            Assert.Fail("avatarIslandFeatureSink")
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSink,
            Mock.AvatarIslandFeatureSource (Dummies.GivenValidLocation, IslandFeatureIdentifier.DarkAlley),
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExpected ["Come back when you've got more money!"],
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            Fixtures.Common.Fake.CommandSource
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.When in the dark alley, the leave command will take the player back to the dock.`` () =
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
            Assert.AreEqual(Dummies.GivenValidLocation, feature.location)
        | _ ->
            Assert.Fail("avatarIslandFeatureSink")
    let context = 
        TestIslandFeatureRunContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarIslandFeatureSink,
            Mock.AvatarIslandFeatureSource (Dummies.GivenValidLocation, IslandFeatureIdentifier.DarkAlley),
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Leave))
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.When in the dark alley, the gamble command will deal to the player some cards and enter the gambling minigame.`` () =
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
            Mock.AvatarIslandFeatureSource (Dummies.GivenValidLocation, IslandFeatureIdentifier.DarkAlley),
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Gamble))
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(handDealt)

[<Test>]
let ``Run.When in the dark alley, the help command will take the player to the help state for the dark alley.`` () =
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
            Mock.AvatarIslandFeatureSource (Dummies.GivenValidLocation, IslandFeatureIdentifier.DarkAlley),
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Help))
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.When in the dark alley, the status command will take the player to the status state for the dark alley.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        Fixtures.Common.Dummy.AvatarId
        |> Gamestate.InPlay
        |> Gamestate.Status //this line
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
            Mock.AvatarIslandFeatureSource (Dummies.GivenValidLocation, IslandFeatureIdentifier.DarkAlley),
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Status)) //this line
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.When in the dark alley, the quit command will take the player to the confirm quit state for the dark alley.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let islandSingleNameSource (_) = Some ""
    let islandSingleFeatureSource (_) (_) = true
    let islandSingleStatisticSource (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let shipmateSingleStatisticSource (_) (_) (_) = Statistic.Create (5.0,5.0) 5.0 |> Some
    let expected =
        Fixtures.Common.Dummy.AvatarId
        |> Gamestate.InPlay
        |> Gamestate.ConfirmQuit //this line
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
            Mock.AvatarIslandFeatureSource (Dummies.GivenValidLocation, IslandFeatureIdentifier.DarkAlley),
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake (Some Command.Quit)) //this line
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.When in the dark alley but not gambling, the an invalid command gives you an error message.`` () =
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
            Mock.AvatarIslandFeatureSource (Dummies.GivenValidLocation, IslandFeatureIdentifier.DarkAlley),
            Fixtures.Common.Fake.AvatarMessagePurger,
            avatarMessageSinkExplode,
            avatarMessageSourceDummy,
            islandSingleFeatureSource,
            islandSingleNameSource,
            islandSingleStatisticSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource) 
        :> CommonContext
    let actual =
        IslandFeature.Run 
            context
            (commandSourceFake None)
            sinkDummy
            Dummies.GivenValidLocation
            givenFeature
            givenAvatarId
    Assert.AreEqual(expected, actual)
    