module WorldTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures

[<Test>]
let ``Create.It creates a new world.`` () =
    let actual = soloIslandWorld
    Assert.AreEqual(0.0, actual.Avatar.Heading)
    Assert.AreEqual((5.0,5.0), actual.Avatar.Position)
    Assert.AreEqual(1.0, actual.Avatar.Speed)
    Assert.AreEqual(1, actual.Islands.Count)
    Assert.AreNotEqual("", (actual.Islands |> Map.toList |> List.map snd |> List.head).Name)

[<Test>]
let ``ClearMessages.It removes any messages from the world.`` () =
    let actual =
        {soloIslandWorld with Messages = ["test"]}
        |> World.ClearMessages
    Assert.AreEqual([], actual.Messages)

[<Test>]
let ``AddMessages.It appends new messages to previously existing messages in the world.`` () =
    let oldMessages = ["one"; "two"]
    let newMessages = [ "three"; "four"]
    let allMessages = List.append oldMessages newMessages
    let actual = 
        {soloIslandWorld with Messages = oldMessages}
        |> World.AddMessages newMessages
    Assert.AreEqual(allMessages, actual.Messages)

[<Test>]
let ``SetSpeed.It produces all stop in the avatar when less than zero is passed.`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (-1.0)
    Assert.AreEqual(0.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces full speed when greater than one is passed.`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (2.0)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces half speed when one half is passed.`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (0.5)
    Assert.AreEqual(0.5, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It produces full speed when one is passed.`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (1.0)
    Assert.AreEqual(1.0, actual.Avatar.Speed)

[<Test>]
let ``SetSpeed.It sets all stop when given zero`` () =
    let actual =
        soloIslandWorld
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
        soloIslandWorld
        |> World.SetHeading heading
    Assert.AreEqual(heading |> Dms.ToFloat, actual.Avatar.Heading)

[<Test>]
let ``Move.It moves the avatar one unit when give 1u for distance.`` () =
    let actual =
        soloIslandWorld
        |> World.Move 1u
    Assert.AreEqual((6.0,5.0), actual.Avatar.Position)

[<Test>]
let ``Move.It moves the avatar two units when give 2u for distance.`` () =
    let actual =
        soloIslandWorld
        |> World.Move 2u
    Assert.AreEqual((7.0,5.0), actual.Avatar.Position)

