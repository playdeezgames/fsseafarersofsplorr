module WorldTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures
open CommonTestFixtures

[<Test>]
let ``Create.It creates a new world.`` () =
    let actual = soloIslandWorld
    //Assert.AreEqual((5.0,5.0), actual.Avatars.[avatarId].Position)
    Assert.AreEqual(1, actual.Islands.Count)
    Assert.AreNotEqual("", (actual.Islands |> Map.toList |> List.map snd |> List.head).Name)

[<Test>]
let ``ClearMessages.It removes any messages from the given avatar in the world.`` () =
    let inputWorld = soloIslandWorld
    let mutable counter = 0
    let avatarMessagePurger (_) =
        counter <- counter + 1
    inputWorld
    |> World.ClearMessages avatarMessagePurger
    Assert.AreEqual(1, counter)

let avatarExpectedMessagesSink (messages:string list) (_) (message) =
    match messages |> List.tryFind (fun x->x=message) with
    | Some _ ->
        Assert.Pass("Received a valid message.")
    | None ->
        Assert.Fail("Received an invalid message.")

[<Test>]
let ``AddMessages.It appends new messages to previously existing messages in the world.`` () =
    let firstMessage = "three"
    let secondMessage = "four"
    let newMessages = [ firstMessage; secondMessage]
    let inputWorld = soloIslandWorld
    inputWorld
    |> World.AddMessages (avatarExpectedMessagesSink newMessages) newMessages


[<Test>]
let ``SetSpeed.It produces all stop in the avatar when less than zero is passed.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=1.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = -1.0
    soloIslandWorld
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore

[<Test>]
let ``SetSpeed.It produces full speed when greater than one is passed.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=0.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = 2.0
    soloIslandWorld
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It produces half speed when one half is passed.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=0.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 0.5
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = 0.5
    soloIslandWorld
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It does nothing when a bogus avatarid is passed.`` () =
    let inputWorld = 
        {soloIslandWorld with AvatarId = bogusAvatarId}
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            None
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let vesselSingleStatisticSink (_) (_) = 
        //assert speed being set in here
        raise (System.NotImplementedException "Kaboom set")
    let inputSpeed = 1.0
    let avatarMessageSink (_) (_) =
        Assert.Fail("Dont call me.")
    inputWorld
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink inputSpeed

