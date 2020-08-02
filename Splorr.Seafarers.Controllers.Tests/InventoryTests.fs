module InventoryTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures

let private previousGameState =
    None
    |> Gamestate.MainMenu

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = 
        previousGameState
    let expected = 
        previousGameState 
        |> Some
    let actual =
        input
        |> Inventory.Run 
            atSeaItemSource 
            vesselSingleStatisticSourceStub 
            sinkStub
    Assert.AreEqual(expected, actual)
