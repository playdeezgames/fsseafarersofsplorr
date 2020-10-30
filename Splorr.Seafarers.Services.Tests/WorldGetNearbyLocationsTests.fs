module WorldGetNearbyLocationsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetNearbyLocations.It returns a list of island locations within a given distance of a given location.`` () =
    let calledGetIslandList = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource :=
        Spies.Source(calledGetIslandList, [(0.0, 1.0);(2.0,3.0);(4.0,5.0);(6.0,7.0);(8.0,9.0);(10.0,11.0);(12.0,13.0);(14.0,15.0);(16.0,17.0);(18.0,19.0)])
    let actual = 
        World.GetNearbyLocations
            context
            (0.0, 1.0)
            10.0
    Assert.AreEqual([(0.0, 1.0);(2.0,3.0);(4.0,5.0);(6.0,7.0)], actual)
    Assert.IsTrue(calledGetIslandList.Value)



