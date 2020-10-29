module WorldGetNearbyLocationsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetNearbyLocations.It returns a list of island locations within a given distance of a given location.`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetNearbyLocations
            context
            Dummies.ValidIslandLocation
            10.0
    Assert.AreEqual([], actual)



