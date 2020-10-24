module WorldGetItemListTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetItemList.It gets a list of items.`` () =
    let calledGetItemList = ref false
    let context = Contexts.TestContext()
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Map.empty)
    let actual =
        World.GetItemList
            context
    Assert.AreEqual(Map.empty, actual)
    Assert.IsTrue(calledGetItemList.Value)


