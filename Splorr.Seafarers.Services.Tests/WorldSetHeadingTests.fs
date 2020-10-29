module WorldSetHeadingTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``SetHeading.It sets a vessel heading for the given avatar and reports the new status with a message.`` () =
    let calledGetVesselStatistic = ref false
    let calledSetVesselStatistic = ref false
    let calledAddAvatarMessages = ref false
    let context = Contexts.TestContext()
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource :=
        Spies.Source(calledGetVesselStatistic, Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
    (context :> Vessel.SetStatisticContext).vesselSingleStatisticSink :=
        Spies.Expect(calledSetVesselStatistic, (Dummies.ValidAvatarId, VesselStatisticIdentifier.Heading,{MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.017453292519943295}))
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessages)
    World.SetHeading
        context
        1.0
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetVesselStatistic.Value)
    Assert.IsTrue(calledSetVesselStatistic.Value)
    Assert.IsTrue(calledAddAvatarMessages.Value)

