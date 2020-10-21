module WorldBetOnGamblingHandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common
open Tarot


[<Test>]
let ``BetOnGamblingHand.It gives a message when the avatar is not in the dark alley.`` () =
    let calledGetFeature = ref false
    let calledAddMessages = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := Spies.Source (calledGetFeature, None)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessages
    let actual = 
        World.BetOnGamblingHand
            context
            1.0
            Dummies.ValidAvatarId
    Assert.AreEqual(false, actual)
    Assert.IsTrue(calledGetFeature.Value)
    Assert.IsTrue(calledAddMessages.Value)

[<Test>]
let ``BetOnGamblingHand.It gives a message when the avatar is in the dark alley but does not have a gambling hand.`` () =
    let calledGetFeature = ref false
    let calledAddMessages = ref false
    let calledGetGamblingHand = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := Spies.Source (calledGetFeature, Some {featureId = IslandFeatureIdentifier.DarkAlley; location=Dummies.ValidIslandLocation})
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessages
    (context :> AvatarGamblingHand.GetContext).avatarGamblingHandSource := Spies.Source (calledGetGamblingHand, None)
    let actual = 
        World.BetOnGamblingHand
            context
            1.0
            Dummies.ValidAvatarId
    Assert.AreEqual(false, actual)
    Assert.IsTrue(calledGetFeature.Value)
    Assert.IsTrue(calledAddMessages.Value)
    Assert.IsTrue(calledGetGamblingHand.Value)

[<Test>]
let ``BetOnGamblingHand.It gives a message when the avatar is in the dark alley with a gambling hand but has bet less than the minimum wager.`` () =
    let calledGetFeature = ref false
    let calledAddMessages = ref false
    let calledGetGamblingHand = ref false
    let calledGetMinimumWager = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := Spies.Source (calledGetFeature, Some {featureId = IslandFeatureIdentifier.DarkAlley; location=Dummies.ValidIslandLocation})
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessages
    (context :> AvatarGamblingHand.GetContext).avatarGamblingHandSource := Spies.Source (calledGetGamblingHand, Some (Card.Minor (Wands, Rank.Ace),Card.Minor (Wands, Rank.Ace),Card.Minor (Wands, Rank.Ace)))
    (context :> Island.GetStatisticContext).islandSingleStatisticSource := Spies.Source (calledGetMinimumWager, Some {MinimumValue = 1.1; MaximumValue=1.1; CurrentValue=1.1})
    let actual = 
        World.BetOnGamblingHand
            context
            1.0
            Dummies.ValidAvatarId
    Assert.AreEqual(false, actual)
    Assert.IsTrue(calledGetFeature.Value)
    Assert.IsTrue(calledAddMessages.Value)
    Assert.IsTrue(calledGetGamblingHand.Value)
    Assert.IsTrue(calledGetMinimumWager.Value)


[<Test>]
let ``BetOnGamblingHand.It gives a message when the avatar is in the dark alley with a gambling hand and has bet more than the minimum wager but has bet more than the avatar has.`` () =
    let calledGetFeature = ref false
    let calledAddMessages = ref false
    let calledGetGamblingHand = ref false
    let calledGetMinimumWager = ref false
    let calledGetMoney = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := Spies.Source (calledGetFeature, Some {featureId = IslandFeatureIdentifier.DarkAlley; location=Dummies.ValidIslandLocation})
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessages
    (context :> AvatarGamblingHand.GetContext).avatarGamblingHandSource := Spies.Source (calledGetGamblingHand, Some (Card.Minor (Wands, Rank.Ace),Card.Minor (Wands, Rank.Ace),Card.Minor (Wands, Rank.Ace)))
    (context :> Island.GetStatisticContext).islandSingleStatisticSource := Spies.Source (calledGetMinimumWager, Some {MinimumValue = 1.0; MaximumValue=1.0; CurrentValue=1.0})
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetMoney, Some {MinimumValue = 0.0; MaximumValue=1.0; CurrentValue=0.0})
    let actual = 
        World.BetOnGamblingHand
            context
            1.0
            Dummies.ValidAvatarId
    Assert.AreEqual(false, actual)
    Assert.IsTrue(calledGetFeature.Value)
    Assert.IsTrue(calledAddMessages.Value)
    Assert.IsTrue(calledGetGamblingHand.Value)
    Assert.IsTrue(calledGetMinimumWager.Value)
    Assert.IsTrue(calledGetMoney.Value)

