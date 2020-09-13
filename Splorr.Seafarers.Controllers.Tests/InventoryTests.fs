module InventoryTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Services

let private previousGameState =
    None
    |> Gamestate.MainMenu
    
type TestInventoryRunContext(avatarInventorySource) =
    interface AvatarGetUsedTonnageContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface InventoryRunContext

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = 
        previousGameState
    let expected = 
        previousGameState 
        |> Some
    let context = TestInventoryRunContext(avatarInventorySourceStub) :> InventoryRunContext
    let actual =
        input
        |> Inventory.Run
            context
            avatarInventorySourceStub
            atSeaItemSource 
            vesselSingleStatisticSourceStub 
            sinkDummy
    Assert.AreEqual(expected, actual)
