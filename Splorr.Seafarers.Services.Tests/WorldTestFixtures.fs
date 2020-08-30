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
let internal defaultRewardrange = (1.0,10.0)
let internal fabricatedDestinationList = [(0.0, 0.0)] |> Set.ofList

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

type TestWorldDockContext
        (
            avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource,
            avatarJobSink                  : AvatarJobSink,
            avatarJobSource                : AvatarJobSource,
            avatarMessageSink              : AvatarMessageSink,
            avatarSingleMetricSink         : AvatarSingleMetricSink,
            avatarSingleMetricSource       : AvatarSingleMetricSource,
            commoditySource                : CommoditySource ,
            islandItemSink                 : IslandItemSink ,
            islandItemSource               : IslandItemSource, 
            islandJobSink                  : IslandJobSink,
            islandJobSource                : IslandJobSource,
            islandMarketSink               : IslandMarketSink ,
            islandMarketSource             : IslandMarketSource, 
            islandSource                   : IslandSource,
            itemSource                     : ItemSource ,
            shipmateSingleStatisticSink    : ShipmateSingleStatisticSink,
            shipmateSingleStatisticSource  : ShipmateSingleStatisticSource,
            termSources                    : TermSources,
            worldSingleStatisticSource     : WorldSingleStatisticSource
        ) =
    interface WorldDockContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource= avatarIslandSingleMetricSource
        member _.avatarJobSink                  : AvatarJobSink= avatarJobSink
        member _.avatarJobSource                : AvatarJobSource=avatarJobSource
        member _.avatarMessageSink              : AvatarMessageSink=avatarMessageSink
        member _.avatarSingleMetricSink         : AvatarSingleMetricSink=avatarSingleMetricSink
        member _.avatarSingleMetricSource       : AvatarSingleMetricSource=avatarSingleMetricSource
        member _.commoditySource                : CommoditySource =commoditySource
        member _.islandItemSink                 : IslandItemSink =islandItemSink
        member _.islandItemSource               : IslandItemSource=islandItemSource
        member _.islandMarketSink               : IslandMarketSink =islandMarketSink
        member _.islandMarketSource             : IslandMarketSource=islandMarketSource
        member _.islandSource                   : IslandSource=islandSource
        member _.itemSource                     : ItemSource =itemSource
        member _.shipmateSingleStatisticSink    : ShipmateSingleStatisticSink=shipmateSingleStatisticSink
        member _.shipmateSingleStatisticSource  : ShipmateSingleStatisticSource=shipmateSingleStatisticSource
    interface IslandJobsGenerationContext with
        member _.islandJobSink              : IslandJobSink=islandJobSink
        member _.islandJobSource            : IslandJobSource=islandJobSource
    interface JobCreationContext with 
        member _.termSources                : TermSources = termSources
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource
        
