module AtSeaTestFixtures

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open CommonTestFixtures

let internal configuration: WorldConfiguration =
    {
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=10u
        RewardRange = (1.0, 10.0)
        RationItems = [1UL]
    }
let internal world = World.Create configuration (System.Random())
let internal deadWorld =
    world
    |> World.TransformAvatar avatarId
        (fun a -> 
            a 
            |> Avatar.TransformShipmate (Shipmate.TransformStatistic 
                StatisticIdentifier.Health 
                (fun x-> 
                    {x with 
                        CurrentValue = x.MinimumValue} 
                    |> Some)) 0u
            |> Some)
    |> World.ClearMessages avatarId
    |> World.AddMessages avatarId ["Yer ded."]


let internal emptyWorldconfiguration: WorldConfiguration =
    {
        WorldSize=(1.0, 1.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=0u
        RewardRange = (1.0, 10.0)
        RationItems = [1UL]
    }
let internal emptyWorld = World.Create emptyWorldconfiguration (System.Random())

let internal dockWorldconfiguration: WorldConfiguration =
    {
        WorldSize=(0.0, 0.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=1u
        RewardRange = (1.0, 10.0)
        RationItems = [1UL]
    }
let internal dockWorld = World.Create dockWorldconfiguration (System.Random())

let internal commodities = Map.empty
let internal headForWorldUnvisited = 
    World.Create dockWorldconfiguration (System.Random())
    |> World.TransformIsland (0.0,0.0) (Island.SetName "yermom" >> Some)
    |> World.Move 1u avatarId
let internal headForWorldVisited = 
    headForWorldUnvisited
    |> World.Dock random commodities Map.empty (0.0, 0.0) avatarId

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar avatarId (fun avatar -> {avatar with Job=Some { FlavorText=""; Reward=0.0; Destination=(0.0,0.0)  }}|>Some)



