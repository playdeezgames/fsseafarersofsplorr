module WorldGetAvatarMessagesTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarMessages.It retrieves the messages awaiting the avatar.`` () =
    let calledGetAvatarMessages = ref false
    let context = Contexts.TestContext()
    (context :> AvatarMessages.GetContext).avatarMessageSource := Spies.Source(calledGetAvatarMessages, ["hello"])
    let actual =
        World.GetAvatarMessages
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(["hello"], actual)
    Assert.IsTrue(calledGetAvatarMessages.Value)


