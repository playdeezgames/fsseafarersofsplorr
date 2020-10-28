module WorldGetIslandJobsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandJobs..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandJobs
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual([], actual)


