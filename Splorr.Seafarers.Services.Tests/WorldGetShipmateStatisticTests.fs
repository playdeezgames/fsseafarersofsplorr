module WorldGetShipmateStatisticTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetShipmateStatistic.It retrieves a given statistic for a given shipmate of a given avatar.`` () =
    let calledGetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource :=
        Spies.Source(calledGetShipmateStatistic, Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
    let actual = 
        World.GetShipmateStatistic
            context
            Dummies.ValidAvatarId
            Primary
            ShipmateStatisticIdentifier.Health
    Assert.AreEqual(Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0}, actual)
    Assert.IsTrue(calledGetShipmateStatistic.Value)
