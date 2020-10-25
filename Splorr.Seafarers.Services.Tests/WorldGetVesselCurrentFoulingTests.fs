module WorldGetVesselCurrentFoulingTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselCurrentFouling.It gets the current fouling.`` () =
    let callsForGetStatistic = ref 0UL
    let context = Contexts.TestContext()
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource :=
        Spies.SourceTable(callsForGetStatistic, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PortFouling) (Some {MinimumValue=0.0; MaximumValue=0.25; CurrentValue=0.1})
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.StarboardFouling) (Some {MinimumValue=0.0; MaximumValue=0.25; CurrentValue=0.15}))
    let actual = 
        World.GetVesselCurrentFouling
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(0.25, actual)
    Assert.AreEqual(2UL, callsForGetStatistic.Value)


