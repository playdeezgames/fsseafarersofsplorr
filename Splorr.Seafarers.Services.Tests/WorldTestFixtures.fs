module WorldTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

let internal bogusAvatarId = "bogus"
let internal random = System.Random()
let internal soloIslandWorldConfiguration: WorldConfiguration =
    {
        AvatarDistances = (10.0,1.0)
        WorldSize=(10.0, 10.0)
        MinimumIslandDistance=30.0 
        MaximumGenerationTries=10u
        RewardRange=(1.0,10.0)
        RationItems = [1UL]
        StatisticDescriptors = statisticDescriptors
    }
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
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        soloIslandWorldConfiguration 
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
                Shipmates = 
                    Map.empty
                    |> Map.add Primary
                        ({
                            RationItems=[1UL]
                            Statistics = Map.empty
                        }
                        |> Shipmate.SetStatistic ShipmateStatisticIdentifier.Satiety (Statistic.Create (0.0, 100.0) (100.0) |> Some)
                        |> Shipmate.SetStatistic ShipmateStatisticIdentifier.Health (Statistic.Create (0.0, 100.0) (100.0) |> Some)
                        |> Shipmate.SetStatistic ShipmateStatisticIdentifier.Turn ({MinimumValue=0.0;CurrentValue=0.0;MaximumValue=15000.0} |> Some))
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
    |> World.TransformIsland  (0.0,0.0) (fun i -> {i with Jobs = [ Job.Create termSources random defaultRewardrange fabricatedDestinationList ]} |> Some)

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

let internal genericWorldConfiguration: WorldConfiguration =
    {
        AvatarDistances = (10.0,1.0)
        WorldSize=(11.0, 11.0)
        MinimumIslandDistance=5.0 
        MaximumGenerationTries=500u
        RewardRange=(1.0,10.0)
        RationItems = [1UL]
        StatisticDescriptors = statisticDescriptors
    }
let internal genericWorld = 
    World.Create 
        nameSource
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        genericWorldConfiguration 
        random 
        avatarId
let internal deadWorld =
    {genericWorld with 
        Avatars = 
            genericWorld.Avatars 
            |> Map.add 
                avatarId 
                (genericWorld.Avatars.[avatarId] 
                |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Health (fun x -> {x with CurrentValue=x.MinimumValue} |> Some)) Primary )}

let internal genericWorldIslandLocation = genericWorld.Islands |> Map.toList |> List.map fst |> List.head
let internal genericWorldInvalidIslandLocation = ((genericWorldIslandLocation |> fst) + 1.0, genericWorldIslandLocation |> snd)
let private genericWorldIslandItemSource (_:Location) = Set.empty
let private genericWorldIslandItemSink (_) (_) = ()
let private genericWorldIslandMarketSource (_:Location) = Map.empty
let private genericWorldIslandMarketSink (_) (_) = ()
let internal genericDockedWorld = World.Dock termSources (fun()->commodities) (fun()->genericWorldItems) genericWorldIslandMarketSource genericWorldIslandMarketSink genericWorldIslandItemSource genericWorldIslandItemSink avatarMessageSinkStub random genericWorldConfiguration.RewardRange genericWorldIslandLocation genericWorld

let internal shopWorld = 
    genericDockedWorld
let internal shopWorldLocation = genericWorldIslandLocation
let internal shopWorldBogusLocation = genericWorldInvalidIslandLocation

let internal jobWorld = genericDockedWorld |> World.AcceptJob avatarMessageSinkStub 1u genericWorldIslandLocation
let internal jobLocation = jobWorld.Avatars.[avatarId].Job.Value.Destination

let internal headForWorld =
    {oneIslandWorld with Avatars =oneIslandWorld.Avatars |> Map.add avatarId oneIslandWorld.Avatars.[avatarId] (*with Position = (1.0,0.0)*) }

