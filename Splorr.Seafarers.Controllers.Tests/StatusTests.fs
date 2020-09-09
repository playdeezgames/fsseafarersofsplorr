module StatusTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures

let private previousGameState =
    None
    |> Gamestate.MainMenu

type TestStatusRunContext() =
    interface StatusRunContext

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = previousGameState
    let expected = input |> Some
    let context = TestStatusRunContext() :> StatusRunContext
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

