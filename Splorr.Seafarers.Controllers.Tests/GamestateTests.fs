module GamestateTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

let private configuration :WorldGenerationConfiguration = 
    {
        WorldSize = (0.0,0.0)
        MaximumGenerationTries=1u
        MinimumIslandDistance=1.0
        RewardRange = (1.0, 10.0)
    }
let private world = World.Create configuration (System.Random())

[<Test>]
let ``GetWorld.It returns the world embedded within the given AtSea Gamestate.`` () =
    let actual = 
        world 
        |> Gamestate.AtSea 
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Docked Gamestate.`` () =
    let actual = 
        ((0.0,0.0), world)
        |> Gamestate.Docked 
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given MainMenu Gamestate when a world is present.`` () =
    let actual = 
        world
        |> Some
        |> Gamestate.MainMenu
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns None from the given MainMenu Gamestate when no world is present.`` () =
    let actual = 
        None
        |> Gamestate.MainMenu
        |> Gamestate.GetWorld
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetWorld.It returns world from the given Jobs Gamestate.`` () =
    let actual =
        ((0.0, 0.0),world)
        |> Gamestate.Jobs
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)