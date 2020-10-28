module WorldGetVesselUsedTonnageTests


open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselUsedTonnage..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselUsedTonnage
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)
