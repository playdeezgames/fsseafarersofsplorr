module WorldGetVesselPositionTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselPosition.It gets the vessel's position for a given avatar.`` () =
    let callsForGetVesselStatistics = ref 0UL
    let context = Contexts.TestContext()
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource := 
        Spies.SourceTable(callsForGetVesselStatistics, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PositionX) (Some {MaximumValue=10.0; MinimumValue=10.0; CurrentValue=10.0})
            |> Map.add (Dummies.ValidAvatarId, VesselStatisticIdentifier.PositionY) (Some {MaximumValue=20.0; MinimumValue=20.0; CurrentValue=20.0}))
    let actual = 
        World.GetVesselPosition
            context
            Dummies.ValidAvatarId
    Assert.AreEqual((10.0, 20.0), actual)
    Assert.AreEqual(2UL, callsForGetVesselStatistics.Value)