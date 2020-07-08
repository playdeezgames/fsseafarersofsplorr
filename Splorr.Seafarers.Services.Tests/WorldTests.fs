module WorldTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures

[<Test>]
let ``Create.It creates a new world.`` () =
    let actual = soloIslandWorld
    Assert.AreEqual(0.0, actual.Avatars.[avatarId].Heading)
    Assert.AreEqual((5.0,5.0), actual.Avatars.[avatarId].Position)
    Assert.AreEqual(1.0, actual.Avatars.[avatarId].Speed)
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
let ``SetHeading.It sets a new heading.`` () =
    let heading = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
    let actual =
        soloIslandWorld
        |> World.SetHeading heading avatarId
    Assert.AreEqual(heading |> Dms.ToFloat, actual.Avatars.[avatarId].Heading)

[<Test>]
let ``Move.It moves the avatar one unit when give 1u for distance.`` () =
    let actual =
        soloIslandWorld
        |> World.Move 1u avatarId
    Assert.AreEqual((6.0,5.0), actual.Avatars.[avatarId].Position)

[<Test>]
let ``Move.It moves the avatar two units when give 2u for distance.`` () =
    let actual =
        soloIslandWorld
        |> World.Move 2u avatarId
    Assert.AreEqual((7.0,5.0), actual.Avatars.[avatarId].Position)

