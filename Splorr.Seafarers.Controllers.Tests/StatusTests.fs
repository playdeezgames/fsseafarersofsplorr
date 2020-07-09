module StatusTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures

let private previousGameState =
    None
    |> Gamestate.MainMenu
let private sink(_:Message) : unit = ()

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input =previousGameState
    let expected =previousGameState |> Some
    let actual =
        input
        |> Status.Run sink avatarId
    Assert.AreEqual(expected, actual)

