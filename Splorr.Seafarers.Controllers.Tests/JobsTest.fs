module JobsTest

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures
open AtSeaTestFixtures

let private dockWorld = 
    World.Create 
        avatarIslandSingleMetricSinkStub
        avatarJobSinkStub
        islandSingleNameSinkStub
        islandSingleStatisticSinkStub
        islandStatisticTemplateSourceStub
        termNameSource
        dockWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random
        avatarId

let private dockLocation = (0.0, 0.0)

[<Test>]
let ``Run.It returns Docked with the given location and world.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let expected = 
        (Dock, inputLocation, inputWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> Jobs.Run 
            islandSingleNameSourceStub
            sinkStub
    Assert.AreEqual(expected, actual)

