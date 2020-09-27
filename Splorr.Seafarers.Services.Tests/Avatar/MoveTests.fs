module AvatarMoveTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open AvatarTestFixtures

let private inputAvatarId = "avatar"

type TestAvatarMoveContext
        (avatarInventorySink,
        avatarInventorySource,
        avatarShipmateSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateRationItemSource, 
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource, 
        vesselSingleStatisticSink, 
        vesselSingleStatisticSource) =
    interface Avatar.EatContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
    interface Avatar.GetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarShipmate.TransformShipmatesContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource

    interface Vessel.BefoulContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.MoveContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarMetric.AddContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource

    interface Vessel.SetPositionContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Vessel.GetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Vessel.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Shipmate.EatContext with
        member this.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Vessel.TransformFoulingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``Move.It moves the avatar.`` () =
    let expectedPosition = (51.0,50.0)
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) =
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            Assert.AreEqual(expectedPosition |> fst,statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionY ->
            Assert.AreEqual(expectedPosition |> snd,statistic.CurrentValue)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "vesselSingleStatisticSink - %s")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.Moved -> 
            0UL
        | Metric.Starved ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let avatarShipmateSource (_) =
        []
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.Moved, 1UL)]),
            avatarSingleMetricSource,
            Fixtures.Common.Stub.ShipmateRationItemSource, 
            Fixtures.Common.Fake.ShipmateSingleStatisticSink, 
            Fixtures.Common.Fake.ShipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            Fixtures.Common.Stub.VesselSingleStatisticSource) :> Avatar.MoveContext
    Avatar.Move 
        context
        inputAvatarId

[<Test>]
let ``Move.It removes a ration when the given avatar has rations and full satiety.`` () =
    let originalInventory = Map.empty |> Map.add 1UL 2UL
    let expectedInventory = Map.empty |> Map.add 1UL 1UL
    let shipmateRationItemSource (_) (_) = [0UL; 1UL]
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 100.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
            Assert.AreEqual(100.0, statistic.Value.MaximumValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(100.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        originalInventory
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(expectedInventory, inventory)
    let vesselSingleStatisticSink (_) (id, stat:Statistic) =
        match id with
        | VesselStatisticIdentifier.PositionX ->
            Assert.AreEqual(51.0, stat.CurrentValue)
        | VesselStatisticIdentifier.PositionY ->
            Assert.AreEqual(50.0, stat.CurrentValue)
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "vesselSingleStatisticSink - %s")
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.Moved
        | Metric.Ate
        | Metric.Starved ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 1UL)]),
            avatarSingleMetricSource,
            shipmateRationItemSource, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            Fixtures.Common.Stub.VesselSingleStatisticSource) :> Avatar.MoveContext
    Avatar.Move 
        context
        inputAvatarId

[<Test>]
let ``Move.It removes a ration and increases satiety when the given avatar has rations and less than full satiety.`` () =
    let originalInventory = Map.empty |> Map.add 1UL 2UL
    let expectedInventory = Map.empty |> Map.add 1UL 1UL
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
            Assert.AreEqual(100.0, statistic.Value.MaximumValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(51.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        originalInventory
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(expectedInventory, inventory)
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) =
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            Assert.AreEqual(51.0,statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionY ->
            Assert.AreEqual(50.0,statistic.CurrentValue)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "vesselSingleStatisticSink - %s")
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.Moved
        | Metric.Ate
        | Metric.Starved ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 1UL)]),
            avatarSingleMetricSource,
            Fixtures.Common.Stub.ShipmateRationItemSource, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            Fixtures.Common.Stub.VesselSingleStatisticSource) :> Avatar.MoveContext
    Avatar.Move 
        context
        inputAvatarId
    


[<Test>]
let ``Move.It lowers the avatar's satiety but does not affect turns when the given avatar has no rations.`` () =
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
            Assert.AreEqual(100.0, statistic.Value.MaximumValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let vesselSingleStatisticSink (_) (id, stat:Statistic) =
        match id with
        | VesselStatisticIdentifier.PositionX ->
            Assert.AreEqual(51.0, stat.CurrentValue)
        | VesselStatisticIdentifier.PositionY ->
            Assert.AreEqual(50.0, stat.CurrentValue)
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "vesselSingleStatisticSink - %s")
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.Moved -> 
            0UL
        | Metric.Starved ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.Moved, 1UL)]),
            avatarSingleMetricSource,
            Fixtures.Common.Stub.ShipmateRationItemSource, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            Fixtures.Common.Stub.VesselSingleStatisticSource) :> Avatar.MoveContext
    Avatar.Move 
        context
        inputAvatarId


[<Test>]
let ``Move.It lowers the avatar's maximum turn and updates the starvation metric when the given avatar has no rations and minimum satiety.`` () =
    let avatarShipmateSource (_) = 
        [ Primary ]
    let mutable sinkCalls = 0u
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create 
                (0.0, 100.0) 
                (if sinkCalls>0u then 1.0 else 0.0) 
            |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic : Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            match sinkCalls with
            | 0u ->
                Assert.AreEqual(1.0, statistic.Value.CurrentValue)
                Assert.AreEqual(100.0, statistic.Value.MaximumValue)
            | 1u ->
                Assert.AreEqual(1.0, statistic.Value.CurrentValue)
                Assert.AreEqual(99.0, statistic.Value.MaximumValue)
            | _ ->
                raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink - bad number of sink calls")
            sinkCalls <- sinkCalls + 1u
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let vesselSingleStatisticSink (_) (id, stat:Statistic) =
        match id with
        | VesselStatisticIdentifier.PositionX ->
            Assert.AreEqual(51.0, stat.CurrentValue)
        | VesselStatisticIdentifier.PositionY ->
            Assert.AreEqual(50.0, stat.CurrentValue)
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "vesselSingleStatisticSink - %s")
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.Moved -> 
            0UL
        | Metric.Starved ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Starved, 1UL)]),
            avatarSingleMetricSource,
            Fixtures.Common.Stub.ShipmateRationItemSource, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            Fixtures.Common.Stub.VesselSingleStatisticSource) :> Avatar.MoveContext
    Avatar.Move 
        context
        inputAvatarId

[<Test>]
let ``Move.It transforms the avatar within the given world.`` () =
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.5; CurrentValue=0.0} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MinimumValue=0.01; MaximumValue=0.01; CurrentValue=0.01} |> Some
        | _ ->
            None
    let mutable xPositionCalled = 0
    let mutable yPositionCalled = 0
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier,_) =
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            xPositionCalled <- xPositionCalled + 1
        | VesselStatisticIdentifier.PositionY ->
            yPositionCalled <- yPositionCalled + 1
        | _ ->
            raise (System.NotImplementedException "Kaboom set")
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
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
    let avatarSingleMetricSource (_) (id:Metric) =
        match id with
        | Metric.Moved
        | Metric.Ate
        | Metric.Starved ->
            0UL
        | _ ->
            Assert.Fail(id.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (Fixtures.Common.Mock.AvatarSingleMetricSink [Metric.Moved, 1UL; Metric.Ate, 0UL]),
            avatarSingleMetricSource,
            Fixtures.Common.Stub.ShipmateRationItemSource, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> Avatar.MoveContext
    Avatar.Move 
            context
            Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(1, xPositionCalled)
    Assert.AreEqual(1, yPositionCalled)


