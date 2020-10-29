module WorldGetVesselEffectiveSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselEffectiveSpeed.It calculates a vessels effective speed based on set speed and current fouling.`` () =
    let callsForGetVesselStatistic = ref 0UL
    let context = Contexts.TestContext()
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource :=
        Spies.SourceTable(callsForGetVesselStatistic, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.Speed) (Some {MinimumValue=0.0; MaximumValue=2.0; CurrentValue=1.0})
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PortFouling) (Some {MinimumValue=0.0; MaximumValue=0.25; CurrentValue=0.125})
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.StarboardFouling) (Some {MinimumValue=0.0; MaximumValue=0.25; CurrentValue=0.125})
            )
    let actual = 
        World.GetVesselEffectiveSpeed
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(0.75, actual)
    Assert.AreEqual(3UL, callsForGetVesselStatistic.Value)

