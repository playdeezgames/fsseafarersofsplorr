module HelpTests

open CommonTestFixtures
open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System

let private sink(_:Message) : unit = ()

let private configuration: WorldConfiguration =
    {
        AvatarDistances        = (10.0, 1.0)
        MaximumGenerationTries = 10u
        MinimumIslandDistance  = 30.0
        RationItems            = [1UL]
        RewardRange            = (1.0, 10.0)
        StatisticDescriptors   = []
        WorldSize              = (10.0, 10.0)
    }

let private random = Random()

let private avatarId = ""

let private world =  
    World.Create 
        nameSource
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        configuration 
        random 
        avatarId

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