[<Test>]
let ``GetNearbyLocations.It returns locations within a given distance from another given location.`` () =
    let blankIsland =
        {
            Name = ""
            LastVisit = None
            VisitCount = None
            Jobs = []
            Markets = Map.empty
            Items = Set.empty
        }
    let world =
        {
            RewardRange = (1.0,10.0)
            Avatars = 
                [avatarId,{
                    Position=(5.0, 5.0)
                    Speed=1.0
                    Heading=0.0
                    ViewDistance = 5.0
                    DockDistance = 1.0
                    Money = 0.0
                    Reputation = 0.0
                    Job = None
                    Inventory = Map.empty
                    Satiety = Statistic.Create (0.0, 100.0) (100.0)
                    Health = Statistic.Create (0.0, 100.0) (100.0)
                }]|>Map.ofList
            Turn = 0u
            Commodities = Map.empty
            Messages = []
            Items = Map.empty
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

[<Test>]
let ``Dock.It adds a message when the given location has no island.`` () =
    let actual = 
        emptyWorld
        |> World.Dock random (0.0, 0.0) avatarId
    Assert.AreEqual({emptyWorld with Messages = [ "There is no place to dock there." ]}, actual)

[<Test>]
let ``Dock.It updates the island's visit count and last visit when the given location has an island.`` () =
    let actual = 
        oneIslandWorld
        |> World.Dock random (0.0, 0.0) avatarId
    let updatedIsland = 
        oneIslandWorld.Islands.[(0.0, 0.0)] |> Island.AddVisit oneIslandWorld.Turn
    Assert.AreEqual({oneIslandWorld with Messages = [ "You dock." ]; Islands = oneIslandWorld.Islands |> Map.add (0.0, 0.0) updatedIsland}, actual)

[<Test>]
let ``HeadFor.It adds a message when the island name does not exist.`` () =
    let actual =
        headForWorld
        |> World.HeadFor "yermom" avatarId
    Assert.AreEqual({headForWorld with Messages=[ "I don't know how to get to `yermom`." ]}, actual)

[<Test>]
let ``HeadFor.It adds a message when the island name exists but is not known.`` () =
    let actual =
        headForWorld
        |> World.HeadFor "Uno" avatarId
    Assert.AreEqual({headForWorld with Messages=[ "I don't know how to get to `Uno`." ]}, actual)

[<Test>]
let ``HeadFor.It sets the heading when the island name exists and is known.`` () =
    let modifiedWorld =
        headForWorld
        |> World.TransformIsland (0.0,0.0) (Island.AddVisit headForWorld.Turn >> Some)
    let actual =
        modifiedWorld
        |> World.HeadFor "Uno" avatarId
    Assert.AreEqual({modifiedWorld with Messages=[ "You set your heading to 180°0'0.000000\"."; "You head for `Uno`." ]; Avatars = modifiedWorld.Avatars |> Map.add avatarId {modifiedWorld.Avatars.[avatarId] with Heading=System.Math.PI}}, actual)

[<Test>]
let ``AcceptJob.It does nothing when given an invalid island location.`` () =
    let actual =
        genericDockedWorld
        |> World.AcceptJob 1u genericWorldInvalidIslandLocation avatarId
    Assert.AreEqual (genericDockedWorld, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when given an 0 job index for the given valid island location.`` () =
    let actual =
        genericDockedWorld
        |> World.AcceptJob 0u genericWorldIslandLocation avatarId
    Assert.AreEqual ({genericDockedWorld with Messages = [ "That job is currently unavailable." ]}, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when given an invalid job index for the given valid island location.`` () =
    let actual =
        genericDockedWorld
        |> World.AcceptJob 0xFFFFFFFFu genericWorldIslandLocation avatarId
    Assert.AreEqual ({genericDockedWorld with Messages = [ "That job is currently unavailable." ]}, actual)

[<Test>]
let ``AcceptJob.It adds a message to the world when the job is valid but the avatar already has a job.`` () =
    let inputWorld = 
        genericDockedWorld
        |> World.TransformAvatar avatarId
            (fun avatar -> {avatar with Job =Some {FlavorText="";Destination=(0.0,0.0); Reward=0.0}}|>Some)
    let actual =
        inputWorld
        |> World.AcceptJob 1u genericWorldIslandLocation avatarId
    Assert.AreEqual ({inputWorld with Messages = [ "You must complete or abandon your current job before taking on a new one." ]}, actual)


[<Test>]
let ``AcceptJob.It adds the given job to the avatar and eliminates it from the island's job list when given a valid island location and a valid job index and the avatar has no current job.`` () =
    let inputWorld = genericDockedWorld
    let inputLocation = genericWorldIslandLocation
    let inputJob = inputWorld.Islands.[inputLocation].Jobs.Head
    let inputDestination = inputWorld.Islands.[inputJob.Destination]
    let expectedAvatar = 
        inputWorld.Avatars.[avatarId]
        |> Avatar.SetJob inputJob
    let expectedIsland = 
        {inputWorld.Islands.[inputLocation] with Jobs = []}
    let expectedDestination =
        {inputDestination with VisitCount=Some 0u}
    let actual =
        inputWorld
        |> World.AcceptJob 1u inputLocation avatarId
    Assert.AreEqual( "You accepted the job!", actual.Messages.Head)
    Assert.AreEqual(1, actual.Messages.Length)
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
    let expected = {input with Messages=["You have no job to abandon."]}
    let actual = 
        input
        |> World.AbandonJob avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AbandonJob.It adds a messages and abandons the job when the avatar has a a job`` () =
    let input = jobWorld
    let expected = {input with Messages=["You abandon your job."]; Avatars = input.Avatars |> Map.add avatarId {input.Avatars.[avatarId] with Job = None; Reputation = input.Avatars.[avatarId].Reputation - 1.0}}
    let actual = 
        input
        |> World.AbandonJob avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Dock.It does not modify avatar when given avatar has a job for a different destination.`` () =
    let input = jobWorld
    let actual = 
        jobWorld
        |> World.Dock random genericWorldIslandLocation avatarId
    Assert.AreEqual(input.Avatars.[avatarId], actual.Avatars.[avatarId])

[<Test>]
let ``Dock.It adds a message and completes the job when given avatar has a job for this location.`` () =
    let input = jobWorld
    let inputJob = jobWorld.Avatars.[avatarId].Job.Value
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Job = None;
            Money = input.Avatars.[avatarId].Money + inputJob.Reward;
            Reputation = input.Avatars.[avatarId].Reputation + 1.0}
    let expectedMessages = ["You complete your job."; "You dock."]
    let actual = 
        jobWorld
        |> World.Dock random jobLocation avatarId
    Assert.AreEqual(expectedAvatar, actual.Avatars.[avatarId])
    Assert.AreEqual(expectedMessages, actual.Messages)

[<Test>]
let ``BuyItems.It gives a message when given a bogus island location.`` () =
    let input = shopWorld
    let inputLocation = shopWorldBogusLocation
    let inputQuantity = 2u
    let inputItemName = "item under test"
    let expected =
        input
        |> World.AddMessages ["You cannot buy items here."]
    let actual = 
        input 
        |> World.BuyItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when given a valid island location and a bogus item to buy.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u
    let inputItemName = "bogus item"
    let expected =
        input
        |> World.AddMessages ["Round these parts, we don't sell things like that."]
    let actual = 
        input 
        |> World.BuyItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient funds.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u
    let inputItemName = "item under test"
    let expected =
        input
        |> World.AddMessages ["You don't have enough money to buy those."]
    let actual = 
        input 
        |> World.BuyItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``BuyItems.It gives a message and completes the purchase when the avatar has sufficient funds.`` () =
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Money = 1000000.0}
    let input = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u
    let inputItemName = "item under test"
    let expectedMarket =
        {input.Islands.[inputLocation].Markets.[Grain] with
            Demand = 7.0}
    let expectedIsland = 
        {input.Islands.[inputLocation] with
            Markets = input.Islands.[inputLocation].Markets |> Map.add Grain expectedMarket}
    let expectedAvatar = 
        {input.Avatars.[avatarId] with
            Money = 999998.0
            Inventory = Map.empty |> Map.add Ration 2u}
    let expected =
        {input with
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar
            Islands = input.Islands |> Map.add inputLocation expectedIsland}
        |> World.AddMessages ["You complete the purchase."]
    let actual = 
        input 
        |> World.BuyItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when given a bogus island location.`` () =
    let input = shopWorld
    let inputLocation = shopWorldBogusLocation
    let inputQuantity = 2u
    let inputItemName = "item under test"
    let expected =
        input
        |> World.AddMessages ["You cannot sell items here."]
    let actual = 
        input 
        |> World.SellItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when given a valid island location and bogus item to buy.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u
    let inputItemName = "bogus item"
    let expected =
        input
        |> World.AddMessages ["Round these parts, we don't buy things like that."]
    let actual = 
        input 
        |> World.SellItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message when the avatar has insufficient items in inventory.`` () =
    let input = shopWorld
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u
    let inputItemName = "item under test"
    let expected =
        input
        |> World.AddMessages ["You don't have enough of those to sell."]
    let actual = 
        input 
        |> World.SellItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SellItems.It gives a message and completes the purchase when the avatar has sufficient funds.`` () =
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Inventory = Map.empty |> Map.add Ration 2u}
    let input = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputLocation = shopWorldLocation
    let inputQuantity = 2u
    let inputItemName = "item under test"
    let expectedMarket =
        {input.Islands.[inputLocation].Markets.[Grain] with
            Supply = 7.0}
    let expectedIsland = 
        {input.Islands.[inputLocation] with
            Markets = input.Islands.[inputLocation].Markets |> Map.add Grain expectedMarket}
    let expectedAvatar = 
        {input.Avatars.[avatarId] with
            Money = 1.0
            Inventory = Map.empty}
    let expected =
        {input with
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar
            Islands = input.Islands |> Map.add inputLocation expectedIsland}
        |> World.AddMessages ["You complete the sale."]
    let actual = 
        input 
        |> World.SellItems inputLocation inputQuantity inputItemName avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AVATAR_ALIVE/AVATAR_DEAD.It returns a AVATAR_ALIVE when given a world with an avatar with above minimum health.`` () =
    if genericWorld |> World.IsAvatarAlive avatarId then
        Assert.Pass("It detected that the avatar is alive")
    else
        Assert.Fail("It detected that the avatar is not alive")

[<Test>]
let ``AVATAR_ALIVE/AVATAR_DEAD.It returns a AVATAR_DEAD when given a world with an avatar minimum health (zero).`` () =
    if genericWorld |> World.IsAvatarAlive avatarId |> not then
        Assert.Pass("It detected that the avatar is dead")
    else
        Assert.Fail("It detected that the avatar is not dead")

//[<Test>]
//let ``FunctionName.It returns a SOMETHING when given SOMETHINGELSE.`` () =
//    raise (System.NotImplementedException "Not Implemented")
