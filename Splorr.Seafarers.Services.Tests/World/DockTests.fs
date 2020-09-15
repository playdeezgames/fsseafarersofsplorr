module WorldDockTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures

[<Test>]
let ``Dock.It does not modify avatar when given avatar has a job for a different destination.`` () =
    let expectedMessage = "You dock."
    let inputLocation = (0.0, 0.0)
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, _: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        {
            FlavorText=""
            Reward=0.0
            Destination=(99.0, 99.0)
        }
        |> Some
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        ()
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | _ ->
            None
    let islandJobSink (_) (_) =
        Assert.Fail("islandJobSink")
    let islandJobSource (_) =
        []
    let islandSource () =
        [
            inputLocation
        ]
    let avatarIslandFeatureSink (feature: AvatarIslandFeature option, _) =
        Assert.AreEqual(IslandFeatureIdentifier.Dock, feature.Value.featureId)
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.VisitedIsland ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let islandMarketSource (_) =
        Map.empty
    let islandMarketSink (_) (markets)= 
        Assert.AreEqual(1, markets |> Map.count)
    let islandItemSource (_) =
        Set.empty
    let islandItemSink (_) (x:Set<uint64>) = 
        Assert.AreEqual(1, x.Count)
    let context : WorldDockContext =
        TestWorldDockContext
            (avatarIslandFeatureSink,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            (Fixtures.Common.Mock.AvatarSingleMetricSink [Metric.VisitedIsland, 0UL; Metric.VisitedIsland, 1UL]),
            avatarSingleMetricSource,
            Fixtures.Common.Stub.CommoditySource,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandItemSink,
            islandItemSource, 
            islandJobSink,
            islandJobSource,
            islandMarketSink,
            islandMarketSource,
            islandSource,
            Fixtures.Common.Stub.ItemSource ,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            Fixtures.Common.Stub.TermSources ,
            Fixtures.Common.Stub.WorldSingleStatisticSource) :> WorldDockContext
    Fixtures.Common.Dummy.AvatarId
    |> World.Dock
        context
        inputLocation
    |> ignore

[<Test>]
let ``Dock.It adds a message and completes the job when given avatar has a job for this location.`` () =
    let expectedMessages = 
        [
            "You complete your job."
            "You dock."
        ]
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Money
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (-50000.0, 50000.0) 0.0 |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "shipmateSingleStatisticSource %s")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier, statistic with
        | ShipmateStatisticIdentifier.Money, Some stat ->
            Assert.AreEqual(10.0, stat.CurrentValue)
        | ShipmateStatisticIdentifier.Reputation, Some stat ->
            Assert.AreEqual(1.0, stat.CurrentValue)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "shipmateSingleStatisticSink %s")
    let avatarJobSink (_) (job: Job option) =
        Assert.AreEqual(None, job)
    let jobLocation = 
        (0.0, 0.0)
    let avatarJobSource (_) =
        {
            FlavorText=""
            Reward=10.0
            Destination = jobLocation
        } |> Some
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        ()
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | _ ->
            None
    let islandJobSink (_) (_) =
        Assert.Fail("islandJobSink")
    let islandJobSource (_) = 
        []
    let islandSource () =
        [jobLocation]
    let avatarIslandFeatureSink (feature: AvatarIslandFeature option, _) =
        Assert.AreEqual(IslandFeatureIdentifier.Dock, feature.Value.featureId)
    let visitCount = 0UL
    let avatarSingleMetricSource (_) (metric:Metric) : uint64=
        match metric with
        | Metric.CompletedJob
        | Metric.VisitedIsland ->
            0UL
        | _ ->
            Assert.Fail(sprintf "avatarSingleMetricSource - %s" (metric.ToString()))
            0UL
    let avatarSingleMetricSink (_) (metric:Metric, value:uint64) : unit =
        match metric with
        | Metric.VisitedIsland ->
            Assert.AreEqual(visitCount, value) //the fixture has not been visited
        | Metric.CompletedJob ->
            Assert.AreEqual(1UL, value)
        | _ ->
            Assert.Fail(metric.ToString() |> sprintf "avatarSingleMetricSink - %s")
    let islandMarketSource (_) =
        Map.empty
    let islandMarketSink (_) (markets)= 
        Assert.AreEqual(1, markets |> Map.count)
    let islandItemSource (_) =
        Set.empty
    let islandItemSink (_) (x:Set<uint64>) = 
        Assert.AreEqual(1, x.Count)
    let context : WorldDockContext =
        TestWorldDockContext
            (avatarIslandFeatureSink,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            (Fixtures.Common.Mock.AvatarMessagesSink expectedMessages),
            avatarSingleMetricSink,
            avatarSingleMetricSource,
            Fixtures.Common.Stub.CommoditySource ,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandItemSink ,
            islandItemSource,
            islandJobSink,
            islandJobSource,
            islandMarketSink,
            islandMarketSource, 
            islandSource,
            Fixtures.Common.Stub.ItemSource ,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            Fixtures.Common.Stub.TermSources ,
            Fixtures.Common.Stub.WorldSingleStatisticSource)
        :> WorldDockContext
    Fixtures.Common.Dummy.AvatarId
    |> World.Dock
        context
        jobLocation

