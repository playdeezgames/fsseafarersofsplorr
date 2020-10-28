module WorldGetAvatarReputationTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarReputation..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarReputation
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)


