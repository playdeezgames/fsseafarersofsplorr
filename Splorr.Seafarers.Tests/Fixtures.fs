namespace Fixtures

open System

module Dummy = 
    let internal Random = Random() 


module Stub =

    let internal AvatarGamblingHandSink(_) = raise (NotImplementedException "avatarGamblingHandSink")
    let internal AvatarGamblingHandSource(_) = raise (NotImplementedException "avatarGamblingHandSource")
    let internal AvatarInventorySink(_)=raise (NotImplementedException "avatarInventorySink") 
    let internal AvatarInventorySource(_)=raise (NotImplementedException "avatarInventorySource") 
    let internal AvatarIslandFeatureSink (_)=raise (NotImplementedException "avatarIslandFeatureSink") 
    let internal AvatarIslandFeatureSource (_)=raise (NotImplementedException "avatarIslandFeatureSource")
    let internal AvatarIslandSingleMetricSink(_)=raise (NotImplementedException "avatarIslandSingleMetricSink") 
    let internal AvatarIslandSingleMetricSource(_)=raise (NotImplementedException "avatarIslandSingleMetricSource") 
    let internal AvatarJobSink(_)=raise (NotImplementedException "avatarJobSink") 
    let internal AvatarJobSource(_)=raise (NotImplementedException "avatarJobSource") 
    let internal AvatarMessagePurger(_)=raise (NotImplementedException "avatarMessagePurger") 
    let internal AvatarMessageSink(_)=raise (NotImplementedException "avatarMessageSink") 
    let internal AvatarMessageSource(_)=raise (NotImplementedException "avatarMessageSource")
    let internal AvatarMetricSource(_)=raise (NotImplementedException "avatarMetricSource")
    let internal AvatarShipmateSource(_)=raise (NotImplementedException "avatarShipmateSource") 
    let internal AvatarSingleMetricSink(_)=raise (NotImplementedException "avatarSingleMetricSink") 
    let internal AvatarSingleMetricSource(_)=raise (NotImplementedException "avatarSingleMetricSource") 
    let internal CommoditySource(_)=raise (NotImplementedException "commoditySource") 
    let internal EpochSecondsSource(_)=raise (NotImplementedException "epochSecondsSource")
    let internal IslandFeatureGeneratorSource(_)=raise (NotImplementedException "islandFeatureGeneratorSource") 
    let internal IslandFeatureSource(_)=raise (NotImplementedException "islandFeatureSource")
    let internal IslandItemSink(_)=raise (NotImplementedException "islandItemSink") 
    let internal IslandItemSource(_)=raise (NotImplementedException "islandItemSource") 
    let internal IslandJobPurger(_)=raise (NotImplementedException "islandJobPurger") 
    let internal IslandJobSink(_)=raise (NotImplementedException "islandJobSink") 
    let internal IslandJobSource(_)=raise (NotImplementedException "islandJobSource") 
    let internal IslandLocationByNameSource(_)=raise (NotImplementedException "islandLocationByNameSource") 
    let internal IslandMarketSink(_)=raise (NotImplementedException "islandMarketSink") 
    let internal IslandMarketSource(_)=raise (NotImplementedException "islandMarketSource") 
    let internal IslandSingleFeatureSink (_)=raise (NotImplementedException "islandSingleFeatureSink")
    let internal IslandSingleFeatureSource (_)=raise (NotImplementedException "islandSingleFeatureSource")
    let internal IslandSingleJobSource(_)=raise (NotImplementedException "islandSingleJobSource") 
    let internal IslandSingleMarketSink(_)=raise (NotImplementedException "islandSingleMarketSink") 
    let internal IslandSingleMarketSource(_)=raise (NotImplementedException "islandSingleMarketSource") 
    let internal IslandSingleNameSink(_)=raise (NotImplementedException "islandSingleNameSink") 
    let internal IslandSingleNameSource(_)=raise (NotImplementedException "islandSingleNameSource") 
    let internal IslandSingleStatisticSink(_)=raise (NotImplementedException "islandSingleStatisticSink") 
    let internal IslandSingleStatisticSource(_)=raise (NotImplementedException "islandSingleStatisticSource") 
    let internal IslandSource(_)=raise (NotImplementedException "islandSource") 
    let internal IslandStatisticTemplateSource(_)=raise (NotImplementedException "islandStatisticTemplateSource") 
    let internal ItemSingleSource(_)=raise (NotImplementedException "itemSingleSource")
    let internal ItemSource(_)=raise (NotImplementedException "itemSource") 
    let internal RationItemSource(_)=raise (NotImplementedException "rationItemSource") 
    let internal ShipmateRationItemSink(_)=raise (NotImplementedException "shipmateRationItemSink") 
    let internal ShipmateRationItemSource(_)=raise (NotImplementedException "shipmateRationItemSource") 
    let internal ShipmateSingleStatisticSink(_)=raise (NotImplementedException "shipmateSingleStatisticSink") 
    let internal ShipmateSingleStatisticSource(_)=raise (NotImplementedException "shipmateSingleStatisticSource") 
    let internal ShipmateStatisticTemplateSource(_)=raise (NotImplementedException "shipmateStatisticTemplateSource") 
    let internal SwitchSource(_)=raise (NotImplementedException "switchSource")
    let internal TermNameSource(_)=raise (NotImplementedException "termNameSource") 
    let internal TermSources = TermNameSource, TermNameSource, TermNameSource, TermNameSource, TermNameSource, TermNameSource
    let internal VesselSingleStatisticSink(_)=raise (NotImplementedException "vesselSingleStatisticSink") 
    let internal VesselSingleStatisticSource(_)=raise (NotImplementedException "vesselSingleStatisticSource") 
    let internal VesselStatisticSink(_)=raise (NotImplementedException "vesselStatisticSink") 
    let internal VesselStatisticTemplateSource(_)=raise (NotImplementedException "vesselStatisticTemplateSource") 
    let internal WorldSingleStatisticSource (_)=raise (NotImplementedException "worldSingleStatisticSource")



