﻿module WorldTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

let internal bogusAvatarId = "bogus"
let internal random = System.Random()
let internal soloIslandSingleStatisticSource (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=30.0; MaximumValue=30.0; CurrentValue=30.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | WorldStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.5}
    | WorldStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "soloIslandSingleStatisticSource")
let private vesselStatisticTemplateSourceStub () = Map.empty
let private vesselStatisticSinkStub (_) (_) = ()
let private vesselSingleStatisticSourceStub (_) (identifier:VesselStatisticIdentifier) =
    match identifier with
    | VesselStatisticIdentifier.ViewDistance ->
        {MinimumValue=10.0; CurrentValue=10.0; MaximumValue=10.0} |> Some
    | VesselStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
    | VesselStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; CurrentValue=50.0; MaximumValue=100.0} |> Some
    | _ -> None
let internal soloIslandWorld = 
    World.Create 
        avatarIslandSingleMetricSinkStub
        avatarJobSinkStub
        nameSource
        soloIslandSingleStatisticSource
        shipmateStatisticTemplateSource
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random 
        avatarId
let internal emptyWorld = 
    {
        AvatarId = avatarId
        Islands = Map.empty
    }
let internal defaultRewardrange = (1.0,10.0)
let internal fabricatedDestinationList = [(0.0, 0.0)] |> Set.ofList
let internal oneIslandWorld = 
    emptyWorld
    |> World.SetIsland (0.0,0.0) (Island.Create() |> Island.SetName "Uno" |> Some)
    |> World.TransformIsland  (0.0,0.0) (fun i -> {i with Jobs = [ Job.Create termSources soloIslandSingleStatisticSource random fabricatedDestinationList ]} |> Some)

let internal commoditySource() = 
    Map.empty
    |> Map.add 
        1UL 
        {
            CommodityName=""
            BasePrice=1.0
            PurchaseFactor=1.0
            SaleFactor=1.0
            Discount=0.5
        }

let internal genericWorldItemSource () = 
    Map.empty
    |> Map.add 
        1UL 
        {
            ItemName="item under test"
            Commodities= Map.empty |> Map.add 1UL 1.0
            Occurrence=1.0
            Tonnage = 1.0
        }

let internal genericWorldSingleStatisticSource (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=500.0; MaximumValue=500.0; CurrentValue=500.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=5.0; MaximumValue=5.0; CurrentValue=5.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | WorldStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=11.0; CurrentValue=5.5}
    | WorldStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=11.0; CurrentValue=5.5}
    | _ ->
        raise (System.NotImplementedException "soloIslandSingleStatisticSource")

let internal genericWorld = 
    World.Create 
        avatarIslandSingleMetricSinkStub
        avatarJobSinkStub
        nameSource
        genericWorldSingleStatisticSource
        shipmateStatisticTemplateSource
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random 
        avatarId
let internal deadWorld =
    genericWorld

let internal genericWorldIslandLocation = genericWorld.Islands |> Map.toList |> List.map fst |> List.head
let internal genericWorldInvalidIslandLocation = ((genericWorldIslandLocation |> fst) + 1.0, genericWorldIslandLocation |> snd)
let private genericWorldIslandItemSource (_:Location) = Set.empty
let private genericWorldIslandItemSink (_) (_) = ()
let private genericWorldIslandMarketSource (_:Location) = Map.empty
let private genericWorldIslandMarketSink (_) (_) = ()
let private genericWorldShipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
    match identifier with
    | ShipmateStatisticIdentifier.Turn ->
        Statistic.Create(0.0, 50000.0) 0.0 |> Some
    | _ ->
        raise (System.NotImplementedException "genericWorldShipmateSingleStatisticSource")
        None
let private genericWorldShipmateSingleStatisticSink (_) (_) (_) = 
    ()
let internal genericDockedWorld = 
    World.Dock
        avatarIslandSingleMetricSinkStub
        avatarIslandSingleMetricSourceStub
        avatarJobSinkStub
        avatarJobSourceStub
        avatarMessageSinkStub 
        avatarSingleMetricSinkStub
        avatarSingleMetricSourceStub
        commoditySource
        genericWorldIslandItemSink 
        genericWorldIslandItemSource 
        genericWorldIslandMarketSink 
        genericWorldIslandMarketSource 
        genericWorldItemSource 
        genericWorldShipmateSingleStatisticSink
        genericWorldShipmateSingleStatisticSource
        termSources 
        genericWorldSingleStatisticSource
        random 
        genericWorldIslandLocation 
        genericWorld

let internal shopWorld = 
    genericDockedWorld
let internal shopWorldLocation = genericWorldIslandLocation
let internal shopWorldBogusLocation = genericWorldInvalidIslandLocation

let internal jobWorld = 
    genericDockedWorld 
    |> World.AcceptJob 
        avatarIslandSingleMetricSinkStub
        avatarIslandSingleMetricSourceStub
        avatarJobSinkStub
        avatarJobSourceStub
        avatarMessageSinkStub 
        avatarSingleMetricSinkStub
        avatarSingleMetricSourceStub
        1u 
        genericWorldIslandLocation


let internal headForWorld =
    oneIslandWorld

