module IslandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open IslandTestFixtures
open System

type TestIslandGetDisplayNameContext
        (avatarIslandSingleMetricSource, 
        islandSingleNameSource)=
    interface AvatarIslandMetric.GetContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
    interface IslandName.GetNameContext with
        member _.islandSingleNameSource         : IslandSingleNameSource = islandSingleNameSource

type TestIslandGenerateCommoditiesContext(commoditySource, islandMarketSink, islandMarketSource) =
    interface Utility.RandomContext with
        member _.random : Random = random
    interface Island.GenerateCommoditiesContext with
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
    interface Commodity.GetCommoditiesContext with
        member this.commoditySource: CommoditySource = commoditySource

type TestIslandGenerateItemsContext(islandItemSink, islandItemSource, itemSource, random) =
    interface Utility.RandomContext with
        member this.random: Random = random
    interface Island.GenerateItemsContext with
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.itemSource: ItemSource = itemSource

type TestIslandUpdateMarketForItemSaleContext(commoditySource, islandSingleMarketSink, islandSingleMarketSource) =
    interface Island.UpdateMarketForItemContext
    interface Commodity.GetCommoditiesContext with
        member this.commoditySource: CommoditySource = commoditySource
    interface Island.ChangeMarketContext with
        member this.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member this.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource

[<Test>]
let ``GetDisplayName.It returns (unknown) when there is no visit count.`` () =
    let inputLocation = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandSingleNameSource (_) =
        "island name" |> Some
    let context = 
        TestIslandGetDisplayNameContext
            (avatarIslandSingleMetricSource, 
            islandSingleNameSource) :> ServiceContext
    let actual = 
        inputLocation
        |> IslandName.GetDisplayName 
            context
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual("(unknown)", actual)

[<Test>]
let ``GetDisplayName.It returns the island's name when there is a visit count.`` () =
    let name = "Uno"
    let inputLocation = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            0UL |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandSingleNameSource (location:Location) =
        if location = inputLocation then
            name
            |> Some
        else
            None
    let context = 
        TestIslandGetDisplayNameContext
            (avatarIslandSingleMetricSource, 
            islandSingleNameSource) 
            :> ServiceContext
    let actual = 
        IslandName.GetDisplayName 
            context
            Fixtures.Common.Dummy.AvatarId
            inputLocation
    Assert.AreEqual(name, actual)

type TestIslandJobsGenerationContext
        (islandJobSink             : IslandJobSink, 
        islandJobSource            : IslandJobSource,
        termListSource : TermListSource,
        worldSingleStatisticSource : WorldSingleStatisticSource) =
    interface IslandJob.AddContext with
        member _.islandJobSink   : IslandJobSink = islandJobSink

    interface IslandJob.GetContext with
        member _.islandJobSource : IslandJobSource = islandJobSource

    interface Utility.RandomContext with
        member _.random : Random = random

    interface Job.CreateContext with
        member _.termListSource : TermListSource = termListSource
        member this.jobRewardStatisticSource: JobRewardStatisticSource = fun () -> worldSingleStatisticSource WorldStatisticIdentifier.JobReward

[<Test>]
let ``GenerateJob.It generates a job when no job is present on the island.`` () =
    let inputLocation = (0.0, 0.0)
    let mutable sinkCalled = false
    let islandJobSink (_) (_) =
        sinkCalled<-true
    let islandJobSource (_) =
        []
    let context : IslandJob.AddContext =
        TestIslandJobsGenerationContext
            (islandJobSink,
            islandJobSource,
            Fixtures.Common.Stub.TermListSource,
            Fixtures.Common.Stub.WorldSingleStatisticSource) 
        :> IslandJob.AddContext
    inputLocation
    |> IslandJob.Generate 
        context
        singleDestination
    Assert.IsTrue(sinkCalled)

[<Test>]
let ``GenerateJob.It does nothing when no job is present on the island and no potential job destinations are given.`` () =
    let inputLocation = (0.0, 0.0)
    let islandJobSink (_) (_) =
        Assert.Fail("islandJobSink")
    let islandJobSource (_) =
        [
            {
                FlavorText=""
                Reward=0.0
                Destination=(0.0, 0.0)
            }
        ]
    let context : IslandJob.AddContext =
        TestIslandJobsGenerationContext
            (islandJobSink,
            islandJobSource,
            Fixtures.Common.Stub.TermListSource,
            Fixtures.Common.Stub.WorldSingleStatisticSource) 
        :> IslandJob.AddContext
    inputLocation
    |> IslandJob.Generate 
        context 
        Set.empty


