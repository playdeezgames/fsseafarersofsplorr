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
        (None, world)
        |> Gamestate.InPlay
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
        (None, world)
        |> Gamestate.InPlay
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
        (Some(IslandFeatureIdentifier.Dock, (0.0, 0.0)), world)
        |> Gamestate.InPlay
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
        (Some(IslandFeatureIdentifier.DarkAlley, (0.0, 0.0)), world)
        |> Gamestate.InPlay
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

