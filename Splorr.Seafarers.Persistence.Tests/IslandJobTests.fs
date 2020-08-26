module IslandJob

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

[<Test>]
let ``GetForIsland.It retrieves a list of jobs for a location.`` () =
    let connection = SetupConnection()
    let expected : Result<Job list, string> = 
        [
            {
                FlavorText = "description"
                Reward = 1.0
                Destination = UnvisitedIslandLocation
            }
        ]
        |> Ok
    let actual = IslandJob.GetForIsland connection VisitedIslandLocation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AddToIsland.It add a new job to the island.`` () =
    let connection = SetupConnection()
    let givenLocation = UnvisitedIslandLocation
    let givenJob : Job =
        {
            FlavorText="new job"
            Reward=10.0
            Destination = VisitedIslandLocation
        }
    let expectedInitial : Result<Job list, string> = 
        [
        ]
        |> Ok
    let actualInitial = IslandJob.GetForIsland connection givenLocation
    Assert.AreEqual(expectedInitial, actualInitial)
    let expected : Result<unit, string> = Ok ()
    let actual = IslandJob.AddToIsland connection givenLocation givenJob
    Assert.AreEqual(expected, actual)
    let expectedFinal : Result<Job list, string> = 
        [
            givenJob
        ]
        |> Ok
    let actualFinal = IslandJob.GetForIsland connection givenLocation
    Assert.AreEqual(expectedFinal, actualFinal)
    