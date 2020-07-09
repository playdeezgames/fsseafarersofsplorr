module IslandTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let internal unvisitedIsland = 
    {
        Island.Name = "Island"
        LastVisit = None
        VisitCount = None
        Jobs = []
        Markets = Map.empty
        Items = Set.empty
    }
let internal visitedIslandNoLastVisit = 
    {
        Island.Name = "Island"
        LastVisit = None
        VisitCount = Some 0u
        Jobs = []
        Markets = Map.empty
        Items = Set.empty
    }
let internal visitedIsland =
    {
        Island.Name = "Island"
        LastVisit = Some 10.0
        VisitCount = Some 1u
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
        Markets = [(Grain, {Supply=1.0; Demand=1.0; Traded=true})] |> Map.ofList}
let internal commodities = 
    [(Grain, {Name="grain"; PurchaseFactor=1.0; SaleFactor=1.0; Occurrence=1.0; Discount=0.5; BasePrice=1.0})]
    |> Map.ofList
let internal items =
    [(Ration, 
        {
            DisplayName = "rations"
            Commodities=[(Grain, 1.0)]|>Map.ofList
            Occurrence=1.0
        })]
    |> Map.ofList
let shopIsland = {commodityIsland with Items = commodityIsland.Items |> Set.add Ration}
let noShopIsland = commodityIsland

