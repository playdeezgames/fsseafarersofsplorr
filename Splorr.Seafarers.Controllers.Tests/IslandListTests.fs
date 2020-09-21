﻿module IslandListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open Splorr.Seafarers.Services

let private previousGameState =
    None
    |> Gamestate.MainMenu

type TestIslandListRunContext(vesselSingleStatisticSource) =
    interface IslandListRunContext
    interface Avatar.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource


[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input = 
        previousGameState
    let expected =
        input
        |> Some
    let context = TestIslandListRunContext(vesselSingleStatisticSourceStub) :> IslandListRunContext
    let actual =
        input
        |> IslandList.Run
            context
            avatarIslandSingleMetricSourceStub
            islandSingleNameSourceStub
            islandSourceStub
            sinkDummy 
            0u
    Assert.AreEqual(expected, actual)
