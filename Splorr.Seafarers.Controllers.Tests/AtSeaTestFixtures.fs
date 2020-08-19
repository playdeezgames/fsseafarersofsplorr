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
    | WorldStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.0}
    | WorldStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=10.0; CurrentValue=5.0}
    | _ ->
        raise (System.NotImplementedException "worldSingleStatisticSourceStub")

let private random = Random()

let internal avatarJobSinkStub (_) (_) = ()
let internal avatarJobSourceStub (_) = None

let internal world = 
    World.Create 
        avatarInventorySinkStub
        termNameSource
        worldSingleStatisticSourceStub
        shipmateStatisticTemplateSourceStub
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random 
        avatarId

let internal deadWorld =
    world
    

let internal emptyWorldSingleStatisticSource (identfier: WorldStatisticIdentifier) : Statistic =
    match identfier with
    | WorldStatisticIdentifier.IslandGenerationRetries ->
        {MinimumValue=0.0; MaximumValue=0.0; CurrentValue=0.0}
    | WorldStatisticIdentifier.IslandDistance ->
        {MinimumValue=30.0; MaximumValue=30.0; CurrentValue=30.0}
    | WorldStatisticIdentifier.JobReward ->
        {MinimumValue=1.0; MaximumValue=10.0; CurrentValue=5.5}
    | WorldStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=5.0}
    | WorldStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=5.0}

    | _ ->
        raise (System.NotImplementedException "emptyWorldSingleStatisticSource")

let internal emptyWorld = 
    World.Create 
        avatarJobSinkStub
        termNameSource
        emptyWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
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
    | WorldStatisticIdentifier.PositionX ->
        {MinimumValue=0.0; MaximumValue=0.0; CurrentValue=5.0}
    | WorldStatisticIdentifier.PositionY ->
        {MinimumValue=0.0; MaximumValue=0.0; CurrentValue=5.0}
    | _ ->
        raise (System.NotImplementedException (sprintf "dockWorldSingleStatisticSource %s" (identfier.ToString())))
let internal dockWorld = 
    World.Create 
        avatarJobSinkStub
        termNameSource
        dockWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random
        avatarId

let internal commoditySourceStub () = Map.empty

let internal headForWorldUnvisited = 
    World.Create 
        avatarJobSinkStub
        termNameSource
        dockWorldSingleStatisticSource
        shipmateStatisticTemplateSourceStub
        shipmateRationItemSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random
        avatarId
    |> World.TransformIsland 
        (0.0,0.0) 
        (Island.SetName "yermom" >> Some)

    |> World.Move
        avatarInventorySinkStub
        avatarInventorySourceStub
        avatarMessageSinkStub
        avatarShipmateSourceStub
        avatarSingleMetricSinkStub
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub
        shipmateSingleStatisticSinkStub
        shipmateSingleStatisticSourceStub
        vesselSingleStatisticSinkStub 
        vesselSingleStatisticSourceStub 
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
        avatarJobSinkStub
        avatarJobSourceStub
        avatarMessageSinkStub
        avatarSingleMetricSinkStub
        avatarSingleMetricSourceStub
        commoditySourceStub 
        headForWorldIslandItemSink 
        headForWorldIslandItemSource 
        headForWorldIslandMarketSink 
        headForWorldIslandMarketSource 
        itemSourceStub 
        shipmateSingleStatisticSinkStub
        shipmateSingleStatisticSourceStub
        termSources
        dockWorldSingleStatisticSource
        random 
        (0.0, 0.0)

let internal abandonJobWorld =
    dockWorld
    

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


