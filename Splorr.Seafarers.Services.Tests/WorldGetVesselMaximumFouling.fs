module WorldGetVesselMaximumFouling

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselMaximumFouling.It retrieves the vessel's maximum fouling.`` () =
    let callsForGetStatistic = ref 0UL
    let context = Contexts.TestContext()
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource :=
        Spies.SourceTable(callsForGetStatistic, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PortFouling) (Some {MinimumValue=0.0; MaximumValue=0.2; CurrentValue=0.1})
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.StarboardFouling) (Some {MinimumValue=0.0; MaximumValue=0.3; CurrentValue=0.15}))
    let actual = 
        World.GetVesselMaximumFouling
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(0.5, actual)
    Assert.AreEqual(2UL, callsForGetStatistic.Value)


