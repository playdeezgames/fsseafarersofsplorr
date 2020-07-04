module DockedTestFixtures

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open CommonTestFixtures

let internal dockWorldconfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(0.0, 0.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=1u
        RewardRange = (1.0, 10.0)
        Commodities = Map.empty
        Items = Map.empty
    }
let internal dockWorld = World.Create dockWorldconfiguration random
let internal dockLocation = (0.0, 0.0)
let internal smallWorldCommodities:Map<Commodity, CommodityDescriptor> = 
    [(Grain, {Name="commodity under test";BasePrice=1.0;PurchaseFactor=1.0;SaleFactor=1.0;Discount=0.5;Occurrence=1.0})] |> Map.ofList
let internal smallWorldItems:Map<Item, ItemDescriptor> =
    [(Ration, {DisplayName="item under test"; Commodities=[(Grain,1.0)]|>Map.ofList; Occurrence =1.0})] |> Map.ofList
let internal smallWorldconfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(11.0, 11.0)
        MinimumIslandDistance=5.0
        MaximumGenerationTries=500u
        RewardRange = (1.0, 10.0)
        Commodities = smallWorldCommodities
        Items = smallWorldItems
    }
let internal smallWorld = World.Create smallWorldconfiguration random
let internal smallWorldIslandLocation = smallWorld.Islands |> Map.toList |> List.map fst |> List.head
let internal smallWorldDocked = smallWorld |> World.Dock random smallWorldIslandLocation
let internal shopWorld = smallWorldDocked |> World.ClearMessages

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar (fun avatar -> {avatar with Job=Some { FlavorText="";Reward=0.0; Destination=(0.0,0.0)}})



