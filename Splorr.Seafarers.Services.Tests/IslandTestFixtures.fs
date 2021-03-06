module IslandTestFixtures

open Splorr.Seafarers.Models

let internal random = System.Random()
let internal rewardRange = (1.0, 10.0)
let internal singleDestination = [(0.0, 0.0)] |> Set.ofList

let internal commodities = 
    [(1UL, {CommodityName="grain"; PurchaseFactor=1.0; SaleFactor=1.0; Discount=0.5; BasePrice=1.0})]
    |> Map.ofList
let internal commoditySource = fun () -> commodities
let internal items =
    [(1UL, 
        {
            ItemName = "rations"
            Commodities=[(1UL, 1.0)]|>Map.ofList
            Occurrence=1.0
            Tonnage = 1.0
        })]
    |> Map.ofList
let internal itemSource () = items
let internal shopIslandItemSource = fun (_:Location) -> [1UL] |> Set.ofList

