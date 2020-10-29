module WorldGetIslandDisplayNameTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandDisplayName.It raises an exception when the given island doesn't exist.`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandDisplayName
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
    Assert.AreEqual("", actual)

[<Test>]
let ``GetIslandDisplayName.It returns "(unknown)" then the island exists but the avatar doesn't know about it.`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandDisplayName
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
    Assert.AreEqual("", actual)

[<Test>]
let ``GetIslandDisplayName.It returns the island name when the island exists and the avatar knows about it.`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandDisplayName
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
    Assert.AreEqual("", actual)

