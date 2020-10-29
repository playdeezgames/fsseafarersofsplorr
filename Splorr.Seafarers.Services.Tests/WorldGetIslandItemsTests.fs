module WorldGetIslandItemsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandItems.It gets a set of items sold at the given island locatoin.`` () =
    let calledGetIslandItems = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetItemsContext).islandItemSource := 
        Spies.Source(calledGetIslandItems, 
            [0UL] |> Set.ofList)
    let actual = 
        World.GetIslandItems
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual(Set.empty |> Set.add 0UL, actual)
    Assert.IsTrue(calledGetIslandItems.Value)

