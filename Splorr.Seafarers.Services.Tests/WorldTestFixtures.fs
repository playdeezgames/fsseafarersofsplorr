module WorldTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let internal random = System.Random()
let internal soloIslandWorldConfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0 
        MaximumGenerationTries=10u
        RewardRange=(1.0,10.0)
        Commodities = Map.empty
        Items = Map.empty
    }
let internal soloIslandWorld = World.Create soloIslandWorldConfiguration random

let internal emptyWorld = 
    {
        RewardRange = (1.0,10.0)
        Messages=[]
        Avatar = 
            {
                Position = (0.0,0.0)
                Heading = 0.0
                Speed = 1.0
                ViewDistance = 10.0
                DockDistance = 1.0
                Money = 0.0
                Reputation = 0.0
                Job = None
                Inventory = Map.empty
            }
        Islands = Map.empty
        Turn = 0u
        Commodities = Map.empty
        Items = Map.empty
    }
let internal defaultRewardrange = (1.0,10.0)
let internal fabricatedDestinationList = [(0.0, 0.0)] |> Set.ofList
let internal oneIslandWorld = 
    emptyWorld
    |> World.SetIsland (0.0,0.0) (Island.Create() |> Island.SetName "Uno" |> Some)
    |> World.TransformIsland  (0.0,0.0) (fun i -> {i with Jobs = [ Job.Create random defaultRewardrange fabricatedDestinationList ]} |> Some)

let internal genericWorldConfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(11.0, 11.0)
        MinimumIslandDistance=5.0 
        MaximumGenerationTries=500u
        RewardRange=(1.0,10.0)
        Commodities = Map.empty
        Items = Map.empty
    }
let internal genericWorld = World.Create genericWorldConfiguration random
let internal genericWorldIslandLocation = genericWorld.Islands |> Map.toList |> List.map fst |> List.head
let internal genericWorldInvalidIslandLocation = ((genericWorldIslandLocation |> fst) + 1.0, genericWorldIslandLocation |> snd)
let internal genericDockedWorld = World.Dock random genericWorldIslandLocation genericWorld |> World.ClearMessages

let internal jobWorld = genericDockedWorld |> World.AcceptJob 1u genericWorldIslandLocation |> World.ClearMessages
let internal jobLocation = jobWorld.Avatar.Job.Value.Destination

let internal headForWorld =
    {oneIslandWorld with Avatar = {oneIslandWorld.Avatar with Position = (1.0,0.0)}}

