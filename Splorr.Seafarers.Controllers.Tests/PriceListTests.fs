module PriceListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures

[<Test>]
let ``Run.It returns Docked gamestate.`` () =
    let subjects = (dockLocation, dockWorld)
    let expected = subjects |> Gamestate.Docked |> Some
    let actual = 
        subjects
        ||> PriceList.Run sink
    Assert.AreEqual(expected, actual)