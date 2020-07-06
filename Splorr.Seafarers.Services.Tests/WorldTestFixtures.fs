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
                Satiety = Statistic.Create (0.0, 100.0) (100.0)
                Health = Statistic.Create (0.0, 100.0) (100.0)
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

let internal genericWorldCommodities = 
    Map.empty
    |> Map.add Grain {
        Name=""
        BasePrice=1.0
        PurchaseFactor=1.0
        SaleFactor=1.0
        Discount=0.5
        Occurrence=1.0}

let internal genericWorldItems = 
    Map.empty
    |> Map.add Ration {
        DisplayName="item under test"
        Commodities= Map.empty |> Map.add Grain 1.0
        Occurrence=1.0
        }

let internal genericWorldConfiguration: WorldGenerationConfiguration =
    {
        WorldSize=(11.0, 11.0)
        MinimumIslandDistance=5.0 
        MaximumGenerationTries=500u
        RewardRange=(1.0,10.0)
        Commodities = genericWorldCommodities
        Items = genericWorldItems
    }
let internal genericWorld = World.Create genericWorldConfiguration random
let internal deadWorld =
    {genericWorld with Avatar = {genericWorld.Avatar with Health ={genericWorld.Avatar.Health with CurrentValue = genericWorld.Avatar.Health.MinimumValue}}}

let internal genericWorldIslandLocation = genericWorld.Islands |> Map.toList |> List.map fst |> List.head
let internal genericWorldInvalidIslandLocation = ((genericWorldIslandLocation |> fst) + 1.0, genericWorldIslandLocation |> snd)
let internal genericDockedWorld = World.Dock random genericWorldIslandLocation genericWorld |> World.ClearMessages

let internal shopWorld = 
    genericDockedWorld
    |> World.TransformIsland genericWorldIslandLocation
        (fun i -> 
            {i with Markets = i.Markets |> Map.add Grain {Supply=5.0;Demand=5.0;Traded=true}} |> Some)
let internal shopWorldLocation = genericWorldIslandLocation
let internal shopWorldBogusLocation = genericWorldInvalidIslandLocation

let internal jobWorld = genericDockedWorld |> World.AcceptJob 1u genericWorldIslandLocation |> World.ClearMessages
let internal jobLocation = jobWorld.Avatar.Job.Value.Destination

let internal headForWorld =
    {oneIslandWorld with Avatar = {oneIslandWorld.Avatar with Position = (1.0,0.0)}}

