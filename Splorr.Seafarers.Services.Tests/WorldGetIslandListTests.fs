module WorldGetIslandListTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Tests.Common

[<Test>]
let ``GetIslandList.It gets a list of island locations.`` () =
    let calledGetIslandList = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source(calledGetIslandList, Dummies.ValidIslandList)
    let actual =
        World.GetIslandList
            context
    Assert.AreEqual(Dummies.ValidIslandList, actual)
    Assert.IsTrue(calledGetIslandList.Value)
