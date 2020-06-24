﻿module WorldTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let configuration: WorldGenerationConfiguration =
    {
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0 
        MaximumGenerationTries=10u
        RewardRange=(1.0,10.0)
    }
let world = World.Create configuration (System.Random())

[<Test>]
let ``Create.It creates a new world.`` () =
    let actual = world
    Assert.AreEqual(0.0, actual.Avatar.Heading)
    Assert.AreEqual((5.0,5.0), actual.Avatar.Position)
    Assert.AreEqual(1.0, actual.Avatar.Speed)
    Assert.AreEqual(1, actual.Islands.Count)
    Assert.AreNotEqual("", (actual.Islands |> Map.toList |> List.map snd |> List.head).Name)

[<Test>]
let ``ClearMessages.It removes any messages from the world.`` () =
    let actual =
        {world with Messages = ["test"]}
        |> World.ClearMessages
    Assert.AreEqual([], actual.Messages)

[<Test>]
let ``AddMessages.It appends new messages to previously existing messages in the world.`` () =
    let oldMessages = ["one"; "two"]
    let newMessages = [ "three"; "four"]
    let allMessages = List.append oldMessages newMessages
    let actual = 
        {world with Messages = oldMessages}
        |> World.AddMessages newMessages
    Assert.AreEqual(allMessages, actual.Messages)

