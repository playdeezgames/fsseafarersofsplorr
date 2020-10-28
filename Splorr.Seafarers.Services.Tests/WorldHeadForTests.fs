module WorldHeadForTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``HeadFor..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.HeadFor
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)


