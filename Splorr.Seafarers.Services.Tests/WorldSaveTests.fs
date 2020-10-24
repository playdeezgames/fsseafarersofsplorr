module WorldSaveTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``Save.It adds a message when the game fails to save.`` () = 
    let calledSave = ref false
    let calledAddAvatarMessage = ref false
    let context = Contexts.TestContext()
    (context :> WorldExport.SaveContext).gameDataSink := Spies.Source(calledSave, None)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Expect(calledAddAvatarMessage, (Dummies.ValidAvatarId,"Could not save game!"))
    World.Save
        context
        "filename"
        Dummies.ValidAvatarId
    Assert.IsTrue(calledSave.Value)
    Assert.IsTrue(calledAddAvatarMessage.Value)

[<Test>]
let ``Save.It adds a message when the game successfully saves.`` () = 
    let calledSave = ref false
    let calledAddAvatarMessage = ref false
    let context = Contexts.TestContext()
    (context :> WorldExport.SaveContext).gameDataSink := Spies.Source(calledSave, Some "yep")
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Expect(calledAddAvatarMessage, (Dummies.ValidAvatarId,"Saved game to 'yep'."))
    World.Save
        context
        "filename"
        Dummies.ValidAvatarId
    Assert.IsTrue(calledSave.Value)
    Assert.IsTrue(calledAddAvatarMessage.Value)


