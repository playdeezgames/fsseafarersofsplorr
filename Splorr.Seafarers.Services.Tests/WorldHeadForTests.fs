module WorldHeadForTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``HeadFor.It adds a message when the given island name is not known to the given avatar.`` () =
    let calledGetIslandByName = ref false
    let calledGetAvatarIslandMetric = ref false
    let callsForGetVesselStatistic = ref 0UL
    let calledAddAvatarMessages = ref false
    let context = Contexts.TestContext()
    (context :> WorldIslands.GetIslandByNameContext).islandLocationByNameSource :=
        Spies.Source(calledGetIslandByName, Some Dummies.ValidIslandLocation)
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource :=
        Spies.Source(calledGetAvatarIslandMetric, None)
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource :=
        Spies.SourceTable(callsForGetVesselStatistic, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PositionX) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PositionY) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=51.0}))
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessages)
    let actual = 
        World.HeadFor
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledGetIslandByName.Value)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)
    Assert.AreEqual(2UL, callsForGetVesselStatistic.Value)
    Assert.IsTrue(calledAddAvatarMessages.Value)

[<Test>]
let ``HeadFor.It causes the given avatars vessel to head for the island when the given avatar knows about the given island name.`` () =
    let calledGetIslandByName = ref false
    let calledGetAvatarIslandMetric = ref false
    let callsForGetVesselStatistic = ref 0UL
    let calledAddAvatarMessages = ref false
    let calledSetVesselStatistic = ref false
    let context = Contexts.TestContext()
    (context :> WorldIslands.GetIslandByNameContext).islandLocationByNameSource :=
        Spies.Source(calledGetIslandByName, Some Dummies.ValidIslandLocation)
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource :=
        Spies.Source(calledGetAvatarIslandMetric, Some 0UL)
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource :=
        Spies.SourceTable(callsForGetVesselStatistic, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PositionX) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PositionY) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=51.0})
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.Heading) (Some {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=3.14159}))
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessages)
    (context :> Vessel.SetStatisticContext).vesselSingleStatisticSink := Spies.Expect(calledSetVesselStatistic, (Dummies.ValidAvatarId, VesselStatisticIdentifier.Heading, {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=3.926990816987241}))
    let actual = 
        World.HeadFor
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledGetIslandByName.Value)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)
    Assert.AreEqual(4UL, callsForGetVesselStatistic.Value)
    Assert.IsTrue(calledAddAvatarMessages.Value)
    Assert.IsTrue(calledSetVesselStatistic.Value)



    