module ShopTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open DockedTestFixtures

[<Test>]
let ``Run.It adds a message and returns Shop gamestate when given an invalid command.`` () =
    let subject =
        (dockLocation, dockWorld )
    let expected = 
        (subject |> fst, subject |> snd |> World.AddMessages ["Maybe try 'help'?"])
        |> Gamestate.Shop
        |> Some
    let actual =
        subject
        ||> Shop.Run (fun ()->None) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked gamestate when given the Dock command.`` () =
    let subject =
        (dockLocation, dockWorld)
    let expected = 
        subject
        |> Gamestate.Docked
        |> Some
    let actual =
        subject
        ||> Shop.Run (fun ()->Command.Dock |> Some) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Help gamestate when given the Help command.`` () =
    let subject =
        (dockLocation, dockWorld)
    let expected = 
        subject
        |> Gamestate.Shop
        |> Gamestate.Help
        |> Some
    let actual =
        subject
        ||> Shop.Run (fun ()->Command.Help |> Some) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status gamestate when given the Status command.`` () =
    let subject =
        (dockLocation, dockWorld)
    let expected = 
        subject
        |> Gamestate.Shop
        |> Gamestate.Status
        |> Some
    let actual =
        subject
        ||> Shop.Run (fun ()->Command.Status |> Some) (sink)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit gamestate when given the Quit command.`` () =
    let subject =
        (dockLocation, dockWorld)
    let expected = 
        subject
        |> Gamestate.Shop
        |> Gamestate.ConfirmQuit
        |> Some
    let actual =
        subject
        ||> Shop.Run (fun ()->Command.Quit |> Some) (sink)
    Assert.AreEqual(expected, actual)




