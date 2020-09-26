module WorldSellItemsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldSellItemsContext
        (avatarInventorySink,
        avatarInventorySource,
        avatarMessageSink,
        commoditySource, 
        islandMarketSource, 
        islandSingleMarketSink,
        islandSingleMarketSource,
        islandSource,
        itemSingleSource,
        itemSource,
        shipmateSingleStatisticSink,
        shipmateSingleStatisticSource) =
    interface Island.UpdateMarketForItemContext
    interface Commodity.GetCommoditiesContext with
        member this.commoditySource: CommoditySource = commoditySource
    interface IslandMarket.DeterminePriceContext with
        member this.islandMarketSource: IslandMarketSource = islandMarketSource
        member this.itemSingleSource: ItemSingleSource = itemSingleSource
    interface Avatar.RemoveInventoryContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface World.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface Avatar.GetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface World.SellItemsContext with
        member _.islandSource                  : IslandSource = islandSource
        member _.itemSource                    : ItemSource = itemSource
    interface AvatarMessages.AddContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface Island.ChangeMarketContext with
        member this.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member this.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``SellItems.It gives a message when given a bogus island location.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
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
        Fixtures.Common.Stub.ItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            Fixtures.Common.Fake.IslandMarketSource, 
            Fixtures.Common.Fake.IslandSingleMarketSink,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            itemSingleSource,
            Fixtures.Common.Stub.ItemSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource) :> World.SellItemsContext
    input 
    |> World.SellItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message when given a valid island location and bogus item to buy.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = 2UL |> Specific
    let inputItemName = "bogus item"
    let expectedMessage = "Round these parts, we don't buy things like that."
    let avatarInventorySource (_) =
        raise (System.NotImplementedException "avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        raise (System.NotImplementedException "avatarInventorySink")
        ()
    let islandSource() =
        [inputLocation]
    let itemSingleSource (x) =
        Fixtures.Common.Stub.ItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            Fixtures.Common.Fake.IslandMarketSource, 
            Fixtures.Common.Fake.IslandSingleMarketSink,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            itemSingleSource,
            Fixtures.Common.Stub.ItemSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource) :> World.SellItemsContext
    input 
    |> World.SellItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message when the avatar has insufficient items in inventory.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
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
        Fixtures.Common.Stub.ItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            Fixtures.Common.Fake.IslandMarketSource, 
            Fixtures.Common.Fake.IslandSingleMarketSink,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            itemSingleSource,
            Fixtures.Common.Stub.ItemSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource) :> World.SellItemsContext
    input 
    |> World.SellItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message when the avatar has no items in inventory and specifies maximum.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
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
        Fixtures.Common.Stub.ItemSource()
        |> Map.tryFind x
    let context = 
        TestWorldSellItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            Fixtures.Common.Fake.IslandMarketSource, 
            Fixtures.Common.Fake.IslandSingleMarketSink,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            itemSingleSource,
            Fixtures.Common.Stub.ItemSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource) :> World.SellItemsContext
    input 
    |> World.SellItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message and completes the sale when the avatar has sufficient quantity.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
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
        Fixtures.Common.Stub.ItemSource()
        |> Map.tryFind x
    let islandSingleMarketSource (_) (item:uint64) =
        islandMarketSource()
        |> Map.tryFind item
    let context = 
        TestWorldSellItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            islandMarketSource, 
            islandSingleMarketSink ,
            islandSingleMarketSource ,
            islandSource,
            itemSingleSource,
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource) :> World.SellItemsContext
    input 
    |> World.SellItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``SellItems.It gives a message and completes the sale when the avatar has sufficient quantity and specified a maximum sell.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
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
        Fixtures.Common.Stub.ItemSource()
        |> Map.tryFind x
    let islandSingleMarketSource (_) (item:uint64) =
        islandMarketSource()
        |> Map.tryFind item
    let context = 
        TestWorldSellItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            islandMarketSource, 
            islandSingleMarketSink ,
            islandSingleMarketSource ,
            islandSource,
            itemSingleSource,
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource) :> World.SellItemsContext
    input 
    |> World.SellItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName



