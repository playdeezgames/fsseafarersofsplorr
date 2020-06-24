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

let private unvisitedIsland = 
    {
        Island.Name = "Island"
        LastVisit = None
        VisitCount = None
        Jobs = []
    }
let private visitedIslandNoLastVisit = 
    {
        Island.Name = "Island"
        LastVisit = None
        VisitCount = Some 0u
        Jobs = []
    }
let private visitedIsland =
    {
        Island.Name = "Island"
        LastVisit = Some 10u
        VisitCount = Some 1u
        Jobs = []
    }

[<Test>]
let ``AddVisit.It increases visit count to one and sets last visit to given turn when there is no last visit and no visit count.`` () =
    let turn = 100u
    let actual =
        unvisitedIsland
        |> Island.AddVisit turn
    Assert.AreEqual(Some 1u, actual.VisitCount)
    Assert.AreEqual(Some turn, actual.LastVisit)

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when there is no last visit.`` () =
    let turn = 100u
    let actual = 
        visitedIslandNoLastVisit
        |> Island.AddVisit turn
    Assert.AreEqual(Some (visitedIslandNoLastVisit.VisitCount.Value + 1u), actual.VisitCount)
    Assert.AreEqual(Some turn, actual.LastVisit)

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when the given turn is after the last visit.`` () =
    let turn = 100u
    let actual = 
        visitedIsland
        |> Island.AddVisit turn
    Assert.AreEqual(Some (visitedIsland.VisitCount.Value + 1u), actual.VisitCount)
    Assert.AreEqual(Some turn, actual.LastVisit)

[<Test>]
let ``AddVisit.It does not update visit count when given turn was prior or equal to last visit.`` () =
    let turn = 0u
    let actual = 
        visitedIsland
        |> Island.AddVisit turn
    Assert.AreEqual(visitedIsland, actual)

let private random = System.Random()
let private rewardRange = (1.0, 10.0)
let private singleDestination = [(0.0, 0.0)] |> Set.ofList

[<Test>]
let ``GenerateJob.It generates a job when no job is present on the island.`` () =
    let actual =
        visitedIsland
        |> Island.GenerateJobs random rewardRange singleDestination
    Assert.False(actual.Jobs.IsEmpty)

[<Test>]
let ``GenerateJob.It does nothing when no job is present on the island and no potential job destinations are given.`` () =
    let actual =
        visitedIsland
        |> Island.GenerateJobs random rewardRange Set.empty
    Assert.True(actual.Jobs.IsEmpty)

//[<Test>]
//let ``AddVisit..`` () =
//    raise (System.NotImplementedException "Not Implemented")
