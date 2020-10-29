module WorldGetAvatarReputationTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarReputation.It gets the reputation of the primary shipmate of the given avatar.`` () =
    let calledGetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := 
        Spies.Source(calledGetShipmateStatistic, Some {MinimumValue = 0.0; MaximumValue = 100.0; CurrentValue = 50.0})
    let actual = 
        World.GetAvatarReputation
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(50.0, actual)
    Assert.IsTrue(calledGetShipmateStatistic.Value)


