module ConfirmQuitTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

let internal configuration: WorldConfiguration =
    {
        AvatarDistances = (10.0,1.0)
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0
        MaximumGenerationTries=10u
        RewardRange = (1.0,10.0)
        RationItems = [1UL]
        StatisticDescriptors = []
    }
let internal previousState = 
    World.Create configuration (System.Random()) ""
    |> Gamestate.AtSea


