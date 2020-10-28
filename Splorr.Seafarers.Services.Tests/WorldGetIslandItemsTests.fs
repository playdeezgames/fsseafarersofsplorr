module WorldGetIslandItemsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandItems..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandItems
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual(Set.empty, actual)

