module InventoryTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Services

let private previousGameState =
    None
    |> Gamestate.MainMenu
    
type TestInventoryRunContext(avatarInventorySource, itemSource, vesselSingleStatisticSource) =
    interface Avatar.GetUsedTonnageContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface InventoryRunContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member this.itemSource: ItemSource = itemSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = 
        previousGameState
    let expected = 
        previousGameState 
        |> Some
    let context = TestInventoryRunContext(avatarInventorySourceStub, atSeaItemSource, vesselSingleStatisticSourceStub) :> ServiceContext
    let actual =
        input
        |> Inventory.Run
            context
            sinkDummy
    Assert.AreEqual(expected, actual)
