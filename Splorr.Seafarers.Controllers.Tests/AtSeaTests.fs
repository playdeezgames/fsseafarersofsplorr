module AtSeaTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit command.`` () =
    let input = world
    let inputSource = 
        Command.Quit 
        |> Some 
        |> toSource
    let expected = 
        input
        |> Gamestate.AtSea 
        |> Gamestate.ConfirmQuit 
        |> Some
    let actual = 
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given invalid command.`` () =
    let input = world
    let inputSource = 
        None 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea with new speed when given Set Speed command.`` () =
    let newSpeed = 0.5
    let input = world
    let inputSource = 
        newSpeed 
        |> SetCommand.Speed 
        |> Command.Set 
        |> Some 
        |> toSource
    let expectedAvatar = 
        {input.Avatar with 
            Speed=newSpeed}
    let expectedMessages = ["You set your speed to 0.500000."]
    let expected = 
        {input with 
            Avatar = expectedAvatar
            Messages= expectedMessages}
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea with new heading when given Set Heading command.`` () =
    let newHeading = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
    let input = world
    let inputSource = 
        newHeading 
        |> SetCommand.Heading 
        |> Command.Set 
        |> Some 
        |> toSource
    let expectedAvatar = 
        {input.Avatar with 
            Heading = 
                newHeading 
                |> Dms.ToFloat}
    let expectedMessages = ["You set your heading to 1\u00b02'3.000000\"."]
    let expected = 
        {input with 
            Avatar = expectedAvatar
            Messages= expectedMessages}
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It moves the avatar when given Move command.`` () =
    let input = world
    let inputSource = 
        1u 
        |> Command.Move 
        |> Some 
        |> toSource
    let expectedAvatar =
        {input.Avatar with 
            Position=(6.0,5.0)
            Satiety = {input.Avatar.Satiety with CurrentValue=99.0}}
    let expectedMessages = ["Steady as she goes."]
    let expectedTurn = input.Turn + 1u
    let expected = 
        {input with 
            Avatar   = expectedAvatar
            Messages = expectedMessages
            Turn     = expectedTurn} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea Help when given the Help command.`` () =
    let input = world
    let inputSource = 
        Command.Help 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Gamestate.Help 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns At Sea Inventory when given the Inventory command.`` () =
    let input = world
    let inputSource = 
        Command.Inventory 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Gamestate.Inventory 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Main Menu when given the Menu command.`` () =
    let input = world
    let inputSource = 
        Command.Menu 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Some 
        |> Gamestate.MainMenu 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Island List when given the Islands command.`` () =
    let input = world
    let inputSource = 
        0u 
        |> Command.Islands 
        |> Some 
        |> toSource
    let expected = 
        (0u, input |> Gamestate.AtSea) 
        |> Gamestate.IslandList 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given the Dock command and there is no near enough island.`` () =
    let input = emptyWorld
    let inputSource = 
        Command.Dock 
        |> Some 
        |> toSource
    let expectedMessages = ["There is no place to dock."]
    let expected = 
        {input with 
            Messages=expectedMessages}
        |>Gamestate.AtSea
        |>Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at Dock) when given the Dock command and there is a near enough island.`` () =
    let input = dockWorld
    let inputSource = Command.Dock |> Some |> toSource
    let expectedLocation = (0.0, 0.0)
    let expectedIsland = 
        input.Islands.[expectedLocation] 
        |> Island.AddVisit input.Turn
    let expectedIslands = 
        input.Islands 
        |> Map.add expectedLocation expectedIsland
    let expectedMessages = ["You dock."]
    let expectedWorld = 
        {input with 
            Messages = expectedMessages
            Islands = expectedIslands }
    let expected = 
        (Dock, expectedLocation, expectedWorld)
        |>Gamestate.Docked
        |>Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given a Head For command and the given island does not exist.`` () =
    let input = headForWorldUnvisited
    let inputSource = 
        "foo" 
        |> Command.HeadFor 
        |> Some 
        |> toSource
    let expectedMessages = ["I don't know how to get to `foo`."]
    let expected = 
        {input with 
            Messages= expectedMessages} 
        |> Gamestate.AtSea 
        |> Some
    let actual = 
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given a Head For command and the given island exists but is not known.`` () =
    let input = headForWorldUnvisited
    let inputSource = 
        "yermom" 
        |> Command.HeadFor 
        |> Some 
        |> toSource
    let expectedMessages = ["I don't know how to get to `yermom`."]
    let expected = 
        {input with 
            Messages=expectedMessages} 
        |> Gamestate.AtSea 
        |> Some
    let actual = 
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and changes heading when given a Head For command and the given island exists and is known.`` () =
    let input = headForWorldVisited
    let inputSource = "yermom" |> Command.HeadFor |> Some |> toSource
    let expectedMessages = ["You set your heading to 180\u00b00'0.000000\"."; "You head for `yermom`."]
    let expectedAvatar = {input.Avatar with Heading = System.Math.PI}
    let expected = 
        {input with 
            Messages=expectedMessages
            Avatar=expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual = 
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let input = world
    let inputSource = 
        Command.Status 
        |> Some 
        |> toSource
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Gamestate.Status 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given the command Abandon Job and the avatar has no current job.`` () =
    let input = dockWorld
    let inputSource = 
        Job 
        |> Command.Abandon 
        |> Some 
        |> toSource
    let expectedMessages = ["You have no job to abandon."]
    let expected = 
        {input with 
            Messages = expectedMessages} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let input = abandonJobWorld
    let inputSource = 
        Job 
        |> Command.Abandon 
        |> Some 
        |> toSource
    let expectedMessages = ["You abandon your job."]
    let expectedAvatar = 
        {input.Avatar with 
            Job=None
            Reputation = input.Avatar.Reputation - 1.0}
    let expected = 
        {input with 
            Messages = expectedMessages
            Avatar=expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        input
        |> AtSea.Run random inputSource sinkStub
    Assert.AreEqual(expected, actual)
