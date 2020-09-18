module WorldBuyItemsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldBuyItemsContext
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
        shipmateSingleStatisticSource,
        vesselSingleStatisticSource)=
    interface Island.UpdateMarketForItemContext with
        member this.commoditySource: CommoditySource = commoditySource
        member this.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member this.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource
        
    interface AvatarAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        
    interface WorldAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        
    interface AvatarGetUsedTonnageContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        
    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        
    interface AvatarGetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
        
    interface AvatarAddInventoryContext with
        member _.avatarInventorySink   : AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
        
    interface WorldBuyItemsContext with
        member _.islandSource                  : IslandSource = islandSource
        member _.itemSource                    : ItemSource =  itemSource
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource
        member this.commoditySource: CommoditySource = commoditySource
        member this.islandMarketSource: IslandMarketSource = islandMarketSource
        member this.itemSingleSource : ItemSingleSource = itemSingleSource
        
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``BuyItems.It gives a message when given a bogus island location.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let expectedMessage = "You cannot buy items here."
    let shipmateSingleStatisticSource (_) (_) (_) =
        Assert.Fail("kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        Assert.Fail("kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Assert.Fail("avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        Assert.Fail("avatarInventorySink")
        ()
    let islandSource() =
        []
    let context = 
        TestWorldBuyItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            Fixtures.Common.Fake.IslandMarketSource,
            Fixtures.Common.Fake.IslandSingleMarketSink,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            (fun x -> Fixtures.Common.Stub.ItemSource() |> Map.tryFind x),
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            Fixtures.Common.Stub.VesselSingleStatisticSource)
    input 
    |> World.BuyItems
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``BuyItems.It gives a message when given a valid island location and a bogus item to buy.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = 2UL |> Specific
    let inputItemName = "bogus item"
    let expectedMessage = "Round these parts, we don't sell things like that."
    let shipmateSingleStatisticSource (_) (_) (_) =
        Assert.Fail("kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        Assert.Fail("kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Assert.Fail("avatarInventorySource")
        Map.empty
    let avatarInventorySink (_) (_) =
        Assert.Fail("avatarInventorySink")
        ()
    let islandSource() =
        [
            inputLocation
        ]
    let context = 
        TestWorldBuyItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            Fixtures.Common.Fake.IslandMarketSource,
            Fixtures.Common.Fake.IslandSingleMarketSink ,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            (fun x -> Fixtures.Common.Stub.ItemSource() |> Map.tryFind x),
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            Fixtures.Common.Stub.VesselSingleStatisticSource)
    input 
    |> World.BuyItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient funds.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expectedMessage = "You don't have enough money."
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 0.0 |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, _: Statistic option) =
        match identifier with
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let islandSource() =
        [inputLocation]
    let vesselSingleStatisticSource (_) (identifier: VesselStatisticIdentifier) : Statistic option=
        match identifier with
        | VesselStatisticIdentifier.Tonnage ->
            Statistic.Create (0.0,100.0) 0.0
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "vesselSingleStatisticSource - %s")
            None
    let context = 
        TestWorldBuyItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            islandMarketSource,
            Fixtures.Common.Fake.IslandSingleMarketSink ,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            (fun x -> Fixtures.Common.Stub.ItemSource() |> Map.tryFind x),
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSource)
    input 
    |> World.BuyItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient tonnage.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = 1000UL |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expectedMessage = "You don't have enough tonnage."
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 5000.0 |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, _: Statistic option) =
        match identifier with
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s")
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add 1UL 1000UL
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let islandSource() =
        [inputLocation]
    let vesselSingleStatisticSource (_) (identifier: VesselStatisticIdentifier) : Statistic option=
        match identifier with
        | VesselStatisticIdentifier.Tonnage ->
            Statistic.Create (0.0,0.0) 0.0
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "vesselSingleStatisticSource - %s")
            None
    let context = 
        TestWorldBuyItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            islandMarketSource,
            Fixtures.Common.Fake.IslandSingleMarketSink ,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            (fun x -> Fixtures.Common.Stub.ItemSource() |> Map.tryFind x),
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSource)
    input 
    |> World.BuyItems
        context
        inputLocation
        inputQuantity
        inputItemName

