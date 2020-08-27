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
let ``GetForIslandByIndex.It retrieves a job on the island at the given location's given index.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let givenIndex = 1u
    let expected : Result<Job option, string> = 
        {
            FlavorText = "description"
            Reward = 1.0
            Destination = UnvisitedIslandLocation
        }
        |> Some
        |> Ok
    let actual = IslandJob.GetForIslandByIndex connection givenLocation givenIndex
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetForIslandByIndex.It returns none when given an index of zero.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let givenIndex = 0u
    let expected : Result<Job option, string> = 
        None
        |> Ok
    let actual = IslandJob.GetForIslandByIndex connection givenLocation givenIndex
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetForIslandByIndex.It none on an invalid index.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let givenIndex = 0xffffffffu
    let expected : Result<Job option, string> = 
        None
        |> Ok
    let actual = IslandJob.GetForIslandByIndex connection givenLocation givenIndex
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

[<Test>]
let ``RemoveFromIsland.It removes a job at a given index from a given island when there is a job at that index.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let givenIndex = 1u
    match IslandJob.GetForIsland connection givenLocation with
    | Error _ ->
        Assert.Fail "GetForIsland blew up"
    | Ok x ->
        Assert.AreEqual(1, x.Length)
    let expected : Result<unit, string> = 
        Ok ()
    let actual = IslandJob.RemoveFromIsland connection givenLocation givenIndex
    Assert.AreEqual(expected, actual)
    match IslandJob.GetForIsland connection givenLocation with
    | Error _ ->
        Assert.Fail "GetForIsland blew up"
    | Ok x ->
        Assert.AreEqual(0, x.Length)

[<Test>]
let ``RemoveFromIsland.It does nothing when there given an index of zero.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let givenIndex = 0u
    match IslandJob.GetForIsland connection givenLocation with
    | Error _ ->
        Assert.Fail "GetForIsland blew up"
    | Ok x ->
        Assert.AreEqual(1, x.Length)
    let expected : Result<unit, string> = 
        Ok ()
    let actual = IslandJob.RemoveFromIsland connection givenLocation givenIndex
    Assert.AreEqual(expected, actual)
    match IslandJob.GetForIsland connection givenLocation with
    | Error _ ->
        Assert.Fail "GetForIsland blew up"
    | Ok x ->
        Assert.AreEqual(1, x.Length)

[<Test>]
let ``RemoveFromIsland.It does nothing when there given an invalid index.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let givenIndex = 0xffffffffu
    match IslandJob.GetForIsland connection givenLocation with
    | Error _ ->
        Assert.Fail "GetForIsland blew up"
    | Ok x ->
        Assert.AreEqual(1, x.Length)
    let expected : Result<unit, string> = 
        Ok ()
    let actual = IslandJob.RemoveFromIsland connection givenLocation givenIndex
    Assert.AreEqual(expected, actual)
    match IslandJob.GetForIsland connection givenLocation with
    | Error _ ->
        Assert.Fail "GetForIsland blew up"
    | Ok x ->
        Assert.AreEqual(1, x.Length)