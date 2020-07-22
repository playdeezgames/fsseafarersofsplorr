module ItemListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures

[<Test>]
let ``Run.It returns Docked (at Shop) gamestate.`` () =
    let inputLocation = dockLocation
    let islandItemSource (_) = Set.empty
    let islandMarketSource (_) = Map.empty
    let inputWorld = dockWorld
    let expected = 
        (Dock, inputLocation, inputWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual = 
        (inputLocation, inputWorld)
        ||> ItemList.Run islandMarketSource islandItemSource commodities smallWorldItems sinkStub
    Assert.AreEqual(expected, actual)