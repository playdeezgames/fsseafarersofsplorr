﻿module WorldTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

let internal bogusAvatarId = "bogus"
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
        Avatars = 
            ["",{
                Messages = []
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
                Turn = {MinimumValue=0.0;CurrentValue=0.0;MaximumValue=15000.0}
                RationItem = 1u
            }] 
            |> Map.ofList
        Islands = Map.empty
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
    |> Map.add 1u {
        Name=""
        BasePrice=1.0
        PurchaseFactor=1.0
        SaleFactor=1.0
        Discount=0.5}

let internal genericWorldItems = 
    Map.empty
    |> Map.add 1u {
        DisplayName="item under test"
        Commodities= Map.empty |> Map.add 1u 1.0
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
    {genericWorld with Avatars = genericWorld.Avatars |> Map.add avatarId {genericWorld.Avatars.[avatarId] with Health ={genericWorld.Avatars.[avatarId].Health with CurrentValue = genericWorld.Avatars.[avatarId].Health.MinimumValue}}}

let internal genericWorldIslandLocation = genericWorld.Islands |> Map.toList |> List.map fst |> List.head
let internal genericWorldInvalidIslandLocation = ((genericWorldIslandLocation |> fst) + 1.0, genericWorldIslandLocation |> snd)
let internal genericDockedWorld = World.Dock random genericWorldIslandLocation avatarId genericWorld |> World.ClearMessages avatarId

let internal shopWorld = 
    genericDockedWorld
    |> World.TransformIsland genericWorldIslandLocation
        (fun i -> 
            {i with Markets = i.Markets |> Map.add 1u {Supply=5.0;Demand=5.0}} |> Some)
let internal shopWorldLocation = genericWorldIslandLocation
let internal shopWorldBogusLocation = genericWorldInvalidIslandLocation

let internal jobWorld = genericDockedWorld |> World.AcceptJob 1u genericWorldIslandLocation avatarId |> World.ClearMessages avatarId
let internal jobLocation = jobWorld.Avatars.[avatarId].Job.Value.Destination

let internal headForWorld =
    {oneIslandWorld with Avatars =oneIslandWorld.Avatars |> Map.add avatarId {oneIslandWorld.Avatars.[avatarId] with Position = (1.0,0.0)}}

