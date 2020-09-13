module WorldDockTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures
open CommonTestFixtures

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
    let context : WorldDockContext =
        TestWorldDockContext
            (avatarIslandFeatureSink,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            (avatarExpectedMessageSink expectedMessage),
            (assertAvatarSingleMetricSink [Metric.VisitedIsland, 0UL; Metric.VisitedIsland, 1UL]),
            avatarSingleMetricSourceStub,
            commoditySource,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandItemSinkStub ,
            islandItemSourceStub, 
            islandJobSink,
            islandJobSource,
            islandMarketSinkStub ,
            islandMarketSourceStub ,
            islandSource,
            genericWorldItemSource ,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            termSourcesStub ,
            worldSingleStatisticSourceStub) :> WorldDockContext
    avatarId
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
    let context : WorldDockContext =
        TestWorldDockContext
            (avatarIslandFeatureSink,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            (avatarMessagesSinkFake expectedMessages),
            avatarSingleMetricSink,
            avatarSingleMetricSource,
            commoditySource ,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandItemSinkStub ,
            islandItemSourceStub,
            islandJobSink,
            islandJobSource,
            islandMarketSinkStub ,
            islandMarketSourceStub, 
            islandSource,
            genericWorldItemSource ,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            termSourcesStub ,
            worldSingleStatisticSourceStub)
        :> WorldDockContext
    avatarId
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
            avatarMessageSinkStub,
            avatarSingleMetricSinkExplode,
            avatarSingleMetricSourceStub,
            commoditySource,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandItemSinkStub,
            islandItemSourceStub, 
            islandJobSink,
            islandJobSource,
            islandMarketSinkStub,
            islandMarketSourceStub, 
            islandSource,
            genericWorldItemSource,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSourceStub,
            termSourcesStub,
            worldSingleStatisticSourceStub) :> WorldDockContext
    bogusAvatarId
    |> World.Dock 
        context
        (0.0, 0.0)

[<Test>]
let ``Dock.It adds a message when the given location has no island.`` () =
    let inputWorld = avatarId
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
            (avatarExpectedMessageSink expectedMessage),
            avatarSingleMetricSinkExplode,
            avatarSingleMetricSourceStub,
            commoditySource,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandItemSinkStub ,
            islandItemSourceStub, 
            islandJobSink,
            islandJobSource,
            islandMarketSinkStub ,
            islandMarketSourceStub, 
            islandSource,
            genericWorldItemSource ,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSourceStub,
            termSourcesStub ,
            worldSingleStatisticSourceStub) :> WorldDockContext
    inputWorld
    |> World.Dock
        context
        (0.0, 0.0)

[<Test>]
let ``Dock.It updates the island's visit count and last visit when the given location has an island.`` (): unit =
    let inputWorld = avatarId
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
    let context : WorldDockContext =
        TestWorldDockContext
            (avatarIslandFeatureSink,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            (avatarExpectedMessageSink expectedMessage),
            (assertAvatarSingleMetricSink [Metric.VisitedIsland, 1UL]),
            avatarSingleMetricSourceStub,
            (fun()->Map.empty),
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandItemSinkStub, 
            islandItemSourceStub,
            islandJobSink,
            islandJobSource,
            islandMarketSinkStub ,
            islandMarketSourceStub, 
            islandSource,
            (fun()->Map.empty) ,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            termSourcesStub ,
            worldSingleStatisticSourceStub) 
        :> WorldDockContext
    inputWorld
    |> World.Dock
        context
        inputLocation


