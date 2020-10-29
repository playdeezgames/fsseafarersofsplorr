module WorldGetVesselSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselSpeed.It sets the speed of the give avatar's vessel.`` () =
    let calledGetVesselStatistic = ref false
    let context = Contexts.TestContext()
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource := 
        Spies.Source(calledGetVesselStatistic, Some {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0})
    let actual = 
        World.GetVesselSpeed
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(Some 50.0, actual)
    Assert.IsTrue(calledGetVesselStatistic.Value)


