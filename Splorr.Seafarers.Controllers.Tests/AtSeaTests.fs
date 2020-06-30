module AtSeaTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

open AtSeaTestFixtures

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit command.`` () =
    let actual = 
        world
        |> AtSea.Run random (fun ()->Some Command.Quit) sink
    Assert.AreEqual(world |> Gamestate.AtSea |> Gamestate.ConfirmQuit |> Some, actual)

[<Test>]
let ``Run.It returns AtSea when given invalid command.`` () =
    let actual =
        world
        |> AtSea.Run random (fun()->None) sink
    Assert.AreEqual(world |> Gamestate.AtSea |> Some, actual)

[<Test>]
let ``Run.It returns AtSea with new speed when given Set Speed command.`` () =
    let newSpeed = 0.5
    let actual =
        world
        |> AtSea.Run random (fun()->newSpeed |> SetCommand.Speed |> Command.Set |> Some) sink
    Assert.AreEqual({world with Avatar = { world.Avatar with Speed=newSpeed}; Messages=["You set your speed to 0.500000."] }|> Gamestate.AtSea |> Some, actual)

[<Test>]
let ``Run.It returns AtSea with new heading when given Set Heading command.`` () =
    let newHeading = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
    let actual =
        world
        |> AtSea.Run random (fun()->newHeading |> SetCommand.Heading |> Command.Set |> Some) sink
    Assert.AreEqual({world with Avatar = { world.Avatar with Heading = newHeading |> Dms.ToFloat}; Messages=["You set your heading to 1\u00b02'3.000000\"."] }|> Gamestate.AtSea |> Some, actual)

[<Test>]
let ``Run.It moves the avatar when given Move command.`` () =
    let actual =
        world
        |> AtSea.Run random (fun()->1u |> Command.Move |> Some) sink
    Assert.AreEqual({world with Avatar = {world.Avatar with Position=(6.0,5.0)}; Messages=["Steady as she goes."]; Turn=1u} |> Gamestate.AtSea |> Some, actual)

[<Test>]
let ``Run.It returns At Sea Help when given the Help command.`` () =
    let actual =
        world
        |> AtSea.Run random (fun()->Command.Help |> Some) sink
    Assert.AreEqual(world |> Gamestate.AtSea |> Gamestate.Help |> Some, actual)


[<Test>]
let ``Run.It returns Main Menu when given the Menu command.`` () =
    let actual =
        world
        |> AtSea.Run random (fun()->Command.Menu |> Some) sink
    Assert.AreEqual(world |> Some |> Gamestate.MainMenu |> Some, actual)


[<Test>]
let ``Run.It returns Island List when given the Islands command.`` () =
    let actual =
        world
        |> AtSea.Run random (fun()->0u |> Command.Islands |> Some) sink
    Assert.AreEqual((0u, world |> Gamestate.AtSea) |> Gamestate.IslandList |> Some, actual)

[<Test>]
let ``Run.It returns AtSea when given the Dock command and there is no near enough island.`` () =
    let actual =
        emptyWorld
        |> AtSea.Run random (fun()->Command.Dock |> Some) sink
    Assert.AreEqual({emptyWorld with Messages=["There is no place to dock."]}|>Gamestate.AtSea|>Some, actual)

[<Test>]
let ``Run.It returns Docked when given the Dock command and there is a near enough island.`` () =
    let actual =
        dockWorld
        |> AtSea.Run random (fun()->Command.Dock |> Some) sink
    let updatedIsland = dockWorld.Islands.[(0.0, 0.0)] |> Island.AddVisit dockWorld.Turn
    Assert.AreEqual((Dock, (0.0,0.0),{dockWorld with Messages = ["You dock."]; Islands = dockWorld.Islands |> Map.add (0.0,0.0) updatedIsland })|>Gamestate.Docked|>Some, actual)

[<Test>]
let ``Run.It gives a message when given a Head For command and the given island does not exist.`` () =
    let actual = 
        headForWorldUnvisited
        |> AtSea.Run random (fun () -> "foo" |> Command.HeadFor |> Some) sink
    Assert.AreEqual({headForWorldUnvisited with Messages=["I don't know how to get to `foo`."]} |> Gamestate.AtSea |> Some, actual)

[<Test>]
let ``Run.It gives a message when given a Head For command and the given island exists but is not known.`` () =
    let actual = 
        headForWorldUnvisited
        |> AtSea.Run random (fun () -> "yermom" |> Command.HeadFor |> Some) sink
    Assert.AreEqual({headForWorldUnvisited with Messages=["I don't know how to get to `yermom`."]} |> Gamestate.AtSea |> Some, actual)

[<Test>]
let ``Run.It gives a message and changes heading when given a Head For command and the given island exists and is known.`` () =
    let actual = 
        headForWorldVisited
        |> AtSea.Run random (fun () -> "yermom" |> Command.HeadFor |> Some) sink
    Assert.AreEqual({headForWorldVisited with Messages=["You set your heading to 180\u00b00'0.000000\"."; "You head for `yermom`."]; Avatar={headForWorldVisited.Avatar with Heading = System.Math.PI}} |> Gamestate.AtSea |> Some, actual)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let actual =
        world
        |> AtSea.Run random (fun () -> Command.Status |> Some) sink
    Assert.AreEqual(world |> Gamestate.AtSea |> Gamestate.Status |> Some, actual)

[<Test>]
let ``Run.It gives a message when given the command Abandon Job and the avatar has no current job.`` () =
    let subject = dockWorld
    let expected = {subject with Messages = ["You have no job to abandon."]} |> Gamestate.AtSea |> Some
    let actual =
        subject
        |> AtSea.Run random (fun () -> Job |> Command.Abandon |> Some) sink
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let subject = abandonJobWorld
    let expected = {subject with Messages = ["You abandon your job."]; Avatar={subject.Avatar with Job=None; Reputation = subject.Avatar.Reputation - 1.0}} |> Gamestate.AtSea |> Some
    let actual =
        subject
        |> AtSea.Run random (fun () -> Job |> Command.Abandon |> Some) sink
    Assert.AreEqual(expected, actual)
