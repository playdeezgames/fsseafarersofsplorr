module WorldUpdateChartTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

type TestWorldUpdateChartsContext
            (avatarIslandSingleMetricSink,
            islandSource,
            vesselSingleStatisticSource) =
    interface AvatarGetPositionContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface WorldUpdateChartsContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.islandSource: IslandSource = islandSource
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

[<Test>]
let ``UpdateChart.It does nothing when the given avatar is not near enough to any islands within the avatar's view distance.`` () =
    let input =
        avatarId
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=11.0; CurrentValue=5.5} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=0.0; MaximumValue=0.0; CurrentValue=0.0} |> Some
        | _ ->
            Assert.Fail()
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier: AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.Seen ->
            Assert.AreEqual(1UL, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let islandSource() =
        []
    let context = TestWorldUpdateChartsContext(avatarIslandSingleMetricSink, islandSource, vesselSingleStatisticSource) :> WorldUpdateChartsContext
    input
    |> World.UpdateCharts 
        context


[<Test>]
let ``UpdateChart.It does nothing when the given avatar has already seen all nearby islands.`` () =
    let input =
        avatarId
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX 
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=1000.0} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier: AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.Seen ->
            Assert.AreEqual(1UL, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let islandSource() =
        []
    let context = TestWorldUpdateChartsContext(avatarIslandSingleMetricSink, islandSource, vesselSingleStatisticSource) :> WorldUpdateChartsContext
    input
    |> World.UpdateCharts 
        context

[<Test>]
let ``UpdateChart.It does set all islands within the avatar's view distance to "seen" when given avatar is near enough to previously unseen islands.`` () =
    let input =
        avatarId
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=11.0; CurrentValue=5.5} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=20.0; MaximumValue=20.0; CurrentValue=20.0} |> Some
        | _ ->
            Assert.Fail()
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier: AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.Seen ->
            Assert.AreEqual(1UL, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let islandSource() =
        []
    let context = TestWorldUpdateChartsContext(avatarIslandSingleMetricSink, islandSource, vesselSingleStatisticSource) :> WorldUpdateChartsContext
    input
    |> World.UpdateCharts
        context

