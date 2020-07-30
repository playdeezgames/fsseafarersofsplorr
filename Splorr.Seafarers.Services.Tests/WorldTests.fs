module WorldTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures
open CommonTestFixtures

[<Test>]
let ``Create.It creates a new world.`` () =
    let actual = soloIslandWorld
    Assert.AreEqual(0.0, actual.Avatars.[avatarId].Heading)
    Assert.AreEqual((5.0,5.0), actual.Avatars.[avatarId].Position)
    Assert.AreEqual(1.0, actual.Avatars.[avatarId].Speed)
    Assert.AreEqual(1, actual.Islands.Count)
    Assert.AreNotEqual("", (actual.Islands |> Map.toList |> List.map snd |> List.head).Name)

[<Test>]
let ``ClearMessages.It removes any messages from the given avatar in the world.`` () =
    let inputAvatar = {soloIslandWorld.Avatars.[avatarId] with Messages = ["test"]} 
    let inputWorld = {soloIslandWorld with Avatars = soloIslandWorld.Avatars |> Map.add avatarId inputAvatar}
    let actual =
        inputWorld
        |> World.ClearMessages avatarId
    Assert.AreEqual([], actual.Avatars.[avatarId].Messages)

[<Test>]
let ``AddMessages.It appends new messages to previously existing messages in the world.`` () =
    let oldMessages = ["one"; "two"]
    let newMessages = [ "three"; "four"]
    let allMessages = List.append oldMessages newMessages
    let inputAvatar = {soloIslandWorld.Avatars.[avatarId] with Messages = oldMessages}
    let inputWorld = {soloIslandWorld with Avatars= soloIslandWorld.Avatars |> Map.add avatarId inputAvatar}
    let actual = 
        inputWorld
        |> World.AddMessages avatarId newMessages
    Assert.AreEqual(allMessages, actual.Avatars.[avatarId].Messages)

[<Test>]
let ``SetSpeed.It produces all stop in the avatar when less than zero is passed.`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (-1.0) avatarId
    Assert.AreEqual(0.0, actual.Avatars.[avatarId].Speed)

[<Test>]
let ``SetSpeed.It produces full speed when greater than one is passed.`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (2.0) avatarId
    Assert.AreEqual(1.0, actual.Avatars.[avatarId].Speed)

[<Test>]
let ``SetSpeed.It produces half speed when one half is passed.`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (0.5) avatarId
    Assert.AreEqual(0.5, actual.Avatars.[avatarId].Speed)

[<Test>]
let ``SetSpeed.It does nothing when a bogus avatarid is passed.`` () =
    let inputWorld = soloIslandWorld
    let inputAvatarId = bogusAvatarId
    let expected = inputWorld
    let actual =
        inputWorld
        |> World.SetSpeed (1.0) inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetSpeed.It produces full speed when one is passed.`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (1.0) avatarId
    Assert.AreEqual(1.0, actual.Avatars.[avatarId].Speed)

[<Test>]
let ``SetSpeed.It sets all stop when given zero`` () =
    let actual =
        soloIslandWorld
        |> World.SetSpeed (0.0) avatarId
    Assert.AreEqual(0.0, actual.Avatars.[avatarId].Speed)

[<Test>]
let ``SetHeading.It sets a new heading when given a valid avatar id.`` () =
    let heading = 1.5
    let actual =
        soloIslandWorld
        |> World.SetHeading heading avatarId
    Assert.AreEqual(heading |> Angle.ToRadians, actual.Avatars.[avatarId].Heading)

[<Test>]
let ``SetHeading.It does nothing when given an invalid avatar id`` () =
    let heading = 1.5
    let actual =
        soloIslandWorld
        |> World.SetHeading heading bogusAvatarId
    Assert.AreEqual(soloIslandWorld, actual)


