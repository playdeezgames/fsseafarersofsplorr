module DockedTestFixtures

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open CommonTestFixtures

let internal dockWorldconfiguration: WorldConfiguration =
    {
        WorldSize=(0.0, 0.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=1u
        RewardRange = (1.0, 10.0)
    }
let internal dockWorld = World.Create dockWorldconfiguration random
let internal dockLocation = (0.0, 0.0)
let internal deadDockWorld =
    {dockWorld with Avatars = Map.empty |> Map.add avatarId {dockWorld.Avatars.[avatarId] with Health={dockWorld.Avatars.[avatarId].Health with CurrentValue=dockWorld.Avatars.[avatarId].Health.MinimumValue}}}
let internal deadDockLocation = dockLocation
let internal commodities:Map<uint64, CommodityDescriptor> = 
    [(1UL, {CommodityId = 1UL; CommodityName="commodity under test";BasePrice=1.0;PurchaseFactor=1.0;SaleFactor=1.0;Discount=0.5})] |> Map.ofList
let internal smallWorldItems:Map<uint64, ItemDescriptor> =
    [(1UL, {ItemId = 1UL;ItemName="item under test"; Commodities=[(1UL,1.0)]|>Map.ofList; Occurrence =1.0; Tonnage=1.0})] |> Map.ofList
let internal smallWorldconfiguration: WorldConfiguration =
    {
        WorldSize=(11.0, 11.0)
        MinimumIslandDistance=5.0
        MaximumGenerationTries=500u
        RewardRange = (1.0, 10.0)
    }
let internal smallWorld = World.Create smallWorldconfiguration random
let internal smallWorldIslandLocation = smallWorld.Islands |> Map.toList |> List.map fst |> List.head
let internal smallWorldDocked = smallWorld |> World.Dock random commodities smallWorldItems smallWorldIslandLocation avatarId
let internal shopWorld = smallWorldDocked |> World.ClearMessages avatarId

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar avatarId (fun avatar -> {avatar with Job=Some { FlavorText="";Reward=0.0; Destination=(0.0,0.0)}} |> Some)



