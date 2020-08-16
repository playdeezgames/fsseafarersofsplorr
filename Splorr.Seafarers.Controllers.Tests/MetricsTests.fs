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
        |> Metrics.Run sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It works when all of the metrics have counters.`` () =
    let inputMetrics = 
        (System.Enum.GetValues(typedefof<Metric>)) :?> (Metric array)
        |> Array.map (fun m -> (m, 1UL))
        |> Map.ofArray
    let inputAvatar =
        {world.Avatars.[avatarId] with Metrics = inputMetrics}
    let inputWorld =
        {world with Avatars =world.Avatars |> Map.add avatarId inputAvatar}
    let input = 
        inputWorld
        |> Gamestate.AtSea
    let expected = input |> Some
    let actual =
        input
        |> Metrics.Run sinkStub
    Assert.AreEqual(expected, actual)

