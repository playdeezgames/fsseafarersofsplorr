module WorldGetIslandDisplayNameTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandDisplayName..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandDisplayName
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
    Assert.AreEqual("", actual)


