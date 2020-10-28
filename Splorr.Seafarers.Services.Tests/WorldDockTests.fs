module WorldDockTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common
let private CommonDockTestSetupAndValidation 
        (setups : Contexts.TestContext -> unit)
        (location : Location, avatarId: string)
        : unit =
    let context = Contexts.TestContext()
    setups context
    World.Dock
        context
        location
        avatarId

[<Test>]
let ``Dock.It gives a message when the island does not exist.`` () =
    let calledGetIslandList = ref false
    let calledAvatarAddMessage = ref false
    CommonDockTestSetupAndValidation
        (fun context -> 
            (context :> Island.GetListContext).islandSource := Spies.Source(calledGetIslandList, Dummies.ValidIslandList)
            (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAvatarAddMessage))
        (Dummies.InvalidIslandLocation, Dummies.ValidAvatarId)
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAvatarAddMessage.Value)

let private CommonDockBigTestSetupAndValidation 
        (setups : Contexts.TestContext -> unit)
        (location : Location, avatarId: string)
        : unit =
    let calledGetIslandList = ref false
    let calledAvatarAddMessage = ref false
    let calledGetEpochSeconds = ref false
    let calledSetAvatarIslandFeature = ref false
    let callsForGetAvatarIslandMetric = ref 0UL
    let callsForSetAvatarIslandMetrics = ref 0UL
    CommonDockTestSetupAndValidation
        (fun context -> 
            (context :> Island.GetListContext).islandSource := Spies.Source(calledGetIslandList, Dummies.ValidIslandList)
            (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAvatarAddMessage)
            (context :> IslandVisit.EpochSecondsSourceContext).epochSecondsSource := Spies.Source(calledGetEpochSeconds, 2000UL)
            (context :> AvatarIslandFeature.SetFeatureContext).avatarIslandFeatureSink := 
                Spies.Expect(calledSetAvatarIslandFeature, 
                    (Some {featureId = IslandFeatureIdentifier.Dock; location = Dummies.ValidIslandLocation}, Dummies.ValidAvatarId))
            (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource := 
                Spies.SourceTable(callsForGetAvatarIslandMetric, 
                    Map.empty
                    |> Map.add (Dummies.ValidAvatarId, Dummies.ValidIslandLocation, AvatarIslandMetricIdentifier.VisitCount) (Some 1UL)
                    |> Map.add (Dummies.ValidAvatarId, Dummies.ValidIslandLocation, AvatarIslandMetricIdentifier.LastVisit) (Some 1000UL))
            (context :> AvatarIslandMetric.PutContext).avatarIslandSingleMetricSink :=
                Spies.SinkCounter(callsForSetAvatarIslandMetrics)
            setups context)
        (location, avatarId)
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAvatarAddMessage.Value)
    Assert.IsTrue(calledGetEpochSeconds.Value)
    Assert.IsTrue(calledSetAvatarIslandFeature.Value)
    Assert.AreEqual(4UL, callsForGetAvatarIslandMetric.Value)
    Assert.AreEqual(2UL, callsForSetAvatarIslandMetrics.Value)

