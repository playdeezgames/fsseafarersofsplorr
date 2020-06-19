﻿module Splorr.Seafarers.Views.Tests.ConfirmQuitTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Views

let private previousState = World.Create() |> AtSea
let private sink (_:string) : unit = ()
let private makeSource (command:Command option) = fun () -> command

[<Test>]
let ``Run function.When Yes command passed, return None.`` () =
    let actual = 
        previousState
        |> ConfirmQuit.Run (Yes |> Some |> makeSource) sink 
    Assert.AreEqual(None, actual)

[<Test>]
let ``Run function.When No command passed, return previous State.`` () =
    let actual = 
        previousState
        |> ConfirmQuit.Run (No |> Some |> makeSource) sink 
    Assert.AreEqual(previousState |> Some, actual)
    
[<Test>]
let ``Run function.When invalid command passed, return ConfirmQuit.`` () =
    let actual = 
        previousState
        |> ConfirmQuit.Run (None |> makeSource) sink 
    Assert.AreEqual(previousState |> ConfirmQuit |> Some, actual)

