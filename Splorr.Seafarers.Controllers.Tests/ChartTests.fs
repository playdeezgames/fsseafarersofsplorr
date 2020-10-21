module ChartTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Services

type TestChartRunContext
            (avatarIslandSingleMetricSource,
            islandSingleNameSource,
            islandSource,
            vesselSingleStatisticSource, 
            worldSingleStatisticSource) =
    interface World.GetStatisticContext with
        member this.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource
    interface Vessel.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

[<Test>]
let ``Run.It returns the At Sea state with the given world.`` () =
    let inputName = "chartname"
    let inputWorld = world
    let expected =
        world
        |> Gamestate.InPlay
        |> Some
    let context = 
        TestChartRunContext
            (avatarIslandSingleMetricSourceStub,
            islandSingleNameSourceStub,
            islandSourceStub,
            vesselSingleStatisticSourceStub, 
            worldSingleStatisticSourceStub) :> CommonContext
    let actual =
        Chart.Run
            context
            sinkDummy 
            inputName 
            inputWorld
    Assert.AreEqual(expected, actual)
