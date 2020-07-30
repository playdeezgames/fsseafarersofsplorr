module GamestateTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open System

let private configuration :WorldConfiguration = 
    {
        WorldSize = (0.0,0.0)
        MaximumGenerationTries=1u
        MinimumIslandDistance=1.0
        RewardRange = (1.0, 10.0)
        RationItems = [1UL]
        StatisticDescriptors = []
        AvatarDistances = (10.0, 1.0)
    }
let private random = Random()
let private avatarId = ""
let private world = World.Create configuration random avatarId

[<Test>]
let ``GetWorld.It returns the world embedded within the given AtSea Gamestate.`` () =
    let expected = 
        world
        |> Some
    let actual = 
        world 
        |> Gamestate.AtSea 
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Docked (at Dock) Gamestate.`` () =
    let expected = world |> Some
    let actual = 
        (Dock, (0.0,0.0), world)
        |> Gamestate.Docked 
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given MainMenu Gamestate when a world is present.`` () =
    let expected = world |> Some
    let actual = 
        world
        |> Some
        |> Gamestate.MainMenu
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Status Gamestate when a world is present.`` () =
    let expected =world |> Some
    let actual = 
        world
        |> Gamestate.AtSea
        |> Gamestate.Status
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Chart Gamestate when a world is present.`` () =
    let expected =world |> Some
    let actual = 
        ("chartname", world)
        |> Gamestate.Chart
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Help Gamestate when a world is present.`` () =
    let expected =world |> Some
    let actual = 
        world
        |> Gamestate.AtSea
        |> Gamestate.Help
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Metrics Gamestate when a world is present.`` () =
    let expected = world |> Some
    let actual = 
        world
        |> Gamestate.AtSea
        |> Gamestate.Metrics
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given InvalidInput Gamestate when a world is present.`` () =
    let expected = world |> Some
    let actual = 
        ("Maybe try 'help'?",world
        |> Gamestate.AtSea)
        |> Gamestate.ErrorMessage
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Careened Gamestate.`` () =
    let expected = world |> Some
    let actual = 
        (Port, world)
        |> Gamestate.Careened
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns None from the given MainMenu Gamestate when no world is present.`` () =
    let expected = None
    let actual = 
        None
        |> Gamestate.MainMenu
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns world from the given Docked (at Jobs) Gamestate.`` () =
    let actual =
        (Jobs, (0.0, 0.0),world)
        |> Gamestate.Docked
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns world from the given ItemList Gamestate.`` () =
    let expected = world |> Some
    let actual =
        (ItemList, (0.0, 0.0),world)
        |> Gamestate.Docked
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns world from the given Inventory Gamestate.`` () =
    let expected = world |> Some
    let actual =
        (ItemList, (0.0, 0.0),world)
        |> Gamestate.Docked
        |> Gamestate.Inventory
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns None from the given GameOver Gamestate.`` () =
    let input = Gamestate.GameOver []
    let expected = None
    let actual =
        input
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CheckForAvatarDeath.It returns the original gamestate when the avatar embedded therein is not dead.`` () =
    let avatarTransformHealth:Avatar -> Avatar option =
        Avatar.TransformShipmate 
            (Shipmate.SetStatistic AvatarStatisticIdentifier.Health ({MinimumValue=0.0; CurrentValue=1.0; MaximumValue=1.0}|>Some)) 0u
        >> Some
    let avatarTransformTurn:Avatar -> Avatar option =
        Avatar.TransformShipmate 
            (Shipmate.SetStatistic AvatarStatisticIdentifier.Turn ({MinimumValue=0.0; CurrentValue=0.0; MaximumValue=1.0}|>Some)) 0u
        >> Some
    let input =
        world
        |> World.TransformAvatar world.AvatarId avatarTransformHealth
        |> World.TransformAvatar world.AvatarId avatarTransformTurn
        |> Gamestate.AtSea
        |> Some
    let expected =
        input
    let actual =
        input
        |> Gamestate.CheckForAvatarDeath
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CheckForAvatarDeath.It returns the original gamestate when there is not a world embedded in the gamestate.`` () =
    let input =
        None
        |> Gamestate.MainMenu
        |> Some
    let expected =
        input
    let actual =
        input
        |> Gamestate.CheckForAvatarDeath
    Assert.AreEqual(expected, actual)


[<Test>]
let ``CheckForAvatarDeath.It returns gameover when the avatar embedded therein is dead.`` () =
    let avatarTransformHealth:Avatar -> Avatar option =
        Avatar.TransformShipmate 
            (Shipmate.SetStatistic AvatarStatisticIdentifier.Health ({MinimumValue=0.0; CurrentValue=0.0; MaximumValue=1.0}|>Some)) 0u
        >> Some
    let avatarTransformTurn:Avatar -> Avatar option =
        Avatar.TransformShipmate 
            (Shipmate.SetStatistic AvatarStatisticIdentifier.Turn ({MinimumValue=0.0; CurrentValue=0.0; MaximumValue=1.0}|>Some)) 0u
        >> Some
    let input =
        world
        |> World.TransformAvatar world.AvatarId avatarTransformHealth
        |> World.TransformAvatar world.AvatarId avatarTransformTurn
        |> Gamestate.AtSea
        |> Some
    let expected =
        []
        |> Gamestate.GameOver
        |> Some
    let actual =
        input
        |> Gamestate.CheckForAvatarDeath
    Assert.AreEqual(expected, actual)
