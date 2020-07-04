﻿module StatusTests

open NUnit.Framework
open Splorr.Seafarers.Controllers

let private previousGameState =
    None
    |> Gamestate.MainMenu
let private sink(_:string) : unit = ()

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input =previousGameState
    let expected =previousGameState |> Some
    let actual =
        input
        |> Status.Run sink
    Assert.AreEqual(expected, actual)

