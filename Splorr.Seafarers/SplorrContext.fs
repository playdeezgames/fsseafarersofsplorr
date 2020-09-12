namespace Splorr.Seafarers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open System


type SplorrContext 
        (avatarInventorySink: AvatarInventorySink ,
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
    interface WorldUndockContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink
        member _.avatarIslandFeatureSink : AvatarIslandFeatureSink = avatarIslandFeatureSink

    interface IslandGetDisplayNameContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.islandSingleNameSource         : IslandSingleNameSource = islandSingleNameSource

    interface VesselTransformFoulingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface VesselBefoulContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface ShipmateGetStatusContext with
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface ShipmateEatContext with
        member _.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface AvatarGetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarGetHeadingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarSetPositionContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarSetSpeedContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarSetHeadingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarEatContext with
        member _.avatarInventorySink           : AvatarInventorySink=avatarInventorySink
        member _.avatarInventorySource         : AvatarInventorySource=avatarInventorySource
        member _.avatarShipmateSource          : AvatarShipmateSource=avatarShipmateSource

    interface AvatarTransformShipmatesContext with
        member _.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource

    interface WorldAddMessagesContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink

    interface WorldClearMessagesContext with
        member this.avatarMessagePurger: AvatarMessagePurger = avatarMessagePurger
        
    interface AvatarMoveContext with
        member _.vesselSingleStatisticSource   : VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AtSeaHandleCommandContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
        member this.islandLocationByNameSource: IslandLocationByNameSource = islandLocationByNameSource
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
        member this.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarGetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AtSeaRunContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource

    interface IslandCreateContext with
        member _.islandSingleStatisticSink     : IslandSingleStatisticSink    =islandSingleStatisticSink    
        member _.islandStatisticTemplateSource : IslandStatisticTemplateSource=islandStatisticTemplateSource

    interface UtilitySortListRandomlyContext with
        member _.random : Random = random

    interface IslandFeatureGeneratorGenerateContext with
        member _.random : Random = random

    interface WorldPopulateIslandsContext with
        member _.islandFeatureGeneratorSource : IslandFeatureGeneratorSource=islandFeatureGeneratorSource
        member _.islandSingleFeatureSink      : IslandSingleFeatureSink     =islandSingleFeatureSink     
        member _.islandSource                 : IslandSource                =islandSource     

    interface WorldGenerateIslandNameContext with
        member _.random: Random = random

    interface WorldNameIslandsContext with
        member _.islandSingleNameSink: IslandSingleNameSink = islandSingleNameSink
        member _.islandSource: IslandSource = islandSource
        member _.nameSource: TermSource = termNameSource

    interface WorldGenerateIslandsContext with
        member _.islandSingleNameSink          : IslandSingleNameSink=islandSingleNameSink
        member _.termNameSource                : TermSource          =termNameSource      

    interface VesselCreateContext with
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource

    interface ShipmateCreateContext with
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource

    interface AvatarCreateContext with
        member _.avatarJobSink: AvatarJobSink = avatarJobSink

    interface AvatarGetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface WorldCreateContext with
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

    interface WorldUpdateChartsContext with
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member this.islandSource: IslandSource = islandSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface DockedUpdateDisplayContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource 
        member _.avatarMessageSource            : AvatarMessageSource            = avatarMessageSource            
        member _.islandSingleNameSource         : IslandSingleNameSource         = islandSingleNameSource  
        member _.islandFeatureSource            : IslandFeatureSource            = islandFeatureSource

    interface ItemDeterminePriceContext with
        member _.commoditySource                : CommoditySource                =commoditySource               
        member _.islandMarketSource             : IslandMarketSource             =islandMarketSource     
        member _.itemSingleSource               : ItemSingleSource               = itemSingleSource

    interface IslandMakeKnownContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
    
    interface IslandUpdateMarketForItemContext with
        member _.commoditySource: CommoditySource = commoditySource
        member _.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member _.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource

    interface AvatarRemoveInventoryContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource

    interface AvatarGetUsedTonnageContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource

    interface AvatarAbandonJobContext with
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
        
    interface AvatarGetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface AvatarAddInventoryContext with
        member _.avatarInventorySink   : AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface WorldAcceptJobContext with
        member _.avatarJobSink         : AvatarJobSink = avatarJobSink
        member _.avatarJobSource       : AvatarJobSource = avatarJobSource
        member _.islandJobPurger       : IslandJobPurger = islandJobPurger
        member _.islandSingleJobSource : IslandSingleJobSource = islandSingleJobSource
        member _.islandSource          : IslandSource = islandSource
    interface WorldAbandonJobContext with
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
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
        member this.commoditySource: CommoditySource = commoditySource
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

    interface JobCreateContext with
        member _.termSources: TermSources = termSources
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource

    interface IslandAddVisitContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink   
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource

    interface IslandGenerateCommoditiesContext with
        member _.commoditySource: CommoditySource = commoditySource
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.random : Random = random

    interface IslandGenerateItemsContext with
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.itemSource: ItemSource = itemSource
        member _.random: Random = random

    interface ShipmateTransformStatisticContext with
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface AvatarAddMetricContext with
        member _.avatarSingleMetricSink   : AvatarSingleMetricSink = avatarSingleMetricSink
        member _.avatarSingleMetricSource : AvatarSingleMetricSource = avatarSingleMetricSource

    interface AvatarAddMessagesContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink

    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarCompleteJobContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource
    interface WorldDockContext with
        member this.avatarIslandFeatureSink: AvatarIslandFeatureSink = avatarIslandFeatureSink
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
        member this.commoditySource: CommoditySource = commoditySource
        member this.islandItemSink: IslandItemSink = islandItemSink
        member this.islandItemSource: IslandItemSource = islandItemSource
        member this.islandJobSink: IslandJobSink = islandJobSink
        member this.islandJobSource: IslandJobSource = islandJobSource
        member this.islandMarketSink: IslandMarketSink = islandMarketSink
        member this.islandMarketSource: IslandMarketSource = islandMarketSource
        member this.islandSource: IslandSource = islandSource
        member this.itemSource: ItemSource = itemSource
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

    interface HelpRunContext with
        member _.avatarIslandFeatureSource : AvatarIslandFeatureSource = avatarIslandFeatureSource

    interface AvatarGetMaximumFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface AvatarCleanHullContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource

    interface RunnerRunContext with
        member _.avatarMetricSource              : AvatarMetricSource=avatarMetricSource
        member _.switchSource                    : SwitchSource      =switchSource      

    interface IslandFeatureRunIslandContext with
        member this.islandSingleFeatureSource: IslandSingleFeatureSource = islandSingleFeatureSource
    
    interface IslandFeatureRunContext with
        member this.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource

    interface IslandFeatureRunDarkAlleyContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
        member this.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

