module WorldCleanHullTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``CleanHull.It removes fouling from the given side and spends a turn for all shipmates and increments the clean hull counter.`` () = 
    let calledGetVesselStatistic = ref false
    let calledSetVesselStatistic = ref false
    let calledGetShipmates = ref false
    let calledGetShipmateStatistic = ref false
    let calledPutShipmateStatistic = ref false
    let calledGetAvatarMetric = ref false
    let calledSetAvatarMetric = ref false
    let context = Contexts.TestContext()
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource := Spies.Source(calledGetVesselStatistic,Some {MaximumValue=0.5; MinimumValue=0.0; CurrentValue=0.25})
    (context :> Vessel.SetStatisticContext).vesselSingleStatisticSink := Spies.Expect(calledSetVesselStatistic, (Dummies.ValidAvatarId, VesselStatisticIdentifier.PortFouling, {MaximumValue=0.5; MinimumValue=0.0; CurrentValue=0.0}))
    (context :> AvatarShipmates.GetShipmatesContext).avatarShipmateSource := Spies.Source(calledGetShipmates, Dummies.ValidShipmates)
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := Spies.Source(calledGetShipmateStatistic, Some {MaximumValue=100.0;MinimumValue=0.0;CurrentValue=50.0})
    (context :> ShipmateStatistic.PutContext).shipmateSingleStatisticSink := Spies.Expect(calledPutShipmateStatistic, (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Turn, Some {MaximumValue=100.0;MinimumValue=0.0;CurrentValue=51.0}))
    (context :> AvatarMetric.GetMetricContext).avatarSingleMetricSource := Spies.Source(calledGetAvatarMetric, 0UL)
    (context :> AvatarMetric.SetMetricContext).avatarSingleMetricSink := Spies.Expect(calledSetAvatarMetric, (Dummies.ValidAvatarId, Metric.CleanedHull, 1UL))
    World.CleanHull
        context
        Side.Port
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetVesselStatistic.Value)
    Assert.IsTrue(calledSetVesselStatistic.Value)
    Assert.IsTrue(calledGetShipmates.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)
    Assert.IsTrue(calledPutShipmateStatistic.Value)
    Assert.IsTrue(calledGetAvatarMetric.Value)
    Assert.IsTrue(calledSetAvatarMetric.Value)
