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
        Commodities = Map.empty
        Items = Map.empty
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
let ``GetWorld.It returns the world embedded within the given Docked (at Dock) Gamestate.`` () =
    let actual = 
        (Dock, (0.0,0.0), world)
        |> Gamestate.Docked 
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)


[<Test>]
let ``GetWorld.It returns the world embedded within the given Docked (at PriceList) Gamestate.`` () =
    let actual = 
        (PriceList, (0.0,0.0), world)
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
let ``GetWorld.It returns world from the given Docked (at Jobs) Gamestate.`` () =
    let actual =
        (Jobs, (0.0, 0.0),world)
        |> Gamestate.Docked
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns world from the given Docked (at Shop) Gamestate.`` () =
    let actual =
        (Shop, (0.0, 0.0),world)
        |> Gamestate.Docked
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns world from the given ItemList Gamestate.`` () =
    let actual =
        (ItemList, (0.0, 0.0),world)
        |> Gamestate.Docked
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns world from the given Inventory Gamestate.`` () =
    let actual =
        (ItemList, (0.0, 0.0),world)
        |> Gamestate.Docked
        |> Gamestate.Inventory
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns None from the given GameOver Gamestate.`` () =
    let input = Gamestate.GameOver []
    let expected = None
    let actual =
        input
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns world from the given SaveGame Gamestate.`` () =
    let actual =
        ("savename", world)
        |> Gamestate.SaveGame
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)
