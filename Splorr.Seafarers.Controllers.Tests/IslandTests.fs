module IslandTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
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
    }
let private visitedIslandNoLastVisit = 
    {
        Island.Name = "Island"
        LastVisit = None
        VisitCount = Some 0u
    }
let private visitedIsland =
    {
        Island.Name = "Island"
        LastVisit = Some 10u
        VisitCount = Some 1u
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

//[<Test>]
//let ``AddVisit..`` () =
//    raise (System.NotImplementedException "Not Implemented")
