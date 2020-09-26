module WorldTestFixtures

open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

let internal genericWorldSingleStatisticSource (identifier: WorldStatisticIdentifier) : Statistic =
    match identifier with
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
        raise (NotImplementedException "soloIslandSingleStatisticSource")

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
    interface Commodity.GetCommoditiesContext with
        member this.commoditySource: CommoditySource = commoditySource
    interface World.DockContext with
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member _.avatarIslandFeatureSink        : AvatarIslandFeatureSink = avatarIslandFeatureSink
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource= avatarIslandSingleMetricSource
        member _.avatarJobSink                  : AvatarJobSink= avatarJobSink
        member _.avatarJobSource                : AvatarJobSource=avatarJobSource
        member _.avatarSingleMetricSink         : AvatarSingleMetricSink=avatarSingleMetricSink
        member _.avatarSingleMetricSource       : AvatarSingleMetricSource=avatarSingleMetricSource
        member _.islandItemSink                 : IslandItemSink =islandItemSink
        member _.islandItemSource               : IslandItemSource=islandItemSource
        member _.islandMarketSink               : IslandMarketSink =islandMarketSink
        member _.islandMarketSource             : IslandMarketSource=islandMarketSource
        member _.islandSource                   : IslandSource=islandSource
        member _.itemSource                     : ItemSource =itemSource
        member _.shipmateSingleStatisticSink    : ShipmateSingleStatisticSink=shipmateSingleStatisticSink
    interface AvatarJob.CompleteContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
    interface World.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Avatar.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink

    interface Avatar.AddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource

    interface Shipmate.TransformStatisticContext with
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Island.GenerateItemsContext with
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.itemSource: ItemSource = itemSource
        member _.random: Random = Fixtures.Common.Dummy.Random

    interface Island.GenerateCommoditiesContext with
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.random : Random = Fixtures.Common.Dummy.Random

    interface Island.AddVisitContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource

    interface Island.GenerateJobsContext with
        member _.islandJobSink              : IslandJobSink=islandJobSink
        member _.islandJobSource            : IslandJobSource=islandJobSource

    interface Utility.RandomContext with
        member _.random : Random = Fixtures.Common.Dummy.Random

    interface Job.CreateContext with 
        member _.termSources                : TermSources = termSources
        member this.jobRewardStatisticSource: JobRewardStatisticSource = fun () -> worldSingleStatisticSource WorldStatisticIdentifier.JobReward
        member _.random : Random = Fixtures.Common.Dummy.Random
        

