module WorldDealAvatarGamblingHandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common
open Tarot

[<Test>]
let ``DealAvatarGamblingHand.It deals a hand of three cards.`` () =
    let calledSetGamblingHand = ref false
    let context = Contexts.TestContext()
    (context :> AvatarGamblingHand.SetContext).avatarGamblingHandSink := Spies.Expect(calledSetGamblingHand, (Dummies.ValidAvatarId, Some((Major Arcana.Devil, Minor (Cups, Rank.Ace), Minor (Wands, Rank.Deuce)))))
    let actual =
        World.DealAvatarGamblingHand
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledSetGamblingHand.Value)


