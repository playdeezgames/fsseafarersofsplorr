module HelpTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

let private sink(_:string) : unit = ()
let private configuration: WorldGenerationConfiguration ={WorldSize=(10.0, 10.0); MinimumIslandDistance=30.0; MaximumGenerationTries=10u}
let private world =  World.Create configuration (System.Random())

[<Test>]
let ``Run.It returns the given AtSea Gamestate`` () =
    let originalState = 
        world
        |> AtSea
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)

[<Test>]
let ``Run.It returns the given ConfirmQuit Gamestate`` () =
    let originalState = 
        world
        |> AtSea
        |> ConfirmQuit
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)

[<Test>]
let ``Run.It returns the given Docked Gamestate`` () =
    let originalState = 
        ((0.0, 0.0), world)
        |> Docked
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)

