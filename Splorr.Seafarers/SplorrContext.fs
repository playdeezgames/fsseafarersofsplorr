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
        termNameSource: TermSource ,
        termSources: TermSources ,
        vesselSingleStatisticSink: VesselSingleStatisticSink ,
        vesselSingleStatisticSource: VesselSingleStatisticSource ,
        vesselStatisticSink: VesselStatisticSink ,
        vesselStatisticTemplateSource: VesselStatisticTemplateSource ,
        worldSingleStatisticSource : WorldSingleStatisticSource) =
    interface Vessel.GetStatisticContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Shipmate.GetStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Avatar.GetJobContext with
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
    interface GamestateCheckForAvatarDeathContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
    interface AtSeaGetVisibleIslandsContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Island.GetNameContext with
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
    interface Island.GetItemsContext with
        member this.islandItemSource: IslandItemSource = islandItemSource
    interface Item.GetListContext with
        member this.itemSource: ItemSource = itemSource
    interface InventoryRunContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member this.itemSource: ItemSource = itemSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface InventoryRunWorldContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member this.itemSource: ItemSource = itemSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface DockedRunBoilerplateContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
        member this.islandSource: IslandSource = islandSource
    interface IslandListRunWorldContext with
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        member this.islandSource: IslandSource = islandSource
    interface AtSeaCanCareenContext with
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AtSeaUpdateDisplayContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface World.UndockContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink
        member _.avatarIslandFeatureSink : AvatarIslandFeatureSink = avatarIslandFeatureSink

    interface Commodity.GetCommoditiesContext with
        member this.commoditySource: CommoditySource = commoditySource

    interface Island.GenerateJobsContext with
        member this.islandJobSink: IslandJobSink = islandJobSink
        member this.islandJobSource: IslandJobSource = islandJobSource

    interface Island.ChangeMarketContext with
        member this.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member this.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource
    
    interface Island.GetListContext with
        member this.islandSource: IslandSource = islandSource
    
    interface Island.GetJobsContext with
        member this.islandJobSource: IslandJobSource = islandJobSource

    interface Island.GetDisplayNameContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.islandSingleNameSource         : IslandSingleNameSource = islandSingleNameSource

    interface Vessel.TransformFoulingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Vessel.BefoulContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Shipmate.GetStatusContext with
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Shipmate.EatContext with
        member _.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Avatar.GetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.GetHeadingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.SetPositionContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.SetSpeedContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.SetHeadingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.EatContext with
        member _.avatarInventorySink           : AvatarInventorySink=avatarInventorySink
        member _.avatarInventorySource         : AvatarInventorySource=avatarInventorySource
        member _.avatarShipmateSource          : AvatarShipmateSource=avatarShipmateSource

    interface Avatar.TransformShipmatesContext with
        member _.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource

    interface World.AddMessagesContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink

    interface World.ClearMessagesContext with
        member this.avatarMessagePurger: AvatarMessagePurger = avatarMessagePurger
        
    interface Avatar.MoveContext with
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource
        
    interface World.DistanceToContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.islandLocationByNameSource     : IslandLocationByNameSource = islandLocationByNameSource
    interface World.HeadForContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.islandLocationByNameSource     : IslandLocationByNameSource = islandLocationByNameSource

    interface World.GetNearbyLocationsContext with
        member _.islandSource : IslandSource = islandSource

    interface AtSeaHandleCommandContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.GetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AtSeaRunContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource

    interface Island.CreateContext with
        member _.islandSingleStatisticSink     : IslandSingleStatisticSink    =islandSingleStatisticSink    
        member _.islandStatisticTemplateSource : IslandStatisticTemplateSource=islandStatisticTemplateSource

    interface Utility.RandomContext with
        member _.random : Random = random

    interface IslandFeature.CreateContext with
        member _.random : Random = random

    interface World.PopulateIslandsContext with
        member _.islandFeatureGeneratorSource : IslandFeatureGeneratorSource=islandFeatureGeneratorSource
        member _.islandSingleFeatureSink      : IslandSingleFeatureSink     =islandSingleFeatureSink     
        member _.islandSource                 : IslandSource                =islandSource     

    interface World.GenerateIslandNameContext with
        member _.random: Random = random

    interface World.NameIslandsContext with
        member _.islandSingleNameSink: IslandSingleNameSink = islandSingleNameSink
        member _.islandSource: IslandSource = islandSource
        member _.nameSource: TermSource = termNameSource

    interface World.GenerateIslandsContext with
        member _.islandSingleNameSink          : IslandSingleNameSink=islandSingleNameSink
        member _.termNameSource                : TermSource          =termNameSource     
        member _.islandSource : IslandSource = islandSource
        member _.random : Random = random

    interface Vessel.CreateContext with
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource

    interface Shipmate.CreateContext with
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource

    interface Avatar.CreateContext with
        member _.avatarJobSink: AvatarJobSink = avatarJobSink

    interface Avatar.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface World.CreateContext with
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.rationItemSource: RationItemSource = rationItemSource
        member this.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        member this.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member this.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource
        member this.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource

    interface World.UpdateChartsContext with
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member this.islandSource: IslandSource = islandSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface DockedUpdateDisplayContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource 
        member _.avatarMessageSource            : AvatarMessageSource            = avatarMessageSource            
        member _.islandSingleNameSource         : IslandSingleNameSource         = islandSingleNameSource  
        member _.islandFeatureSource            : IslandFeatureSource            = islandFeatureSource

    interface IslandMarket.DeterminePriceContext with
        member _.islandMarketSource             : IslandMarketSource             =islandMarketSource     
        member _.itemSingleSource               : ItemSingleSource               = itemSingleSource

    interface Island.MakeKnownContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
    
    interface Island.UpdateMarketForItemContext

    interface Avatar.RemoveInventoryContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource

    interface Avatar.GetUsedTonnageContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource

    interface Avatar.AbandonJobContext with
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
        
    interface Avatar.GetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface Avatar.AddInventoryContext with
        member _.avatarInventorySink   : AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface World.AcceptJobContext with
        member _.avatarJobSink         : AvatarJobSink = avatarJobSink
        member _.avatarJobSource       : AvatarJobSource = avatarJobSource
        member _.islandJobPurger       : IslandJobPurger = islandJobPurger
        member _.islandSingleJobSource : IslandSingleJobSource = islandSingleJobSource
        member _.islandSource          : IslandSource = islandSource
    interface World.AbandonJobContext with
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
    interface World.BuyItemsContext with
        member _.islandSource                  : IslandSource = islandSource
        member _.itemSource                    : ItemSource =  itemSource
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.EnterIslandFeatureContext with
        member this.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
        member this.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink
    interface DockedHandleCommandContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
        member this.avatarMessagePurger: AvatarMessagePurger = avatarMessagePurger
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
        member this.islandJobPurger: IslandJobPurger = islandJobPurger
        member this.islandMarketSource: IslandMarketSource = islandMarketSource
        member this.islandSingleJobSource: IslandSingleJobSource = islandSingleJobSource
        member this.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member this.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource
        member this.islandSource: IslandSource = islandSource
        member this.itemSource: ItemSource = itemSource
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface World.SellItemsContext with
      member _.islandSource                  : IslandSource = islandSource
       member _.itemSource                    : ItemSource = itemSource

    interface Job.CreateContext with
        member _.termSources: TermSources = termSources
        member _.jobRewardStatisticSource : JobRewardStatisticSource = fun () -> worldSingleStatisticSource WorldStatisticIdentifier.JobReward
        member _.random = random

    interface Island.AddVisitContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink   
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource

    interface Island.GenerateCommoditiesContext with
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.random : Random = random

    interface Island.GenerateItemsContext with
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.itemSource: ItemSource = itemSource
        member _.random: Random = random

    interface Shipmate.TransformStatisticContext with
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface Avatar.AddMetricContext with
        member _.avatarSingleMetricSink   : AvatarSingleMetricSink = avatarSingleMetricSink
        member _.avatarSingleMetricSource : AvatarSingleMetricSource = avatarSingleMetricSource

    interface Avatar.AddMessagesContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink

    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Avatar.CompleteJobContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
    interface World.DockContext with
        member this.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
        member this.islandItemSink: IslandItemSink = islandItemSink
        member this.islandItemSource: IslandItemSource = islandItemSource
        member this.islandMarketSink: IslandMarketSink = islandMarketSink
        member this.islandMarketSource: IslandMarketSource = islandMarketSource
        member this.islandSource: IslandSource = islandSource
        member this.itemSource: ItemSource = itemSource
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface HelpRunContext with
        member _.avatarIslandFeatureSource : AvatarIslandFeatureSource = avatarIslandFeatureSource

    interface Avatar.GetMaximumFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface Avatar.CleanHullContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource

    interface ConfirmQuit.RunContext with
        member this.switchSource: SwitchSource = switchSource

    interface IslandFeatureRunIslandContext with
        member this.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
    
    interface IslandFeatureRunContext with
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        
    interface Avatar.GetGamblingHandContext with
        member _.avatarGamblingHandSource : AvatarGamblingHandSource = avatarGamblingHandSource
    interface Avatar.DealGamblingHandContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink
        member _.random : Random = random

    interface Avatar.FoldGamblingHandContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink

    interface ChartRunContext with
        member this.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource
    interface ChartOutputChartContext with
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        member this.islandSource: IslandSource = islandSource

    interface World.HasDarkAlleyMinimumStakesContext with
        member _.shipmateSingleStatisticSource : ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        member _.islandSingleStatisticSource : IslandSingleStatisticSource = islandSingleStatisticSource
        member _.avatarIslandFeatureSource : AvatarIslandFeatureSource = avatarIslandFeatureSource
    interface CareenedRunContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource

    interface IslandFeatureRunDarkAlleyContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Avatar.GetMetricsContext with
        member this.avatarMetricSource: AvatarMetricSource = avatarMetricSource

