module DockedTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open DockedTestFixtures

[<Test>]
let ``Run.It returns AtSea when given Undock Command.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> Command.Undock |> Some) sink 
    Assert.AreEqual({dockWorld with Messages = ["You undock."]} |> Gamestate.AtSea |> Some,actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit Command.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> Command.Quit |> Some) sink 
    Assert.AreEqual((dockLocation, dockWorld) |> Gamestate.Docked |> Gamestate.ConfirmQuit |> Some,actual)

[<Test>]
let ``Run.It returns Help when given Help Command.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> Command.Help |> Some) sink 
    Assert.AreEqual((dockLocation, dockWorld) |> Gamestate.Docked |> Gamestate.Help |> Some,actual)

[<Test>]
let ``Run.It returns Docked when given invalid Command.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> None) sink 
    Assert.AreEqual((dockLocation, dockWorld) |> Gamestate.Docked |> Some,actual)


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
    Assert.AreEqual(dockWorld |> Gamestate.AtSea |> Some,actual)
    Assert.IsFalse(called)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> Command.Status |> Some) sink 
    Assert.AreEqual((dockLocation, dockWorld) |> Gamestate.Docked |> Gamestate.Status |> Some, actual)


[<Test>]
let ``Run.It returns Jobs gamestate when given the command Jobs.`` () =
    let actual =
        (dockLocation, dockWorld)
        ||> Docked.Run (fun _ -> Command.Jobs |> Some) sink 
    Assert.AreEqual((dockLocation, dockWorld) |> Gamestate.Jobs |> Some, actual)

[<Test>]
let ``Run.It gives a message when given the Accept Job command and the given job number does not exist.`` () =
    let actual =
        smallWorldDocked
        |> Docked.Run (fun () -> 0u |> Command.AcceptJob |> Some) sink smallWorldIslandLocation
    Assert.AreEqual((smallWorldIslandLocation, {smallWorldDocked with Messages = [ "That job is currently unavailable." ] } ) |> Gamestate.Docked |> Some, actual)

[<Test>]
let ``Run.It gives a message when given the command Abandon Job and the avatar has no current job.`` () =
    let subject = dockWorld
    let expected = (dockLocation, {subject with Messages = ["You have no job to abandon."]}) |> Gamestate.Docked |> Some
    let actual =
        subject
        |> Docked.Run (fun () -> Job |> Command.Abandon |> Some) sink dockLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let subject = abandonJobWorld
    let expected = (dockLocation, {subject with Messages = ["You abandon your job."]; Avatar={subject.Avatar with Job=None; Reputation = subject.Avatar.Reputation-1.0}}) |> Gamestate.Docked |> Some
    let actual =
        subject
        |> Docked.Run (fun () -> Job |> Command.Abandon |> Some) sink dockLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns the PriceList state when given the Prices command.`` () =
    let subject = dockWorld
    let expected = (dockLocation, dockWorld) |> Gamestate.PriceList |> Some
    let actual =
        subject
        |> Docked.Run (fun () -> Command.Prices |> Some) sink dockLocation
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns the Shop state when given the Shop command.`` () =
    let subject = dockWorld
    let expected = (dockLocation, dockWorld) |> Gamestate.Shop |> Some
    let actual =
        subject
        |> Docked.Run (fun () -> Command.Shop |> Some) sink dockLocation
    Assert.AreEqual(expected, actual)

//[<Test>]
//let ``Run.It .`` () =
//    raise (System.NotImplementedException "Not Implemented")

