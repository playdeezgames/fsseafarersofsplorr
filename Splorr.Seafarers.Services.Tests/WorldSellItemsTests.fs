module WorldSellItemsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

type TestWorldSellItemsContext
        (commoditySource, 
        islandMarketSource, 
        islandSingleMarketSink,
        islandSingleMarketSource,
        itemSingleSource,
        shipmateSingleStatisticSink,
        shipmateSingleStatisticSource) =
    interface IslandUpdateMarketForItemContext with
        member _.commoditySource: CommoditySource = commoditySource
        member _.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member _.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource
    interface WorldSellItemsContext with
        member _.commoditySource: CommoditySource = commoditySource
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.itemSingleSource : ItemSingleSource = itemSingleSource
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``SellItems.It gives a message when given a bogus island location.`` () =
    let input = avatarId
    let inputLocation = (-1.0, -1.0)
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let expectedMessage = "You cannot sell items here."
    let avatarInventorySource (_) =
        raise (System.NotImplementedException "avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        raise (System.NotImplementedException "avatarInventorySink")
        ()
    let islandSource() =
        []
    let itemSingleSource (x) =
        genericWorldItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (commoditySource, 
            islandMarketSourceStub, 
            islandSingleMarketSinkStub ,
            islandSingleMarketSourceStub, 
            itemSingleSource,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSourceStub) :> WorldSellItemsContext
    input 
    |> World.SellItems 
        context
        avatarInventorySink
        avatarInventorySource
        (avatarExpectedMessageSink expectedMessage)
        commoditySource 
        islandMarketSourceStub 
        islandSingleMarketSinkStub 
        islandSingleMarketSourceStub 
        islandSource
        genericWorldItemSource
        shipmateSingleStatisticSinkStub
        shipmateSingleStatisticSourceStub
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message when given a valid island location and bogus item to buy.`` () =
    let input = avatarId
    let inputLocation = (0.0, 0.0)
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
    let islandSource() =
        [inputLocation]
    let itemSingleSource (x) =
        genericWorldItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (commoditySource, 
            islandMarketSourceStub, 
            islandSingleMarketSinkStub ,
            islandSingleMarketSourceStub, 
            itemSingleSource,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSourceStub) :> WorldSellItemsContext
    input 
    |> World.SellItems 
        context
        avatarInventorySink
        avatarInventorySource
        (avatarExpectedMessageSink expectedMessage)
        commoditySource 
        islandMarketSourceStub 
        islandSingleMarketSinkStub 
        islandSingleMarketSourceStub 
        islandSource
        genericWorldItemSource
        shipmateSingleStatisticSinkStub
        shipmateSingleStatisticSourceStub
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message when the avatar has insufficient items in inventory.`` () =
    let input = avatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let expectedMessage = "You don't have enough of those to sell."
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let islandSource() =
        [inputLocation]
    let itemSingleSource (x) =
        genericWorldItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (commoditySource, 
            islandMarketSourceStub, 
            islandSingleMarketSinkStub ,
            islandSingleMarketSourceStub, 
            itemSingleSource,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSourceStub) :> WorldSellItemsContext
    input 
    |> World.SellItems 
        context
        avatarInventorySink
        avatarInventorySource
        (avatarExpectedMessageSink expectedMessage)
        commoditySource 
        islandMarketSourceStub 
        islandSingleMarketSinkStub 
        islandSingleMarketSourceStub 
        islandSource
        genericWorldItemSource 
        shipmateSingleStatisticSinkStub
        shipmateSingleStatisticSourceStub
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message when the avatar has no items in inventory and specifies maximum.`` () =
    let input = avatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = Maximum
    let inputItemName = "item under test"
    let expectedMessage = "You don't have any of those to sell."
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let islandSource() =
        [inputLocation]
    let itemSingleSource (x) =
        genericWorldItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (commoditySource, 
            islandMarketSourceStub, 
            islandSingleMarketSinkStub ,
            islandSingleMarketSourceStub,
            itemSingleSource,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSourceStub) :> WorldSellItemsContext
    input 
    |> World.SellItems 
        context
        avatarInventorySink
        avatarInventorySource
        (avatarExpectedMessageSink expectedMessage)
        commoditySource 
        islandMarketSourceStub 
        islandSingleMarketSinkStub 
        islandSingleMarketSourceStub 
        islandSource
        genericWorldItemSource 
        shipmateSingleStatisticSinkStub
        shipmateSingleStatisticSourceStub
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message and completes the sale when the avatar has sufficient quantity.`` () =
    let input = avatarId
    let inputLocation = (0.0, 0.0)
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
    let islandSource() =
        [inputLocation]
    let itemSingleSource (x) =
        genericWorldItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (commoditySource, 
            islandMarketSource, 
            islandSingleMarketSink ,
            islandSingleMarketSourceStub ,
            itemSingleSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource) :> WorldSellItemsContext
    input 
    |> World.SellItems 
        context
        avatarInventorySink
        avatarInventorySource
        (avatarExpectedMessageSink expectedMessage)
        commoditySource 
        islandMarketSource 
        islandSingleMarketSink 
        islandSingleMarketSourceStub 
        islandSource
        genericWorldItemSource 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message and completes the sale when the avatar has sufficient quantity and specified a maximum sell.`` () =
    let input = avatarId
    let inputLocation = (0.0, 0.0)
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
    let islandSource() =
        [inputLocation]
    let itemSingleSource (x) =
        genericWorldItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (commoditySource, 
            islandMarketSource, 
            islandSingleMarketSink ,
            islandSingleMarketSourceStub ,
            itemSingleSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource) :> WorldSellItemsContext
    input 
    |> World.SellItems 
        context
        avatarInventorySink
        avatarInventorySource
        (avatarExpectedMessageSink expectedMessage)
        commoditySource 
        islandMarketSource 
        islandSingleMarketSink 
        islandSingleMarketSourceStub 
        islandSource
        genericWorldItemSource 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        inputLocation 
        inputQuantity 
        inputItemName



