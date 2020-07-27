module DockedTestFixtures

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open CommonTestFixtures

let internal dockWorldconfiguration: WorldConfiguration =
    {
        AvatarDistances = (10.0,1.0)
        WorldSize=(0.0, 0.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=1u
        RewardRange = (1.0, 10.0)
        RationItems = [1UL]
        StatisticDescriptors = statisticDescriptors
    }
let internal dockWorld = World.Create dockWorldconfiguration random avatarId
let internal dockLocation = (0.0, 0.0)
let internal deadDockWorld =
    {dockWorld with 
        Avatars = 
            Map.empty 
            |> Map.add 
                avatarId 
                (dockWorld.Avatars.[avatarId] 
                |> Avatar.TransformShipmate (Shipmate.TransformStatistic 
                    AvatarStatisticIdentifier.Health 
                    (fun x -> 
                        {x with CurrentValue = x.MinimumValue} 
                        |> Some)) 0u)}
let internal deadDockLocation = dockLocation
let internal commodities:Map<uint64, CommodityDescriptor> = 
    [(1UL, {CommodityId = 1UL; CommodityName="commodity under test";BasePrice=1.0;PurchaseFactor=1.0;SaleFactor=1.0;Discount=0.5})] |> Map.ofList
let internal smallWorldItems:Map<uint64, ItemDescriptor> =
    [(1UL, {ItemId = 1UL;ItemName="item under test"; Commodities=[(1UL,1.0)]|>Map.ofList; Occurrence =1.0; Tonnage=1.0})] |> Map.ofList
let internal smallWorldconfiguration: WorldConfiguration =
    {
        AvatarDistances = (10.0,1.0)
        WorldSize=(11.0, 11.0)
        MinimumIslandDistance=5.0
        MaximumGenerationTries=500u
        RewardRange = (1.0, 10.0)
        RationItems = [1UL]
        StatisticDescriptors = statisticDescriptors
    }
let internal smallWorld = World.Create smallWorldconfiguration random avatarId
let internal smallWorldIslandLocation = smallWorld.Islands |> Map.toList |> List.map fst |> List.head
let private smallWorldIslandItemSource (_) = [1UL] |> Set.ofList
let private smallWorldIslandItemSink (_) (_) = ()
let private smallWorldIslandMarketSource (_) = [1UL, {Supply=1.0;Demand=1.0}] |> Map.ofList
let private smallWorldIslandMarketSink (_) (_) = ()
let internal smallWorldDocked = smallWorld |> World.Dock (fun()->commodities) (fun()->smallWorldItems) smallWorldIslandMarketSource smallWorldIslandMarketSink smallWorldIslandItemSource smallWorldIslandItemSink random smallWorldconfiguration.RewardRange smallWorldIslandLocation
let internal shopWorld = smallWorldDocked |> World.ClearMessages avatarId

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar avatarId (fun avatar -> {avatar with Job=Some { FlavorText="";Reward=0.0; Destination=(0.0,0.0)}} |> Some)



