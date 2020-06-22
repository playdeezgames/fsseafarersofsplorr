module DockedTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

let private dockWorldconfiguration: WorldGenerationConfiguration ={WorldSize=(0.0, 0.0); MinimumIslandDistance=30.0; MaximumGenerationTries=1u}
let private dockWorld = World.Create dockWorldconfiguration (System.Random())
let private dockLocation = (0.0, 0.0)
let private sink (_:string) : unit = ()

[<Test>]
let ``Run.It returns AtSea when given Undock Command.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> Undock |> Some) sink 
    Assert.AreEqual({dockWorld with Messages = ["You undock."]} |> AtSea |> Some,actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit Command.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> Quit |> Some) sink 
    Assert.AreEqual((dockLocation, dockWorld) |> Docked |> ConfirmQuit |> Some,actual)

[<Test>]
let ``Run.It returns Help when given Help Command.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> Command.Help |> Some) sink 
    Assert.AreEqual((dockLocation, dockWorld) |> Docked |> Help |> Some,actual)

[<Test>]
let ``Run.It returns Docked when given invalid Command.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> None) sink 
    Assert.AreEqual((dockLocation, dockWorld) |> Docked |> Some,actual)


[<Test>]
let ``Run.It returns AtSea when given invalid docked location.`` () =
    let mutable called:bool = false
    let actual =
        ((1.0,1.0), dockWorld)
        ||> Docked.Run 
            (fun _ -> 
                called <- true
                Command.Help 
                |> Some) sink 
    Assert.AreEqual(dockWorld |> AtSea |> Some,actual)
    Assert.IsFalse(called)


//[<Test>]
//let ``Run.It .`` () =
//    raise (System.NotImplementedException "Not Implemented")

