﻿module WorldMoveTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures
open CommonTestFixtures

type TestWorldMoveContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface VesselTransformFoulingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface VesselBefoulContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface WorldMoveContext

[<Test>]
let ``Move.It moves the avatar one unit when give 1u for distance when given a valid avatar id.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
        | VesselStatisticIdentifier.PortFouling
        | VesselStatisticIdentifier.StarboardFouling ->
            {MinimumValue=0.0; CurrentValue=0.0; MaximumValue=0.25} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MinimumValue=0.001; MaximumValue=0.001; CurrentValue=0.001} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | _ ->
            Assert.Fail("Don't get me.")
            None
    let mutable positionXCalls = 0
    let mutable positionYCalls = 0
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) : unit =
        match identifier with
        | VesselStatisticIdentifier.StarboardFouling
        | VesselStatisticIdentifier.PortFouling ->
            Assert.AreEqual(0.00050000000000000001, statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionX ->
            positionXCalls <- positionXCalls + 1
        | VesselStatisticIdentifier.PositionY ->
            positionYCalls <- positionYCalls + 1
        | _ ->
            Assert.Fail("Don't set me.")
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let islandSource () =
        []
    let context = TestWorldMoveContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> WorldMoveContext
    avatarId
    |> World.Move 
        context
        avatarInventorySink
        avatarInventorySource
        avatarIslandSingleMetricSinkStub
        avatarMessageSinkStub 
        avatarShipmateSource
        (assertAvatarSingleMetricSink [Metric.Moved, 1UL; Metric.Ate, 0UL])
        avatarSingleMetricSourceStub
        islandSource
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        1u
    |> ignore
    Assert.AreEqual(1, positionXCalls)
    Assert.AreEqual(1, positionYCalls)

[<Test>]
let ``Move.It moves the avatar almost two units when give 2u for distance.`` () =
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
        | VesselStatisticIdentifier.PortFouling
        | VesselStatisticIdentifier.StarboardFouling ->
            {MinimumValue=0.0; CurrentValue=0.0; MaximumValue=0.25} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MinimumValue=0.001; MaximumValue=0.001; CurrentValue=0.001} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | _ ->
            Assert.Fail("Dont call me.")
            None
    let mutable positionXCalls = 0
    let mutable positionYCalls = 0
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) : unit =
        match identifier with
        | VesselStatisticIdentifier.StarboardFouling
        | VesselStatisticIdentifier.PortFouling ->
            Assert.AreEqual(0.00050000000000000001, statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionX ->
            positionXCalls <- positionXCalls + 1
        | VesselStatisticIdentifier.PositionY ->
            positionYCalls <- positionYCalls + 1
        | _ ->
            Assert.Fail("Don't set me.")
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let islandSource () =
        []
    let context = TestWorldMoveContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> WorldMoveContext
    avatarId
    |> World.Move 
        context
        avatarInventorySink
        avatarInventorySource
        avatarIslandSingleMetricSinkStub
        avatarMessageSinkStub 
        avatarShipmateSource
        (assertAvatarSingleMetricSink [Metric.Moved, 1UL; Metric.Ate, 0UL])
        avatarSingleMetricSourceStub
        islandSource
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        2u
    |> ignore
    Assert.AreEqual(2, positionXCalls)
    Assert.AreEqual(2, positionYCalls)
    



