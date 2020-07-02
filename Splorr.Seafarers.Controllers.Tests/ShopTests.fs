module ShopTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures

[<Test>]
let ``Run.It adds a message and returns Docked (at Shop) gamestate when given an invalid command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let input =
        (inputLocation, inputWorld)
    let inputSource = 
        None 
        |> toSource
    let expectedWorld = 
        inputWorld 
        |> World.AddMessages ["Maybe try 'help'?"]
    let expected = 
        (Shop, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        input
        ||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at Dock) gamestate when given the Dock command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = 
        Command.Dock 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at ItemList) gamestate when given the Items command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = 
        Command.Items 
        |> Some 
        |> toSource
    let expected = 
        (ItemList, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Help gamestate when given the Help command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = Command.Help |> Some |> toSource
    let expected = 
        (Shop, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Gamestate.Help
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status gamestate when given the Status command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = Command.Status |> Some |> toSource
    let expected = 
        (Shop, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Gamestate.Status
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit gamestate when given the Quit command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = Command.Quit |> Some |> toSource
    let expected = 
        (Shop, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Gamestate.ConfirmQuit
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)




