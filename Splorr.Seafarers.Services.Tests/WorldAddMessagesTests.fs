module WorldAddMessagesTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``AddMessages.It adds messages for the avatar.`` () =
    let called = ref false
    let context = Contexts.TestContext()
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Expect (called, (Dummies.ValidAvatarId, "hi"))
    World.AddMessages
        context
        [ "hi" ]
        Dummies.ValidAvatarId
    Assert.IsTrue(called.Value)


