module WorldGetVesselHeadingTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselHeading..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselHeading
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