[<Test>]
let ``Move.It moves the avatar one unit when give 1u for distance when given a valid avatar id.`` () =
    let actual =
        soloIslandWorld
        |> World.Move 1u
    Assert.AreEqual((6.0,5.0), actual.Avatars.[avatarId].Position)

[<Test>]
let ``Move.It does nothing when given an invalid avatar id`` () =
    let inputWorld =
        {soloIslandWorld with AvatarId = bogusAvatarId}
    let actual =
        inputWorld
        |> World.Move 1u
    Assert.AreEqual(inputWorld, actual)

[<Test>]
let ``Move.It moves the avatar almost two units when give 2u for distance.`` () =
    let actual =
        soloIslandWorld
        |> World.Move 2u
    Assert.AreEqual((6.9989999999999997,5.0), actual.Avatars.[avatarId].Position)

[<Test>]
let ``GetNearbyLocations.It returns locations within a given distance from another given location.`` () =
    let blankIsland =
        {
            Name = ""
            AvatarVisits = Map.empty
            Jobs = []
            CareenDistance = 0.0
        }
    let world =
        {
            AvatarId = avatarId
            Avatars = 
                [avatarId,{
                    Messages = []
                    Position=(5.0, 5.0)
                    Speed=1.0
                    Heading=0.0
                    ViewDistance = 5.0
                    DockDistance = 1.0
                    Money = 0.0
                    Reputation = 0.0
                    Job = None
                    Inventory = Map.empty
                    Metrics = Map.empty
                    Vessel  = 
                        {
                            Tonnage=100.0
                            Fouling = 
                                Map.empty
                                |> Map.add Port {MinimumValue = 0.0; MaximumValue = 0.5; CurrentValue=0.0}
                                |> Map.add Starboard {MinimumValue = 0.0; MaximumValue = 0.5; CurrentValue=0.0}
                            FoulRate=0.1
                        }
                    Shipmates = 
                        [|{
                            Statistics = Map.empty
                            RationItems = [1UL]
                        }
                        |> Shipmate.SetStatistic AvatarStatisticIdentifier.Satiety (Statistic.Create (0.0, 100.0) (100.0)|>Some)
                        |> Shipmate.SetStatistic AvatarStatisticIdentifier.Health (Statistic.Create (0.0, 100.0) (100.0)|>Some)
                        |> Shipmate.SetStatistic AvatarStatisticIdentifier.Turn (Statistic.Create (0.0, 15000.0) (0.0)|>Some)
                        |]
                }
                ]|>Map.ofList
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
        |> World.GetNearbyLocations world.Avatars.[avatarId].Position world.Avatars.[avatarId].ViewDistance
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
    let actual = 
        {emptyWorld with AvatarId = bogusAvatarId}
        |> World.Dock (fun()->commodities) (fun()->genericWorldItems) islandMarketSourceStub islandMarketSinkStub islandItemSourceStub islandItemSinkStub random (0.0, 0.0) (0.0, 0.0)
    Assert.AreEqual({emptyWorld with AvatarId = bogusAvatarId}, actual)

[<Test>]
let ``Dock.It adds a message when the given location has no island.`` () =
    let inputWorld = emptyWorld
    let expectedAvatar = {inputWorld.Avatars.[avatarId] with Messages = [ "There is no place to dock there." ]}
    let expected =
        {inputWorld with 
            Avatars = inputWorld.Avatars |> Map.add avatarId expectedAvatar}
    let actual = 
        inputWorld
        |> World.Dock (fun()->commodities) (fun()->genericWorldItems) islandMarketSourceStub islandMarketSinkStub islandItemSourceStub islandItemSinkStub random (0.0, 0.0) (0.0, 0.0)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Dock.It updates the island's visit count and last visit when the given location has an island.`` () =
    let inputWorld = oneIslandWorld
    let expectedIsland = 
        inputWorld.Islands.[(0.0, 0.0)] 
        |> Island.AddVisit inputWorld.Avatars.[avatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].CurrentValue avatarId
    let expectedAvatar = 
        {inputWorld.Avatars.[avatarId] with  
            Messages = [ "You dock." ]
            Metrics = inputWorld.Avatars.[avatarId].Metrics |> Map.add Metric.VisitedIsland 1u}
    let expected = 
        {inputWorld with 
            Islands = inputWorld.Islands |> Map.add (0.0, 0.0) expectedIsland
            Avatars = inputWorld.Avatars |> Map.add avatarId expectedAvatar}        
    let actual = 
        inputWorld
        |> World.Dock (fun()->Map.empty) (fun()->Map.empty) islandMarketSourceStub islandMarketSinkStub islandItemSourceStub islandItemSinkStub random (0.0, 0.0) (0.0, 0.0)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``HeadFor.It adds a message when the island name does not exist.`` () =
    let inputWorld = headForWorld
    let expectedAvatar = {inputWorld.Avatars.[avatarId] with Messages = [ "I don't know how to get to `yermom`." ]}
    let expected =
        {inputWorld with Avatars= inputWorld.Avatars |> Map.add avatarId expectedAvatar}        
    let actual =
        inputWorld
        |> World.HeadFor "yermom"
    Assert.AreEqual(expected, actual)

[<Test>]
let ``HeadFor.It adds a message when the island name exists but is not known.`` () =
    let inputWorld = headForWorld
    let expectedAvatar = {inputWorld.Avatars.[avatarId] with Messages = [ "I don't know how to get to `Uno`." ]}
    let expected =
        {inputWorld with Avatars= inputWorld.Avatars |> Map.add avatarId expectedAvatar}        
    let actual =
        inputWorld
        |> World.HeadFor "Uno"
    Assert.AreEqual(expected, actual)

[<Test>]
let ``HeadFor.It sets the heading when the island name exists and is known.`` () =
    let inputWorld =
        headForWorld
        |> World.TransformIsland (0.0,0.0) (Island.AddVisit headForWorld.Avatars.[avatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].CurrentValue avatarId >> Some)
    let expected = {inputWorld with Avatars = inputWorld.Avatars |> Map.add avatarId {inputWorld.Avatars.[avatarId] with Messages=[ "You set your heading to 180.00°."; "You head for `Uno`." ]; Heading=System.Math.PI}}
    let actual =
        inputWorld
        |> World.HeadFor "Uno"
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AcceptJob.It does nothing when given an invalid island location.`` () =
    let actual =
        genericDockedWorld
        |> World.AcceptJob 1u genericWorldInvalidIslandLocation avatarId
    Assert.AreEqual (genericDockedWorld, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when given an 0 job index for the given valid island location.`` () =
    let inputWorld = genericDockedWorld
    let expectedAvatar = {inputWorld.Avatars.[avatarId] with Messages = [ "That job is currently unavailable." ]}
    let expected =
        {inputWorld with Avatars= inputWorld.Avatars |> Map.add avatarId expectedAvatar}        
    let actual =
        inputWorld
        |> World.AcceptJob 0u genericWorldIslandLocation avatarId
    Assert.AreEqual (expected, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when given an invalid job index for the given valid island location.`` () =
    let inputWorld = genericDockedWorld
    let expectedAvatar = {inputWorld.Avatars.[avatarId] with Messages = [ "That job is currently unavailable." ]}
    let expected =
        {inputWorld with Avatars= inputWorld.Avatars |> Map.add avatarId expectedAvatar}        
    let actual =
        inputWorld
        |> World.AcceptJob 0xFFFFFFFFu genericWorldIslandLocation avatarId
    Assert.AreEqual (expected, actual)

[<Test>]
let ``AcceptJob.It does nothing when given an invalid avatar id.`` () =
    let inputWorld = 
        genericDockedWorld
    let actual =
        inputWorld
        |> World.AcceptJob 1u genericWorldIslandLocation bogusAvatarId
    Assert.AreEqual (inputWorld, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when the job is valid but the avatar already has a job.`` () =
    let inputWorld = 
        genericDockedWorld
        |> World.TransformAvatar avatarId
            (fun avatar -> {avatar with Job =Some {FlavorText="";Destination=(0.0,0.0); Reward=0.0}}|>Some)
    let expectedAvatar = {inputWorld.Avatars.[avatarId] with Messages = [ "You must complete or abandon your current job before taking on a new one." ]}
    let expected =
        {inputWorld with Avatars= inputWorld.Avatars |> Map.add avatarId expectedAvatar}        
    let actual =
        inputWorld
        |> World.AcceptJob 1u genericWorldIslandLocation avatarId
    Assert.AreEqual (expected, actual)


[<Test>]
let ``AcceptJob.It adds the given job to the avatar and eliminates it from the island's job list when given a valid island location and a valid job index and the avatar has no current job.`` () =
    let inputWorld = genericDockedWorld
    let inputLocation = genericWorldIslandLocation
    let inputJob = inputWorld.Islands.[inputLocation].Jobs.Head
    let inputDestination = inputWorld.Islands.[inputJob.Destination]
    let expectedAvatar = 
        inputWorld.Avatars.[avatarId]
        |> Avatar.AddMessages ["You accepted the job!"]
        |> Avatar.SetJob inputJob
        |> Avatar.AddMetric Metric.AcceptedJob 1u
    let expectedIsland = 
        {inputWorld.Islands.[inputLocation] with Jobs = []}
    let expectedDestination =
        {inputDestination with 
            AvatarVisits = Map.empty |> Map.add avatarId {VisitCount=0u |> Some;LastVisit=None}}
    let actual =
        inputWorld
        |> World.AcceptJob 1u inputLocation avatarId
    Assert.AreEqual(1, actual.Avatars.[avatarId].Messages.Length)
    Assert.AreEqual(expectedAvatar, actual.Avatars.[avatarId])
    Assert.AreEqual(expectedIsland, actual.Islands.[inputLocation])
    Assert.AreEqual(expectedDestination, actual.Islands.[inputJob.Destination])

[<Test>]
let ``TransformAvatar.It transforms the avatar within the given world.`` () =
    let expectedAvatar = genericWorld.Avatars.[avatarId] |> Avatar.Move
    let actual =
        genericWorld
        |> World.TransformAvatar avatarId (Avatar.Move >> Some)
    Assert.AreEqual(expectedAvatar,actual.Avatars.[avatarId])

[<Test>]
let ``AbandonJob.It adds a message when the avatar has no job.`` () =
    let input = genericDockedWorld
    let expectedAvatar = {input.Avatars.[avatarId] with Messages=["You have no job to abandon."]}
    let expected = {input with Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
    let actual = 
        input
        |> World.AbandonJob avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AbandonJob.It adds a messages and abandons the job when the avatar has a a job`` () =
    let input = jobWorld
    let expected = 
        {input with 
            Avatars = 
                input.Avatars 
                |> Map.add avatarId 
                    {input.Avatars.[avatarId] with 
                        Messages=["You abandon your job."]
                        Job = None
                        Reputation = input.Avatars.[avatarId].Reputation - 1.0
                        Metrics = input.Avatars.[avatarId].Metrics |> Map.add Metric.AbandonedJob 1u}}
    let actual = 
        input
        |> World.AbandonJob avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Dock.It does not modify avatar when given avatar has a job for a different destination.`` () =
    let input = jobWorld
    let expectedAvatar =
        {input.Avatars.[avatarId] with 
            Messages = ["You dock."]
            Metrics = input.Avatars.[avatarId].Metrics |> Map.add Metric.VisitedIsland 1u}
    let actual = 
        jobWorld
        |> World.Dock (fun()->commodities) (fun()->genericWorldItems) islandMarketSourceStub islandMarketSinkStub islandItemSourceStub islandItemSinkStub random (0.0, 0.0) genericWorldIslandLocation
    Assert.AreEqual(expectedAvatar, actual.Avatars.[avatarId])

[<Test>]
let ``Dock.It adds a message and completes the job when given avatar has a job for this location.`` () =
    let input = jobWorld
    let inputJob = jobWorld.Avatars.[avatarId].Job.Value
    let expectedMessages = ["You complete your job."; "You dock."]
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Job = None;
            Money = input.Avatars.[avatarId].Money + inputJob.Reward;
            Reputation = input.Avatars.[avatarId].Reputation + 1.0
            Messages = expectedMessages
            Metrics = 
                input.Avatars.[avatarId].Metrics 
                |> Map.add Metric.VisitedIsland 2u
                |> Map.add Metric.CompletedJob 1u}
    let actual = 
        jobWorld
        |> World.Dock (fun()->commodities) (fun()->genericWorldItems) islandMarketSourceStub islandMarketSinkStub islandItemSourceStub islandItemSinkStub random (0.0, 0.0) jobLocation
    Assert.AreEqual(expectedAvatar, actual.Avatars.[avatarId])
    Assert.AreEqual(expectedMessages, actual.Avatars.[avatarId].Messages)

let islandSingleMarketSinkStub (_) (_) = ()

[<Test>]
let ``BuyItems.It gives a message when given a bogus island location.`` () =
    let input = shopWorld
    let inputLocation = shopWorldBogusLocation
    let inputQuantity = 2u |> Specific
    let inputItemName = "item under test"
    let expected =
        input
        |> World.AddMessages avatarId ["You cannot buy items here."]
    let actual = 
        input 
        |> World.BuyItems islandMarketSourceStub islandSingleMarketSinkStub commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when given a valid island location and a bogus item to buy.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u |> Specific
    let inputItemName = "bogus item"
    let expected =
        input
        |> World.AddMessages avatarId ["Round these parts, we don't sell things like that."]
    let actual = 
        input 
        |> World.BuyItems islandMarketSourceStub islandSingleMarketSinkStub commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient funds.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expected =
        input
        |> World.AddMessages avatarId ["You don't have enough money."]
    let actual = 
        input 
        |> World.BuyItems islandMarketSource islandSingleMarketSinkStub commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient tonnage.`` () =
    let inputAvatar = 
        {shopWorld.Avatars.[avatarId] with
            Money = 1000000.0}
    let input = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputLocation = shopWorldLocation
    let inputQuantity = 1000u |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expected =
        input
        |> World.AddMessages avatarId ["You don't have enough tonnage."]
    let actual = 
        input 
        |> World.BuyItems islandMarketSource islandSingleMarketSinkStub commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message and completes the purchase when the avatar has sufficient funds.`` () =
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Money = 1000000.0}
    let input = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(5.0, market.Supply)
        Assert.AreEqual(7.0, market.Demand)
    let expectedAvatar = 
        {input.Avatars.[avatarId] with
            Money = 999998.0
            Inventory = Map.empty |> Map.add 1UL 2u}
    let expected =
        {input with
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |> World.AddMessages avatarId ["You complete the purchase of 2 item under test."]
    let actual = 
        input 
        |> World.BuyItems islandMarketSource islandSingleMarketSink commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
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
    let expected =
        input
        |> World.AddMessages avatarId ["You don't have enough money to buy any of those."]
    let actual = 
        input 
        |> World.BuyItems islandMarketSource islandSingleMarketSink commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message indicating purchased quantity and completes the purchase when the avatar has sufficient funds for at least one and has specified a maximum buy.`` () =
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Money = 100.0}
    let input = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
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
    let expectedAvatar = 
        {input.Avatars.[avatarId] with
            Money = 0.0
            Inventory = Map.empty |> Map.add 1UL 100u}
    let expected =
        {input with
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |> World.AddMessages avatarId ["You complete the purchase of 100 item under test."]
    let actual = 
        input 
        |> World.BuyItems islandMarketSource islandSingleMarketSink commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``SellItems.It gives a message when given a bogus island location.`` () =
    let input = shopWorld
    let inputLocation = shopWorldBogusLocation
    let inputQuantity = 2u |> Specific
    let inputItemName = "item under test"
    let expected =
        input
        |> World.AddMessages avatarId ["You cannot sell items here."]
    let actual = 
        input 
        |> World.SellItems islandMarketSourceStub islandSingleMarketSourceStub islandSingleMarketSinkStub commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when given a valid island location and bogus item to buy.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u |> Specific
    let inputItemName = "bogus item"
    let expected =
        input
        |> World.AddMessages avatarId ["Round these parts, we don't buy things like that."]
    let actual = 
        input 
        |> World.SellItems islandMarketSourceStub islandSingleMarketSourceStub islandSingleMarketSinkStub commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when the avatar has insufficient items in inventory.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u |> Specific
    let inputItemName = "item under test"
    let expected =
        input
        |> World.AddMessages avatarId ["You don't have enough of those to sell."]
    let actual = 
        input 
        |> World.SellItems islandMarketSourceStub islandSingleMarketSourceStub islandSingleMarketSinkStub commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when the avatar has no items in inventory and specifies maximum.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = Maximum
    let inputItemName = "item under test"
    let expected =
        input
        |> World.AddMessages avatarId ["You don't have any of those to sell."]
    let actual = 
        input 
        |> World.SellItems islandMarketSourceStub islandSingleMarketSourceStub islandSingleMarketSinkStub commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message and completes the sale when the avatar has sufficient quantity.`` () =
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Inventory = Map.empty |> Map.add 1UL 2u}
    let input = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(7.0, market.Supply)
        Assert.AreEqual(5.0, market.Demand)
    let expectedAvatar = 
        {input.Avatars.[avatarId] with
            Money = 1.0
            Inventory = Map.empty}
    let expected =
        {input with
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |> World.AddMessages avatarId ["You complete the sale of 2 item under test."]
    let actual = 
        input 
        |> World.SellItems islandMarketSource islandSingleMarketSourceStub islandSingleMarketSink commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message and completes the salewhen the avatar has sufficient quantity and specified a maximum sell.`` () =
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Inventory = Map.empty |> Map.add 1UL 2u}
    let input = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
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
    let expectedAvatar = 
        {input.Avatars.[avatarId] with
            Money = 1.0
            Inventory = Map.empty}
    let expected =
        {input with
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
        |> World.AddMessages avatarId ["You complete the sale of 2 item under test."]
    let actual = 
        input 
        |> World.SellItems islandMarketSource islandSingleMarketSourceStub islandSingleMarketSink commodities genericWorldItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``IsAvatarAlive.It returns a true when given a world with an avatar with above minimum health.`` () =
    if genericWorld |> World.IsAvatarAlive avatarId then
        Assert.Pass("It detected that the avatar is alive")
    else
        Assert.Fail("It detected that the avatar is not alive")

[<Test>]
let ``IsAvatarAlive.It returns a false when given a world with an avatar minimum health (zero).`` () =
    if deadWorld |> World.IsAvatarAlive avatarId |> not then
        Assert.Pass("It detected that the avatar is dead")
    else
        Assert.Fail("It detected that the avatar is not dead")


[<Test>]
let ``CleanHull.It returns the original world when given a bogus avatar id and world.`` () =
    let inputWorld = 
        genericWorld
    let inputSide = Port
    let expected =
        inputWorld
    let actual =
        inputWorld
        |> World.CleanHull bogusAvatarId inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CleanHull.It returns a cleaned hull when given a particular avatar id and world.`` () =
    let inputSide = Port
    let inputVessel =
        genericWorld.Avatars.[avatarId].Vessel
        |> Vessel.TransformFouling Port (fun x->{x with CurrentValue = x.MaximumValue})
        |> Vessel.TransformFouling Starboard (fun x->{x with CurrentValue = x.MaximumValue})
    let inputAvatar =
        {genericWorld.Avatars.[avatarId] with
            Vessel = inputVessel}
    let inputWorld = 
        {genericWorld with
            Avatars = genericWorld.Avatars |> Map.add avatarId inputAvatar}
    let expectedVessel =
        inputVessel
        |> Vessel.TransformFouling inputSide (fun x-> {x with CurrentValue=x.MinimumValue})
    let expectedAvatar =
        {inputAvatar with
            Vessel = expectedVessel}
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic AvatarStatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some)) 0u
        |> Avatar.AddMetric Metric.CleanedHull 1u
    let expected =
        {inputWorld with
            Avatars = inputWorld.Avatars |> Map.add avatarId expectedAvatar}
    let actual =
        inputWorld
        |> World.CleanHull avatarId inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``DistanceTo.It adds a 'unknown island' message when given a bogus island name.`` () =
    let input = genericWorld
    let inputName = "$$$$$$$"
    let expectedMessage = inputName |> sprintf "I don't know how to get to `%s`."
    let expected =
        input
        |> World.TransformAvatar input.AvatarId (Avatar.AddMessages [expectedMessage] >> Some)
    let actual =
        input
        |> World.DistanceTo inputName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``DistanceTo.It adds a 'unknown island' message when given a valid island name that is not known.`` () =
    let inputName = ((genericWorld.Islands |> Map.toList).Head |> snd).Name
    let input = genericWorld
    let expectedMessage = inputName |> sprintf "I don't know how to get to `%s`."
    let expected =
        input
        |> World.TransformAvatar input.AvatarId (Avatar.AddMessages [expectedMessage] >> Some)
    let actual =
        input
        |> World.DistanceTo inputName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``DistanceTo.It adds a 'distance to island' message when given a valid island name that is known.`` () =
    let inputLocation = (genericWorld.Islands |> Map.toList).Head |> fst
    let inputName = ((genericWorld.Islands |> Map.toList).Head |> snd).Name
    let input = 
        genericWorld
        |> World.TransformIsland inputLocation (Island.MakeKnown genericWorld.AvatarId >> Some)
    let expectedMessage = (inputName, Location.DistanceTo inputLocation input.Avatars.[input.AvatarId].Position) ||> sprintf "Distance to `%s` is %f."
    let expected =
        input
        |> World.TransformAvatar input.AvatarId (Avatar.AddMessages [expectedMessage] >> Some)
    let actual =
        input
        |> World.DistanceTo inputName
    Assert.AreEqual(expected, actual)

[<Test>]
let ``UpdateChart.It does nothing when the given avatar is not near any nearby islands.`` () =
    let input =
        genericWorld
        |> World.TransformAvatar 
            avatarId 
            (fun a -> 
                {a with Position = genericWorldConfiguration.WorldSize |> Location.ScaleBy 10.0} |> Some)
    let expected =
        input
    let actual =
        input
        |> World.UpdateCharts
    Assert.AreEqual(expected, actual)

[<Test>]
let ``UpdateChart.It does nothing when the given avatar has already seen all nearby islands.`` () =
    let input =
        genericWorld
    let expected =
        input
    let actual =
        input
        |> World.UpdateCharts
    Assert.AreEqual(expected, actual)

[<Test>]
let ``UpdateChart.It does sets all nearby island to "seen" when given avatar is near previously unseen islands.`` () =
    let input =
        genericWorld.Islands
        |> Map.fold 
            (fun w k v -> 
                w
                |> World.TransformIsland k (fun _ -> {v with AvatarVisits = Map.empty} |> Some)) genericWorld
    let expected =
        genericWorld
    let actual =
        input
        |> World.UpdateCharts
    Assert.AreEqual(expected, actual)

//[<Test>]
//let ``FunctionName.It returns a SOMETHING when given SOMETHINGELSE.`` () =
//    raise (System.NotImplementedException "Not Implemented")