[<Test>]
let ``Dock.When the island exists generates a new island job when the island has no jobs and generates markets when the island has no markets and generates island items when the island has no items and completes the avatar job when this island is the destination.`` () =
    let calledGetIslandJobs = ref false
    let calledGetIslandCommodities = ref false
    let calledGetIslandItems = ref false
    let calledGetAvatarJob = ref false
    let calledGetGlobalCommodities = ref false
    let calledPutIslandCommodities = ref false
    let calledGetItemList = ref false
    let calledPutIslandItems = ref false
    let calledGetShipmateStatistic = ref false
    let calledGetAvatarMetric = ref false
    let calledSetAvatarJob = ref false
    let callsForPutShipmateStatistic = ref 0UL
    let callsForSetAvatarMetric = ref 0UL
    let calledGetTerms = ref false
    let calledCreateJob = ref false
    let calledAddIslandJob = ref false
    CommonDockBigTestSetupAndValidation
        (fun context -> 
            (context :> Utility.RandomContext).random := System.Random(0)
            (context :> Island.PutCommoditiesContext).islandMarketSink :=
                Spies.Expect(calledPutIslandCommodities, 
                    (Dummies.ValidIslandLocation, 
                        Map.empty
                        |> Map.add 0UL { Supply = 13.717546202110848; Demand = 11.354725073256867 }))
            (context :> Item.GetListContext).itemSource :=
                Spies.Source(calledGetItemList, Dummies.ValidItemTable)
            (context :> Commodity.GetCommoditiesContext).commoditySource := 
                Spies.Source(calledGetGlobalCommodities, Dummies.ValidCommodityTable)
            (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetShipmateStatistic, Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
            (context :> ShipmateStatistic.PutContext).shipmateSingleStatisticSink := 
                Spies.ExpectSet(callsForPutShipmateStatistic, 
                    Set.empty
                    |> Set.add (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Reputation, Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=51.0})
                    |> Set.add (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Money, Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=52.0}))
            (context :> AvatarMetric.GetMetricContext).avatarSingleMetricSource := Spies.Source(calledGetAvatarMetric, 5UL)
            (context :> AvatarMetric.SetMetricContext).avatarSingleMetricSink := 
                Spies.ExpectSet(callsForSetAvatarMetric, 
                    Set.empty
                    |> Set.add (Dummies.ValidAvatarId, Metric.CompletedJob, 6UL))
            (context :> Avatar.SetJobContext).avatarJobSink := Spies.Expect(calledSetAvatarJob, (Dummies.ValidAvatarId, None))
            (context :> Island.PutIslandItemsContext).islandItemSink :=
                Spies.Expect(calledPutIslandItems, (Dummies.ValidIslandLocation,[0UL]|>Set.ofList))
            (context :> Island.GetCommoditiesContext).islandMarketSource := 
                Spies.Source(calledGetIslandCommodities, Map.empty)
            (context :> Island.GetIslandItemsContext).islandItemSource := 
                Spies.Source(calledGetIslandItems, 
                    Set.empty)
            (context :> IslandJob.GetContext).islandJobSource := Spies.Source(calledGetIslandJobs, [])
            (context :> AvatarJob.GetContext).avatarJobSource := Spies.Source(calledGetAvatarJob, Some { Destination = Dummies.ValidIslandLocation; FlavorText=""; Reward = 2.0})
            (context :> Utility.GetTermsContext).termListSource := Spies.Source(calledGetTerms, ["term"])
            (context :> Job.CreateContext).jobRewardStatisticSource := Spies.Source(calledCreateJob, {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.0})
            (context :> IslandJob.AddContext).islandJobSink := Spies.Sink(calledAddIslandJob))
        (Dummies.ValidIslandLocation, Dummies.ValidAvatarId)
    Assert.IsTrue(calledGetAvatarJob.Value)
    Assert.IsTrue(calledGetIslandJobs.Value)
    Assert.IsTrue(calledGetIslandItems.Value)
    Assert.IsTrue(calledGetIslandCommodities.Value)
    Assert.IsTrue(calledGetGlobalCommodities.Value)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledPutIslandItems.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)
    Assert.IsTrue(calledGetAvatarMetric.Value)
    Assert.IsTrue(calledSetAvatarJob.Value)
    Assert.AreEqual(2UL, callsForPutShipmateStatistic.Value)
    Assert.AreEqual(1UL, callsForSetAvatarMetric.Value)
    Assert.IsTrue(calledGetTerms.Value)
    Assert.IsTrue(calledCreateJob.Value)
    Assert.IsTrue(calledAddIslandJob.Value)
    Assert.IsTrue(calledPutIslandCommodities.Value)

[<Test>]
let ``Dock.When the island exists does nothing to island jobs when there is one already and does nothing to markets when they exist and does nothing to items when they have been generated and does nothing to avatar job when this island is not the destination.`` () =
    let calledGetIslandJobs = ref false
    let calledGetIslandCommodities = ref false
    let calledGetIslandItems = ref false
    let calledGetAvatarJob = ref false
    CommonDockBigTestSetupAndValidation
        (fun context -> 
            (context :> IslandJob.GetContext).islandJobSource := Spies.Source(calledGetIslandJobs, [{Destination=Dummies.ValidIslandLocation; FlavorText=""; Reward=3.0}])
            (context :> Island.GetCommoditiesContext).islandMarketSource := 
                Spies.Source(calledGetIslandCommodities, Dummies.ValidMarketTable)
            (context :> Island.GetIslandItemsContext).islandItemSource := 
                Spies.Source(calledGetIslandItems, 
                    [0UL] |> Set.ofList)
            (context :> AvatarJob.GetContext).avatarJobSource := Spies.Source(calledGetAvatarJob, None))
        (Dummies.ValidIslandLocation, Dummies.ValidAvatarId)
    Assert.IsTrue(calledGetAvatarJob.Value)
    Assert.IsTrue(calledGetIslandJobs.Value)
    Assert.IsTrue(calledGetIslandItems.Value)
    Assert.IsTrue(calledGetIslandCommodities.Value)
    

