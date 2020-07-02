module PriceListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures

[<Test>]
let ``Run.It returns Docked (at Dock) gamestate.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let expected = 
        (Dock, inputLocation, inputWorld)
        |> Gamestate.Docked |> Some
    let actual = 
        (inputLocation, inputWorld)
        ||> PriceList.Run sinkStub
    Assert.AreEqual(expected, actual)