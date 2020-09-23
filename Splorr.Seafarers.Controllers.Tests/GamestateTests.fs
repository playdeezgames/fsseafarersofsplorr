module GamestateTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open CommonTestFixtures

type TestGamestateCheckForAvatarDeathContext (avatarMessageSource, shipmateSingleStatisticSource) =
    interface Shipmate.GetStatusContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface GamestateCheckForAvatarDeathContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource

let private avatarId = ""

let private world = 
    avatarId

[<Test>]
let ``GetWorld.It returns the world embedded within the given AtSea Gamestate.`` () =
    let expected = 
        world
        |> Some
    let actual = 
        world
        |> Gamestate.InPlay
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Docked (at Dock) Gamestate.`` () =
    let expected = world |> Some
    let actual = 
        world
        |> Gamestate.InPlay 
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
        |> Gamestate.InPlay
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
        |> Gamestate.InPlay
        |> Gamestate.Help
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given Metrics Gamestate when a world is present.`` () =
    let expected = world |> Some
    let actual = 
        world
        |> Gamestate.InPlay
        |> Gamestate.Metrics
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns the world embedded within the given InvalidInput Gamestate when a world is present.`` () =
    let expected = world |> Some
    let actual = 
        ("Maybe try 'help'?",world
        |> Gamestate.InPlay)
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
        world
        |> Gamestate.InPlay
        |> Gamestate.Jobs
        |> Gamestate.GetWorld
    Assert.AreEqual(world |> Some, actual)

[<Test>]
let ``GetWorld.It returns world from the given ItemList Gamestate.`` () =
    let expected = world |> Some
    let actual =
        world
        |> Gamestate.InPlay
        |> Gamestate.ItemList
        |> Gamestate.GetWorld
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetWorld.It returns world from the given Inventory Gamestate.`` () =
    let expected = world |> Some
    let actual =
        world
        |> Gamestate.InPlay
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
    let input =
        world
        |> Gamestate.InPlay
        |> Some
    let expected =
        input
    let context = TestGamestateCheckForAvatarDeathContext (avatarMessageSourceDummy, shipmateSingleStatisticSourceStub) :> GamestateCheckForAvatarDeathContext
    let actual =
        input
        |> Gamestate.CheckForAvatarDeath
            context
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CheckForAvatarDeath.It returns the original gamestate when there is not a world embedded in the gamestate.`` () =
    let input =
        None
        |> Gamestate.MainMenu
        |> Some
    let expected =
        input
    let context = TestGamestateCheckForAvatarDeathContext (avatarMessageSourceDummy, shipmateSingleStatisticSourceStub) :> GamestateCheckForAvatarDeathContext
    let actual =
        input
        |> Gamestate.CheckForAvatarDeath
            context
    Assert.AreEqual(expected, actual)


[<Test>]
let ``CheckForAvatarDeath.It returns gameover when the avatar embedded therein is dead.`` () =
    let input =
        world
        |> Gamestate.InPlay
        |> Some
    let expected =
        []
        |> Gamestate.GameOver
        |> Some
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSource - %s"))
    let context = TestGamestateCheckForAvatarDeathContext (avatarMessageSourceDummy, shipmateSingleStatisticSource) :> GamestateCheckForAvatarDeathContext
    let actual =
        input
        |> Gamestate.CheckForAvatarDeath 
            context
    Assert.AreEqual(expected, actual)
