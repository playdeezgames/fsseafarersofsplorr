module MainMenuTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

let private configuration: WorldGenerationConfiguration =
    {
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=10u
        RewardRange = (1.0, 10.0)
    }
let private world = World.Create configuration (System.Random())
let private sink(_:Message) : unit = ()

[<Test>]
let ``Run.It returns Confirm Quit when given Quit command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run configuration (fun()->Command.Quit |> Some) sink 
    Assert.AreEqual(None |> Gamestate.MainMenu |> Gamestate.ConfirmQuit |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu when given Quit command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run configuration (fun()->Command.Quit |> Some) sink 
    Assert.AreEqual(world |> Some |> Gamestate.MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu when given invalid command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run configuration (fun()->None) sink 
    Assert.AreEqual(None |> Gamestate.MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu when given invalid command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run configuration (fun()->None) sink 
    Assert.AreEqual(world |> Some |> Gamestate.MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns At Sea when given Start command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run configuration (fun()->Command.Start |> Some) sink 
    match actual with
    | Some (Gamestate.AtSea _) -> true
    | _ -> false
    |> Assert.True


[<Test>]
let ``Run.It returns Main Menu when given Start command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run configuration (fun()->Command.Start |> Some) sink 
    Assert.AreEqual(world |> Some |> Gamestate.MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu with no world when given Abandon Game command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run configuration (fun()->Game |> Command.Abandon |> Some) sink 
    Assert.AreEqual(None |> Gamestate.MainMenu |> Some, actual)


[<Test>]
let ``Run.It returns Main Menu with no world when given Abandon Game command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run configuration (fun()->Game |> Command.Abandon |> Some) sink 
    Assert.AreEqual(None |> Gamestate.MainMenu |> Some, actual)


[<Test>]
let ``Run.It returns At Sea when given Resume command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run configuration (fun()->Command.Resume |> Some) sink 
    Assert.AreEqual(world |> Gamestate.AtSea |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu with no world when given Resume command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run configuration (fun()->Command.Resume |> Some) sink 
    Assert.AreEqual(None |> Gamestate.MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu when given the Save command and there is no world.`` () =
    let inputName = "name"
    let inputWorld = None
    let expected = None |> Gamestate.MainMenu |> Some
    let actual =
        inputWorld
        |> MainMenu.Run configuration (fun()->inputName |> Command.Save |> Some) sink 
    Assert.AreEqual(expected, actual)

//[<Test>]
//let ``Run.It returns YYYY when given XXXX command.`` () =
//    raise (System.NotImplementedException "Not Implemented")


