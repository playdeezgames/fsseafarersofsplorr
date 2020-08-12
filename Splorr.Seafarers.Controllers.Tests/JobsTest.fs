module JobsTest

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures
open AtSeaTestFixtures

let private dockWorldconfiguration: WorldConfiguration =
    {
        WorldSize              = (0.0, 0.0)
    }

let private dockWorld = 
    World.Create 
        nameSource
        dockWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        dockWorldconfiguration 
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
        ||> Jobs.Run sinkStub
    Assert.AreEqual(expected, actual)

