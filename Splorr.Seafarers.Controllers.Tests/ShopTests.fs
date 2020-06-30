module ShopTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open DockedTestFixtures

[<Test>]
let ``Run.It adds a message and returns Docked (at Shop) gamestate when given an invalid command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let subject =
        (subjectLocation, subjectWorld)
    let expected = 
        (Shop, subjectLocation, subjectWorld |> World.AddMessages ["Maybe try 'help'?"])
        |> Gamestate.Docked
        |> Some
    let actual =
        subject
        ||> Shop.Run (fun ()->None) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at Dock) gamestate when given the Dock command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let subject =
        (Dock, subjectLocation, subjectWorld)
    let expected = 
        subject
        |> Gamestate.Docked
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Dock |> Some) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at ItemList) gamestate when given the Items command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let subject =
        (ItemList, subjectLocation, subjectWorld)
    let expected = 
        subject
        |> Gamestate.Docked
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Items |> Some) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Help gamestate when given the Help command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let subject =
        (Shop, subjectLocation, subjectWorld)
    let expected = 
        subject
        |> Gamestate.Docked
        |> Gamestate.Help
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Help |> Some) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status gamestate when given the Status command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let subject =
        (Shop, subjectLocation, subjectWorld)
    let expected = 
        subject
        |> Gamestate.Docked
        |> Gamestate.Status
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Status |> Some) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit gamestate when given the Quit command.`` () =
    let subjectLocation = dockLocation
    let subjectWorld = dockWorld
    let subject =
        (Shop, subjectLocation, subjectWorld)
    let expected = 
        subject
        |> Gamestate.Docked
        |> Gamestate.ConfirmQuit
        |> Some
    let actual =
        (subjectLocation, subjectWorld)
        ||> Shop.Run (fun ()->Command.Quit |> Some) (sink)
    Assert.AreEqual(expected, actual)




