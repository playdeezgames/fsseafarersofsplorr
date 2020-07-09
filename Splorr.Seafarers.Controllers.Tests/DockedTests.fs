module DockedTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open DockedTestFixtures


[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let input = deadDockWorld   
    let inputLocation= deadDockLocation
    let inputSource(): Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let expected =
        input.Messages
        |> Gamestate.GameOver
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given Undock Command.`` () =
    let input = dockWorld
    let inputLocation= dockLocation
    let inputSource = Command.Undock |> Some |> toSource
    let expectedMessages = ["You undock."]
    let expected = 
        {input with 
            Messages = expectedMessages} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = Command.Quit |> Some |> toSource
    let expected =
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.ConfirmQuit 
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub 
    Assert.AreEqual(expected,actual)

[<Test>]
let ``Run.It returns Help when given Help Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Help 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.Help 
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Inventory when given Inventory Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Inventory 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.Inventory 
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked when given invalid Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource =
        None 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns AtSea when given invalid docked location.`` () =
    let mutable sourceCalled:bool = false
    let input = dockWorld
    let inputLocation = (1.0, 1.0)
    let inputSource () = 
        sourceCalled <- true
        Command.Help 
        |> Some
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub 
    Assert.AreEqual(expected,actual)
    Assert.IsFalse(sourceCalled)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Status 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.Status 
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Docked (at Jobs) gamestate when given the command Jobs.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Jobs 
        |> Some 
        |> toSource
    let expected = 
        (Jobs, inputLocation, input) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        (inputLocation, avatarId, input)
        |||> Docked.Run inputSource sinkStub  
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given the Accept Job command and the given job number does not exist.`` () =
    let input = smallWorldDocked
    let inputLocation = smallWorldIslandLocation
    let inputSource = 0u |> Command.AcceptJob |> Some |> toSource
    let expectedMessages = [ "That job is currently unavailable." ]
    let expectedWorld = 
        {input with 
            Messages = expectedMessages}
    let expected = 
        (Dock, inputLocation,  expectedWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> Docked.Run inputSource sinkStub inputLocation avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given the command Abandon Job and the avatar has no current job.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Job 
        |> Command.Abandon 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, {input with Messages = ["You have no job to abandon."]}) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> Docked.Run inputSource sinkStub inputLocation avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let input = abandonJobWorld
    let inputLocation = dockLocation
    let inputSource = Job |> Command.Abandon |> Some |> toSource
    let expectedMessages = ["You abandon your job."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Job=None
            Reputation = input.Avatars.[avatarId].Reputation-1.0}
    let expectedWorld = 
        {input with 
            Messages = expectedMessages
            Avatars= input.Avatars |> Map.add avatarId expectedAvatar}
    let expected = 
        (Dock, inputLocation, expectedWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> Docked.Run inputSource sinkStub inputLocation avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns the Docked (at PriceList) gamestate when given the Prices command.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Prices 
        |> Some 
        |> toSource
    let expected = 
        (PriceList, inputLocation, input) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> Docked.Run inputSource sinkStub inputLocation avatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns the Docked (at Shop) gamestate when given the Shop command.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource =  
        Command.Shop 
        |> Some 
        |> toSource
    let expected = 
        (Shop, inputLocation, dockWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> Docked.Run inputSource sinkStub inputLocation avatarId
    Assert.AreEqual(expected, actual)

//[<Test>]
//let ``Run.It .`` () =
//    raise (System.NotImplementedException "Not Implemented")

