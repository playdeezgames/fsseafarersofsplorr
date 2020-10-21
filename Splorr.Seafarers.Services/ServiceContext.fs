namespace Splorr.Seafarers.Services

open Splorr.Common

type ServiceContext =
    inherit CommonContext
    inherit Avatar.SetJobContext
    inherit AvatarGamblingHand.GetContext
    inherit AvatarGamblingHand.SetContext
    inherit AvatarIslandFeature.GetFeatureContext
    inherit AvatarIslandMetric.GetContext
    inherit AvatarIslandMetric.PutContext
    inherit AvatarJob.GetContext
    inherit AvatarMessages.AddContext
    inherit AvatarMetric.GetMetricContext
    inherit AvatarMetric.SetMetricContext
    inherit Island.GetListContext
    inherit Island.GetStatisticContext
    inherit IslandJob.PurgeContext
    inherit ShipmateStatistic.GetContext
    inherit ShipmateStatistic.PutContext
    inherit WorldIslands.GetIslandJobContext
    inherit WorldMessages.ClearMessagesContext
    inherit WorldStatistic.GetStatisticContext

