module WorldUnsortedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open CommonTestFixtures

type TestWorldAddMessagesContext(avatarMessageSink) =
    interface WorldAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink

type TestWorldClearMessagesContext() =
    interface WorldClearMessagesContext

[<Test>]
let ``GetNearbyLocations.It returns locations within a given distance from another given location.`` () =
    let viewDistance = 5.0
    let avatarPosition = (5.0, 5.0)
    let islandSource () =
        [
            ( 0.0,  0.0)
            ( 5.0,  0.0)
            (10.0,  0.0)
            ( 0.0,  5.0)
            ( 5.0,  5.0)
            (10.0,  5.0)
            ( 0.0, 10.0)
            ( 5.0, 10.0)
            (10.0, 10.0)
        ]
    let actual = 
        World.GetNearbyLocations 
            islandSource
            avatarPosition 
            viewDistance
    Assert.AreEqual(5, actual.Length)
    Assert.IsFalse (actual |> List.exists(fun i -> i=( 0.0,  0.0)))
    Assert.IsTrue  (actual |> List.exists(fun i -> i=( 5.0,  0.0)))
    Assert.IsFalse (actual |> List.exists(fun i -> i=(10.0,  0.0)))
    Assert.IsTrue  (actual |> List.exists(fun i -> i=( 0.0,  5.0)))
    Assert.IsTrue  (actual |> List.exists(fun i -> i=( 5.0,  5.0)))
    Assert.IsTrue  (actual |> List.exists(fun i -> i=(10.0,  5.0)))
    Assert.IsFalse (actual |> List.exists(fun i -> i=( 0.0, 10.0)))
    Assert.IsTrue  (actual |> List.exists(fun i -> i=( 5.0, 10.0)))
    Assert.IsFalse (actual |> List.exists(fun i -> i=(10.0, 10.0)))

[<Test>]
let ``ClearMessages.It removes any messages from the given avatar in the world.`` () =
    let inputWorld = avatarId
    let mutable counter = 0
    let avatarMessagePurger (_) =
        counter <- counter + 1
    let context = TestWorldClearMessagesContext() :> WorldClearMessagesContext
    inputWorld
    |> World.ClearMessages context avatarMessagePurger
    Assert.AreEqual(1, counter)

[<Test>]
let ``AddMessages.It appends new messages to previously existing messages in the world.`` () =
    let firstMessage = "three"
    let secondMessage = "four"
    let newMessages = [ firstMessage; secondMessage]
    let inputWorld = avatarId
    let context = TestWorldAddMessagesContext(avatarMessagesSinkFake newMessages) :> WorldAddMessagesContext
    inputWorld
    |> World.AddMessages 
        context
        (avatarMessagesSinkFake newMessages) 
        newMessages

