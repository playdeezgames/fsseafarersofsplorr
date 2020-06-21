module ConfirmQuitTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Views

let configuration: WorldGenerationConfiguration ={WorldSize=(10.0, 10.0); MinimumIslandDistance=30.0; MaximumGenerationTries=10u}
let private previousState = 
    World.Create configuration (System.Random())
    |> AtSea
let private sink (_:string) : unit = ()
let private makeSource (command:Command option) = fun () -> command

[<Test>]
let ``Run function.When Yes command passed, return None.`` () =
    let actual = 
        previousState
        |> ConfirmQuit.Run (Yes |> Some |> makeSource) sink 
    Assert.AreEqual(None, actual)

[<Test>]
let ``Run function.When No command passed, return previous State.`` () =
    let actual = 
        previousState
        |> ConfirmQuit.Run (No |> Some |> makeSource) sink 
    Assert.AreEqual(previousState |> Some, actual)
    
[<Test>]
let ``Run function.When invalid command passed, return ConfirmQuit.`` () =
    let actual = 
        previousState
        |> ConfirmQuit.Run (None |> makeSource) sink 
    Assert.AreEqual(previousState |> ConfirmQuit |> Some, actual)


[<Test>]
let ``Run.It initiates Confirm Quit Help when given the Help command.`` () =
    let actual =
        previousState
        |> ConfirmQuit.Run  (fun()->Command.Help |> Some) sink
    Assert.AreEqual(previousState |> ConfirmQuit |> Help |> Some, actual)
