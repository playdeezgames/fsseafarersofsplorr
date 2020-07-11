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
let internal deadDockWorld =
    {dockWorld with Avatars = Map.empty |> Map.add avatarId {dockWorld.Avatars.[avatarId] with Health={dockWorld.Avatars.[avatarId].Health with CurrentValue=dockWorld.Avatars.[avatarId].Health.MinimumValue}}}
let internal deadDockLocation = dockLocation
let internal smallWorldCommodities:Map<uint, CommodityDescriptor> = 
    [(1u, {Name="commodity under test";BasePrice=1.0;PurchaseFactor=1.0;SaleFactor=1.0;Discount=0.5})] |> Map.ofList
let internal smallWorldItems:Map<uint, ItemDescriptor> =
    [(1u, {DisplayName="item under test"; Commodities=[(1u,1.0)]|>Map.ofList; Occurrence =1.0})] |> Map.ofList
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
let internal smallWorldDocked = smallWorld |> World.Dock random smallWorldIslandLocation avatarId
let internal shopWorld = smallWorldDocked |> World.ClearMessages avatarId

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar avatarId (fun avatar -> {avatar with Job=Some { FlavorText="";Reward=0.0; Destination=(0.0,0.0)}} |> Some)



