module DockedTests

open System
open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open DockedTestFixtures
open Splorr.Seafarers.Services

type TestDockedRunContext
        (
            avatarInventorySink,
            avatarInventorySource,
            avatarIslandFeatureSink, 
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            avatarMessagePurger,
            avatarMessageSink,
            avatarMessageSource,
            avatarSingleMetricSink,
            avatarSingleMetricSource,
            commoditySource,
            islandFeatureSource,
            islandJobPurger,
            islandMarketSource,
            islandSingleFeatureSource,
            islandSingleJobSource,
            islandSingleMarketSink,
            islandSingleNameSource,
            islandSingleMarketSource,
            islandSource,
            itemSingleSource,
            itemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSource   
        ) =
    interface AvatarMessages.GetContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource

    interface Island.GetListContext with
        member this.islandSource: IslandSource = islandSource

    interface Shipmate.GetStatusContext with
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Island.GetNameContext with
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource

    interface Island.ChangeMarketContext with
        member this.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member this.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource
    
    interface AvatarMetric.GetForIslandContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource

    interface Island.GetFeaturesContext with
        member _.islandFeatureSource            : IslandFeatureSource            = islandFeatureSource
       
    interface AvatarMessages.AddContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        
    interface World.AddMessagesContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        
    interface World.UndockContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink
        member _.avatarIslandFeatureSink : AvatarIslandFeatureSink = avatarIslandFeatureSink

    interface IslandMarket.DeterminePriceContext with
        member _.islandMarketSource             : IslandMarketSource            =islandMarketSource   
        member _.itemSingleSource               : ItemSingleSource              = itemSingleSource

    interface Island.MakeKnownContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
    
    interface Commodity.GetCommoditiesContext with
        member _.commoditySource: CommoditySource = commoditySource

    interface Island.UpdateMarketForItemContext

    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Avatar.RemoveInventoryContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource

    interface AvatarMetric.AddContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource

    interface Avatar.GetUsedTonnageContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarJob.AbandonContext with
        member _.avatarJobSink                  : AvatarJobSink                 =avatarJobSink                 
        member _.avatarJobSource                : AvatarJobSource               =avatarJobSource
    interface World.ClearMessagesContext with
        member _.avatarMessagePurger : AvatarMessagePurger = avatarMessagePurger
        
    interface Avatar.GetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface Avatar.AddInventoryContext with
        member _.avatarInventorySink   : AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface World.AcceptJobContext with
        member _.avatarJobSink         : AvatarJobSink = avatarJobSink
        member _.avatarJobSource       : AvatarJobSource = avatarJobSource
        member _.islandJobPurger       : IslandJobPurger = islandJobPurger
        member _.islandSingleJobSource : IslandSingleJobSource = islandSingleJobSource
        member _.islandSource          : IslandSource = islandSource
    interface World.AbandonJobContext with
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
    interface World.BuyItemsContext with
        member _.islandSource                  : IslandSource = islandSource
        member _.itemSource                    : ItemSource =  itemSource
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource
    interface World.SellItemsContext with
        member _.islandSource                  : IslandSource = islandSource
        member _.itemSource                    : ItemSource = itemSource
    interface Avatar.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarIslandFeature.EnterContext with
        member this.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
        member this.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink

let private functionUnderTest
        (avatarInventorySink           : AvatarInventorySink)
        (avatarInventorySource         : AvatarInventorySource)
        (avatarIslandFeatureSink       : AvatarIslandFeatureSink)
        (avatarMessageSink             : AvatarMessageSink)
        (avatarSingleMetricSink        : AvatarSingleMetricSink)
        (islandMarketSource            : IslandMarketSource) 
        (islandSingleFeatureSource     : IslandSingleFeatureSource)
        (islandSingleNameSource        : IslandSingleNameSource)
        (islandSingleMarketSink        : IslandSingleMarketSink) 
        (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
        =
    let context =
        TestDockedRunContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarIslandFeatureSink,
            avatarIslandSingleMetricSinkStub,
            avatarIslandSingleMetricSourceStub,
            avatarJobSinkStub,
            avatarJobSourceStub,
            avatarMessagePurgerStub,
            avatarMessageSink,
            avatarMessageSourceDummy,
            avatarSingleMetricSink,
            avatarSingleMetricSourceStub,
            commoditySource ,
            islandFeatureSourceStub,
            islandJobPurgerStub,
            islandMarketSource ,
            islandSingleFeatureSource,
            islandSingleJobSourceStub,
            islandSingleMarketSink,
            islandSingleNameSource,
            dockedItemSingleMarketSourceStub,
            islandSourceStub,
            (fun x -> itemSource() |> Map.tryFind x),
            itemSource ,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSourceStub) :> ServiceContext
    Docked.Run 
        context

