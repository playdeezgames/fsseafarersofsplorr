module WorldGetShipmateStatisticTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetShipmateStatistic..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetShipmateStatistic
            context
            Dummies.ValidAvatarId
            Primary
            ShipmateStatisticIdentifier.Health
    Assert.AreEqual(None, actual)
