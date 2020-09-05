module HelpTests

open CommonTestFixtures
open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open System


let private random = Random()

let private avatarId = ""

let private world =  
    avatarId

[<Test>]
let ``Run.It returns the given AtSea Gamestate`` () =
    let input = 
        world
        |> Gamestate.AtSea
    let expected = 
        input
        |> Some
    let mutable sinkCalled = false
    let sink(_:Message) : unit =
        sinkCalled <- true
    let actual = 
        input
        |> Help.Run sink
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(sinkCalled)

[<Test>]
let ``Run.It returns the given ConfirmQuit Gamestate`` () =
    let input = 
        world
        |> Gamestate.AtSea
        |> Gamestate.ConfirmQuit
    let expected = 
        input
        |> Some
    let mutable sinkCalled = false
    let sink(_:Message) : unit =
        sinkCalled <- true
    let actual = 
        input
        |> Help.Run sink
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(sinkCalled)

[<Test>]
let ``Run.It returns the given Docked (at Dock) Gamestate`` () =
    let input = 
        (Feature IslandFeatureIdentifier.Dock, (0.0, 0.0), world)
        |> Gamestate.Docked
    let expected = 
        input
        |> Some
    let mutable sinkCalled = false
    let sink(_:Message) : unit =
        sinkCalled <- true
    let actual = 
        input
        |> Help.Run sink
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(sinkCalled)

[<Test>]
let ``Run.It returns the given Docked (at Feature) Gamestate`` () =
    let input = 
        (Feature IslandFeatureIdentifier.DarkAlley, (0.0, 0.0), world)
        |> Gamestate.Docked
    let expected = 
        input
        |> Some
    let mutable sinkCalled = false
    let sink(_:Message) : unit =
        sinkCalled <- true
    let actual = 
        input
        |> Help.Run sink
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(sinkCalled)