[<Test>]
let ``Dock.It does nothing when given an invalid avatar id.`` () =
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSink")
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandJobSink (_) (_) =
        Assert.Fail("islandJobSink")
    let islandJobSource (_) =
        Assert.Fail("islandJobSource")
        []
    let islandSource () =
        []
    let avatarIslandFeatureSink (_) =
        raise (System.NotImplementedException "avatarIslandFeatureSink")
    let context : WorldDockContext =
        TestWorldDockContext
            (avatarIslandFeatureSink,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            Fixtures.Common.Mock.AvatarMessageSink "There is no place to dock there.",
            Fixtures.Common.Fake.AvatarSingleMetricSink,
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            Fixtures.Common.Stub.CommoditySource,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            Fixtures.Common.Fake.IslandItemSink,
            Fixtures.Common.Fake.IslandItemSource, 
            islandJobSink,
            islandJobSource,
            Fixtures.Common.Fake.IslandMarketSink,
            Fixtures.Common.Fake.IslandMarketSource, 
            islandSource,
            Fixtures.Common.Stub.ItemSource,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource,
            Fixtures.Common.Stub.TermSources,
            Fixtures.Common.Stub.WorldSingleStatisticSource) :> WorldDockContext
    Fixtures.Common.Dummy.BogusAvatarId
    |> World.Dock 
        context
        (0.0, 0.0)

[<Test>]
let ``Dock.It adds a message when the given location has no island.`` () =
    let inputWorld = Fixtures.Common.Dummy.AvatarId
    let expectedMessage = "There is no place to dock there."
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        Assert.Fail("avatarJobSource")
        None
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSink")
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandJobSink (_) (_) =
        Assert.Fail("islandJobSink")
    let islandJobSource (_) =
        Assert.Fail("islandJobSource")
        []
    let islandSource () =
        []
    let avatarIslandFeatureSink (_) =
        raise (System.NotImplementedException "avatarIslandFeatureSink")
    let context : WorldDockContext =
        TestWorldDockContext
            (avatarIslandFeatureSink,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            Fixtures.Common.Fake.AvatarSingleMetricSink,
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            Fixtures.Common.Stub.CommoditySource,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            Fixtures.Common.Fake.IslandItemSink ,
            Fixtures.Common.Fake.IslandItemSource, 
            islandJobSink,
            islandJobSource,
            Fixtures.Common.Fake.IslandMarketSink ,
            Fixtures.Common.Fake.IslandMarketSource, 
            islandSource,
            Fixtures.Common.Stub.ItemSource ,
            Fixtures.Common.Fake.ShipmateSingleStatisticSink,
            Fixtures.Common.Fake.ShipmateSingleStatisticSource,
            Fixtures.Common.Stub.TermSources ,
            Fixtures.Common.Stub.WorldSingleStatisticSource) :> WorldDockContext
    inputWorld
    |> World.Dock
        context
        (0.0, 0.0)

[<Test>]
let ``Dock.It updates the island's visit count and last visit when the given location has an island.`` (): unit =
    let inputWorld = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let expectedMessage = "You dock."
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, _: Statistic option) =
        match identifier with
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "kaboom get %s"))
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let expectedVisitTime = 
        System.DateTimeOffset.Now.ToUnixTimeSeconds()
        |> uint64
    let avatarIslandSingleMetricSink (_) (_) (identifier:AvatarIslandMetricIdentifier) (value:uint64) =
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(1UL, value)
        | AvatarIslandMetricIdentifier.LastVisit ->
            Assert.GreaterOrEqual(value, expectedVisitTime)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let mutable counter:int = 0
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            counter <- counter + 1
            match counter with
            | 1
            | 2 ->
                None
            | 3 ->
                Some 1UL
            | _ ->
                Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
                None
        | AvatarIslandMetricIdentifier.LastVisit ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandJobSink (_) (_) =
        Assert.Fail("islandJobSink")
    let islandJobSource (_) =
        []
    let islandSource () =
        [inputLocation]
    let avatarIslandFeatureSink (feature: AvatarIslandFeature option, _) =
        Assert.AreEqual(IslandFeatureIdentifier.Dock, feature.Value.featureId)
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.VisitedIsland ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let islandMarketSource (_) =
        Map.empty
    let islandMarketSink (_) (markets)= 
        Assert.AreEqual(0, markets |> Map.count)
    let islandItemSource (_) =
        Set.empty
    let islandItemSink (_) (x:Set<uint64>) = 
        Assert.AreEqual(0, x.Count)

    let context : WorldDockContext =
        TestWorldDockContext
            (avatarIslandFeatureSink,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            (Fixtures.Common.Mock.AvatarMessageSink expectedMessage),
            (Fixtures.Common.Mock.AvatarSingleMetricSink [Metric.VisitedIsland, 1UL]),
            avatarSingleMetricSource,
            (fun()->Map.empty),
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandItemSink, 
            islandItemSource,
            islandJobSink,
            islandJobSource,
            islandMarketSink,
            islandMarketSource, 
            islandSource,
            (fun()->Map.empty),
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            Fixtures.Common.Stub.TermSources,
            Fixtures.Common.Stub.WorldSingleStatisticSource) 
        :> WorldDockContext
    inputWorld
    |> World.Dock
        context
        inputLocation


