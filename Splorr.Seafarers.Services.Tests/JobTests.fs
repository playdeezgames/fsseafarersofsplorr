module JobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open AvatarTestFixtures

let rewardRange = (1.0, 10.0)
let random = System.Random()
let singleDestination =
    [(0.0, 0.0)]
    |> Set.ofList

type TestJobCreationContext
        (
            termSources                : TermSources, 
            worldSingleStatisticSource : WorldSingleStatisticSource
        ) =
    interface UtilitySortListRandomlyContext with
        member _.random : Random = Fixtures.Common.Dummy.Random

    interface JobCreateContext with
        member _.termSources : TermSources = termSources
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource

let internal jobCreationContextStub =
    TestJobCreationContext
        (Fixtures.Common.Stub.TermSources, 
        Fixtures.Common.Stub.WorldSingleStatisticSource)

[<Test>]
let ``Create.It generates a job.`` () =
    let actual =
        Job.Create 
            jobCreationContextStub
            singleDestination
    Assert.AreEqual((0.0,0.0), actual.Destination)
    Assert.GreaterOrEqual(actual.Reward,rewardRange |> fst);
    Assert.LessOrEqual(actual.Reward,rewardRange |> snd);
    Assert.AreNotEqual("", actual.FlavorText)


[<Test>]
let ``Create.It throws an exception when an empty set of destinations is given.`` () =
    Assert.Throws<System.ArgumentException>(
        fun () -> 
            Job.Create 
                jobCreationContextStub
                Set.empty 
                |> ignore)
    |> ignore

