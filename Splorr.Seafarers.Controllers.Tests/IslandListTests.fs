module IslandListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures

let private previousGameState =
    None
    |> Gamestate.MainMenu

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = 
        previousGameState
    let expected =
        input
        |> Some
    let actual =
        input
        |> IslandList.Run
            avatarIslandSingleMetricSourceStub
            islandSingleNameSourceStub
            vesselSingleStatisticSourceStub 
            sinkStub 
            0u
    Assert.AreEqual(expected, actual)
