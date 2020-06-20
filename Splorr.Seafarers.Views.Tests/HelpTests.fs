module HelpTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Views

let private sink(_:string) : unit = ()

[<Test>]
let ``Run.It returns the given AtSea ViewState`` () =
    let originalState = 
        World.Create()
        |> AtSea
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)


[<Test>]
let ``Run.It returns the given ConfirmQuit ViewState`` () =
    let originalState = 
        World.Create()
        |> AtSea
        |> ConfirmQuit
    let actual = 
        originalState
        |> Help.Run sink
    Assert.AreEqual(originalState |> Some, actual)

