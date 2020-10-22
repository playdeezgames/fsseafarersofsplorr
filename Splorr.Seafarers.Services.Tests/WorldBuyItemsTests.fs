module WorldBuyItemsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``BuyItems.It adds a message when the island does not exist.`` () = 
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, [])
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    World.BuyItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)


[<Test>]
let ``BuyItems.It adds a message when the island exists but the item name is not traded there.`` () = 
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetItemList = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Map.empty)
    World.BuyItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetItemList.Value)


[<Test>]
let ``BuyItems.It adds a messages when the island and item exist but the avatar has insufficient funds.`` () = 
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetItemList = ref false
    let calledGetItem = ref false
    let calledGetCommodities = ref false
    let calledGetMarket = ref false
    let calledGetVesselStatistic = ref false
    let calledGetInventory = ref false
    let calledGetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Dummies.ValidItemTable)
    (context :> Item.GetContext).itemSingleSource := Spies.Source(calledGetItem, Some Dummies.ValidItemDescription)
    (context :> Commodity.GetCommoditiesContext).commoditySource := Spies.Source(calledGetCommodities, Dummies.ValidCommodityTable)
    (context :> IslandMarket.DeterminePriceContext).islandMarketSource := Spies.Source(calledGetMarket, Dummies.ValidMarketTable)
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource := Spies.Source(calledGetVesselStatistic, Some {MaximumValue=1000000.0;MinimumValue=0.0;CurrentValue=0.0})
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetInventory, Map.empty)
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetShipmateStatistic, Some {MaximumValue=1000000.0;MinimumValue=0.0;CurrentValue=0.0})
    World.BuyItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledGetItem.Value)
    Assert.IsTrue(calledGetCommodities.Value)
    Assert.IsTrue(calledGetMarket.Value)
    Assert.IsTrue(calledGetVesselStatistic.Value)
    Assert.IsTrue(calledGetInventory.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)

[<Test>]
let ``BuyItems.It adds a messages when the island and item exist but the avatar has insufficient tonnage.`` () = 
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetItemList = ref false
    let calledGetItem = ref false
    let calledGetCommodities = ref false
    let calledGetMarket = ref false
    let calledGetVesselStatistic = ref false
    let calledGetInventory = ref false
    let calledGetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Dummies.ValidItemTable)
    (context :> Item.GetContext).itemSingleSource := Spies.Source(calledGetItem, Some Dummies.ValidItemDescription)
    (context :> Commodity.GetCommoditiesContext).commoditySource := Spies.Source(calledGetCommodities, Dummies.ValidCommodityTable)
    (context :> IslandMarket.DeterminePriceContext).islandMarketSource := Spies.Source(calledGetMarket, Dummies.ValidMarketTable)
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource := Spies.Source(calledGetVesselStatistic, Some {MaximumValue=0.0;MinimumValue=0.0;CurrentValue=0.0})
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetInventory, Map.empty)
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetShipmateStatistic, Some {MaximumValue=1000000.0;MinimumValue=0.0;CurrentValue=1000.0})
    World.BuyItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledGetItem.Value)
    Assert.IsTrue(calledGetCommodities.Value)
    Assert.IsTrue(calledGetMarket.Value)
    Assert.IsTrue(calledGetVesselStatistic.Value)
    Assert.IsTrue(calledGetInventory.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)


[<Test>]
let ``BuyItems.It adds a message when the island and item exist but a zero quantity has been calculated.`` () = 
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetItemList = ref false
    let calledGetItem = ref false
    let calledGetCommodities = ref false
    let calledGetMarket = ref false
    let calledGetVesselStatistic = ref false
    let calledGetInventory = ref false
    let calledGetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Dummies.ValidItemTable)
    (context :> Item.GetContext).itemSingleSource := Spies.Source(calledGetItem, Some Dummies.ValidItemDescription)
    (context :> Commodity.GetCommoditiesContext).commoditySource := Spies.Source(calledGetCommodities, Dummies.ValidCommodityTable)
    (context :> IslandMarket.DeterminePriceContext).islandMarketSource := Spies.Source(calledGetMarket, Dummies.ValidMarketTable)
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource := Spies.Source(calledGetVesselStatistic, Some {MaximumValue=1000.0;MinimumValue=0.0;CurrentValue=0.0})
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetInventory, Map.empty)
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetShipmateStatistic, Some {MaximumValue=1000000.0;MinimumValue=0.0;CurrentValue=0.0})
    World.BuyItems
        context
        Dummies.ValidIslandLocation
        TradeQuantity.Maximum
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledGetItem.Value)
    Assert.IsTrue(calledGetCommodities.Value)
    Assert.IsTrue(calledGetMarket.Value)
    Assert.IsTrue(calledGetVesselStatistic.Value)
    Assert.IsTrue(calledGetInventory.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)


[<Test>]
let ``BuyItems.It will complete the purchase when the island and item exist and the avatar has sufficient funds and tonnage available.`` () = 
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetItemList = ref false
    let calledGetItem = ref false
    let calledGetCommodities = ref false
    let calledGetMarket = ref false
    let calledGetVesselStatistic = ref false
    let calledGetInventory = ref false
    let calledGetShipmateStatistic = ref false
    let calledGetIslandMarket = ref false
    let calledPutislandMarket = ref false
    let calledPutShipmateStatistic = ref false
    let calledSetInventory = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Dummies.ValidItemTable)
    (context :> Item.GetContext).itemSingleSource := Spies.Source(calledGetItem, Some Dummies.ValidItemDescription)
    (context :> Commodity.GetCommoditiesContext).commoditySource := Spies.Source(calledGetCommodities, Dummies.ValidCommodityTable)
    (context :> IslandMarket.DeterminePriceContext).islandMarketSource := Spies.Source(calledGetMarket, Dummies.ValidMarketTable)
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource := Spies.Source(calledGetVesselStatistic, Some {MaximumValue=10.0;MinimumValue=10.0;CurrentValue=10.0})
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetInventory, Map.empty)
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetShipmateStatistic, Some {MaximumValue=1000000.0;MinimumValue=0.0;CurrentValue=1000.0})
    (context :> Island.GetIslandMarketContext).islandSingleMarketSource := Spies.Source(calledGetIslandMarket, Some Dummies.ValidMarket)
    (context :> Island.PutIslandMarketContext).islandSingleMarketSink := Spies.Expect(calledPutislandMarket, (Dummies.ValidIslandLocation, Dummies.ValidCommodityId, {Supply=2.0;Demand=3.015}))
    (context :> ShipmateStatistic.PutContext).shipmateSingleStatisticSink := Spies.Expect(calledPutShipmateStatistic, (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Money, Some {MaximumValue=1000000.0;MinimumValue=0.0;CurrentValue=988.75}))
    (context :> AvatarInventory.SetInventoryContext).avatarInventorySink := Spies.Expect(calledSetInventory, (Dummies.ValidAvatarId, Map.empty |> Map.add Dummies.ValidItemId 1UL))
    World.BuyItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledGetItem.Value)
    Assert.IsTrue(calledGetCommodities.Value)
    Assert.IsTrue(calledGetMarket.Value)
    Assert.IsTrue(calledGetVesselStatistic.Value)
    Assert.IsTrue(calledGetInventory.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)
    Assert.IsTrue(calledGetIslandMarket.Value)
    Assert.IsTrue(calledPutislandMarket.Value)
    Assert.IsTrue(calledPutShipmateStatistic.Value)
    Assert.IsTrue(calledSetInventory.Value)