[<Test>]
let ``SetSpeed.It produces full speed when one is passed.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=0.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = 1.0
    soloIslandWorld
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It sets all stop when given zero`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=0.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = 0.0
    soloIslandWorld
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore


[<Test>]
let ``SetHeading.It sets a new heading when given a valid avatar id.`` () =
    let heading = 1.5
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedHeading = heading |> Angle.ToRadians
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) =
        Assert.AreEqual(VesselStatisticIdentifier.Heading, identifier)
        Assert.AreEqual(expectedHeading, statistic.CurrentValue)
    soloIslandWorld
    |> World.SetHeading vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub heading
    |> ignore
    

[<Test>]
let ``SetHeading.It does nothing when given an invalid avatar id`` () =
    let input = 
        {soloIslandWorld with AvatarId = bogusAvatarId}
    let vesselSingleStatisticSource (_) (_) =
        None
    let vesselSingleStatisticSink (_) (_) =
        raise (System.NotImplementedException "Kaboom set")
    let heading = 1.5
    input
    |> World.SetHeading vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub heading
    |> ignore


[<Test>]
let ``Move.It moves the avatar one unit when give 1u for distance when given a valid avatar id.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
        | VesselStatisticIdentifier.PortFouling
        | VesselStatisticIdentifier.StarboardFouling ->
            {MinimumValue=0.0; CurrentValue=0.0; MaximumValue=0.25} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MinimumValue=0.001; MaximumValue=0.001; CurrentValue=0.001} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | _ ->
            Assert.Fail("Don't get me.")
            None
    let mutable positionXCalls = 0
    let mutable positionYCalls = 0
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) : unit =
        match identifier with
        | VesselStatisticIdentifier.StarboardFouling
        | VesselStatisticIdentifier.PortFouling ->
            Assert.AreEqual(0.00050000000000000001, statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionX ->
            positionXCalls <- positionXCalls + 1
        | VesselStatisticIdentifier.PositionY ->
            positionYCalls <- positionYCalls + 1
        | _ ->
            Assert.Fail("Don't set me.")
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    soloIslandWorld
    |> World.Move 
        avatarInventorySink
        avatarInventorySource
        avatarMessageSinkStub 
        avatarShipmateSource
        (assertAvatarSingleMetricSink [Metric.Moved, 1UL; Metric.Ate, 0UL])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        1u
    |> ignore
    Assert.AreEqual(1, positionXCalls)
    Assert.AreEqual(1, positionYCalls)

[<Test>]
let ``Move.It does nothing when given an invalid avatar id`` () =
    let vesselSingleStatisticSource (_) (_) =
        Assert.Fail("Dont ask for vessel statistics")
        None
    let vesselSingleStatisticSink (_) (_) : unit =
        Assert.Fail("Dont try to set a vessel statistic")
    let inputWorld =
        {soloIslandWorld with AvatarId = bogusAvatarId}
    let avatarShipmateSource (_) = 
        raise (System.NotImplementedException "kaboom avatarShipmateSource")
        []
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        raise (System.NotImplementedException "avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        raise (System.NotImplementedException "avatarInventorySink")
        ()
    let actual =
        inputWorld
        |> World.Move 
            avatarInventorySink
            avatarInventorySource
            avatarMessageSinkStub 
            avatarShipmateSource
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            shipmateRationItemSourceStub 
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSink 
            vesselSingleStatisticSource 
            1u
    Assert.AreEqual(inputWorld, actual)

[<Test>]
let ``Move.It moves the avatar almost two units when give 2u for distance.`` () =
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
        | VesselStatisticIdentifier.PortFouling
        | VesselStatisticIdentifier.StarboardFouling ->
            {MinimumValue=0.0; CurrentValue=0.0; MaximumValue=0.25} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MinimumValue=0.001; MaximumValue=0.001; CurrentValue=0.001} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | _ ->
            Assert.Fail("Dont call me.")
            None
    let mutable positionXCalls = 0
    let mutable positionYCalls = 0
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) : unit =
        match identifier with
        | VesselStatisticIdentifier.StarboardFouling
        | VesselStatisticIdentifier.PortFouling ->
            Assert.AreEqual(0.00050000000000000001, statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionX ->
            positionXCalls <- positionXCalls + 1
        | VesselStatisticIdentifier.PositionY ->
            positionYCalls <- positionYCalls + 1
        | _ ->
            Assert.Fail("Don't set me.")
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    soloIslandWorld
    |> World.Move 
        avatarInventorySink
        avatarInventorySource
        avatarMessageSinkStub 
        avatarShipmateSource
        (assertAvatarSingleMetricSink [Metric.Moved, 1UL; Metric.Ate, 0UL])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        2u
    |> ignore
    Assert.AreEqual(2, positionXCalls)
    Assert.AreEqual(2, positionYCalls)
    

[<Test>]
let ``GetNearbyLocations.It returns locations within a given distance from another given location.`` () =
    let blankIsland =
        {
            Name = ""
            AvatarVisits = Map.empty
            Jobs = []
            CareenDistance = 0.0
        }
    let viewDistance = 5.0
    let avatarPosition = (5.0, 5.0)
    let world =
        {
            AvatarId = avatarId
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
        |> World.GetNearbyLocations avatarPosition viewDistance
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

let private islandItemSourceStub (_) = Set.empty
let private islandItemSinkStub (_) (_) = ()
let private islandMarketSourceStub (_) = Map.empty
let private islandSingleMarketSourceStub (_) (_) = None
let private islandMarketSinkStub (_) (_) = ()

[<Test>]
let ``Dock.It does nothing when given an invalid avatar id.`` () =
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual = 
        {emptyWorld with AvatarId = bogusAvatarId}
        |> World.Dock 
            avatarJobSink
            avatarJobSource
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            commoditySource
            islandItemSinkStub 
            islandItemSourceStub 
            islandMarketSinkStub 
            islandMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSinkStub
            shipmateSingleStatisticSourceStub
            termSources 
            worldSingleStatisticSourceStub
            random 
            (0.0, 0.0)
    Assert.AreEqual({emptyWorld with AvatarId = bogusAvatarId}, actual)

let private avatarExpectedMessageSink (expected:string) (_) (actual:string) =
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Dock.It adds a message when the given location has no island.`` () =
    let inputWorld = emptyWorld
    let expectedMessage = "There is no place to dock there."
    let expected =
        inputWorld
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual = 
        inputWorld
        |> World.Dock
            avatarJobSink
            avatarJobSource
            (avatarExpectedMessageSink expectedMessage)
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            commoditySource
            islandItemSinkStub 
            islandItemSourceStub 
            islandMarketSinkStub 
            islandMarketSourceStub 
            genericWorldItemSource 
            shipmateSingleStatisticSinkStub
            shipmateSingleStatisticSourceStub
            termSources 
            worldSingleStatisticSourceStub
            random 
            (0.0, 0.0)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Dock.It updates the island's visit count and last visit when the given location has an island.`` () =
    let inputWorld = oneIslandWorld
    let expectedIsland = 
        inputWorld.Islands.[(0.0, 0.0)] 
        |> Island.AddVisit 
            (System.DateTimeOffset.Now.ToUnixTimeSeconds())
            avatarId
    let expectedMessage = "You dock."
    let expected = 
        {inputWorld with 
            Islands = inputWorld.Islands |> Map.add (0.0, 0.0) expectedIsland} 
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual = 
        inputWorld
        |> World.Dock
            avatarJobSink
            avatarJobSource
            (avatarExpectedMessageSink expectedMessage)
            (assertAvatarSingleMetricSink [Metric.VisitedIsland, 1UL])
            avatarSingleMetricSourceStub
            (fun()->Map.empty) 
            islandItemSinkStub 
            islandItemSourceStub 
            islandMarketSinkStub 
            islandMarketSourceStub 
            (fun()->Map.empty) 
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            termSources 
            worldSingleStatisticSourceStub
            random 
            (0.0, 0.0)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``HeadFor.It adds a message when the island name does not exist.`` () =
    let inputWorld = headForWorld
    let expectedMessage = "I don't know how to get to `yermom`."
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PositionX 
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0}|> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let vesselSingleStatisticSink (_) (_) =
        raise (System.NotImplementedException "Kaboom set")
    inputWorld
    |> World.HeadFor 
        vesselSingleStatisticSource 
        vesselSingleStatisticSink 
        (avatarExpectedMessageSink expectedMessage)
        "yermom"

[<Test>]
let ``HeadFor.It adds a message when the island name exists but is not known.`` () =
    let inputWorld = headForWorld
    let expectedMessage = "I don't know how to get to `Uno`."
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PositionX 
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0}|> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let vesselSingleStatisticSink (_) (_) =
        raise (System.NotImplementedException "Kaboom set")
    inputWorld
    |> World.HeadFor 
        vesselSingleStatisticSource 
        vesselSingleStatisticSink 
        (avatarExpectedMessageSink expectedMessage)
        "Uno"

[<Test>]
let ``HeadFor.It sets the heading when the island name exists and is known.`` () =
    let inputWorld =
        headForWorld
        |> World.TransformIsland 
            (0.0,0.0) 
            (Island.AddVisit 
                99L avatarId >> Some)
    let firstExpectedMessage = "You set your heading to 0.00°." //note - value for heading not actually stored, but is really 180
    let secondExpectedMessage = "You head for `Uno`."
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=1.0}|> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0}|> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let expectedHeading = System.Math.PI
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic: Statistic) =
        Assert.AreEqual(VesselStatisticIdentifier.Heading, identifier)
        Assert.AreEqual(expectedHeading, statistic.CurrentValue)
    inputWorld
    |> World.HeadFor 
        vesselSingleStatisticSource 
        vesselSingleStatisticSink 
        (avatarExpectedMessagesSink [firstExpectedMessage; secondExpectedMessage])
        "Uno"


[<Test>]
let ``AcceptJob.It does nothing when given an invalid island location.`` () =
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual =
        genericDockedWorld
        |> World.AcceptJob 
            avatarJobSink
            avatarJobSource
            avatarMessageSinkStub 
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            1u 
            genericWorldInvalidIslandLocation
    Assert.AreEqual (genericDockedWorld, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when given an 0 job index for the given valid island location.`` () =
    let inputWorld = genericDockedWorld
    let expectedMessage = "That job is currently unavailable."
    let expected = inputWorld
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual =
        inputWorld
        |> World.AcceptJob 
            avatarJobSink
            avatarJobSource
            (avatarExpectedMessageSink expectedMessage)
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            0u 
            genericWorldIslandLocation
    Assert.AreEqual(expected, actual)


[<Test>]
let ``AcceptJob.It adds a message to the world when given an invalid job index for the given valid island location.`` () =
    let inputWorld = genericDockedWorld
    let expectedMessage = "That job is currently unavailable."
    let expected =
        inputWorld
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual =
        inputWorld
        |> World.AcceptJob 
            avatarJobSink
            avatarJobSource
            (avatarExpectedMessageSink expectedMessage)
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            0xFFFFFFFFu 
            genericWorldIslandLocation
    Assert.AreEqual (expected, actual)

[<Test>]
let ``AcceptJob.It does nothing when given an invalid avatar id.`` () =
    let inputWorld = 
        {genericDockedWorld with AvatarId = bogusAvatarId}
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual =
        inputWorld
        |> World.AcceptJob
            avatarJobSink
            avatarJobSource
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            1u 
            genericWorldIslandLocation
    Assert.AreEqual (inputWorld, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when the job is valid but the avatar already has a job.`` () =
    let inputWorld = 
        genericDockedWorld
    let expectedMessage = "You must complete or abandon your current job before taking on a new one."
    let expected =
        inputWorld
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual =
        inputWorld
        |> World.AcceptJob 
            avatarJobSink
            avatarJobSource
            (avatarExpectedMessageSink expectedMessage)
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            1u 
            genericWorldIslandLocation
    Assert.AreEqual (expected, actual)


[<Test>]
let ``AcceptJob.It adds the given job to the avatar and eliminates it from the island's job list when given a valid island location and a valid job index and the avatar has no current job.`` () =
    let inputWorld = genericDockedWorld
    let inputLocation = genericWorldIslandLocation
    let inputJob = inputWorld.Islands.[inputLocation].Jobs.Head
    let inputDestination = inputWorld.Islands.[inputJob.Destination]
    let expectedMessage = "You accepted the job!"
    let expectedIsland = 
        {inputWorld.Islands.[inputLocation] with Jobs = []}
    let expectedDestination =
        {inputDestination with 
            AvatarVisits = Map.empty |> Map.add avatarId {VisitCount=0u |> Some;LastVisit=None}}
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual =
        inputWorld
        |> World.AcceptJob 
            avatarJobSink
            avatarJobSource
            (avatarExpectedMessageSink expectedMessage)
            (assertAvatarSingleMetricSink [Metric.AcceptedJob, 1UL])
            avatarSingleMetricSourceStub
            1u 
            inputLocation
    Assert.AreEqual(expectedIsland, actual.Islands.[inputLocation])
    Assert.AreEqual(expectedDestination, actual.Islands.[inputJob.Destination])

[<Test>]
let ``TransformAvatar.It transforms the avatar within the given world.`` () =
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.5; CurrentValue=0.0} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MinimumValue=0.01; MaximumValue=0.01; CurrentValue=0.01} |> Some
        | _ ->
            None
    let mutable xPositionCalled = 0
    let mutable yPositionCalled = 0
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier,_) =
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            xPositionCalled <- xPositionCalled + 1
        | VesselStatisticIdentifier.PositionY ->
            yPositionCalled <- yPositionCalled + 1
        | _ ->
            raise (System.NotImplementedException "Kaboom set")
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    Avatar.Move 
            avatarInventorySink
            avatarInventorySource
            avatarShipmateSource
            (assertAvatarSingleMetricSink [Metric.Moved, 1UL; Metric.Ate, 0UL])
            avatarSingleMetricSourceStub
            shipmateRationItemSourceStub 
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSink 
            vesselSingleStatisticSource 
            avatarId
    Assert.AreEqual(1, xPositionCalled)
    Assert.AreEqual(1, yPositionCalled)

[<Test>]
let ``AbandonJob.It adds a message when the avatar has no job.`` () =
    let input = genericDockedWorld
    let expectedMessage = "You have no job to abandon."
    let expected = input
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual = 
        input
        |> World.AbandonJob
            avatarJobSink
            avatarJobSource
            (avatarExpectedMessageSink expectedMessage)
            (assertAvatarSingleMetricSink [Metric.AcceptedJob, 1UL])
            avatarSingleMetricSourceStub
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AbandonJob.It adds a messages and abandons the job when the avatar has a a job`` () =
    let input = jobWorld
    let expectedMessage = "You abandon your job."
    let expected = 
        input
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create(-100.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(-1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let actual = 
        input
        |> World.AbandonJob
            avatarJobSink
            avatarJobSource
            (avatarExpectedMessageSink expectedMessage)
            (assertAvatarSingleMetricSink [Metric.AbandonedJob, 1UL])
            avatarSingleMetricSourceStub
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Dock.It does not modify avatar when given avatar has a job for a different destination.`` () =
    let expectedMessage = "You dock."
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    jobWorld
    |> World.Dock
        avatarJobSink
        avatarJobSource
        (avatarExpectedMessageSink expectedMessage)
        (assertAvatarSingleMetricSink [Metric.VisitedIsland, 0UL; Metric.VisitedIsland, 1UL])
        avatarSingleMetricSourceStub
        commoditySource
        islandItemSinkStub 
        islandItemSourceStub 
        islandMarketSinkStub 
        islandMarketSourceStub 
        genericWorldItemSource 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        termSources 
        worldSingleStatisticSourceStub
        random 
        genericWorldIslandLocation

[<Test>]
let ``Dock.It adds a message and completes the job when given avatar has a job for this location.`` () =
    let input = jobWorld
    let expectedMessages = ["You complete your job."; "You dock."]
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let jobLocation = (99.0, 99.0)
    jobWorld
    |> World.Dock
        avatarJobSink
        avatarJobSource
        (avatarExpectedMessagesSink expectedMessages)
        avatarSingleMetricSinkExplode
        avatarSingleMetricSourceStub
        commoditySource 
        islandItemSinkStub 
        islandItemSourceStub 
        islandMarketSinkStub 
        islandMarketSourceStub 
        genericWorldItemSource 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        termSources 
        worldSingleStatisticSourceStub
        random 
        jobLocation

let islandSingleMarketSinkStub (_) (_) = ()
let vesselSingleStatisticSourceStub (_) (identifier) = 
    match identifier with
    | VesselStatisticIdentifier.Tonnage -> {MinimumValue=100.0; CurrentValue=100.0; MaximumValue=100.0} |> Some
    | VesselStatisticIdentifier.ViewDistance -> {MinimumValue=10.0; CurrentValue=10.0; MaximumValue=10.0} |> Some
    | _ -> None

[<Test>]
let ``BuyItems.It gives a message when given a bogus island location.`` () =
    let input = shopWorld
    let inputLocation = shopWorldBogusLocation
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let expectedMessage = "You cannot buy items here."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        raise (System.NotImplementedException "avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        raise (System.NotImplementedException "avatarInventorySink")
        ()
    let actual = 
        input 
        |> World.BuyItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSourceStub 
            islandSingleMarketSinkStub 
            islandSingleMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSourceStub 
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when given a valid island location and a bogus item to buy.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2UL |> Specific
    let inputItemName = "bogus item"
    let expectedMessage = "Round these parts, we don't sell things like that."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        raise (System.NotImplementedException "avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        raise (System.NotImplementedException "avatarInventorySink")
        ()
    let actual = 
        input 
        |> World.BuyItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSourceStub 
            islandSingleMarketSinkStub 
            islandSingleMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSourceStub 
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient funds.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expectedMessage = "You don't have enough money."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s"))
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let actual = 
        input 
        |> World.BuyItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSource 
            islandSingleMarketSinkStub 
            islandSingleMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSourceStub 
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient tonnage.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 1000UL |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expectedMessage = "You don't have enough tonnage."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 5000.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s"))
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let actual = 
        input 
        |> World.BuyItems
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource
            islandMarketSource 
            islandSingleMarketSinkStub
            islandSingleMarketSourceStub
            genericWorldItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSourceStub
            inputLocation
            inputQuantity
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message and completes the purchase when the avatar has sufficient funds.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(5.0, market.Supply)
        Assert.AreEqual(7.0, market.Demand)
    let expectedMessage = "You complete the purchase of 2 item under test."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 5000.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(4998.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s"))
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty |> Map.add 1UL 2UL, inventory)
    let actual = 
        input 
        |> World.BuyItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSource 
            islandSingleMarketSink 
            islandSingleMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSourceStub 
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient funds for a single unit when specifying a maximum buy.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = Maximum
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let islandSingleMarketSink (_) (_) =
        Assert.Fail("This should not be called.")
    let expectedMessage = "You don't have enough money to buy any of those."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s"))
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let actual = 
        input 
        |> World.BuyItems
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSource 
            islandSingleMarketSink 
            islandSingleMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSourceStub 
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message indicating purchased quantity and completes the purchase when the avatar has sufficient funds for at least one and has specified a maximum buy.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = Maximum
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(5.0, market.Supply)
        Assert.AreEqual(105.0, market.Demand)
    let expectedMessage = "You complete the purchase of 100 item under test."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 5000.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(4900.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s"))
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty |> Map.add 1UL 100UL, inventory)
    let actual = 
        input 
        |> World.BuyItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSource 
            islandSingleMarketSink 
            islandSingleMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSourceStub 
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)


[<Test>]
let ``SellItems.It gives a message when given a bogus island location.`` () =
    let input = shopWorld
    let inputLocation = shopWorldBogusLocation
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let expectedMessage = "You cannot sell items here."
    let expected =
        input
    let avatarInventorySource (_) =
        raise (System.NotImplementedException "avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        raise (System.NotImplementedException "avatarInventorySink")
        ()
    let actual = 
        input 
        |> World.SellItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSourceStub 
            islandSingleMarketSinkStub 
            islandSingleMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSinkStub
            shipmateSingleStatisticSourceStub
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when given a valid island location and bogus item to buy.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2UL |> Specific
    let inputItemName = "bogus item"
    let expectedMessage = "Round these parts, we don't buy things like that."
    let expected =
        input
    let avatarInventorySource (_) =
        raise (System.NotImplementedException "avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        raise (System.NotImplementedException "avatarInventorySink")
        ()
    let actual = 
        input 
        |> World.SellItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSourceStub 
            islandSingleMarketSinkStub 
            islandSingleMarketSourceStub 
            genericWorldItemSource
            shipmateSingleStatisticSinkStub
            shipmateSingleStatisticSourceStub
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when the avatar has insufficient items in inventory.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let expectedMessage = "You don't have enough of those to sell."
    let expected =
        input
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let actual = 
        input 
        |> World.SellItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSourceStub 
            islandSingleMarketSinkStub 
            islandSingleMarketSourceStub 
            genericWorldItemSource 
            shipmateSingleStatisticSinkStub
            shipmateSingleStatisticSourceStub
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when the avatar has no items in inventory and specifies maximum.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = Maximum
    let inputItemName = "item under test"
    let expectedMessage = "You don't have any of those to sell."
    let expected =
        input
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let actual = 
        input 
        |> World.SellItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSourceStub 
            islandSingleMarketSinkStub 
            islandSingleMarketSourceStub 
            genericWorldItemSource 
            shipmateSingleStatisticSinkStub
            shipmateSingleStatisticSourceStub
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message and completes the sale when the avatar has sufficient quantity.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(7.0, market.Supply)
        Assert.AreEqual(5.0, market.Demand)
    let expectedMessage = "You complete the sale of 2 item under test."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s"))
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add 1UL 2UL
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let actual = 
        input 
        |> World.SellItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSource 
            islandSingleMarketSink 
            islandSingleMarketSourceStub 
            genericWorldItemSource 
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message and completes the sale when the avatar has sufficient quantity and specified a maximum sell.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = Maximum
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(7.0, market.Supply)
        Assert.AreEqual(5.0, market.Demand)
    let expectedMessage = "You complete the sale of 2 item under test."
    let expected =
        input
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s"))
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add 1UL 2UL
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let actual = 
        input 
        |> World.SellItems 
            avatarInventorySink
            avatarInventorySource
            (avatarExpectedMessageSink expectedMessage)
            commoditySource 
            islandMarketSource 
            islandSingleMarketSink 
            islandSingleMarketSourceStub 
            genericWorldItemSource 
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            inputLocation 
            inputQuantity 
            inputItemName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``IsAvatarAlive.It returns a true when given a world with an avatar with above minimum health.`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    if genericWorld |> World.IsAvatarAlive shipmateSingleStatisticSource then
        Assert.Pass("It detected that the avatar is alive")
    else
        Assert.Fail("It detected that the avatar is not alive")

