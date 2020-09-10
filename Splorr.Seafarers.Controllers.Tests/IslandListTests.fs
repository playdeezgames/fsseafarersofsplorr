module IslandListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures

let private previousGameState =
    None
    |> Gamestate.MainMenu

type TestIslandListRunContext() =
    interface IslandListRunContext

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = 
        previousGameState
    let expected =
        input
        |> Some
    let context = TestIslandListRunContext() :> IslandListRunContext
    let actual =
        input
        |> IslandList.Run
            context
            avatarIslandSingleMetricSourceStub
            islandSingleNameSourceStub
            islandSourceStub
            vesselSingleStatisticSourceStub 
            sinkDummy 
            0u
    Assert.AreEqual(expected, actual)
