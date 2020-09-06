module InvalidInputTests

open Splorr.Seafarers.Controllers
open NUnit.Framework
open CommonTestFixtures

let private previousGameState =
    None
    |> Gamestate.MainMenu

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input =previousGameState
    let expected =previousGameState |> Some
    let actual =
        input
        |> ErrorMessage.Run sinkDummy ""
    Assert.AreEqual(expected, actual)