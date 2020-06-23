module StatusTests

open NUnit.Framework
open Splorr.Seafarers.Controllers

let private previousGameState =
    None
    |> MainMenu
let private sink(_:string) : unit = ()

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let actual =
        previousGameState
        |> Status.Run sink
    Assert.AreEqual(previousGameState |> Some, actual)