[<Test>]
let ``GetNearbyLocations.It returns locations within a given distance from another given location.`` () =
    let blankIsland =
        {
            Name = ""
            LastVisit = None
            VisitCount = None
            Jobs = []
            Markets = Map.empty
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
            Commodities = Map.empty
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

[<Test>]
let ``SetIsland.It adds an island to a world when given an island where there was none.`` () =
    let actual = 
        emptyWorld
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
        emptyWorld
        |> World.TransformIsland (0.0, 0.0) (fun _-> Island.Create() |> Island.SetName "Uno" |> Some)
    Assert.AreEqual(0, actual.Islands.Count)

[<Test>]
let ``Dock.It adds a message when the given location has no island.`` () =
    let actual = 
        emptyWorld
        |> World.Dock random (0.0, 0.0)
    Assert.AreEqual({emptyWorld with Messages = [ "There is no place to dock there." ]}, actual)

[<Test>]
let ``Dock.It updates the island's visit count and last visit when the given location has an island.`` () =
    let actual = 
        oneIslandWorld
        |> World.Dock random (0.0, 0.0)
    let updatedIsland = 
        oneIslandWorld.Islands.[(0.0, 0.0)] |> Island.AddVisit oneIslandWorld.Turn
    Assert.AreEqual({oneIslandWorld with Messages = [ "You dock." ]; Islands = oneIslandWorld.Islands |> Map.add (0.0, 0.0) updatedIsland}, actual)

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

[<Test>]
let ``AcceptJob.It does nothing when given an invalid island location.`` () =
    let actual =
        genericDockedWorld
        |> World.AcceptJob 1u genericWorldInvalidIslandLocation
    Assert.AreEqual (genericDockedWorld, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when given an 0 job index for the given valid island location.`` () =
    let actual =
        genericDockedWorld
        |> World.AcceptJob 0u genericWorldIslandLocation
    Assert.AreEqual ({genericDockedWorld with Messages = [ "That job is currently unavailable." ]}, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when given an invalid job index for the given valid island location.`` () =
    let actual =
        genericDockedWorld
        |> World.AcceptJob 0xFFFFFFFFu genericWorldIslandLocation
    Assert.AreEqual ({genericDockedWorld with Messages = [ "That job is currently unavailable." ]}, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when the job is valid but the avatar already has a job.`` () =
    let subjectWorld = 
        genericDockedWorld
        |> World.TransformAvatar
            (fun avatar -> {avatar with Job =Some {FlavorText="";Destination=(0.0,0.0); Reward=0.0}})
    let actual =
        subjectWorld
        |> World.AcceptJob 1u genericWorldIslandLocation
    Assert.AreEqual ({subjectWorld with Messages = [ "You must complete or abandon your current job before taking on a new one." ]}, actual)


[<Test>]
let ``AcceptJob.It adds the given job to the avatar and eliminates it from the island's job list when given a valid island location and a valid job index and the avatar has no current job.`` () =
    let subjectWorld = genericDockedWorld
    let subjectLocation = genericWorldIslandLocation
    let subjectJob = subjectWorld.Islands.[subjectLocation].Jobs.Head
    let subjectDestination = subjectWorld.Islands.[subjectJob.Destination]
    let expectedAvatar = 
        subjectWorld.Avatar
        |> Avatar.SetJob subjectJob
    let expectedIsland = 
        {subjectWorld.Islands.[subjectLocation] with Jobs = []}
    let expectedDestination =
        {subjectDestination with VisitCount=Some 0u}
    let actual =
        subjectWorld
        |> World.AcceptJob 1u subjectLocation
    Assert.AreEqual( "You accepted the job!", actual.Messages.Head)
    Assert.AreEqual(1, actual.Messages.Length)
    Assert.AreEqual(expectedAvatar, actual.Avatar)
    Assert.AreEqual(expectedIsland, actual.Islands.[subjectLocation])
    Assert.AreEqual(expectedDestination, actual.Islands.[subjectJob.Destination])

[<Test>]
let ``TransformAvatar.It transforms the avatar within the given world.`` () =
    let expectedAvatar = genericWorld.Avatar |> Avatar.Move
    let actual =
        genericWorld
        |> World.TransformAvatar (Avatar.Move)
    Assert.AreEqual(expectedAvatar,actual.Avatar)

[<Test>]
let ``AbandonJob.It adds a message when the avatar has no job.`` () =
    let subject = genericDockedWorld
    let expected = {subject with Messages=["You have no job to abandon."]}
    let actual = 
        subject
        |> World.AbandonJob
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AbandonJob.It adds a messages and abandons the job when the avatar has a a job`` () =
    let subject = jobWorld
    let expected = {subject with Messages=["You abandon your job."]; Avatar = {subject.Avatar with Job = None; Reputation = subject.Avatar.Reputation - 1.0}}
    let actual = 
        subject
        |> World.AbandonJob
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Dock.It does not modify avatar when given avatar has a job for a different destination.`` () =
    let subject = jobWorld
    let actual = 
        jobWorld
        |> World.Dock random genericWorldIslandLocation
    Assert.AreEqual(subject.Avatar, actual.Avatar)

[<Test>]
let ``Dock.It adds a message and completes the job when given avatar has a job for this location.`` () =
    let subject = jobWorld
    let subjectJob = jobWorld.Avatar.Job.Value
    let subjectIsland = subject.Islands.[jobLocation]
    let expectedAvatar = 
        {subject.Avatar with 
            Job = None;
            Money = subject.Avatar.Money + subjectJob.Reward;
            Reputation = subject.Avatar.Reputation + 1.0}
    let expectedMessages = ["You complete your job."; "You dock."]
    let actual = 
        jobWorld
        |> World.Dock random jobLocation
    Assert.AreEqual(expectedAvatar, actual.Avatar)
    Assert.AreEqual(expectedMessages, actual.Messages)

//[<Test>]
//let ``FunctionName.It returns a SOMETHING when given SOMETHINGELSE.`` () =
//    raise (System.NotImplementedException "Not Implemented")
