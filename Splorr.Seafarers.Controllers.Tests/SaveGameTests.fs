module SaveGameTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open AtSeaTestFixtures
open CommonTestFixtures

[<Test>]
let ``Run.It returns Main Menu after saving the game.`` () =
    let input = world
    let inputName = "name"
    let expected =
        input
        |> Some
        |> Gamestate.MainMenu
        |> Some
    let actual =
        (inputName, input)
        ||> SaveGame.Run sinkStub
    Assert.AreEqual(expected, actual)

