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
let internal avatarIslandSingleMetricSinkStub (_) (_) (_) (_) = ()
let internal avatarIslandSingleMetricSourceStub (_) (_) (_) = None

let internal world = { AvatarId = avatarId }

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

let internal emptyWorld = world

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
let internal dockWorld = world

let internal commoditySourceStub () = Map.empty

let internal headForWorldUnvisited = world

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


