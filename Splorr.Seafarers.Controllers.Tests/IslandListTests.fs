module IslandListTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures

let private previousGameState =
    None
    |> Gamestate.MainMenu
let private sink(_:Message) : unit = ()

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let actual =
        previousGameState
        |> IslandList.Run sink 0u avatarId
    Assert.AreEqual(previousGameState |> Some, actual)
