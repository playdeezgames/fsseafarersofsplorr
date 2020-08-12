﻿module DockedTestFixtures

open CommonTestFixtures
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open AtSeaTestFixtures

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

let internal dockLocation : Location = (0.0, 0.0)

let internal deadDockWorld =
    {dockWorld with 
        Avatars = 
            Map.empty 
            |> Map.add 
                avatarId 
                (dockWorld.Avatars.[avatarId] 
                |> Avatar.TransformShipmate (Shipmate.TransformStatistic 
                    ShipmateStatisticIdentifier.Health 
                    (fun x -> 
                        {x with CurrentValue = x.MinimumValue} 
                        |> Some)) Primary)}

let internal deadDockLocation = dockLocation

let internal commoditySource() : Map<uint64, CommodityDescriptor> = 
    [
        1UL, 
            {
                CommodityId    = 1UL
                CommodityName  = "commodity under test"
                BasePrice      = 1.0
                PurchaseFactor = 1.0
                SaleFactor     = 1.0
                Discount       = 0.5
            }
    ] 
    |> Map.ofList

let internal itemSource () : Map<uint64, ItemDescriptor> =
    [
        1UL, 
            {
                ItemId = 1UL
                ItemName="item under test"
                Commodities=
                    Map.empty 
                    |> Map.add 1UL 1.0
                Occurrence =1.0
                Tonnage=1.0
            }
    ] 
    |> Map.ofList

let internal smallWorldSingleStatisticSource (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=500.0; MaximumValue=500.0; CurrentValue=500.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=5.0; MaximumValue=5.0; CurrentValue=5.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "soloIslandSingleStatisticSource")

let internal smallWorldconfiguration: WorldConfiguration =
    {
        WorldSize   = (11.0, 11.0)
    }

let internal smallWorld = 
    World.Create 
        nameSource
        smallWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        smallWorldconfiguration 
        random 
        avatarId

let internal smallWorldIslandLocation = 
    smallWorld.Islands 
    |> Map.toList 
    |> List.map fst 
    |> List.head

let private smallWorldIslandItemSource (_) = 
    Set.empty 
    |> Set.add 1UL

let private smallWorldIslandItemSink (_) (_) = ()

let private smallWorldIslandMarketSource (_) = 
    [
        1UL, 
            {
                Supply=1.0
                Demand=1.0
            }
    ] 
    |> Map.ofList

let private smallWorldIslandMarketSink (_) (_) = ()

let internal smallWorldDocked = 
    smallWorld 
    |> World.Dock 
        termSources
        commoditySource 
        itemSource
        smallWorldSingleStatisticSource
        smallWorldIslandMarketSource 
        smallWorldIslandMarketSink 
        smallWorldIslandItemSource 
        smallWorldIslandItemSink 
        avatarMessageSinkStub
        random 
        smallWorldIslandLocation

let internal shopWorld = smallWorldDocked

let internal abandonJobWorld =
    dockWorld
    |> World.TransformAvatar (fun avatar -> {avatar with Job=Some { FlavorText="";Reward=0.0; Destination=(0.0,0.0)}} |> Some)

let internal dockedItemMarketSourceStub (_) = 
    Map.empty

let internal dockedItemSingleMarketSourceStub (_) (_) = 
    None

let internal dockedItemSingleMarketSinkStub (_) (_) = ()
