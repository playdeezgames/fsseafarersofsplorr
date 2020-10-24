namespace Splorr.Seafarers.Services

open Splorr.Common

type ServiceContext =
    inherit CommonContext
    inherit Avatar.SetJobContext
    inherit AvatarGamblingHand.GetContext
    inherit AvatarGamblingHand.SetContext
    inherit AvatarInventory.GetInventoryContext
    inherit AvatarInventory.SetInventoryContext
    inherit AvatarIslandFeature.GetFeatureContext
    inherit AvatarIslandMetric.GetContext
    inherit AvatarIslandMetric.PutContext
    inherit AvatarJob.GetContext
    inherit AvatarMessages.AddContext
    inherit AvatarMetric.GetMetricContext
    inherit AvatarMetric.SetMetricContext
    inherit AvatarShipmates.GetShipmatesContext
    inherit Commodity.GetCommoditiesContext
    inherit Island.GetIslandMarketContext
    inherit Island.PutIslandMarketContext
    inherit Island.GetListContext
    inherit Island.GetStatisticContext
    inherit Island.GetStatisticTemplatesContext
    inherit Island.SetIslandStatisticContext
    inherit IslandJob.PurgeContext
    inherit IslandMarket.DeterminePriceContext
    inherit Item.GetContext
    inherit Item.GetListContext
    inherit Shipmate.GetGlobalRationItemsContext
    inherit Shipmate.GetStatisticTemplatesContext
    inherit Shipmate.SetRationItemsContext
    inherit ShipmateStatistic.GetContext
    inherit ShipmateStatistic.PutContext
    inherit Utility.GetTermsContext
    inherit Utility.RandomContext
    inherit Vessel.GetStatisticContext
    inherit Vessel.GetStatisticTemplateContext
    inherit Vessel.SetStatisticContext
    inherit WorldCreation.GenerateIslandFeatureContext
    inherit WorldCreation.SetIslandFeatureContext
    inherit WorldCreation.SetIslandNameContext
    inherit WorldIslands.GetIslandJobContext
    inherit WorldMessages.ClearMessagesContext
    inherit WorldStatistic.GetStatisticContext