[<Test>]
let ``GenerateCommodities.It does nothing when commodities already exists for the given island.`` () =
    let input = (0.0, 0.0)
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL { Supply=1.0; Demand=1.0 }
    let islandMarketSink (_) (_) =
        Assert.Fail("This should not be called.")
    let context = TestIslandGenerateCommoditiesContext(commoditySource, islandMarketSink, islandMarketSource) :> Island.GenerateCommoditiesContext
    input
    |> Island.GenerateCommodities 
        context

[<Test>]
let ``GenerateCommodities.It generates commodities when the given island has no commodities.`` () =
    let input = (0.0, 0.0)
    let islandMarketSource (_) =
        Map.empty
    let islandMarketSink (_) (markets:Map<uint64, Market>) =
        Assert.AreEqual(commodities.Count, markets.Count)
    let context = TestIslandGenerateCommoditiesContext(commoditySource, islandMarketSink, islandMarketSource) :> Island.GenerateCommoditiesContext
    input
    |> Island.GenerateCommodities 
        context

[<Test>]
let ``GenerateItems.It has no effect when the given island already has items in the shop.`` () =
    let input = (0.0,0.0)
    let islandItemSource (_:Location) = [1UL] |> Set.ofList
    let islandItemSink (_) (_) =
        Assert.Fail("This should not be called")
    let context = 
        TestIslandGenerateItemsContext
            (
                islandItemSink,
                islandItemSource ,
                itemSource,
                random 
            ) :> Island.GenerateItemsContext
    input
    |> Island.GenerateItems 
        context


[<Test>]
let ``GenerateItems.It generates the shop when the given island has no items in the shop.`` () =
    let mutable counter = 0
    let input = (0.0,0.0)
    let islandItemSource (_:Location) = Set.empty
    let islandItemSink (_) (_) =
        counter <- counter + 1
    let context = 
        TestIslandGenerateItemsContext
            (
                islandItemSink,
                islandItemSource ,
                itemSource,
                random 
            ) :> Island.GenerateItemsContext
    input
    |> Island.GenerateItems 
        context
    Assert.AreEqual(1,counter)

[<Test>]
let ``UpdateMarketForItemSale.It updates market commodity demands based on the given number of units sold.`` () =
    let input = (0.0, 0.0)
    let inputQuantity = 1UL
    let inputDescriptor = items.[1UL]
    let market = {Supply=1.0;Demand=1.0}
    let expectedMarket = 
        {Supply=1.0;Demand=market.Demand+1.0}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(expectedMarket, market)
    let islandSingleMarketSource (_) (_) =
        None
    let context = 
        TestIslandUpdateMarketForItemSaleContext
            (commoditySource, 
            islandSingleMarketSink, 
            islandSingleMarketSource) 
        :> Island.UpdateMarketForItemContext
    input
    |> Island.UpdateMarketForItemSale 
        context
        inputDescriptor 
        inputQuantity

[<Test>]
let ``UpdateMarketForItemPurchase.It updates market commodity supply based on the given number of units purchased.`` () =
    let input = (0.0, 0.0)
    let inputQuantity = 1UL
    let inputDescriptor = items.[1UL]
    let market = {Supply=1.0; Demand=1.0}
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL market
    let islandSingleMarketSource location commodity =
        islandMarketSource location
        |> Map.tryFind commodity

    let expectedMarket = 
        {market with
            Supply = 2.0}
    let islandMarketSink (_) (commodityId: uint64,market:Market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(expectedMarket, market)
    let context = 
        TestIslandUpdateMarketForItemSaleContext
            (commoditySource, 
            islandMarketSink, 
            islandSingleMarketSource) 
        :> Island.UpdateMarketForItemContext
    input
    |> Island.UpdateMarketForItemPurchase 
        context
        inputDescriptor 
        inputQuantity

type TestIslandCreateContext
        (islandSingleStatisticSink : IslandSingleStatisticSink,
        islandStatisticTemplateSource : IslandStatisticTemplateSource)=
    interface Island.CreateContext with
        member this.islandStatisticTemplateSource: IslandStatisticTemplateSource = islandStatisticTemplateSource
        member _.islandSingleStatisticSink : IslandSingleStatisticSink = islandSingleStatisticSink

[<Test>]
let ``Create.It sets up statistics for an island.`` () =
    let givenLocation = (1.0, 2.0)
    let mutable statisticCounter : uint64 = 0UL
    let islandSingleStatisticSink (location:Location) (_: IslandStatisticIdentifier, _: Statistic option) =
        statisticCounter <- statisticCounter + 1UL
        Assert.AreEqual(givenLocation, location)
    let islandStatisticTemplateSource () =
        Map.empty
        |> Map.add 
            IslandStatisticIdentifier.CareenDistance
            {
                StatisticTemplate.StatisticName="careen distance"
                MinimumValue = 0.0
                MaximumValue = 100.0
                CurrentValue = 50.0
            }
    let context : Island.CreateContext =
        TestIslandCreateContext
            (islandSingleStatisticSink,
            islandStatisticTemplateSource) 
        :> Island.CreateContext
    Island.Create context givenLocation
    Assert.AreEqual(1UL, statisticCounter)

type TestIslandGetStatisticContext
        (islandSingleStatisticSource) =
    interface Island.GetStatisticContext with
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource

[<Test>]
let ``Get Statistic.It returns None when the statistic does not exist or the island does not exist.`` () =
    let givenLocation = Fixtures.Common.Dummy.IslandLocation
    let mutable called = false
    let islandSingleStatisticSource (_) (_) =
        called <- true
        None
    let context = 
        TestIslandGetStatisticContext
            (islandSingleStatisticSource) :> Island.GetStatisticContext
    let expected : Statistic option = None
    let actual =
        Island.GetStatistic
            context
            IslandStatisticIdentifier.CareenDistance
            givenLocation
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)

