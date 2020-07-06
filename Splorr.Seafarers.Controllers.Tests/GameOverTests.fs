module GameOverTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures

[<Test>]
let ``Run.It returns MainMenu None``() =
    let input = []
    let expected =
        None
        |> Gamestate.MainMenu
        |> Some
    let actual =
        input   
        |> GameOver.Run sinkStub
    Assert.AreEqual(expected, actual)
