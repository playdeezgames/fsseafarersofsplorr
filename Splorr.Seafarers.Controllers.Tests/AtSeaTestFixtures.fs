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
    }
let internal world = World.Create configuration (System.Random())
let internal deadWorld =
    world
    |> World.TransformAvatar avatarId
        (fun a -> {a with Health = {a.Health with CurrentValue = a.Health.MinimumValue}}|>Some)
    |> World.ClearMessages avatarId
    |> World.AddMessages avatarId ["Yer ded."]


let internal emptyWorldconfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(1.0, 1.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=0u
        RewardRange = (1.0, 10.0)
    }
let internal emptyWorld = World.Create emptyWorldconfiguration (System.Random())

let internal dockWorldconfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(0.0, 0.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=1u
        RewardRange = (1.0, 10.0)
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



