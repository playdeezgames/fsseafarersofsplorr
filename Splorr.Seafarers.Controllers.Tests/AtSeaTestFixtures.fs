module AtSeaTestFixtures

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open CommonTestFixtures

let internal configuration: WorldGenerationConfiguration =
    {
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=10u
        RewardRange = (1.0, 10.0)
        Commodities = Map.empty
        Items = Map.empty
    }
let internal world = World.Create configuration (System.Random())


let internal emptyWorldconfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(1.0, 1.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=0u
        RewardRange = (1.0, 10.0)
        Commodities = Map.empty
        Items = Map.empty
    }
let internal emptyWorld = World.Create emptyWorldconfiguration (System.Random())

let internal dockWorldconfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(0.0, 0.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=1u
        RewardRange = (1.0, 10.0)
        Commodities = Map.empty
        Items = Map.empty
    }
let internal dockWorld = World.Create dockWorldconfiguration (System.Random())

let internal headForWorldUnvisited = 
    World.Create dockWorldconfiguration (System.Random())
    |> World.TransformIsland (0.0,0.0) (Island.SetName "yermom" >> Some)
    |> World.Move 1u
let internal headForWorldVisited = 
    headForWorldUnvisited
    |> World.Dock random (0.0, 0.0)

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar (fun avatar -> {avatar with Job=Some { FlavorText=""; Reward=0.0; Destination=(0.0,0.0)  }})



