module AtSeaTestFixtures

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open CommonTestFixtures

let internal configuration: WorldConfiguration =
    {
        AvatarDistances = (10.0,1.0)
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=10u
        RewardRange = (1.0, 10.0)
        RationItems = [1UL]
        StatisticDescriptors = statisticDescriptors
    }
let internal world = World.Create configuration (System.Random()) avatarId
let internal deadWorld =
    world
    |> World.TransformAvatar
        (fun a -> 
            a 
            |> Avatar.TransformShipmate (Shipmate.TransformStatistic 
                AvatarStatisticIdentifier.Health 
                (fun x-> 
                    {x with 
                        CurrentValue = x.MinimumValue} 
                    |> Some)) 0u
            |> Some)
    |> World.ClearMessages
    |> World.AddMessages ["Yer ded."]


let internal emptyWorldconfiguration: WorldConfiguration =
    {
        AvatarDistances = (10.0,1.0)
        WorldSize=(1.0, 1.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=0u
        RewardRange = (1.0, 10.0)
        RationItems = [1UL]
        StatisticDescriptors = statisticDescriptors
    }
let internal emptyWorld = World.Create emptyWorldconfiguration (System.Random()) avatarId

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
let internal dockWorld = World.Create dockWorldconfiguration (System.Random()) avatarId
let internal commodities = Map.empty
let internal headForWorldUnvisited = 
    World.Create dockWorldconfiguration (System.Random()) avatarId
    |> World.TransformIsland (0.0,0.0) (Island.SetName "yermom" >> Some)
    |> World.Move 1u
let private headForWorldIslandItemSource (_) = [1UL] |> Set.ofList
let private headForWorldIslandItemSink (_) (_) = ()
let private headForWorldIslandMarketSource (_) = [1UL, {Supply=5.0; Demand=5.0}] |> Map.ofList
let private headForWorldIslandMarketSink (_) (_) = ()
let internal headForWorldVisited = 
    headForWorldUnvisited
    |> World.Dock (fun()->commodities) (fun()->Map.empty) headForWorldIslandMarketSource headForWorldIslandMarketSink headForWorldIslandItemSource headForWorldIslandItemSink random dockWorldconfiguration.RewardRange (0.0, 0.0)

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar (fun avatar -> {avatar with Job=Some { FlavorText=""; Reward=0.0; Destination=(0.0,0.0)  }}|>Some)
let internal atSeaIslandItemSource (_) = Set.empty
let internal atSeaIslandItemSink (_) (_) = ()
let internal atSeaIslandMarketSource (_) = Map.empty
let internal atSeaIslandMarketSink (_) (_) = ()
let internal atSeaCommoditySource (_) = Map.empty
let internal atSeaItemSource (_) = Map.empty


