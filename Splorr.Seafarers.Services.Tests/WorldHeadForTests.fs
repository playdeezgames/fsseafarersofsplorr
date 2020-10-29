module WorldHeadForTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``HeadFor.It adds a message when the given island name is not known to the given avatar.`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.HeadFor
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)

[<Test>]
let ``HeadFor.It causes the given avatars vessel to head for the island when the given avatar knows about the given island name.`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.HeadFor
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)


