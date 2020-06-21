module HelpTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Views

let private sink(_:string) : unit = ()
let private configuration: WorldGenerationConfiguration ={WorldSize=(10.0, 10.0); MinimumIslandDistance=30.0; MaximumGenerationTries=10u}
let private world =  World.Create configuration (System.Random())

[<Test>]
let ``Run.It returns the given AtSea ViewState`` () =
    let originalState = 
        world
        |> AtSea
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)


[<Test>]
let ``Run.It returns the given ConfirmQuit ViewState`` () =
    let originalState = 
        world
        |> AtSea
        |> ConfirmQuit
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)