type TestIslandGetListContext(islandSource) =
    interface Island.GetListContext with
        member this.islandSource: IslandSource = islandSource
[<Test>]
let ``GetList.It calls the IslandSource in the ServiceContext.`` () =
    let mutable called = false
    let islandSource() =
        called <- true
        []
    let context = TestIslandGetListContext(islandSource) :> ServiceContext
    let expected = []
    let actual = Island.GetList context
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)


type TestIslandGetJobsContext(islandJobSource) =
    interface IslandJob.GetContext with
        member this.islandJobSource: IslandJobSource = islandJobSource
[<Test>]
let ``GetJobs.It calls the IslandJobSource in the ServiceContext.`` () =
    let mutable called = false
    let islandJobSource (_) =
        called <- true
        []
    let context = TestIslandGetJobsContext(islandJobSource) :> ServiceContext
    let expected = []
    let actual = IslandJob.Get context Fixtures.Common.Dummy.IslandLocation
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)

type TestIslandGetItemsContext(islandItemSource) =
    interface Island.GetItemsContext with
        member this.islandItemSource: IslandItemSource = islandItemSource
[<Test>]
let ``GetItems.It calls the IslandItemSource in the ServiceContext.`` () =
    let mutable called = false
    let islandItemSource (_) =
        called <- true
        Set.empty
    let context = TestIslandGetItemsContext(islandItemSource) :> ServiceContext
    let expected = Set.empty
    let actual = Island.GetItems context Fixtures.Common.Dummy.IslandLocation
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
    
type TestIslandGetNameContext(islandSingleNameSource) =
    interface IslandName.GetNameContext with
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
[<Test>]
let ``GetName.It calls the IslandSingleNameSource in the ServiceContext.`` () =
    let mutable called = false
    let islandSingleNameSource (_) =
        called <- true
        None
    let context = TestIslandGetNameContext(islandSingleNameSource) :> ServiceContext
    let expected = None
    let actual = IslandName.GetName context Fixtures.Common.Dummy.IslandLocation
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)

type TestIslandHasFeatureContext(islandSingleFeatureSource) =
    interface Island.HasFeatureContext with
        member this.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
[<Test>]
let ``HasFeature.It calls the IslandSingleFeatureSource in the ServiceContext.`` () =
    let mutable called = false
    let islandSingleFeatureSource (_) (_) =
        called <- true
        false
    let context = TestIslandHasFeatureContext(islandSingleFeatureSource) :> ServiceContext
    let expected = false
    let actual = Island.HasFeature context IslandFeatureIdentifier.Dock Fixtures.Common.Dummy.IslandLocation
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)

type TestIslandGetFeaturesContext(islandFeatureSource) =
    interface Island.GetFeaturesContext with
        member this.islandFeatureSource: IslandFeatureSource = islandFeatureSource
[<Test>]
let ``GetFeatures.It calls the IslandSingleFeatureSource in the ServiceContext.`` () =
    let mutable called = false
    let islandFeatureSource (_) =
        called <- true
        []
    let context = TestIslandGetFeaturesContext(islandFeatureSource) :> ServiceContext
    let expected = []
    let actual = Island.GetFeatures context Fixtures.Common.Dummy.IslandLocation
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
