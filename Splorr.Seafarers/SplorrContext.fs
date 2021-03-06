namespace Splorr.Seafarers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open System
open Splorr.Seafarers.Models

type SplorrContext 
        (avatarGamblingHandSink: AvatarGamblingHandSink,
        avatarGamblingHandSource: AvatarGamblingHandSource,
        avatarInventorySink: AvatarInventorySink ,
        avatarInventorySource: AvatarInventorySource ,
        avatarIslandFeatureSink : AvatarIslandFeatureSink, 
        avatarIslandFeatureSource : AvatarIslandFeatureSource,
        avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink ,
        avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource ,
        avatarJobSink: AvatarJobSink ,
        avatarJobSource: AvatarJobSource ,
        avatarMessagePurger: AvatarMessagePurger ,
        avatarMessageSink: AvatarMessageSink ,
        avatarMessageSource: AvatarMessageSource,
        avatarMetricSource: AvatarMetricSource,
        avatarShipmateSource: AvatarShipmateSource ,
        avatarSingleMetricSink: AvatarSingleMetricSink ,
        avatarSingleMetricSource: AvatarSingleMetricSource ,
        commoditySource: CommoditySource ,
        epochSecondsSource: EpochSecondsSource,
        gameDataSink : GameDataSink,
        islandFeatureGeneratorSource: IslandFeatureGeneratorSource ,
        islandFeatureSource: IslandFeatureSource,
        islandItemSink: IslandItemSink ,
        islandItemSource: IslandItemSource ,
        islandJobPurger: IslandJobPurger ,
        islandJobSink: IslandJobSink ,
        islandJobSource: IslandJobSource ,
        islandLocationByNameSource: IslandLocationByNameSource ,
        islandMarketSink: IslandMarketSink ,
        islandMarketSource: IslandMarketSource ,
        islandSingleFeatureSink : IslandSingleFeatureSink,
        islandSingleFeatureSource : IslandSingleFeatureSource,
        islandSingleJobSource: IslandSingleJobSource ,
        islandSingleMarketSink: IslandSingleMarketSink ,
        islandSingleMarketSource: IslandSingleMarketSource ,
        islandSingleNameSink: IslandSingleNameSink ,
        islandSingleNameSource: IslandSingleNameSource ,
        islandSingleStatisticSink: IslandSingleStatisticSink ,
        islandSingleStatisticSource: IslandSingleStatisticSource ,
        islandSource: IslandSource ,
        islandStatisticTemplateSource: IslandStatisticTemplateSource ,
        itemSingleSource: ItemSingleSource,
        itemSource: ItemSource ,
        random : Random,
        rationItemSource: RationItemSource ,
        shipmateRationItemSink: ShipmateRationItemSink ,
        shipmateRationItemSource: ShipmateRationItemSource ,
        shipmateSingleStatisticSink: ShipmateSingleStatisticSink ,
        shipmateSingleStatisticSource: ShipmateSingleStatisticSource ,
        shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource ,
        switchSource: SwitchSource,
        termListSource : TermListSource,
        termNameSource: TermSource ,
        vesselSingleStatisticSink: VesselSingleStatisticSink ,
        vesselSingleStatisticSource: VesselSingleStatisticSource ,
        vesselStatisticSink: VesselStatisticSink ,
        vesselStatisticTemplateSource: VesselStatisticTemplateSource ,
        worldSingleStatisticSource : WorldSingleStatisticSource) =
    interface World.SaveContext with
        member this.gameDataSink: GameDataSink = gameDataSink
    interface Utility.TermGeneratorContext with
        member this.termListSource: TermListSource = termListSource
    interface AvatarIslandMetric.GetContext with
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource

    interface AvatarIslandMetric.PutContext with
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink

    interface Item.GetContext with
        member _.itemSingleSource: ItemSingleSource = itemSingleSource

    interface AvatarIslandFeature.GetContext with
        member _.avatarIslandFeatureSource: AvatarIslandFeatureSource = avatarIslandFeatureSource

    interface AvatarMessages.GetContext with
        member _.avatarMessageSource: AvatarMessageSource = avatarMessageSource

    interface Vessel.GetStatisticContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface ShipmateStatistic.GetContext with
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface AvatarJob.GetContext with
        member _.avatarJobSource: AvatarJobSource = avatarJobSource

    interface IslandName.GetNameContext with
        member _.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource

    interface Island.GetItemsContext with
        member _.islandItemSource: IslandItemSource = islandItemSource

    interface Item.GetListContext with
        member _.itemSource: ItemSource = itemSource

    interface AvatarInventory.GetInventoryContext with
        member _.avatarInventorySource: AvatarInventorySource = avatarInventorySource

    interface World.UndockContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink
        member _.avatarIslandFeatureSink : AvatarIslandFeatureSink = avatarIslandFeatureSink

    interface Commodity.GetCommoditiesContext with
        member _.commoditySource: CommoditySource = commoditySource

    interface IslandJob.AddContext with
        member _.islandJobSink: IslandJobSink = islandJobSink

    interface Island.ChangeMarketContext with
        member _.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member _.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource
    
    interface Island.GetListContext with
        member _.islandSource: IslandSource = islandSource
    
    interface IslandJob.GetContext with
        member _.islandJobSource: IslandJobSource = islandJobSource

    interface Vessel.TransformFoulingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink

    interface Shipmate.EatContext with
        member _.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource

    interface Vessel.GetSpeedContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Vessel.GetHeadingContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Vessel.SetPositionContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Vessel.SetSpeedContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Vessel.SetHeadingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.EatContext with
        member _.avatarInventorySink           : AvatarInventorySink=avatarInventorySink
        member _.avatarInventorySource         : AvatarInventorySource=avatarInventorySource
        member _.avatarShipmateSource          : AvatarShipmateSource=avatarShipmateSource

    interface AvatarShipmates.TransformContext with
        member _.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource

    interface World.AddMessagesContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink

    interface World.ClearMessagesContext with
        member _.avatarMessagePurger: AvatarMessagePurger = avatarMessagePurger
        
    interface Avatar.MoveContext with
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource
    interface World.DistanceToContext with
        member _.islandLocationByNameSource     : IslandLocationByNameSource = islandLocationByNameSource
    interface World.HeadForContext with
        member _.islandLocationByNameSource     : IslandLocationByNameSource = islandLocationByNameSource

    interface World.GetNearbyLocationsContext with
        member _.islandSource : IslandSource = islandSource

    interface Vessel.GetCurrentFoulingContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Island.CreateContext with
        member _.islandSingleStatisticSink     : IslandSingleStatisticSink    =islandSingleStatisticSink    
        member _.islandStatisticTemplateSource : IslandStatisticTemplateSource=islandStatisticTemplateSource

    interface Utility.RandomContext with
        member _.random : Random = random

    interface World.PopulateIslandsContext with
        member _.islandFeatureGeneratorSource : IslandFeatureGeneratorSource=islandFeatureGeneratorSource
        member _.islandSingleFeatureSink      : IslandSingleFeatureSink     =islandSingleFeatureSink     
        member _.islandSource                 : IslandSource                =islandSource     

    interface World.NameIslandsContext with
        member _.islandSingleNameSink: IslandSingleNameSink = islandSingleNameSink
        member _.islandSource: IslandSource = islandSource
        member _.nameSource: TermSource = termNameSource

    interface World.GenerateIslandsContext with
        member _.islandSingleNameSink          : IslandSingleNameSink=islandSingleNameSink
        member _.termNameSource                : TermSource          =termNameSource     
        member _.islandSource : IslandSource = islandSource

    interface Vessel.CreateContext with
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource

    interface Shipmate.CreateContext with
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
    interface Shipmate.GetStatisticTemplatesContext with
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource

    interface Avatar.CreateContext with
        member _.avatarJobSink: AvatarJobSink = avatarJobSink

    interface Vessel.GetPositionContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface World.CreateContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarJobSink: AvatarJobSink = avatarJobSink
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource
        member _.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource

    interface World.UpdateChartsContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.islandSource: IslandSource = islandSource
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Island.GetFeaturesContext with
        member _.islandFeatureSource            : IslandFeatureSource            = islandFeatureSource

    interface IslandMarket.DeterminePriceContext with
        member _.islandMarketSource             : IslandMarketSource             =islandMarketSource     

    interface Island.UpdateMarketForItemContext

    interface AvatarInventory.RemoveInventoryContext with
        member _.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource: AvatarInventorySource = avatarInventorySource

    interface AvatarInventory.GetUsedTonnageContext with
        member _.avatarInventorySource: AvatarInventorySource = avatarInventorySource

    interface AvatarJob.AbandonContext with
        member _.avatarJobSink: AvatarJobSink = avatarJobSink
        member _.avatarJobSource: AvatarJobSource = avatarJobSource
        
    interface AvatarInventory.GetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource

    interface AvatarInventory.AddInventoryContext with
        member _.avatarInventorySink   : AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource

    interface IslandJob.PurgeContext with
        member _.islandJobPurger       : IslandJobPurger = islandJobPurger

    interface World.AcceptJobContext with
        member _.avatarJobSink         : AvatarJobSink = avatarJobSink
        member _.avatarJobSource       : AvatarJobSource = avatarJobSource
        member _.islandSingleJobSource : IslandSingleJobSource = islandSingleJobSource
        member _.islandSource          : IslandSource = islandSource

    interface World.AbandonJobContext with
        member _.avatarJobSource : AvatarJobSource = avatarJobSource

    interface World.BuyItemsContext with
        member _.islandSource                  : IslandSource = islandSource
        member _.itemSource                    : ItemSource =  itemSource
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarIslandFeature.EnterContext with
        member _.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
        member _.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink

    interface World.SellItemsContext with
      member _.islandSource                  : IslandSource = islandSource
       member _.itemSource                    : ItemSource = itemSource

    interface Job.CreateContext with
        member this.termListSource: TermListSource = termListSource
        member _.jobRewardStatisticSource : JobRewardStatisticSource = fun () -> worldSingleStatisticSource WorldStatisticIdentifier.JobReward

    interface IslandVisit.AddContext with
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource

    interface Island.GenerateCommoditiesContext with
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource

    interface Island.GenerateItemsContext with
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.itemSource: ItemSource = itemSource

    interface ShipmateStatistic.PutContext with
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink

    interface AvatarMetric.AddContext with
        member _.avatarSingleMetricSink   : AvatarSingleMetricSink = avatarSingleMetricSink
        member _.avatarSingleMetricSource : AvatarSingleMetricSource = avatarSingleMetricSource

    interface AvatarMessages.AddContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink

    interface AvatarShipmates.GetPrimaryStatisticContext with
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface AvatarJob.CompleteContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource

    interface Island.GetStatisticContext with
        member _.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource

    interface World.DockContext with
        member _.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarJobSink: AvatarJobSink = avatarJobSink
        member _.avatarJobSource: AvatarJobSource = avatarJobSource
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member _.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member _.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.islandSource: IslandSource = islandSource
        member _.itemSource: ItemSource = itemSource
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Vessel.GetMaximumFoulingContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.CleanHullContext with
        member _.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource

    interface ConfirmQuit.RunContext with
        member _.switchSource: SwitchSource = switchSource

    interface Island.HasFeatureContext with
        member _.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
    
    interface AvatarGamblingHand.GetContext with
        member _.avatarGamblingHandSource : AvatarGamblingHandSource = avatarGamblingHandSource

    interface AvatarGamblingHand.DealContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink

    interface AvatarGamblingHand.FoldContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink

    interface World.GetStatisticContext with
        member _.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource

    interface AvatarMetric.GetContext with
        member _.avatarMetricSource: AvatarMetricSource = avatarMetricSource
