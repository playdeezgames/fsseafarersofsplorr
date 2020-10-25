module WorldDistanceToTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Tests.Common

[<Test>]
let ``DistanceTo.It adds a message when the island does not exist.`` () =
    let calledGetIslandByName = ref false
    let calledAddAvatarMessage = ref false
    let context = Contexts.TestContext()
    (context :> WorldIslands.GetIslandByNameContext).islandLocationByNameSource := 
        Spies.Source(calledGetIslandByName, None)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessage)
    let actual = 
        World.DistanceTo
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledGetIslandByName.Value)
    Assert.IsTrue(calledAddAvatarMessage.Value)

[<Test>]
let ``DistanceTo.It adds a message when the island is not know to the avatar.`` () =
    let calledGetIslandByName = ref false
    let calledGetAvatarIslandMetric = ref false
    let calledAddAvatarMessage = ref false
    let context = Contexts.TestContext()
    (context :> WorldIslands.GetIslandByNameContext).islandLocationByNameSource := 
        Spies.Source(calledGetIslandByName, Some Dummies.ValidIslandLocation)
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource :=
        Spies.Source(calledGetAvatarIslandMetric, None)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessage)
    let actual = 
        World.DistanceTo
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledGetIslandByName.Value)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)
    Assert.IsTrue(calledAddAvatarMessage.Value)

[<Test>]
let ``DistanceTo.It calculates the distance to an island that exists that is known to the avatar.`` () =
    let calledGetIslandByName = ref false
    let calledGetAvatarIslandMetric = ref false
    let callsForGetVesselStatistic = ref 0UL
    let calledAddAvatarMessage = ref false
    let context = Contexts.TestContext()
    (context :> WorldIslands.GetIslandByNameContext).islandLocationByNameSource := 
        Spies.Source(calledGetIslandByName, Some Dummies.ValidIslandLocation)
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource :=
        Spies.Source(calledGetAvatarIslandMetric, Some 1UL)
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource :=
        Spies.SourceTable(callsForGetVesselStatistic, Dummies.ValidDefaultVesselStatisticTable)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessage)
    let actual = 
        World.DistanceTo
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledGetIslandByName.Value)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)
    Assert.AreEqual(2UL, callsForGetVesselStatistic.Value)
    Assert.IsTrue(calledAddAvatarMessage.Value)


