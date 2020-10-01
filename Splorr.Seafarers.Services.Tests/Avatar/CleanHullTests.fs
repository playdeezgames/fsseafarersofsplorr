module AvatarCleanHullTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

let inputAvatarId = "avatar"

type TestAvatarCleanHullContext
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
    interface AvatarShipmates.TransformContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
    interface AvatarMetric.AddContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface ShipmateStatistic.PutContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
    interface ShipmateStatistic.GetContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Vessel.TransformFoulingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink

[<Test>]
let ``CleanHull.It cleans the hull of the given avatar.`` () =
    let inputSide = Port
    let vesselSingleStatisticSource (_) (_) = {MinimumValue=0.0;MaximumValue=0.5;CurrentValue=0.5} |> Some
    let vesselSingleStatisticSink (_) (_:VesselStatisticIdentifier, statistic:Statistic) : unit =
        Assert.AreEqual(statistic.MinimumValue, statistic.CurrentValue)
    let avatarShipmateSource (_) =
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Kaboom shipmateSingleStatisticSink")
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.CleanedHull ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarCleanHullContext
            (avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.CleanedHull, 1UL)]),
            avatarSingleMetricSource,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> Avatar.CleanHullContext
    Avatar.CleanHull
        context
        inputSide
        inputAvatarId 

