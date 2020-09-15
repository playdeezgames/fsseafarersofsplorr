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

type TestAvatarEnterIslandFeatureContext() =
    interface AvatarEnterIslandFeatureContext


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
[<Ignore("Refactoring this large file into smaller files")>]
let ``EnterIslandFeature.It does not enter the dark alley when one is not present.`` () =
    let inputLocation = (0.0, 0.0)
    let context = TestAvatarEnterIslandFeatureContext() :> AvatarEnterIslandFeatureContext
    Avatar.EnterIslandFeature
        context
        Fixtures.Common.Dummy.AvatarId
        inputLocation
        IslandFeatureIdentifier.DarkAlley