let private functionUnderTestStubbed 
        (avatarIslandFeatureSink       : AvatarIslandFeatureSink)
        (avatarMessageSink             : AvatarMessageSink) 
        (avatarSingleMetricSink        : AvatarSingleMetricSink)
        (islandSingleFeatureSource     : IslandSingleFeatureSource)
        (islandSingleNameSource        : IslandSingleNameSource)
        (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
        =
    functionUnderTest 
        avatarInventorySinkStub
        avatarInventorySourceStub
        avatarIslandFeatureSink
        avatarMessageSink
        avatarSingleMetricSink
        dockedItemMarketSourceStub 
        islandSingleFeatureSource
        islandSingleNameSource
        dockedItemSingleMarketSinkStub 
        shipmateSingleStatisticSource

[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let input: string = deadDockWorld   
    let inputLocation: Location= deadDockLocation
    let inputSource(): Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let expectedMessages = []
    let expected: Gamestate option =
        expectedMessages
        |> Gamestate.GameOver
        |> Some
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSource - %s"))
    let actual: Gamestate option =
        (inputLocation, input)
        ||> functionUnderTestStubbed
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSourceStub
            shipmateSingleStatisticSource
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given Undock Command.`` () =
    let input = dockWorld
    let inputLocation= dockLocation
    let inputSource = Command.Undock |> Some |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let avatarIslandFeatureSink (feature: AvatarIslandFeature option, _) : unit =
        Assert.AreEqual(None, feature)
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed
            avatarIslandFeatureSink
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = Command.Quit |> Some |> Fixtures.Common.Mock.CommandSource
    let expected =
        input
        |> Gamestate.InPlay 
        |> Gamestate.ConfirmQuit 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected,actual)

[<Test>]
let ``Run.It returns Metrics when given Metrics Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Metrics 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Gamestate.Metrics 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Help when given Help Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Help 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Gamestate.Help 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Inventory when given Inventory Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Inventory 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input 
        |> Gamestate.InPlay 
        |> Gamestate.Inventory 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns InvalidInput when given invalid Command.`` () =
    let input =dockWorld
    let inputLocation = dockLocation
    let inputSource =
        None 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        ("Maybe try 'help'?",input 
        |> Gamestate.InPlay)
        |> Gamestate.ErrorMessage
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy
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
        |> Gamestate.InPlay 
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSourceStub
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected,actual)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Status 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input 
        |> Gamestate.InPlay 
        |> Gamestate.Status 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Docked (at Jobs) gamestate when given the command Jobs.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        Command.Jobs 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Gamestate.Jobs
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Docked (at DarkAlley) gamestate when given the command GoTo DarkAlley.`` () =
    let input = dockWorld
    let inputLocation = dockLocation
    let inputSource = 
        IslandFeatureIdentifier.DarkAlley
        |> Command.GoTo 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let mutable didSetFeature = false
    let avatarIslandFeatureSink (feature:AvatarIslandFeature option,_) =
        didSetFeature<-true
        Assert.AreEqual(IslandFeatureIdentifier.DarkAlley, feature.Value.featureId)
        Assert.AreEqual(inputLocation, feature.Value.location)
    let islandSingleFeatureSource (location) (feature) = 
        Assert.AreEqual(inputLocation, location)
        Assert.AreEqual(IslandFeatureIdentifier.DarkAlley, feature)
        true
    let actual =
        (inputLocation, input)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSink
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSource
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(didSetFeature)


