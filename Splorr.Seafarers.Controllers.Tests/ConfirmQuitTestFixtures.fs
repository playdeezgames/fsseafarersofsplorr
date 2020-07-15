module ConfirmQuitTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

let internal configuration: WorldGenerationConfiguration =
    {
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=10u
        RewardRange = (1.0,10.0)
        Items = Map.empty
    }
let internal previousState = 
    World.Create configuration (System.Random())
    |> Gamestate.AtSea


