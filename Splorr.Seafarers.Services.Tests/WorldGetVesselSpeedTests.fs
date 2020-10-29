module WorldGetVesselSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselSpeed.It sets the speed of the give avatar's vessel.`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselSpeed
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)


