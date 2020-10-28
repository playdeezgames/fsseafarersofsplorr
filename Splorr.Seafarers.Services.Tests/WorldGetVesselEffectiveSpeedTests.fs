module WorldGetVesselEffectiveSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselEffectiveSpeed..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselEffectiveSpeed
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)