[<Test>]
let ``IsAvatarAlive.It returns a false when given a world with an avatar minimum health (zero).`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    if deadWorld |> World.IsAvatarAlive shipmateSingleStatisticSource |> not then
        Assert.Pass("It detected that the avatar is dead")
    else
        Assert.Fail("It detected that the avatar is not dead")


[<Test>]
let ``CleanHull.It returns the original world when given a bogus avatar id and world.`` () =
    let inputWorld = 
        {genericWorld with AvatarId = bogusAvatarId}
    let inputSide = Port
    let vesselSingleStatisticSource (_) (_) =
        None
    let vesselSingleStatisticSink (_) (_) =
        Assert.Fail("Dont set statistics")
    let avatarShipmateSource (_) = 
        []
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    inputWorld
    |> World.CleanHull
        avatarShipmateSource
        (assertAvatarSingleMetricSink [Metric.CleanedHull, 1UL])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputSide


[<Test>]
let ``CleanHull.It returns a cleaned hull when given a particular avatar id and world.`` () =
    let inputSide = Port
    let inputWorld = 
        genericWorld
    let vesselSingleStatisticSource (_) (_) =
        {MinimumValue = 0.0; MaximumValue=0.25; CurrentValue = 0.25} |> Some
    let vesselSingleStatisticSink (_) (_, statistic:Statistic) =
        Assert.AreEqual(statistic.MinimumValue, statistic.CurrentValue)
    let avatarShipmateSource (_) = 
        []
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    inputWorld
    |> World.CleanHull 
        avatarShipmateSource
        (assertAvatarSingleMetricSink [Metric.CleanedHull, 1UL])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputSide


[<Test>]
let ``DistanceTo.It adds a 'unknown island' message when given a bogus island name.`` () =
    let input = genericWorld
    let inputName = "$$$$$$$"
    let expectedMessage = inputName |> sprintf "I don't know how to get to `%s`."
    let expected =
        input
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    input
    |> World.DistanceTo 
        vesselSingleStatisticSource 
        (avatarExpectedMessageSink expectedMessage)
        inputName

[<Test>]
let ``DistanceTo.It adds a 'unknown island' message when given a valid island name that is not known.`` () =
    let inputName = ((genericWorld.Islands |> Map.toList).Head |> snd).Name
    let input = genericWorld
    let expectedMessage = inputName |> sprintf "I don't know how to get to `%s`."
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    input
    |> World.DistanceTo 
        vesselSingleStatisticSource 
        (avatarExpectedMessageSink expectedMessage)
        inputName

[<Test>]
let ``DistanceTo.It adds a 'distance to island' message when given a valid island name that is known.`` () =
    let inputLocation = (genericWorld.Islands |> Map.toList).Head |> fst
    let inputName = ((genericWorld.Islands |> Map.toList).Head |> snd).Name
    let input = 
        genericWorld
        |> World.TransformIsland inputLocation (Island.MakeKnown genericWorld.AvatarId >> Some)
    let avatarPosition = (0.0, 0.0)
    let expectedMessage = (inputName, Location.DistanceTo inputLocation avatarPosition) ||> sprintf "Distance to `%s` is %f."
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=avatarPosition |> fst} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=avatarPosition |> snd} |> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    input
    |> World.DistanceTo 
        vesselSingleStatisticSource
        (avatarExpectedMessageSink expectedMessage)
        inputName

[<Test>]
let ``UpdateChart.It does nothing when the given avatar is not near any nearby islands.`` () =
    let input =
        genericWorld
    let expected =
        input
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=11.0; CurrentValue=5.5} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=0.0; MaximumValue=0.0; CurrentValue=0.0} |> Some
        | _ ->
            Assert.Fail()
            None
    let actual =
        input
        |> World.UpdateCharts vesselSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``UpdateChart.It does nothing when the given avatar has already seen all nearby islands.`` () =
    let input =
        genericWorld
    let expected =
        input
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX 
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=1000.0} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let actual =
        input
        |> World.UpdateCharts vesselSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``UpdateChart.It does sets all nearby island to "seen" when given avatar is near previously unseen islands.`` () =
    let input =
        genericWorld.Islands
        |> Map.fold 
            (fun w k v -> 
                w
                |> World.TransformIsland k (fun _ -> {v with AvatarVisits = Map.empty} |> Some)) genericWorld
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=11.0; CurrentValue=5.5} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=20.0; MaximumValue=20.0; CurrentValue=20.0} |> Some
        | _ ->
            Assert.Fail()
            None
    let expected =
        genericWorld.Islands
        |> Map.fold 
            (fun w k v -> 
                w
                |> World.TransformIsland k 
                    (fun _ -> 
                        {v with 
                            AvatarVisits = 
                                Map.empty
                                |> Map.add avatarId {VisitCount=None; LastVisit=None}} |> Some)) genericWorld
    let actual =
        input
        |> World.UpdateCharts vesselSingleStatisticSource
    Assert.AreEqual(expected, actual)

