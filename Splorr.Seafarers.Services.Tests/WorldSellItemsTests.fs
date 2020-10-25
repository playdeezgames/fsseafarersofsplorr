module WorldSellItemsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``SellItems.It gives a message when the island doesn't exist.`` () =
    let calledAddAvatarMessages = ref false
    let calledGetIslandList = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source(calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessages)
    World.SellItems
        context
        Dummies.InvalidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddAvatarMessages.Value)

[<Test>]
let ``SellItems.It gives a message when the item does not exist but the island exists.`` () =
    let calledAddAvatarMessages = ref false
    let calledGetIslandList = ref false
    let calledGetItemList = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source(calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessages)
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Map.empty)
    World.SellItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddAvatarMessages.Value)
    Assert.IsTrue(calledGetItemList.Value);


[<Test>]
let ``SellItems.It gives a message when the island exists and the item is traded at the island and avatar has an insufficient quantity to sell.`` () =
    let calledAddAvatarMessages = ref false
    let calledGetIslandList = ref false
    let calledGetItemList = ref false
    let calledGetAvatarInventory = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source(calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessages)
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Dummies.ValidItemTable)
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetAvatarInventory, Map.empty)
    World.SellItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddAvatarMessages.Value)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledGetAvatarInventory.Value)


[<Test>]
let ``SellItems.It completes the sale when the avatar has a sufficient quantity to sell and the island exists.`` () =
    let calledAddAvatarMessages = ref false
    let calledGetIslandList = ref false
    let calledGetItemList = ref false
    let calledGetAvatarInventory = ref false
    let calledGetItem = ref false
    let calledGetCommodities = ref false
    let calledGetIslandMarket = ref false
    let calledDeterminePriceContext = ref false;
    let calledPutIslandMarket = ref false
    let calledGetShipmateStatistic = ref false
    let calledPutShipmateStatistic = ref false
    let calledSetInventory = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source(calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessages)
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Dummies.ValidItemTable)
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetAvatarInventory, Map.empty |> Map.add Dummies.ValidItemId 1UL)
    (context :> Item.GetContext).itemSingleSource := Spies.Source(calledGetItem, Some Dummies.ValidItemDescription)
    (context :> Commodity.GetCommoditiesContext).commoditySource := Spies.Source(calledGetCommodities, Dummies.ValidCommodityTable)
    (context :> IslandMarket.DeterminePriceContext).islandMarketSource := Spies.Source(calledDeterminePriceContext, Dummies.ValidMarketTable)
    (context :> Island.GetIslandMarketContext).islandSingleMarketSource := Spies.Source(calledGetIslandMarket, Some Dummies.ValidMarket)
    (context :> Island.PutIslandMarketContext).islandSingleMarketSink := Spies.Expect(calledPutIslandMarket, (Dummies.ValidIslandLocation, Dummies.ValidCommodityId, {Supply=2.0075;Demand=3.0}))
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetShipmateStatistic, Some {MaximumValue=1000000.0;MinimumValue=0.0;CurrentValue=1000.0})
    (context :> ShipmateStatistic.PutContext).shipmateSingleStatisticSink := Spies.Expect(calledPutShipmateStatistic, (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Money, Some {MaximumValue=1000000.0;MinimumValue=0.0;CurrentValue=1007.875}))
    (context :> AvatarInventory.SetInventoryContext).avatarInventorySink := Spies.Expect(calledSetInventory, (Dummies.ValidAvatarId, Map.empty))
    World.SellItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddAvatarMessages.Value)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledGetAvatarInventory.Value)
    Assert.IsTrue(calledGetItem.Value)
    Assert.IsTrue(calledGetCommodities.Value)
    Assert.IsTrue(calledGetIslandMarket.Value)
    Assert.IsTrue(calledDeterminePriceContext.Value)
    Assert.IsTrue(calledPutIslandMarket.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)
    Assert.IsTrue(calledPutShipmateStatistic.Value)
    Assert.IsTrue(calledSetInventory.Value)
    


[<Test>]
let ``SellItems.It gives a message when island exists and the item is sold at the island and the avatar sells a zero quantity.`` () =
    let calledAddAvatarMessages = ref false
    let calledGetIslandList = ref false
    let calledGetItemList = ref false
    let calledGetAvatarInventory = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source(calledGetIslandList, Dummies.ValidIslandList)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessages)
    (context :> Item.GetListContext).itemSource := Spies.Source(calledGetItemList, Dummies.ValidItemTable)
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetAvatarInventory, Map.empty |> Map.add Dummies.ValidItemId 1UL)

    World.SellItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 0UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddAvatarMessages.Value)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledGetAvatarInventory.Value)
    
