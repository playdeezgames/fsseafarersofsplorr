module WorldGetAvatarMoneyTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarMoney..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarMoney
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)


