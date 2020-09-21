module ChartTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Services

type TestChartRunContext(vesselSingleStatisticSource) =
    interface ChartRunContext
    interface Avatar.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

[<Test>]
let ``Run.It returns the At Sea state with the given world.`` () =
    let inputName = "chartname"
    let inputWorld = world
    let expected =
        world
        |> Gamestate.InPlay
        |> Some
    let context = TestChartRunContext(vesselSingleStatisticSourceStub) :> ChartRunContext
    let actual =
        Chart.Run
            context
            avatarIslandSingleMetricSourceStub
            islandSingleNameSourceStub
            islandSourceStub
            worldSingleStatisticSourceStub 
            sinkDummy 
            inputName 
            inputWorld
    Assert.AreEqual(expected, actual)
