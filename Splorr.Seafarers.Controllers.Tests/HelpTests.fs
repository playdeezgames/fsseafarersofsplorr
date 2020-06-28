﻿module HelpTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

let private sink(_:string) : unit = ()
let private configuration: WorldGenerationConfiguration =
    {
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=10u
        RewardRange = (1.0, 10.0)
        Commodities = Map.empty
    }
let private world =  World.Create configuration (System.Random())

[<Test>]
let ``Run.It returns the given AtSea Gamestate`` () =
    let originalState = 
        world
        |> Gamestate.AtSea
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)

[<Test>]
let ``Run.It returns the given ConfirmQuit Gamestate`` () =
    let originalState = 
        world
        |> Gamestate.AtSea
        |> Gamestate.ConfirmQuit
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)

[<Test>]
let ``Run.It returns the given Docked Gamestate`` () =
    let originalState = 
        ((0.0, 0.0), world)
        |> Gamestate.Docked
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)

