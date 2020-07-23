module IslandTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

let internal unvisitedIsland = 
    {
        Island.Name = "Island"
        AvatarVisits = Map.empty
        Jobs = []
        CareenDistance = 0.0
    }

let internal visitedIslandNoLastVisit = 
    {
        Island.Name    = "Island"
        AvatarVisits   = Map.empty
        Jobs           = []
        CareenDistance = 0.0
    }

let internal seenIsland =
    {
        Island.Name    = "Island"
        AvatarVisits   = Map.empty |> Map.add avatarId {VisitCount=None;LastVisit=Some 0.0}
        Jobs           = []
        CareenDistance = 0.0
    }

let internal visitedIsland =
    {
        Island.Name    = "Island"
        AvatarVisits   = Map.empty |> Map.add avatarId {VisitCount=1u |> Some;LastVisit=Some 0.0}
        Jobs           = []
        CareenDistance = 0.0
    }

let internal random = System.Random()
let internal rewardRange = (1.0, 10.0)
let internal singleDestination = [(0.0, 0.0)] |> Set.ofList

let internal jobAvailableIsland = 
    {visitedIsland with Jobs =[ Job.Create random rewardRange singleDestination ]}

let internal noCommodityIsland = visitedIsland
let internal commodityIsland = 
    noCommodityIsland
let internal commodities = 
    [(1UL, {CommodityId = 1UL; CommodityName="grain"; PurchaseFactor=1.0; SaleFactor=1.0; Discount=0.5; BasePrice=1.0})]
    |> Map.ofList
let internal items =
    [(1UL, 
        {
            ItemId = 1UL
            ItemName = "rations"
            Commodities=[(1UL, 1.0)]|>Map.ofList
            Occurrence=1.0
            Tonnage = 1.0
        })]
    |> Map.ofList
let shopIsland = commodityIsland
let shopIslandItemSource = fun (_:Location) -> [1UL] |> Set.ofList
let noShopIsland = commodityIsland

