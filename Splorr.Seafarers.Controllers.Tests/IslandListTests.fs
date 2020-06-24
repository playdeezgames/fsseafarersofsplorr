module IslandListTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

let private previousGameState =
    None
    |> Gamestate.MainMenu
let private sink(_:string) : unit = ()

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let actual =
        previousGameState
        |> IslandList.Run sink 0u
    Assert.AreEqual(previousGameState |> Some, actual)
