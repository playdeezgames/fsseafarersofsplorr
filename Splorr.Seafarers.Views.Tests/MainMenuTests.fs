module MainMenuTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Views

let private configuration: WorldGenerationConfiguration ={WorldSize=(10.0, 10.0); MinimumIslandDistance=30.0; MaximumGenerationTries=10u}
let private world = World.Create configuration (System.Random())
let private sink(_:string) : unit = ()

[<Test>]
let ``Run.It returns Confirm Quit when given Quit command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run (fun()->Quit |> Some) sink 
    Assert.AreEqual(None |> MainMenu |> ConfirmQuit |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu when given Quit command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run (fun()->Quit |> Some) sink 
    Assert.AreEqual(world |> Some |> MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu when given invalid command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run (fun()->None) sink 
    Assert.AreEqual(None |> MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu when given invalid command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run (fun()->None) sink 
    Assert.AreEqual(world |> Some |> MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns At Sea when given Start command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run (fun()->Start |> Some) sink 
    match actual with
    | Some (AtSea _) -> true
    | _ -> false
    |> Assert.True


[<Test>]
let ``Run.It returns Main Menu when given Start command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run (fun()->Start |> Some) sink 
    Assert.AreEqual(world |> Some |> MainMenu |> Some, actual)

[<Test>]
let ``Run.It returns Main Menu with no world when given Abandon command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run (fun()->Abandon |> Some) sink 
    Assert.AreEqual(None |> MainMenu |> Some, actual)


[<Test>]
let ``Run.It returns Main Menu with no world when given Abandon command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run (fun()->Abandon |> Some) sink 
    Assert.AreEqual(None |> MainMenu |> Some, actual)


[<Test>]
let ``Run.It returns At Sea when given Resume command and there is a world.`` () =
    let actual =
        world
        |> Some
        |> MainMenu.Run (fun()->Resume |> Some) sink 
    Assert.AreEqual(world |> AtSea |> Some, actual)


[<Test>]
let ``Run.It returns Main Menu with no world when given Resume command and there is no world.`` () =
    let actual =
        None
        |> MainMenu.Run (fun()->Resume |> Some) sink 
    Assert.AreEqual(None |> MainMenu |> Some, actual)

//[<Test>]
//let ``Run.It returns YYYY when given XXXX command.`` () =
//    raise (System.NotImplementedException "Not Implemented")


