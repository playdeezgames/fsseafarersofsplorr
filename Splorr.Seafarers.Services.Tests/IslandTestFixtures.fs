module IslandTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

let internal unvisitedIsland = 
    {
        Island.Name = "Island"
        AvatarVisits = Map.empty
        Jobs = []
        Markets = Map.empty
        Items = Set.empty
    }
let internal visitedIslandNoLastVisit = 
    {
        Island.Name = "Island"
        AvatarVisits = Map.empty
        Jobs = []
        Markets = Map.empty
        Items = Set.empty
    }
let internal visitedIsland =
    {
        Island.Name = "Island"
        AvatarVisits = Map.empty |> Map.add avatarId {VisitCount=1u;LastVisit=Some 0.0}
        Jobs = []
        Markets = Map.empty
        Items = Set.empty
    }

let internal random = System.Random()
let internal rewardRange = (1.0, 10.0)
let internal singleDestination = [(0.0, 0.0)] |> Set.ofList

let internal jobAvailableIsland = 
    {visitedIsland with Jobs =[ Job.Create random rewardRange singleDestination ]}

let internal noCommodityIsland = visitedIsland
let internal commodityIsland = 
    {noCommodityIsland with
        Markets = [(1u, {Supply=1.0; Demand=1.0})] |> Map.ofList}
let internal commodities = 
    [(1u, {Name="grain"; PurchaseFactor=1.0; SaleFactor=1.0; Discount=0.5; BasePrice=1.0})]
    |> Map.ofList
let internal items =
    [(1u, 
        {
            DisplayName = "rations"
            Commodities=[(1u, 1.0)]|>Map.ofList
            Occurrence=1.0
            Tonnage = 1.0
        })]
    |> Map.ofList
let shopIsland = {commodityIsland with Items = commodityIsland.Items |> Set.add 1u}
let noShopIsland = commodityIsland

