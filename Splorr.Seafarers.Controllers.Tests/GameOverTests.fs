module GameOverTests

open NUnit.Framework
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
        |> GameOver.Run sinkDummy
    Assert.AreEqual(expected, actual)
