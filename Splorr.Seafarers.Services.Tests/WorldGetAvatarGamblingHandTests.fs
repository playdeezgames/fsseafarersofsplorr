module WorldGetAvatarGamblingHandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarGamblingHand.It gets the avatars gambling hand.`` () =
    let calledGetGamblingHand = ref false
    let context = Contexts.TestContext()
    (context :> AvatarGamblingHand.GetContext).avatarGamblingHandSource := Spies.Source(calledGetGamblingHand, None)
    let actual =
        World.GetAvatarGamblingHand
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledGetGamblingHand.Value)


