﻿module DockedTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open DockedTestFixtures
open Splorr.Seafarers.Services

let private functionUnderTest (itemMarketSource) (itemSingleMarketSink) =  
    Docked.Run 
        commoditySource 
        itemSource 
        itemMarketSource 
        itemSingleMarketSink

let private functionUnderTestStubbed 
        (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
        (avatarMessageSink             : AvatarMessageSink)= 
    functionUnderTest 
        dockedItemMarketSourceStub 
        dockedItemSingleMarketSourceStub 
        dockedItemSingleMarketSinkStub 
        vesselSingleStatisticSourceStub
        shipmateSingleStatisticSource
        shipmateSingleStatisticSinkStub
        avatarInventorySourceStub
        avatarInventorySinkStub
        avatarMessageSourceStub
        avatarMessageSink
        avatarMessagePurgerStub

[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let input = deadDockWorld   
    let inputLocation= deadDockLocation
    let inputSource(): Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let expectedMessages = []
    let expected =
        expectedMessages
        |> Gamestate.GameOver
        |> Some
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSource - %s"))
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed
            shipmateSingleStatisticSource
            avatarMessageSinkStub
            inputSource 
            sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given Undock Command.`` () =
    let input = dockWorld
    let inputLocation= dockLocation
    let inputSource = Command.Undock |> Some |> toSource
    let expectedAvatar = 
        input.Avatars.[avatarId]
    let expected = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar} 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub 
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
        (inputLocation, input)
        ||> functionUnderTestStubbed
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub 
    Assert.AreEqual(expected,actual)

[<Test>]
let ``Run.It returns Metrics when given Metrics Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Metrics 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, input) 
        |> Gamestate.Docked 
        |> Gamestate.Metrics 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub 
    Assert.AreEqual(expected, actual)

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
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub 
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
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns InvalidInput when given invalid Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource =
        None 
        |> toSource
    let expected = 
        ("Maybe try 'help'?",(Dock, inputLocation, input) 
        |> Gamestate.Docked)
        |> Gamestate.ErrorMessage
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns AtSea when given invalid docked location.`` () =
    let input = dockWorld
    let inputLocation = (1.0, 1.0)
    let inputSource () = 
        Assert.Fail("This should not be called.")
        Command.Help 
        |> Some
    let expected = 
        input 
        |> Gamestate.AtSea 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub
    Assert.AreEqual(expected,actual)

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
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub
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
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given the Accept Job command and the given job number does not exist.`` () =
    let input = smallWorldDocked
    let inputLocation = smallWorldIslandLocation
    let inputSource = 0u |> Command.AcceptJob |> Some |> toSource
    let expectedMessages = [ "That job is currently unavailable." ]
    let expectedAvatar =
        input.Avatars.[avatarId]
    let expectedWorld = 
        {input with 
            Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
    let expected = 
        (Dock, inputLocation,  expectedWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub 
            inputLocation
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
    let expectedAvatar =
        input.Avatars.[avatarId]
    let expectedWorld = 
        {input with Avatars = input.Avatars |> Map.add avatarId expectedAvatar}
    let expected = 
        (Dock, inputLocation, expectedWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub 
            inputLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let input = abandonJobWorld
    let inputLocation = dockLocation
    let inputSource = Job |> Command.Abandon |> Some |> toSource
    let expectedAvatar = 
        {input.Avatars.[avatarId] with 
            Job=None
            Metrics = input.Avatars.[avatarId].Metrics |> Map.add Metric.AbandonedJob 1UL}
    let expectedWorld = 
        {input with 
            Avatars= input.Avatars |> Map.add avatarId expectedAvatar}
    let expected = 
        (Dock, inputLocation, expectedWorld) 
        |> Gamestate.Docked 
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            sinkStub 
            inputLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at ItemList) gamestate when given the Items command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = 
        Command.Items 
        |> Some 
        |> toSource
    let expected = 
        (ItemList, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            avatarMessageSinkStub
            inputSource 
            (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Buy command for a non-existent item.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (1UL |> Specific, "non existent item") |> Command.Buy |> Some |> toSource
    let expectedMessages = ["Round these parts, we don't sell things like that."]
    let expectedWorld =
        inputWorld
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            (avatarExpectedMessagesSink expectedMessages)
            inputSource 
            (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Buy command and the avatar does not have enough money to complete the purchase.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = smallWorldDocked
    let inputSource = (1UL |> Specific, "item under test") |> Command.Buy |> Some |> toSource
    let markets =
        Map.empty
        |> Map.add 1UL {Demand=5.0; Supply=5.0}
    let islandMarketSource (_) = markets
    let islandSingleMarketSource x y =
        islandMarketSource x
        |> Map.tryFind y
    let islandSingleMarketSink (_) (_) =
        Assert.Fail("This should not be called.")
    let expectedMessages = ["You don't have enough money."]
    let expectedWorld =
        inputWorld
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest 
            islandMarketSource 
            islandSingleMarketSource 
            islandSingleMarketSink 
            vesselSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            shipmateSingleStatisticSinkStub
            avatarInventorySourceStub
            avatarInventorySinkStub
            avatarMessageSourceStub
            (avatarExpectedMessagesSink expectedMessages)
            avatarMessagePurgerStub
            inputSource 
            (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and completes the purchase when given the Buy command and the avatar has enough money.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputAvatar = shopWorld.Avatars.[avatarId]
    let inputWorld = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputSource = (1UL |> Specific, "item under test") |> Command.Buy |> Some |> toSource
    let markets =
        Map.empty
        |> Map.add 1UL {Demand=5.0; Supply=5.0}
    let islandMarketSource (_) = markets
    let islandSingleMarketSource x y =
        islandMarketSource x
        |> Map.tryFind y
    let commodities = commoditySource()
    let smallWorldItems = itemSource()
    let expectedPrice = Item.DetermineSalePrice commodities markets smallWorldItems.[1UL]
    let expectedDemand = 
        markets.[1UL].Demand + commodities.[1UL].SaleFactor
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(markets.[commodityId].Supply, market.Supply)
        Assert.AreEqual(expectedDemand, market.Demand)
    let expectedIsland = 
        inputWorld.Islands.[inputLocation]
    let expectedMessages = ["You complete the purchase of 1 item under test."]
    let expectedWorld =
        inputWorld
        |> World.TransformIsland inputLocation (fun _ -> expectedIsland |> Some)
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 100.0 |> Some
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 1000.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSource - %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSink - %s"))
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest 
            islandMarketSource 
            islandSingleMarketSource 
            islandSingleMarketSink 
            vesselSingleStatisticSourceStub 
            shipmateSingleStatisticSource
            shipmateSingleStatisticSink
            avatarInventorySourceStub
            avatarInventorySinkStub
            avatarMessageSourceStub
            (avatarExpectedMessagesSink expectedMessages)
            avatarMessagePurgerStub
            inputSource 
            (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Sell command for a non-existent item.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (Specific 1UL, "non existent item") |> Command.Sell |> Some |> toSource
    let expectedMessages = ["Round these parts, we don't buy things like that."]
    let expectedWorld =
        inputWorld
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            (avatarExpectedMessagesSink expectedMessages)
            inputSource 
            (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Sell command and the avatar does not sufficient items to sell.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (Specific 1UL, "item under test") |> Command.Sell |> Some |> toSource
    let expectedMessages = ["You don't have enough of those to sell."]
    let expectedWorld =
        inputWorld
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed 
            shipmateSingleStatisticSourceStub
            (avatarExpectedMessagesSink expectedMessages)
            inputSource 
            (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and completes the sale when given the Sell command and the avatar sufficient items to sell.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputItems = [(1UL, 1UL)] |> Map.ofList
    let inputAvatar = shopWorld.Avatars.[avatarId]
    let markets =
        Map.empty
        |> Map.add 1UL {Supply = 5.0; Demand =5.0}
    let islandMarketSource (_) = markets
    let islandSingleMarketSource x y =
        islandMarketSource x
        |> Map.tryFind y
    let inputWorld = 
        {shopWorld with 
            Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputSource = (Specific 1UL, "item under test") |> Command.Sell |> Some |> toSource
    let commodities = commoditySource()
    let expectedSupply = 
        markets.[1UL].Supply + commodities.[1UL].PurchaseFactor
    let expectedMarket = 
        {markets.[1UL] with 
            Supply= expectedSupply}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(expectedMarket, market)
    let expectedIsland = 
        inputWorld.Islands.[inputLocation]
    let expectedMessages = ["You complete the sale of 1 item under test.";"You complete the sale."]
    let expectedAvatar = 
        inputAvatar
    let expectedWorld =
        inputWorld
        |> World.TransformIsland inputLocation (fun _ -> expectedIsland |> Some)
        |> World.TransformAvatar (fun _ -> expectedAvatar |> Some)
    let expected = 
        (Dock, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let avatarInventorySource (_) = Map.empty |> Map.add 1UL 1UL
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) = Assert.AreEqual(Map.empty, inventory)
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest 
            islandMarketSource 
            islandSingleMarketSource 
            islandSingleMarketSink 
            vesselSingleStatisticSourceStub 
            shipmateSingleStatisticSourceStub
            shipmateSingleStatisticSinkStub
            avatarInventorySource
            avatarInventorySink
            avatarMessageSourceStub
            (avatarExpectedMessagesSink expectedMessages)
            avatarMessagePurgerStub
            inputSource 
            (sinkStub)
    Assert.AreEqual(expected, actual)

