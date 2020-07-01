module ConfirmQuitTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open ConfirmQuitTestFixtures

[<Test>]
let ``Run function.When Yes command passed, return None.`` () =
    let input = previousState
    let inputSource = 
        Command.Yes 
        |> Some 
        |> toSource
    let expected = None
    let actual = 
        input
        |> ConfirmQuit.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run function.When No command passed, return previous State.`` () =
    let input = previousState
    let inputSource = 
        Command.No 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Some
    let actual = 
        input
        |> ConfirmQuit.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Run function.When invalid command passed, return ConfirmQuit.`` () =
    let input = previousState
    let inputSource = 
        None 
        |> toSource
    let expected = 
        input 
        |> Gamestate.ConfirmQuit 
        |> Some
    let actual = 
        input
        |> ConfirmQuit.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It initiates Confirm Quit Help when given the Help command.`` () =
    let input = previousState
    let inputSource = 
        Command.Help 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Gamestate.ConfirmQuit 
        |> Gamestate.Help 
        |> Some
    let actual =
        input
        |> ConfirmQuit.Run  inputSource sinkStub
    Assert.AreEqual(expected, actual)
