module MetricsTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Services

let private previousGameState =
    None
    |> Gamestate.MainMenu

type TestMetricsRunContext(avatarMetricSource) =
    interface ServiceContext
    interface Metrics.RunWorldContext with
        member this.avatarMetricSource: AvatarMetricSource = avatarMetricSource

[<Test>]
let ``Run.It returns the given gamestate.`` () =
    let input =previousGameState
    let expected =previousGameState |> Some
    let context = TestMetricsRunContext(avatarMetricSourceStub) :> ServiceContext
    let actual =
        input
        |> Metrics.Run
            context
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It works when all of the metrics have counters.`` () =
    let inputWorld =
        world
    let input = 
        inputWorld
        |> Gamestate.InPlay
    let expected = input |> Some
    let context = TestMetricsRunContext(avatarMetricSourceStub) :> ServiceContext
    let actual =
        input
        |> Metrics.Run 
            context
            sinkDummy
    Assert.AreEqual(expected, actual)

