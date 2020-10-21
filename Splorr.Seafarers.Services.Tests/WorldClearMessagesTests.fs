module WorldClearMessagesTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``ClearMessages.It clears avatar messages.`` () = 
    let called = ref false
    let context = Contexts.TestContext()
    (context :> WorldMessages.ClearMessagesContext).avatarMessagePurger := Spies.Sink called
    World.ClearMessages
        context
        Dummies.ValidAvatarId
    Assert.IsTrue(called.Value)



