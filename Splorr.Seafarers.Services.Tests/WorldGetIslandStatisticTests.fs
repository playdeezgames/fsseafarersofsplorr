module WorldGetIslandStatisticTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandStatistic..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandStatistic
            context
            IslandStatisticIdentifier.CareenDistance
            Dummies.ValidIslandLocation
    Assert.AreEqual(None, actual)