[<Test>]
let ``Run.It gives a message when given the Accept Job command and the given job number does not exist.`` () =
    let input = smallWorldDocked
    let inputLocation = smallWorldIslandLocation
    let inputSource = 0u |> Command.AcceptJob |> Some |> Fixtures.Common.Mock.CommandSource
    let expectedWorld = 
        input
    let expected = 
        expectedWorld 
        |> Gamestate.InPlay 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy 
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
        |> Fixtures.Common.Mock.CommandSource
    let expectedWorld = 
        input
    let expected = 
        expectedWorld
        |> Gamestate.InPlay 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy 
            inputLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let input = abandonJobWorld
    let inputLocation = dockLocation
    let inputSource = Job |> Command.Abandon |> Some |> Fixtures.Common.Mock.CommandSource
    let expectedWorld = 
        input
    let expected = 
        expectedWorld
        |> Gamestate.InPlay 
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        input
        |> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            (assertAvatarSingleMetricSink [Metric.AbandonedJob, 1UL])
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            sinkDummy 
            inputLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ItemList gamestate when given the Items command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = 
        Command.Items 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        inputWorld
        |> Gamestate.InPlay
        |> Gamestate.ItemList
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            avatarMessageSinkStub
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            (sinkDummy)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Buy command for a non-existent item.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (1UL |> Specific, "non existent item") |> Command.Buy |> Some |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["Round these parts, we don't sell things like that."]
    let expectedWorld =
        inputWorld
    let expected = 
        expectedWorld
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            (sinkDummy)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Buy command and the avatar does not have enough money to complete the purchase.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = smallWorldDocked
    let inputSource = (1UL |> Specific, "item under test") |> Command.Buy |> Some |> Fixtures.Common.Mock.CommandSource
    let markets =
        Map.empty
        |> Map.add 1UL {Demand=5.0; Supply=5.0}
    let islandMarketSource (_) = markets
    let islandSingleMarketSink (_) (_) =
        Assert.Fail("This should not be called.")
    let expectedMessages = ["You don't have enough money."]
    let expectedWorld =
        inputWorld
    let expected = 
        expectedWorld
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest 
            avatarInventorySinkStub
            avatarInventorySourceStub
            avatarIslandFeatureSinkDummy
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            islandMarketSource 
            islandSingleFeatureSourceStub
            islandSingleNameSource
            islandSingleMarketSink 
            shipmateSingleStatisticSourceStub
            inputSource 
            (sinkDummy)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and completes the purchase when given the Buy command and the avatar has enough money.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (1UL |> Specific, "item under test") |> Command.Buy |> Some |> Fixtures.Common.Mock.CommandSource
    let markets =
        Map.empty
        |> Map.add 1UL {Demand=5.0; Supply=5.0}
    let islandMarketSource (_) = markets
    let commodities = commoditySource()
    let expectedDemand = 
        markets.[1UL].Demand + commodities.[1UL].SaleFactor
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(markets.[commodityId].Supply, market.Supply)
        Assert.AreEqual(expectedDemand, market.Demand)
    let expectedMessages = ["You complete the purchase of 1 item under test."]
    let expectedWorld =
        inputWorld
    let expected = 
        expectedWorld
        |> Gamestate.InPlay
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
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest 
            avatarInventorySinkStub
            avatarInventorySourceStub
            avatarIslandFeatureSinkDummy
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            islandMarketSource 
            islandSingleFeatureSourceStub
            islandSingleNameSource
            islandSingleMarketSink 
            shipmateSingleStatisticSource
            inputSource 
            (sinkDummy)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Sell command for a non-existent item.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (Specific 1UL, "non existent item") |> Command.Sell |> Some |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["Round these parts, we don't buy things like that."]
    let expectedWorld =
        inputWorld
    let expected = 
        expectedWorld
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            (sinkDummy)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Sell command and the avatar does not sufficient items to sell.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (Specific 1UL, "item under test") |> Command.Sell |> Some |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["You don't have enough of those to sell."]
    let expectedWorld =
        inputWorld
    let expected = 
        expectedWorld
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTestStubbed 
            avatarIslandFeatureSinkDummy
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            islandSingleFeatureSourceStub
            islandSingleNameSource
            shipmateSingleStatisticSourceStub
            inputSource 
            (sinkDummy)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and completes the sale when given the Sell command and the avatar sufficient items to sell.`` () =
    let inputLocation = smallWorldIslandLocation
    let markets =
        Map.empty
        |> Map.add 1UL {Supply = 5.0; Demand =5.0}
    let islandMarketSource (_) = markets
    let inputWorld = 
        shopWorld
    let inputSource = (Specific 1UL, "item under test") |> Command.Sell |> Some |> Fixtures.Common.Mock.CommandSource
    let commodities = commoditySource()
    let expectedSupply = 
        markets.[1UL].Supply + commodities.[1UL].PurchaseFactor
    let expectedMarket = 
        {markets.[1UL] with 
            Supply= expectedSupply}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(expectedMarket, market)
    let expectedMessages = ["You complete the sale of 1 item under test.";"You complete the sale."]
    let expectedWorld =
        inputWorld
    let expected = 
        expectedWorld
        |> Gamestate.InPlay
        |> Some
    let avatarInventorySource (_) = Map.empty |> Map.add 1UL 1UL
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) = Assert.AreEqual(Map.empty, inventory)
    let islandSingleNameSource (_) =
        "yermom"
        |> Some
    let actual =
        (inputLocation, inputWorld)
        ||> functionUnderTest 
            avatarInventorySink
            avatarInventorySource
            avatarIslandFeatureSinkDummy
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            islandMarketSource 
            islandSingleFeatureSourceStub
            islandSingleNameSource
            islandSingleMarketSink 
            shipmateSingleStatisticSourceStub
            inputSource 
            (sinkDummy)
    Assert.AreEqual(expected, actual)

