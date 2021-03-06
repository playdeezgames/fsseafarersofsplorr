module WorldCleanHullTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldCleanHullContext
        (avatarShipmateSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink,
        shipmateSingleStatisticSource,
        vesselSingleStatisticSink, 
        vesselSingleStatisticSource) =
    interface Vessel.GetStatisticContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.CleanHullContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
    interface Vessel.TransformFoulingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
    interface AvatarShipmates.TransformContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
    interface AvatarMetric.AddContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface ShipmateStatistic.PutContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
    interface ShipmateStatistic.GetContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``CleanHull.It returns the original world when given a bogus avatar id and world.`` () =
    let inputWorld = 
        Fixtures.Common.Dummy.BogusAvatarId
    let inputSide = Port
    let vesselSingleStatisticSource (_) (_) =
        None
    let vesselSingleStatisticSink (_) (_) =
        Assert.Fail("Dont set statistics")
    let avatarShipmateSource (_) = 
        []
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarSingleMetricSource (_) (metric:Metric) : uint64 =
        match metric with
        | Metric.CleanedHull ->
            0UL
        | _ ->
            Assert.Fail("avatarSingleMetricSource")
            0UL
    let context = 
        TestWorldCleanHullContext
            (avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [Metric.CleanedHull, 1UL]),
            avatarSingleMetricSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> ServiceContext
    inputWorld
    |> World.CleanHull
        context
        inputSide


[<Test>]
let ``CleanHull.It returns a cleaned hull when given a particular avatar id and world.`` () =
    let inputSide = Port
    let inputWorld = 
        Fixtures.Common.Dummy.AvatarId
    let vesselSingleStatisticSource (_) (_) =
        {MinimumValue = 0.0; MaximumValue=0.25; CurrentValue = 0.25} |> Some
    let vesselSingleStatisticSink (_) (_, statistic:Statistic) =
        Assert.AreEqual(statistic.MinimumValue, statistic.CurrentValue)
    let avatarShipmateSource (_) = 
        []
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarSingleMetricSource (_) (metric:Metric) : uint64 =
        match metric with
        | Metric.CleanedHull ->
            0UL
        | _ ->
            Assert.Fail("avatarSingleMetricSource")
            0UL
    let context = 
        TestWorldCleanHullContext
            (avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [Metric.CleanedHull, 1UL]),
            avatarSingleMetricSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) 
        :> ServiceContext
    inputWorld
    |> World.CleanHull 
        context
        inputSide