[<Test>]
let ``BetOnGamblingHand.It applies winnings when the avatar is in the dark alley with a winning gambling hand and has bet more than the minimum wager and has the funds.`` () =
    let calledGetFeature = ref false
    let calledAddMessages = ref false
    let calledGetGamblingHand = ref false
    let calledGetMinimumWager = ref false
    let calledGetMoney = ref false
    let calledSetMoney = ref false
    let calledSetGamblingHand = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := Spies.Source (calledGetFeature, Some {featureId = IslandFeatureIdentifier.DarkAlley; location=Dummies.ValidIslandLocation})
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessages
    (context :> AvatarGamblingHand.GetContext).avatarGamblingHandSource := Spies.Source (calledGetGamblingHand, Some (Card.Minor (Wands, Rank.Ace),Card.Minor (Wands, Rank.Three),Card.Minor (Wands, Rank.Deuce)))
    (context :> Island.GetStatisticContext).islandSingleStatisticSource := Spies.Source (calledGetMinimumWager, Some {MinimumValue = 1.0; MaximumValue=1.0; CurrentValue=1.0})
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetMoney, Some {MinimumValue = 0.0; MaximumValue=10.0; CurrentValue=1.0})
    (context :> ShipmateStatistic.PutContext).shipmateSingleStatisticSink := Spies.Expect(calledSetMoney, (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Money, Some {MinimumValue = 0.0; MaximumValue=10.0; CurrentValue=2.0}))
    (context :> AvatarGamblingHand.SetContext).avatarGamblingHandSink := Spies.Expect(calledSetGamblingHand, (Dummies.ValidAvatarId, None))
    let actual = 
        World.BetOnGamblingHand
            context
            1.0
            Dummies.ValidAvatarId
    Assert.AreEqual(true, actual)
    Assert.IsTrue(calledGetFeature.Value)
    Assert.IsTrue(calledAddMessages.Value)
    Assert.IsTrue(calledGetGamblingHand.Value)
    Assert.IsTrue(calledGetMinimumWager.Value)
    Assert.IsTrue(calledGetMoney.Value)
    Assert.IsTrue(calledSetMoney.Value)
    Assert.IsTrue(calledSetGamblingHand.Value)


[<Test>]
let ``BetOnGamblingHand.It deducts wager when the avatar is in the dark alley with a losing gambling hand and has bet more than the minimum and has the funds.`` () =
    let calledGetFeature = ref false
    let calledAddMessages = ref false
    let calledGetGamblingHand = ref false
    let calledGetMinimumWager = ref false
    let calledGetMoney = ref false
    let calledSetMoney = ref false
    let calledSetGamblingHand = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := Spies.Source (calledGetFeature, Some {featureId = IslandFeatureIdentifier.DarkAlley; location=Dummies.ValidIslandLocation})
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessages
    (context :> AvatarGamblingHand.GetContext).avatarGamblingHandSource := Spies.Source (calledGetGamblingHand, Some (Card.Minor (Wands, Rank.Ace),Card.Minor (Wands, Rank.Three),Card.Minor (Wands, Rank.Four)))
    (context :> Island.GetStatisticContext).islandSingleStatisticSource := Spies.Source (calledGetMinimumWager, Some {MinimumValue = 1.0; MaximumValue=1.0; CurrentValue=1.0})
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetMoney, Some {MinimumValue = 0.0; MaximumValue=10.0; CurrentValue=2.0})
    (context :> ShipmateStatistic.PutContext).shipmateSingleStatisticSink := Spies.Expect(calledSetMoney, (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Money, Some {MinimumValue = 0.0; MaximumValue=10.0; CurrentValue=1.0}))
    (context :> AvatarGamblingHand.SetContext).avatarGamblingHandSink := Spies.Expect(calledSetGamblingHand, (Dummies.ValidAvatarId, None))
    let actual = 
        World.BetOnGamblingHand
            context
            1.0
            Dummies.ValidAvatarId
    Assert.AreEqual(true, actual)
    Assert.IsTrue(calledGetFeature.Value)
    Assert.IsTrue(calledAddMessages.Value)
    Assert.IsTrue(calledGetGamblingHand.Value)
    Assert.IsTrue(calledGetMinimumWager.Value)
    Assert.IsTrue(calledGetMoney.Value)
    Assert.IsTrue(calledSetMoney.Value)
    Assert.IsTrue(calledSetGamblingHand.Value)


