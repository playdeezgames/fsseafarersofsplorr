module WorldTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures
open System
open NUnit.Framework

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
            avatarIslandFeatureSink        : AvatarIslandFeatureSink,
            avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource,
            avatarJobSink                  : AvatarJobSink,
            avatarJobSource                : AvatarJobSource,
            avatarMessageSink              : AvatarMessageSink,
            avatarSingleMetricSink         : AvatarSingleMetricSink,
            avatarSingleMetricSource       : AvatarSingleMetricSource,
            commoditySource                : CommoditySource ,
            epochSecondsSource             : EpochSecondsSource,
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
        member _.avatarIslandFeatureSink        : AvatarIslandFeatureSink = avatarIslandFeatureSink
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
    interface ShipmateTransformStatisticContext with
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface IslandGenerateItemsContext with
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.itemSource: ItemSource = itemSource
        member _.random: Random = random
    interface IslandGenerateCommoditiesContext with
        member _.commoditySource: CommoditySource = commoditySource
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.random : Random = random
    interface IslandAddVisitContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource
    interface IslandGenerateJobsContext with
        member _.islandJobSink              : IslandJobSink=islandJobSink
        member _.islandJobSource            : IslandJobSource=islandJobSource
    interface UtilitySortListRandomlyContext with
        member _.random : Random = random
    interface JobCreateContext with 
        member _.termSources                : TermSources = termSources
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource
        

