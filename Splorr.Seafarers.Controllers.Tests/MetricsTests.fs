module MetricsTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open Splorr.Seafarers.Models
open AtSeaTestFixtures

let private previousGameState =
    None
    |> Gamestate.MainMenu

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input =previousGameState
    let expected =previousGameState |> Some
    let actual =
        input
        |> Metrics.Run
            avatarMetricSourceStub
            sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It works when all of the metrics have counters.`` () =
    let inputAvatar =
        world.Avatars.[avatarId]
    let inputWorld =
        {world with Avatars =world.Avatars |> Map.add avatarId inputAvatar}
    let input = 
        inputWorld
        |> Gamestate.AtSea
    let expected = input |> Some
    let actual =
        input
        |> Metrics.Run 
            avatarMetricSourceStub
            sinkStub
    Assert.AreEqual(expected, actual)

