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
    interface ChartRunContext with
        member this.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource
    interface Avatar.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface ChartOutputChartContext with
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        member this.islandSource: IslandSource = islandSource

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
            worldSingleStatisticSourceStub) :> ChartRunContext
    let actual =
        Chart.Run
            context
            sinkDummy 
            inputName 
            inputWorld
    Assert.AreEqual(expected, actual)