[<Test>]
let ``BuyItems.It gives a message and completes the purchase when the avatar has sufficient funds.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = 2UL |> Specific
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expectedMessage = "You complete the purchase of 2 item under test."
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 5000.0 |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(4998.0, statistic.Value.CurrentValue)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty |> Map.add 1UL 2UL, inventory)
    let islandSource() =
        [
            inputLocation
        ]
    let vesselSingleStatisticSource (_) (identifier: VesselStatisticIdentifier) : Statistic option=
        match identifier with
        | VesselStatisticIdentifier.Tonnage ->
            Statistic.Create (0.0,100.0) 100.0
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "vesselSingleStatisticSource - %s")
            None
    let islandSingleMarketSource (_) (item:uint64) =
        islandMarketSource()
        |> Map.tryFind item
    let islandSingleMarketSink (_) (item:uint64, market:Market) =
        Assert.AreEqual(1UL, item)
        Assert.AreEqual(5.0, market.Supply)
        Assert.AreEqual(7.0, market.Demand)
    let context = 
        TestWorldBuyItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            islandMarketSource,
            islandSingleMarketSink ,
            islandSingleMarketSource,
            islandSource,
            (fun x -> Fixtures.Common.Stub.ItemSource() |> Map.tryFind x),
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSource)
    input 
    |> World.BuyItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``BuyItems.It gives a message when the avatar has insufficient funds for a single unit when specifying a maximum buy.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = Maximum
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expectedMessage = "You don't have enough money to buy any of those."
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 0.0 |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, _: Statistic option) =
        match identifier with
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let islandSource() =
        [inputLocation]
    let vesselSingleStatisticSource (_) (identifier: VesselStatisticIdentifier) : Statistic option=
        match identifier with
        | VesselStatisticIdentifier.Tonnage ->
            Statistic.Create (0.0,100.0) 0.0
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "vesselSingleStatisticSource - %s")
            None
    let context = 
        TestWorldBuyItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            islandMarketSource,
            Fixtures.Common.Fake.IslandSingleMarketSink ,
            Fixtures.Common.Fake.IslandSingleMarketSource,
            islandSource,
            (fun x -> Fixtures.Common.Stub.ItemSource() |> Map.tryFind x),
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSource)
    input 
    |> World.BuyItems
        context
        inputLocation 
        inputQuantity 
        inputItemName

[<Test>]
let ``BuyItems.It gives a message indicating purchased quantity and completes the purchase when the avatar has sufficient funds for at least one and has specified a maximum buy.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputQuantity = Maximum
    let inputItemName = "item under test"
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL {Supply=5.0; Demand=5.0}
    let expectedMessage = "You complete the purchase of 100 item under test."
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000000.0) 5000.0 |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSource %s")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(4900.0, statistic.Value.CurrentValue)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "kaboom shipmateSingleStatisticSink %s")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty |> Map.add 1UL 100UL, inventory)
    let islandSource() =
        [inputLocation]
    let vesselSingleStatisticSource (_) (identifier: VesselStatisticIdentifier) : Statistic option=
        match identifier with
        | VesselStatisticIdentifier.Tonnage ->
            Statistic.Create (0.0,100.0) 100.0
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "vesselSingleStatisticSource - %s")
            None
    let islandSingleMarketSource (_) (item:uint64) =
        islandMarketSource()
        |> Map.tryFind item
    let islandSingleMarketSink (_) (item:uint64, market:Market) =
        Assert.AreEqual(1UL, item)
        Assert.AreEqual(5.0, market.Supply)
        Assert.AreEqual(105.0, market.Demand)
    let context = 
        TestWorldBuyItemsContext
            (avatarInventorySink,
            avatarInventorySource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Stub.CommoditySource, 
            islandMarketSource,
            islandSingleMarketSink ,
            islandSingleMarketSource,
            islandSource,
            (fun x -> Fixtures.Common.Stub.ItemSource() |> Map.tryFind x),
            Fixtures.Common.Stub.ItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSource)
    input 
    |> World.BuyItems 
        context
        inputLocation 
        inputQuantity 
        inputItemName


