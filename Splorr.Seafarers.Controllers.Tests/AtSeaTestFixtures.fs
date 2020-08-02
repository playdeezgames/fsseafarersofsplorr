module AtSeaTestFixtures

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open CommonTestFixtures
open System

let internal configuration: WorldConfiguration =
    {
        AvatarDistances        = (10.0, 1.0)
        MaximumGenerationTries = 10u
        MinimumIslandDistance  = 30.0
        RationItems            = [1UL]
        RewardRange            = (1.0, 10.0)
        StatisticDescriptors   = statisticDescriptors
        WorldSize              = (10.0, 10.0)
    }

let private random = Random()

let internal world = 
    World.Create 
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        configuration 
        random 
        avatarId

let internal deadWorld =
    world
    |> World.TransformAvatar
        (Avatar.TransformShipmate (Shipmate.TransformStatistic 
                AvatarStatisticIdentifier.Health 
                (fun statistic-> 
                    {statistic with 
                        CurrentValue = statistic.MinimumValue} 
                    |> Some)) 0u
            >> Some)
    |> World.ClearMessages
    |> World.AddMessages ["Yer ded."]


let internal emptyWorldconfiguration: WorldConfiguration =
    {
        AvatarDistances        = (10.0, 1.0)
        MaximumGenerationTries = 0u
        MinimumIslandDistance  = 30.0
        RationItems            = [ 1UL ]
        RewardRange            = (1.0, 10.0)
        StatisticDescriptors   = statisticDescriptors
        WorldSize              = (1.0, 1.0)
    }

let internal emptyWorld = 
    World.Create 
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        emptyWorldconfiguration 
        random
        avatarId

let internal dockWorldconfiguration: WorldConfiguration =
    {
        AvatarDistances        = (10.0,1.0)
        MaximumGenerationTries = 1u
        MinimumIslandDistance  = 30.0
        RationItems            = [ 1UL ]
        RewardRange            = (1.0, 10.0)
        StatisticDescriptors   = statisticDescriptors
        WorldSize              = (0.0, 0.0)
    }

let internal dockWorld = 
    World.Create 
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        dockWorldconfiguration 
        random
        avatarId

let internal commoditySourceStub () = Map.empty

let internal headForWorldUnvisited = 
    World.Create 
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        dockWorldconfiguration 
        random
        avatarId
    |> World.TransformIsland 
        (0.0,0.0) 
        (Island.SetName "yermom" >> Some)
    |> World.Move 
        vesselSingleStatisticSourceStub 
        vesselSingleStatisticSinkStub 
        1u

let private headForWorldIslandItemSource (_) = 
    [1UL] 
    |> Set.ofList

let private headForWorldIslandItemSink (_) (_) = ()

let private headForWorldIslandMarketSource (_) = 
    [
        1UL, 
            {
                Supply=5.0
                Demand=5.0
            }
    ] 
    |> Map.ofList

let private headForWorldIslandMarketSink (_) (_) = ()

let private itemSourceStub() = Map.empty

let internal headForWorldVisited = 
    headForWorldUnvisited
    |> World.Dock 
        commoditySourceStub 
        itemSourceStub 
        headForWorldIslandMarketSource 
        headForWorldIslandMarketSink 
        headForWorldIslandItemSource 
        headForWorldIslandItemSink 
        random 
        dockWorldconfiguration.RewardRange 
        (0.0, 0.0)

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar 
        (fun avatar -> 
            {avatar with 
                Job = 
                    { 
                        FlavorText  = ""
                        Reward      = 0.0
                        Destination = (0.0, 0.0)  
                    } 
                    |> Some
            }
            |>Some)

let internal atSeaIslandItemSource (_) = 
    Set.empty

let internal atSeaIslandItemSink (_) (_) = ()

let internal atSeaIslandMarketSource (_) = 
    Map.empty

let internal atSeaIslandMarketSink (_) (_) = ()

let internal atSeaCommoditySource (_) = 
    Map.empty

let internal atSeaItemSource (_) = 
    Map.empty


