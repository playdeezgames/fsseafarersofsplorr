module AtSeaTestFixtures

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open CommonTestFixtures
open System

let internal worldSingleStatisticSourceStub (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=30.0; MaximumValue=30.0; CurrentValue=30.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "soloIslandSingleStatisticSource")
let internal configuration: WorldConfiguration =
    {
        WorldSize              = (10.0, 10.0)
    }

let private random = Random()

let internal world = 
    World.Create 
        nameSource
        worldSingleStatisticSourceStub
        shipmateStatisticTemplateSourceStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        configuration 
        random 
        avatarId

let internal deadWorld =
    world
    |> World.TransformAvatar
        (Avatar.TransformShipmate (Shipmate.TransformStatistic 
                ShipmateStatisticIdentifier.Health 
                (fun statistic-> 
                    {statistic with 
                        CurrentValue = statistic.MinimumValue} 
                    |> Some)) Primary
            >> Some)

let internal emptyWorldSingleStatisticSource (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=0.0; MaximumValue=0.0; CurrentValue=0.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=30.0; MaximumValue=30.0; CurrentValue=30.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "soloIslandSingleStatisticSource")
let internal emptyWorldconfiguration: WorldConfiguration =
    {
        WorldSize              = (1.0, 1.0)
    }

let internal emptyWorld = 
    World.Create 
        nameSource
        emptyWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        emptyWorldconfiguration 
        random
        avatarId

let internal dockWorldSingleStatisticSource (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=1.0; MaximumValue=1.0; CurrentValue=1.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=30.0; MaximumValue=30.0; CurrentValue=30.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "soloIslandSingleStatisticSource")
let internal dockWorldconfiguration: WorldConfiguration =
    {
        WorldSize              = (0.0, 0.0)
    }

let internal dockWorld = 
    World.Create 
        nameSource
        dockWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        dockWorldconfiguration 
        random
        avatarId

let internal commoditySourceStub () = Map.empty

let internal headForWorldUnvisited = 
    World.Create 
        nameSource
        dockWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        dockWorldconfiguration 
        random
        avatarId
    |> World.TransformIsland 
        (0.0,0.0) 
        (Island.SetName "yermom" >> Some)
    |> World.Move 
        vesselSingleStatisticSourceStub 
        vesselSingleStatisticSinkStub 
        shipmateRationItemSourceStub
        avatarMessageSinkStub
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
        termSources
        commoditySourceStub 
        itemSourceStub 
        dockWorldSingleStatisticSource
        headForWorldIslandMarketSource 
        headForWorldIslandMarketSink 
        headForWorldIslandItemSource 
        headForWorldIslandItemSink 
        avatarMessageSinkStub
        random 
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


