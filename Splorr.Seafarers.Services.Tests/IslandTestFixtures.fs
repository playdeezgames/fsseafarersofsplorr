module IslandTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let internal unvisitedIsland = 
    {
        Island.Name = "Island"
        LastVisit = None
        VisitCount = None
        Jobs = []
    }
let internal visitedIslandNoLastVisit = 
    {
        Island.Name = "Island"
        LastVisit = None
        VisitCount = Some 0u
        Jobs = []
    }
let internal visitedIsland =
    {
        Island.Name = "Island"
        LastVisit = Some 10u
        VisitCount = Some 1u
        Jobs = []
    }

let internal random = System.Random()
let internal rewardRange = (1.0, 10.0)
let internal singleDestination = [(0.0, 0.0)] |> Set.ofList

let internal jobAvailableIsland = 
    {visitedIsland with Jobs =[ Job.Create random rewardRange singleDestination ]}
