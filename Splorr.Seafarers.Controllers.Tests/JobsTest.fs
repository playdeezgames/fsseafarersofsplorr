module JobsTest

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

let private dockWorldconfiguration: WorldConfiguration =
    {
        AvatarDistances        = (10.0, 1.0)
        MaximumGenerationTries = 1u
        MinimumIslandDistance  = 30.0
        RationItems            = [ 1UL ]
        RewardRange            = (1.0, 10.0)
        StatisticDescriptors   = []
        WorldSize              = (0.0, 0.0)
    }

let private dockWorld = 
    World.Create 
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
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

