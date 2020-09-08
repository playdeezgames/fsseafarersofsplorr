module WorldCleanHullTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures
open CommonTestFixtures

type TestWorldCleanHullContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface WorldCleanHullContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

[<Test>]
let ``CleanHull.It returns the original world when given a bogus avatar id and world.`` () =
    let inputWorld = 
        bogusAvatarId
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
    let context = TestWorldCleanHullContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> WorldCleanHullContext
    inputWorld
    |> World.CleanHull
        context
        avatarShipmateSource
        (assertAvatarSingleMetricSink [Metric.CleanedHull, 1UL])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputSide


[<Test>]
let ``CleanHull.It returns a cleaned hull when given a particular avatar id and world.`` () =
    let inputSide = Port
    let inputWorld = 
        avatarId
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
    let context = TestWorldCleanHullContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> WorldCleanHullContext
    inputWorld
    |> World.CleanHull 
        context
        avatarShipmateSource
        (assertAvatarSingleMetricSink [Metric.CleanedHull, 1UL])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputSide


