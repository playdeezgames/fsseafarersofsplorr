module WorldFoldGamblingHandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``FoldGamblingHand.It adds a message and purges the avatar's gambling hand.`` () = 
    let calledAddAvatarMessage = ref false
    let calledSetGamblingHand = ref false
    let context = Contexts.TestContext()
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessage)
    (context :> AvatarGamblingHand.SetContext).avatarGamblingHandSink := Spies.Expect(calledSetGamblingHand, (Dummies.ValidAvatarId,None))
    World.FoldGamblingHand
        context
        Dummies.ValidAvatarId
    Assert.IsTrue(calledAddAvatarMessage.Value)
    Assert.IsTrue(calledSetGamblingHand.Value)
