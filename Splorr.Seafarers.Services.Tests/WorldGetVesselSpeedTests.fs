module WorldGetVesselSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselSpeed..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselSpeed
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)


