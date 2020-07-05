module JobsTest

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services

let private dockWorldconfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(0.0, 0.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=1u
        RewardRange = (1.0, 10.0)
        Commodities = Map.empty
        Items = Map.empty
    }
let private dockWorld = World.Create dockWorldconfiguration (System.Random())
let private dockLocation = (0.0, 0.0)
let private sink (_:Message) : unit = ()

[<Test>]
let ``Run.It returns Docked with the given location and world.`` () =
    let actual =
        (dockLocation, dockWorld)
        |> Jobs.Run sink
    Assert.AreEqual((Dock, dockLocation, dockWorld) |> Gamestate.Docked |> Some, actual)

