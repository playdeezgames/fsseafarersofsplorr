module WorldGetAvatarMetricsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarMetrics..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarMetrics
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(Map.empty, actual)


