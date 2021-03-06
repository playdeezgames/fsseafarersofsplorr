module StatusTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open Splorr.Seafarers.Services

let private previousGameState =
    None
    |> Gamestate.MainMenu

type TestStatusRunContext(avatarJobSource, islandSingleNameSource, shipmateSingleStatisticSource, vesselSingleStatisticSource) =
    interface AvatarShipmates.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = previousGameState
    let expected = input |> Some
    let context = 
        TestStatusRunContext
            (avatarJobSourceStub, 
            islandSingleNameSourceStub, 
            shipmateSingleStatisticSourceStub, 
            vesselSingleStatisticSourceStub) 
            :> ServiceContext
    let actual =
        input
        |> Status.Run 
            context
            sinkDummy
    Assert.AreEqual(expected, actual)

