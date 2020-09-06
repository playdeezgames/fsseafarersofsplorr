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

type TestHelpRunContext(avatarIslandFeatureSource) =
    interface HelpRunContext with
        member _.avatarIslandFeatureSource : AvatarIslandFeatureSource = avatarIslandFeatureSource

[<Test>]
let ``Run.It returns the given AtSea Gamestate`` () =
    let input = 
        world
        |> Gamestate.InPlay
    let expected = 
        input
        |> Some
    let mutable sinkCalled = false
    let sink(_:Message) : unit =
        sinkCalled <- true
    let avatarIslandFeatureSource (_) = None
    let context = 
        TestHelpRunContext(avatarIslandFeatureSource)
    let actual = 
        input
        |> Help.Run
            context
            sink
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(sinkCalled)

[<Test>]
let ``Run.It returns the given ConfirmQuit Gamestate`` () =
    let input = 
        world
        |> Gamestate.InPlay
        |> Gamestate.ConfirmQuit
    let expected = 
        input
        |> Some
    let mutable sinkCalled = false
    let sink(_:Message) : unit =
        sinkCalled <- true
    let avatarIslandFeatureSource (_) = None
    let context = 
        TestHelpRunContext(avatarIslandFeatureSource)
    let actual = 
        input
        |> Help.Run 
            context
            sink
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(sinkCalled)

[<Test>]
let ``Run.It returns the given Docked (at Dock) Gamestate`` () =
    let input = 
        world
        |> Gamestate.InPlay
    let expected = 
        input
        |> Some
    let mutable sinkCalled = false
    let sink(_:Message) : unit =
        sinkCalled <- true
    let avatarIslandFeatureSource (_) = None
    let context = 
        TestHelpRunContext(avatarIslandFeatureSource)
    let actual = 
        input
        |> Help.Run 
            context
            sink
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(sinkCalled)

[<Test>]
let ``Run.It returns the given Docked (at Feature) Gamestate`` () =
    let input = 
        world
        |> Gamestate.InPlay
    let expected = 
        input
        |> Some
    let mutable sinkCalled = false
    let sink(_:Message) : unit =
        sinkCalled <- true
    let avatarIslandFeatureSource (_) = None
    let context = 
        TestHelpRunContext(avatarIslandFeatureSource)
    let actual = 
        input
        |> Help.Run 
            context
            sink
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(sinkCalled)

