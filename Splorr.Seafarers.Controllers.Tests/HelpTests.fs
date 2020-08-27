module HelpTests

open CommonTestFixtures
open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System
open AtSeaTestFixtures

let private sink(_:Message) : unit = ()

let private random = Random()

let private avatarId = ""

let private world =  
    { AvatarId = avatarId }

[<Test>]
let ``Run.It returns the given AtSea Gamestate`` () =
    let input = 
        world
        |> Gamestate.AtSea
    let expected = 
        input
        |> Some
    let actual = 
        input
        |> Help.Run sink
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns the given ConfirmQuit Gamestate`` () =
    let input = 
        world
        |> Gamestate.AtSea
        |> Gamestate.ConfirmQuit
    let expected = 
        input
        |> Some
    let actual = 
        input
        |> Help.Run sink
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns the given Docked (at Dock) Gamestate`` () =
    let input = 
        (Dock, (0.0, 0.0), world)
        |> Gamestate.Docked
    let expected = 
        input
        |> Some
    let actual = 
        input
        |> Help.Run sink
    Assert.AreEqual(expected, actual)

