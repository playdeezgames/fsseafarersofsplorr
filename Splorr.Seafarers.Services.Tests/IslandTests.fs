module IslandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

[<Test>]
let ``Create.It returns a new island.`` () =
    let actual = Island.Create()
    Assert.AreEqual("", actual.Name)

[<Test>]
let ``SetName.It sets the name of a given island to a given name.`` () =
    let name = "Uno"
    let actual = Island.Create() |> Island.SetName name
    Assert.AreEqual(name, actual.Name)

[<Test>]
let ``GetDisplayName.It returns ???? when there is no visit count.`` () =
    let actual = Island.Create() |> Island.SetName "Uno" |> Island.GetDisplayName
    Assert.AreEqual("????", actual)

[<Test>]
let ``GetDisplayName.It returns the island's name when there is a visit count.`` () =
    let name = "Uno"
    let actual = {Island.Create() with VisitCount=Some 0u} |> Island.SetName name |> Island.GetDisplayName
    Assert.AreEqual(name, actual)

[<Test>]
let ``AddVisit.It increases visit count to one and sets last visit to given turn when there is no last visit and no visit count.`` () =
    let turn = 100u
    let actual =
        IslandTestFixtures.unvisitedIsland
        |> Island.AddVisit turn
    Assert.AreEqual(Some 1u, actual.VisitCount)
    Assert.AreEqual(Some turn, actual.LastVisit)

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when there is no last visit.`` () =
    let turn = 100u
    let actual = 
        IslandTestFixtures.visitedIslandNoLastVisit
        |> Island.AddVisit turn
    Assert.AreEqual(Some (IslandTestFixtures.visitedIslandNoLastVisit.VisitCount.Value + 1u), actual.VisitCount)
    Assert.AreEqual(Some turn, actual.LastVisit)

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when the given turn is after the last visit.`` () =
    let turn = 100u
    let actual = 
        IslandTestFixtures.visitedIsland
        |> Island.AddVisit turn
    Assert.AreEqual(Some (IslandTestFixtures.visitedIsland.VisitCount.Value + 1u), actual.VisitCount)
    Assert.AreEqual(Some turn, actual.LastVisit)

[<Test>]
let ``AddVisit.It does not update visit count when given turn was prior or equal to last visit.`` () =
    let turn = 0u
    let actual = 
        IslandTestFixtures.visitedIsland
        |> Island.AddVisit turn
    Assert.AreEqual(IslandTestFixtures.visitedIsland, actual)

[<Test>]
let ``GenerateJob.It generates a job when no job is present on the island.`` () =
    let actual =
        IslandTestFixtures.visitedIsland
        |> Island.GenerateJobs IslandTestFixtures.random IslandTestFixtures.rewardRange IslandTestFixtures.singleDestination
    Assert.False(actual.Jobs.IsEmpty)

[<Test>]
let ``GenerateJob.It does nothing when no job is present on the island and no potential job destinations are given.`` () =
    let actual =
        IslandTestFixtures.visitedIsland
        |> Island.GenerateJobs IslandTestFixtures.random IslandTestFixtures.rewardRange Set.empty
    Assert.True(actual.Jobs.IsEmpty)

[<Test>]
let ``RemoveJob.It returns the original island and None when the given job index is 0u.`` () =
    let actual =
        IslandTestFixtures.jobAvailableIsland
        |> Island.RemoveJob 0u
    Assert.AreEqual((IslandTestFixtures.jobAvailableIsland,None),actual)

[<Test>]
let ``RemoveJob.It returns the original island and None when there are no jobs on the given island.`` () =
    let actual =
        IslandTestFixtures.visitedIsland
        |> Island.RemoveJob 1u
    Assert.AreEqual((IslandTestFixtures.visitedIsland,None),actual)

[<Test>]
let ``RemoveJob.It returns the original island and None whenthe given job index is out of range.`` () =
    let actual =
        IslandTestFixtures.jobAvailableIsland
        |> Island.RemoveJob 0xFFFFFFFFu
    Assert.AreEqual((IslandTestFixtures.jobAvailableIsland,None),actual)


[<Test>]
let ``RemoveJob.It returns the modified island and the indicated job when the given job index is in range.`` () =
    let actual =
        IslandTestFixtures.jobAvailableIsland
        |> Island.RemoveJob 1u
    Assert.AreEqual(({IslandTestFixtures.jobAvailableIsland with Jobs = []},Some IslandTestFixtures.jobAvailableIsland.Jobs.Head),actual)

[<Test>]
let ``MakeKnown.It does nothing when the given island is already known.`` () =
    let subject = IslandTestFixtures.visitedIsland
    let actual =
        subject
        |> Island.MakeKnown
    Assert.AreEqual(subject.VisitCount, actual.VisitCount)
    Assert.AreEqual(subject.LastVisit, actual.LastVisit)


[<Test>]
let ``MakeKnown.It mutates the island's visit count to Some 0 when the given island is not known.`` () =
    let subject = IslandTestFixtures.unvisitedIsland
    let actual =
        subject
        |> Island.MakeKnown
    Assert.AreEqual(Some 0u, actual.VisitCount)
    Assert.AreEqual(subject.LastVisit, actual.LastVisit)


//[<Test>]
//let ``AddVisit..`` () =
//    raise (System.NotImplementedException "Not Implemented")
