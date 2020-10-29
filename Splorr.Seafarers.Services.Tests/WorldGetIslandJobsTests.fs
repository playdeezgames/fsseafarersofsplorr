module WorldGetIslandJobsTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetIslandJobs.It gets a list of jobs at the given island location.`` () =
    let calledGetIslandJobs = ref false
    let context = Contexts.TestContext()
    (context :> IslandJob.GetContext).islandJobSource := Spies.Source(calledGetIslandJobs, [])
    let actual = 
        World.GetIslandJobs
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual([], actual)
    Assert.IsTrue(calledGetIslandJobs.Value)


