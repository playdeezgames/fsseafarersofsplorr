module WorldGetVesselStatisticTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselStatistic.It retrieves the vessel statistic for the given avatar.`` () =
    let calledGetStatistic = ref false
    let context = Contexts.TestContext()
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource := Spies.Source(calledGetStatistic, None)
    let actual = 
        World.GetVesselStatistic
            context
            Dummies.ValidAvatarId
            VesselStatisticIdentifier.Heading
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledGetStatistic.Value)


