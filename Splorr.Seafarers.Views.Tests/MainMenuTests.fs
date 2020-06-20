module MainMenuTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Views

let private sink(_:string) : unit = ()

[<Test>]
let ``Run.It returns Confirm Quit when given Quit command.`` () =
    let actual =
        None
        |> MainMenu.Run (fun()->Quit |> Some) sink 
    Assert.AreEqual(None |> MainMenu |> ConfirmQuit |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu when given invalid command.`` () =
    let actual =
        None
        |> MainMenu.Run (fun()->None) sink 
    Assert.AreEqual(None |> MainMenu |> Some, actual)

//let ``Run.It returns YYYY when given XXXX command.`` () =
//    raise (System.NotImplementedException "Not Implemented")


