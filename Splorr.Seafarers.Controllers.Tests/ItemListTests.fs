module ItemListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures
open AtSeaTestFixtures

[<Test>]
let ``Run.It returns Docked (at Shop) gamestate.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let expected = 
        (Dock, inputLocation, inputWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual = 
        (inputLocation, inputWorld)
        ||> ItemList.Run 
            avatarMessageSourceDummy
            atSeaCommoditySource 
            atSeaIslandItemSource 
            atSeaIslandMarketSource 
            islandSourceStub
            atSeaItemSource 
            shipmateSingleStatisticSourceStub
            sinkDummy
    Assert.AreEqual(expected, actual)