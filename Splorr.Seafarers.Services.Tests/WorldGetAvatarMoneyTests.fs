module WorldGetAvatarMoneyTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarMoney.It gets the current money level of the primary shipmate for the given avatar.`` () =
    let calledGetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := 
        Spies.Source(calledGetShipmateStatistic, Some {MinimumValue = 0.0; MaximumValue = 100.0; CurrentValue = 50.0})
    let actual = 
        World.GetAvatarMoney
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(50.0, actual)
    Assert.IsTrue(calledGetShipmateStatistic.Value)


