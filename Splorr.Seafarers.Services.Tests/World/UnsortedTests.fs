module WorldUnsortedTests

open NUnit.Framework
open Splorr.Seafarers.Services

type TestWorldAddMessagesContext(avatarMessageSink) =
    interface World.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface Avatar.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink

type TestWorldClearMessagesContext(avatarMessagePurger) =
    interface World.ClearMessagesContext with
        member this.avatarMessagePurger: AvatarMessagePurger = avatarMessagePurger
        
type TestWorldGetNearbyLocationsContext(islandSource) =
    interface World.GetNearbyLocationsContext with
        member _.islandSource : IslandSource = islandSource

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
    let context = TestWorldGetNearbyLocationsContext(islandSource) :> World.GetNearbyLocationsContext
    let actual = 
        World.GetNearbyLocations
            context
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
    let inputWorld = Fixtures.Common.Dummy.AvatarId
    let mutable counter = 0
    let avatarMessagePurger (_) =
        counter <- counter + 1
    let context = TestWorldClearMessagesContext(avatarMessagePurger) :> World.ClearMessagesContext
    inputWorld
    |> World.ClearMessages context
    Assert.AreEqual(1, counter)

[<Test>]
let ``AddMessages.It appends new messages to previously existing messages in the world.`` () =
    let firstMessage = "three"
    let secondMessage = "four"
    let newMessages = [ firstMessage; secondMessage]
    let inputWorld = Fixtures.Common.Dummy.AvatarId
    let context = TestWorldAddMessagesContext(Fixtures.Common.Mock.AvatarMessagesSink newMessages) :> World.AddMessagesContext
    inputWorld
    |> World.AddMessages 
        context
        newMessages

