module WorldTestFixtures

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
        Avatars = 
            [avatarId,{
                Job = None
                Inventory = Map.empty
                Metrics = Map.empty
            }
            ] 
            |> Map.ofList
        Islands = Map.empty
    }
let internal defaultRewardrange = (1.0,10.0)
let internal fabricatedDestinationList = [(0.0, 0.0)] |> Set.ofList
let internal oneIslandWorld = 
    emptyWorld
    |> World.SetIsland (0.0,0.0) (Island.Create() |> Island.SetName "Uno" |> Some)
    |> World.TransformIsland  (0.0,0.0) (fun i -> {i with Jobs = [ Job.Create termSources soloIslandSingleStatisticSource random fabricatedDestinationList ]} |> Some)

let internal commodities = 
    Map.empty
    |> Map.add 1UL {
        CommodityId = 1UL
        CommodityName=""
        BasePrice=1.0
        PurchaseFactor=1.0
        SaleFactor=1.0
        Discount=0.5}
let internal commoditySource() = commodities

let internal genericWorldItems = 
    Map.empty
    |> Map.add 1UL {
        ItemId = 1UL
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
        termSources 
        (fun()->commodities) 
        (fun()->genericWorldItems) 
        genericWorldSingleStatisticSource
        genericWorldIslandMarketSource 
        genericWorldIslandMarketSink 
        genericWorldIslandItemSource 
        genericWorldIslandItemSink 
        genericWorldShipmateSingleStatisticSource
        genericWorldShipmateSingleStatisticSink
        avatarMessageSinkStub 
        random 
        genericWorldIslandLocation 
        genericWorld

let internal shopWorld = 
    genericDockedWorld
let internal shopWorldLocation = genericWorldIslandLocation
let internal shopWorldBogusLocation = genericWorldInvalidIslandLocation

let internal jobWorld = genericDockedWorld |> World.AcceptJob avatarMessageSinkStub 1u genericWorldIslandLocation
let internal jobLocation = jobWorld.Avatars.[avatarId].Job.Value.Destination

let internal headForWorld =
    {
        oneIslandWorld with 
            Avatars =
                oneIslandWorld.Avatars 
                |> Map.add 
                    avatarId 
                    oneIslandWorld.Avatars.[avatarId]
    }

