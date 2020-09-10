module ChartTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures

type TestChartRunContext() =
    interface ChartRunContext

[<Test>]
let ``Run.It returns the At Sea state with the given world.`` () =
    let inputName = "chartname"
    let inputWorld = world
    let expected =
        world
        |> Gamestate.InPlay
        |> Some
    let context = TestChartRunContext() :> ChartRunContext
    let actual =
        Chart.Run
            context
            avatarIslandSingleMetricSourceStub
            islandSingleNameSourceStub
            islandSourceStub
            vesselSingleStatisticSourceStub 
            worldSingleStatisticSourceStub 
            sinkDummy 
            inputName 
            inputWorld
    Assert.AreEqual(expected, actual)
