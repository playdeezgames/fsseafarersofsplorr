module Splorr.Seafarers.Controllers.Tests.World

open NUnit.Framework
open Splorr.Seafarers.Controllers

[<Test>]
let ``Create function.Creates a new blank world.`` () =
    let actual = World.Create()
    Assert.AreEqual(0.0, actual.Avatar.Heading)
    Assert.AreEqual(0.0, actual.Avatar.X)
    Assert.AreEqual(0.0, actual.Avatar.Y)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``ClearMessages function.Removes any messages from the world.`` () =
    let actual =
        {World.Create() with Messages = ["test"]}
        |> World.ClearMessages
    Assert.AreEqual([], actual.Messages)

[<Test>]
let ``AddMessages function.New messages are appended to previously existing messages.`` () =
    let oldMessages = ["one"; "two"]
    let newMessages = [ "three"; "four"]
    let allMessages = List.append oldMessages newMessages
    let actual = 
        {World.Create() with Messages = oldMessages}
        |> World.AddMessages newMessages
    Assert.AreEqual(allMessages, actual.Messages)

[<Test>]
let ``SetSpeed function.Less than zero sets all stop.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (-1.0)
    Assert.AreEqual(0.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed function.Greater than one sets full speed.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (2.0)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed function.One half sets half speed.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (0.5)
    Assert.AreEqual(0.5, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed function.One sets full speed.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (1.0)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed function.Zero sets all stop.`` () =
    let actual =
        World.Create()
        |> World.SetSpeed (0.0)
    Assert.AreEqual(0.0, actual.Avatar.Speed)
