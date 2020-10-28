module WorldGetAvatarJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarJob..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarJob
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)


