module ShopTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures

[<Test>]
let ``Run.It adds a message and returns Docked (at Shop) gamestate when given an invalid command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let input =
        (subjectLocation, subjectWorld)
    let expected = 
        (Shop, subjectLocation, subjectWorld |> World.AddMessages ["Maybe try 'help'?"])
        |> Gamestate.Docked
        |> Some
    let actual =
        input
        ||> Shop.Run (fun ()->None) (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at Dock) gamestate when given the Dock command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let input =
        (Dock, subjectLocation, subjectWorld)
    let expected = 
        input
        |> Gamestate.Docked
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Dock |> Some) (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at ItemList) gamestate when given the Items command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let input =
        (ItemList, subjectLocation, subjectWorld)
    let expected = 
        input
        |> Gamestate.Docked
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Items |> Some) (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Help gamestate when given the Help command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let input =
        (Shop, subjectLocation, subjectWorld)
    let expected = 
        input
        |> Gamestate.Docked
        |> Gamestate.Help
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Help |> Some) (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status gamestate when given the Status command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let input =
        (Shop, subjectLocation, subjectWorld)
    let expected = 
        input
        |> Gamestate.Docked
        |> Gamestate.Status
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Status |> Some) (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit gamestate when given the Quit command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let input =
        (Shop, subjectLocation, subjectWorld)
    let expected = 
        input
        |> Gamestate.Docked
        |> Gamestate.ConfirmQuit
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Quit |> Some) (sinkStub)
    Assert.AreEqual(expected, actual)




