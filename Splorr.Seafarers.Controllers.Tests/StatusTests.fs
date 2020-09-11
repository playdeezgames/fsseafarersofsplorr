module StatusTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open Splorr.Seafarers.Services

let private previousGameState =
    None
    |> Gamestate.MainMenu

type TestStatusRunContext(shipmateSingleStatisticSource) =
    interface StatusRunContext
    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = previousGameState
    let expected = input |> Some
    let context = TestStatusRunContext(shipmateSingleStatisticSourceStub) :> StatusRunContext
    let actual =
        input
        |> Status.Run 
            context
            avatarJobSourceStub
            islandSingleNameSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            sinkDummy
    Assert.AreEqual(expected, actual)