[<Test>]
let ``SetSpeed.It produces all stop in the avatar when less than zero is passed.`` () =
    let actual =
        world
        |> World.SetSpeed (-1.0)
    Assert.AreEqual(0.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces full speed when greater than one is passed.`` () =
    let actual =
        world
        |> World.SetSpeed (2.0)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces half speed when one half is passed.`` () =
    let actual =
        world
        |> World.SetSpeed (0.5)
    Assert.AreEqual(0.5, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces full speed when one is passed.`` () =
    let actual =
        world
        |> World.SetSpeed (1.0)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It sets all stop when given zero`` () =
    let actual =
        world
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
        world
        |> World.SetHeading heading
    Assert.AreEqual(heading |> Dms.ToFloat, actual.Avatar.Heading)

[<Test>]
let ``Move.It moves the avatar.`` () =
    let actual =
        world
        |> World.Move
    Assert.AreEqual((6.0,5.0), actual.Avatar.Position)

[<Test>]
let ``GetNearbyLocations.It returns locations within a given distance from another given location.`` () =
    let blankIsland =
        {
            Name = ""
            LastVisit = None
            VisitCount = None
            Jobs = []
        }
    let world =
        {
            RewardRange = (1.0,10.0)
            Avatar = 
                {
                    Position=(5.0, 5.0)
                    Speed=1.0
                    Heading=0.0
                    ViewDistance = 5.0
                    DockDistance = 1.0
                    Money = 0.0
                    Reputation = 0.0
                    Job = None
                }
            Turn = 0u
            Messages = []
            Islands=
                Map.empty
                |> Map.add ( 0.0,  0.0) blankIsland
                |> Map.add ( 5.0,  0.0) blankIsland
                |> Map.add (10.0,  0.0) blankIsland
                |> Map.add ( 0.0,  5.0) blankIsland
                |> Map.add ( 5.0,  5.0) blankIsland
                |> Map.add (10.0,  5.0) blankIsland
                |> Map.add ( 0.0, 10.0) blankIsland
                |> Map.add ( 5.0, 10.0) blankIsland
                |> Map.add (10.0, 10.0) blankIsland
        }
    let actual = 
        world
        |> World.GetNearbyLocations world.Avatar.Position world.Avatar.ViewDistance
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

let private zeroIslandWorld = 
    {
        RewardRange = (1.0,10.0)
        Messages=[]
        Avatar = 
            {
                Position = (0.0,0.0)
                Heading = 0.0
                Speed = 1.0
                ViewDistance = 10.0
                DockDistance = 1.0
                Money = 0.0
                Reputation = 0.0
                Job = None
            }
        Islands = Map.empty
        Turn = 0u
    }
let private random = System.Random()
let private rewardRange = (1.0,10.)
let private fakeDestinations = [(0.0, 0.0)] |> Set.ofList
let private oneIslandWorld = 
    zeroIslandWorld
    |> World.SetIsland (0.0,0.0) (Island.Create() |> Island.SetName "Uno" |> Some)
    |> World.TransformIsland  (0.0,0.0) (fun i -> {i with Jobs = [ Job.Create random rewardRange fakeDestinations ]} |> Some)

[<Test>]
let ``SetIsland.It adds an island to a world when given an island where there was none.`` () =
    let actual = 
        zeroIslandWorld
        |> World.SetIsland (0.0,0.0) (Island.Create() |> Island.SetName "Uno" |> Some)
    Assert.AreEqual(1, actual.Islands.Count)
    Assert.AreEqual("Uno", actual.Islands.[(0.0,0.0)].Name)

[<Test>]
let ``SetIsland.It replaces an island to a world when given an island where there was one before.`` () =
    let actual =
        oneIslandWorld
        |> World.SetIsland (0.0,0.0) (Island.Create() |> Island.SetName "Dos" |> Some)
    Assert.AreEqual(1, actual.Islands.Count)
    Assert.AreEqual("Dos", actual.Islands.[(0.0,0.0)].Name)

[<Test>]
let ``SetIsland.It removes an island to a world when given none where there was one before.`` () =
    let actual = 
        oneIslandWorld
        |> World.SetIsland (0.0,0.0) (None)
    Assert.AreEqual(0, actual.Islands.Count)

[<Test>]
let ``TransformIsland.It applies a transform function to an existing island and updates the island to the transformed value.`` () =
    let actual =
        oneIslandWorld
        |> World.TransformIsland (0.0,0.0) (Island.SetName "Dos" >> Some)
    Assert.AreEqual(1, actual.Islands.Count)
    Assert.AreEqual("Dos", actual.Islands.[(0.0,0.0)].Name)

[<Test>]
let ``TransformIsland.It applies a transform function to an existing island and removes the island when the transformer returns None.`` () =
    let actual =
        oneIslandWorld
        |> World.TransformIsland (0.0,0.0) (fun _ -> None)
    Assert.AreEqual(0, actual.Islands.Count)

[<Test>]
let ``TransformIsland.It does nothing when the location given does not have an existing island.`` () =
    let actual =
        zeroIslandWorld
        |> World.TransformIsland (0.0, 0.0) (fun _-> Island.Create() |> Island.SetName "Uno" |> Some)
    Assert.AreEqual(0, actual.Islands.Count)

[<Test>]
let ``Dock.It adds a message when the given location has no island.`` () =
    let actual = 
        zeroIslandWorld
        |> World.Dock random (0.0, 0.0)
    Assert.AreEqual({zeroIslandWorld with Messages = [ "There is no place to dock there." ]}, actual)

[<Test>]
let ``Dock.It updates the island's visit count and last visit when the given location has an island.`` () =
    let actual = 
        oneIslandWorld
        |> World.Dock random (0.0, 0.0)
    let updatedIsland = 
        oneIslandWorld.Islands.[(0.0, 0.0)] |> Island.AddVisit oneIslandWorld.Turn
    Assert.AreEqual({oneIslandWorld with Messages = [ "You dock." ]; Islands = oneIslandWorld.Islands |> Map.add (0.0, 0.0) updatedIsland}, actual)

let private headForWorld =
    {oneIslandWorld with Avatar = {oneIslandWorld.Avatar with Position = (1.0,0.0)}}

[<Test>]
let ``HeadFor.It adds a message when the island name does not exist.`` () =
    let actual =
        headForWorld
        |> World.HeadFor "yermom"
    Assert.AreEqual({headForWorld with Messages=[ "I don't know how to get to `yermom`." ]}, actual)

[<Test>]
let ``HeadFor.It adds a message when the island name exists but is not known.`` () =
    let actual =
        headForWorld
        |> World.HeadFor "Uno"
    Assert.AreEqual({headForWorld with Messages=[ "I don't know how to get to `Uno`." ]}, actual)

[<Test>]
let ``HeadFor.It sets the heading when the island name exists and is known.`` () =
    let modifiedWorld =
        headForWorld
        |> World.TransformIsland (0.0,0.0) (Island.AddVisit headForWorld.Turn >> Some)
    let actual =
        modifiedWorld
        |> World.HeadFor "Uno"
    Assert.AreEqual({modifiedWorld with Messages=[ "You set your heading to 180°0'0.000000\"."; "You head for `Uno`." ]; Avatar = {modifiedWorld.Avatar with Heading=System.Math.PI}}, actual)
