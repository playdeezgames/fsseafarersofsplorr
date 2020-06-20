module WorldTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

[<Test>]
let ``Create.It creates a new world.`` () =
    let actual = World.Create()
    Assert.AreEqual(0.0, actual.Avatar.Heading)
    Assert.AreEqual(0.0, actual.Avatar.X)
    Assert.AreEqual(0.0, actual.Avatar.Y)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``ClearMessages.It removes any messages from the world.`` () =
    let actual =
        {World.Create() with Messages = ["test"]}
        |> World.ClearMessages
    Assert.AreEqual([], actual.Messages)

[<Test>]
let ``AddMessages.It appends new messages to previously existing messages in the world.`` () =
    let oldMessages = ["one"; "two"]
    let newMessages = [ "three"; "four"]
    let allMessages = List.append oldMessages newMessages
    let actual = 
        {World.Create() with Messages = oldMessages}
        |> World.AddMessages newMessages
    Assert.AreEqual(allMessages, actual.Messages)

[<Test>]
let ``SetSpeed.It produces all stop in the avatar when less than zero is passed.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (-1.0)
    Assert.AreEqual(0.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces full speed when greater than one is passed.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (2.0)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces half speed when one half is passed.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (0.5)
    Assert.AreEqual(0.5, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces full speed when one is passed.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (1.0)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed function.It sets all stop when given zero`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (0.0)
    Assert.AreEqual(0.0, actual.Avatar.Speed)

[<Test>]
let ``SetHeading.It sets a new heading.`` () =
    let heading = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
    let actual =
        World.Create()
        |> World.SetHeading heading
    Assert.AreEqual(heading |> Dms.ToFloat, actual.Avatar.Heading)

[<Test>]
let ``Move.It moves the avatar.`` () =
    let actual =
        World.Create()
        |> World.Move
    Assert.AreEqual(1.0, actual.Avatar.X)
    Assert.AreEqual(0.0, actual.Avatar.Y)
