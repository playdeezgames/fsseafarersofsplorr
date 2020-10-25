module WorldGetIslandNameTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandName.It retrieves and island name.`` () =
    let calledGetIslandName = ref false
    let context = Contexts.TestContext()
    (context :> IslandName.GetNameContext).islandSingleNameSource :=
        Spies.Source(calledGetIslandName, Some Dummies.ValidIslandName)
    let actual = 
        World.GetIslandName
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual(Some Dummies.ValidIslandName, actual)
    Assert.IsTrue(calledGetIslandName.Value)


