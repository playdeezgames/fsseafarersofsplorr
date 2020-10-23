﻿module Contexts

open Splorr.Seafarers.Services
open Splorr.Tests.Common
open Splorr.Seafarers.Models

type TestContext() = 
    interface ServiceContext
    interface WorldStatistic.GetStatisticContext with
        member val worldSingleStatisticSource = ref (Fakes.Source("WorldStatistic.GetStatisticContext", {MaximumValue = 0.0; MinimumValue=0.0; CurrentValue=0.0}))
    interface WorldMessages.ClearMessagesContext with
        member val avatarMessagePurger = ref (Fakes.Sink "WorldMessages.ClearMessagesContext")
    interface AvatarJob.GetContext with
        member val avatarJobSource = ref (Fakes.Source ("AvatarJob.GetContext",None))
    interface AvatarMessages.AddContext with
        member val avatarMessageSink = ref (Fakes.Sink "AvatarMessages.AddContext")
    interface ShipmateStatistic.GetContext with
        member val shipmateSingleStatisticSource = ref (Fakes.Source ("ShipmateStatistic.GetContext", None))
    interface AvatarMetric.GetMetricContext with
        member val avatarSingleMetricSource = ref (Fakes.Source ("AvatarMetric.GetMetricContext", 0UL))
    interface AvatarMetric.SetMetricContext with
        member val avatarSingleMetricSink = ref (Fakes.Sink "AvatarMetric.SetMetricContext")
    interface Avatar.SetJobContext with
        member val avatarJobSink = ref (Fakes.Sink "Avatar.SetJobContext")
    interface ShipmateStatistic.PutContext with
        member val shipmateSingleStatisticSink = ref (Fakes.Sink "ShipmateStatistic.PutContext")
    interface Island.GetListContext with
        member val islandSource = ref (Fakes.Source ("Island.GetListContext", []))
    interface WorldIslands.GetIslandJobContext with
        member val islandSingleJobSource = ref (Fakes.Source ("WorldIslands.GetIslandJobContext", None))
    interface AvatarIslandMetric.GetContext with
        member val avatarIslandSingleMetricSource = ref (Fakes.Source ("AvatarIslandMetric.GetContext", None))
    interface AvatarIslandMetric.PutContext with
        member val avatarIslandSingleMetricSink =  ref (Fakes.Sink "AvatarIslandMetric.PutContext")
    interface IslandJob.PurgeContext with
        member val islandJobPurger = ref (Fakes.Sink "IslandJob.PurgeContext")
    interface AvatarIslandFeature.GetFeatureContext with
        member val avatarIslandFeatureSource = ref (Fakes.Source ("AvatarIslandFeature.GetFeatureContext", None))
    interface AvatarGamblingHand.GetContext with
        member val avatarGamblingHandSource = ref (Fakes.Source ("AvatarGamblingHand.GetContext", None))
    interface Island.GetStatisticContext with
        member val islandSingleStatisticSource = ref (Fakes.Source ("Island.GetStatisticContext", None))
    interface AvatarGamblingHand.SetContext with
        member val avatarGamblingHandSink = ref (Fakes.Sink "AvatarGamblingHand.SetContext")
    interface Item.GetListContext with
        member val itemSource = ref (Fakes.Source ("Item.GetListContext", Map.empty))
    interface Item.GetContext with
        member val itemSingleSource = ref (Fakes.Source ("Item.GetContext", None))
    interface Commodity.GetCommoditiesContext with
        member val commoditySource = ref (Fakes.Source ("Commodity.GetCommoditiesContext", Map.empty))
    interface IslandMarket.DeterminePriceContext with
        member val islandMarketSource = ref (Fakes.Source ("IslandMarket.DeterminePriceContext", Map.empty))
    interface Vessel.GetStatisticContext with
        member val vesselSingleStatisticSource = ref (Fakes.Source ("Vessel.GetStatisticContext", None))
    interface AvatarInventory.GetInventoryContext with
        member val avatarInventorySource = ref (Fakes.Source ("AvatarInventory.GetInventoryContext",Map.empty))
    interface Island.GetIslandMarketContext with
        member val islandSingleMarketSource = ref (Fakes.Source ("Island.GetIslandMarketContext", None))
    interface Island.PutIslandMarketContext with
        member val islandSingleMarketSink = ref (Fakes.Sink "Island.PutIslandMarketContext")
    interface AvatarInventory.SetInventoryContext with
        member val avatarInventorySink = ref (Fakes.Sink "AvatarInventory.SetInventoryContext")
    interface Vessel.SetStatisticContext with
        member val vesselSingleStatisticSink = ref (Fakes.Sink "Vessel.SetStatisticContext")
    interface AvatarShipmates.GetShipmatesContext with
        member val avatarShipmateSource = ref (Fakes.Source ("AvatarShipmates.GetShipmatesContext", []))