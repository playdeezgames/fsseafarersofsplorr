module ConfirmQuitTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open ConfirmQuitTestFixtures
open Splorr.Seafarers.Services

type TestConfirmQuitRunContext(switchSource) =
    interface ConfirmQuit.RunContext with
        member this.switchSource: SwitchSource = switchSource

[<Test>]
let ``Run function.When Yes command passed, return None.`` () =
    let input = previousState
    let inputSource = 
        Command.Yes 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = None
    let context = TestConfirmQuitRunContext(fun ()->Set.empty) :> CommonContext
    let actual = 
        input
        |> ConfirmQuit.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Run function.When "on-stream" switch is set, return previous State and do not ask for input.`` () =
    let input = previousState
    let inputSource () : Command option = 
        Assert.Fail "This should not have been called."
        None
    let expected = 
        input 
        |> Some
    let context = TestConfirmQuitRunContext(fun () -> Set.empty |> Set.add "on-stream") :> CommonContext
    let actual = 
        input
        |> ConfirmQuit.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run function.When No command passed, return previous State.`` () =
    let input = previousState
    let inputSource = 
        Command.No 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input 
        |> Some
    let context = TestConfirmQuitRunContext(fun () -> Set.empty) :> CommonContext
    let actual = 
        input
        |> ConfirmQuit.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Run function.When invalid command passed, return ConfirmQuit.`` () =
    let input = previousState
    let inputSource = 
        None 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input 
        |> Gamestate.ConfirmQuit 
        |> Some
    let context = TestConfirmQuitRunContext(fun () -> Set.empty) :> CommonContext
    let actual = 
        input
        |> ConfirmQuit.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It initiates Confirm Quit Help when given the Help command.`` () =
    let input = previousState
    let inputSource = 
        Command.Help 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input 
        |> Gamestate.ConfirmQuit 
        |> Gamestate.Help 
        |> Some
    let context = TestConfirmQuitRunContext(fun () -> Set.empty) :> CommonContext
    let actual =
        input
        |> ConfirmQuit.Run 
            context
            inputSource sinkDummy
    Assert.AreEqual(expected, actual)
