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

    interface AtSeaHandleCommandContext with
        member _.avatarInventorySink            : AvatarInventorySink        =avatarInventorySink        
        member _.avatarInventorySource          : AvatarInventorySource      =avatarInventorySource      
        member _.avatarMessagePurger            : AvatarMessagePurger        =avatarMessagePurger        
        member _.avatarShipmateSource           : AvatarShipmateSource       =avatarShipmateSource       
        member _.islandLocationByNameSource     : IslandLocationByNameSource =islandLocationByNameSource 
        member _.islandSingleNameSource         : IslandSingleNameSource     =islandSingleNameSource     
        member _.islandSingleStatisticSource    : IslandSingleStatisticSource=islandSingleStatisticSource
        member _.shipmateRationItemSource       : ShipmateRationItemSource   =shipmateRationItemSource   
        member _.vesselSingleStatisticSink      : VesselSingleStatisticSink  =vesselSingleStatisticSink  
        member _.vesselSingleStatisticSource    : VesselSingleStatisticSource=vesselSingleStatisticSource

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

    interface WorldNameIslandsContext with
        member this.islandSingleNameSink: IslandSingleNameSink = islandSingleNameSink
        member this.islandSource: IslandSource = islandSource
        member this.nameSource: TermSource = termNameSource

    interface WorldGenerateIslandsContext with
        member _.islandSingleNameSink          : IslandSingleNameSink=islandSingleNameSink
        member _.termNameSource                : TermSource          =termNameSource      

    interface WorldCreateContext with
        member _.avatarIslandSingleMetricSink    : AvatarIslandSingleMetricSink    = avatarIslandSingleMetricSink   
        member _.avatarJobSink                   : AvatarJobSink                   = avatarJobSink                  
        member _.worldSingleStatisticSource      : WorldSingleStatisticSource      = worldSingleStatisticSource     
        member _.shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource
        member _.shipmateSingleStatisticSink     : ShipmateSingleStatisticSink     = shipmateSingleStatisticSink    
        member _.rationItemSource                : RationItemSource                = rationItemSource               
        member _.vesselStatisticTemplateSource   : VesselStatisticTemplateSource   = vesselStatisticTemplateSource  
        member _.vesselStatisticSink             : VesselStatisticSink             = vesselStatisticSink            
        member _.vesselSingleStatisticSource     : VesselSingleStatisticSource     = vesselSingleStatisticSource    
        member _.shipmateRationItemSink          : ShipmateRationItemSink          = shipmateRationItemSink         

    interface DockedUpdateDisplayContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource 
        member _.avatarMessageSource            : AvatarMessageSource            = avatarMessageSource            
        member _.islandSingleNameSource         : IslandSingleNameSource         = islandSingleNameSource  
        member _.islandFeatureSource            : IslandFeatureSource            = islandFeatureSource

    interface ItemDeterminePriceContext with
        member _.commoditySource                : CommoditySource                =commoditySource               
        member _.islandMarketSource             : IslandMarketSource             =islandMarketSource     
        member _.itemSingleSource               : ItemSingleSource               = itemSingleSource

    interface DockedHandleCommandContext with
        member _.commoditySource                : CommoditySource                =commoditySource               
        member _.islandMarketSource             : IslandMarketSource             =islandMarketSource            
        member _.avatarInventorySink            : AvatarInventorySink            =avatarInventorySink            
        member _.avatarInventorySource          : AvatarInventorySource          =avatarInventorySource          
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink   =avatarIslandSingleMetricSink   
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource =avatarIslandSingleMetricSource 
        member _.avatarJobSink                  : AvatarJobSink                  =avatarJobSink                 
        member _.avatarJobSource                : AvatarJobSource                =avatarJobSource               
        member _.avatarMessagePurger            : AvatarMessagePurger            =avatarMessagePurger           
        member _.avatarMessageSink              : AvatarMessageSink              =avatarMessageSink             
        member _.avatarSingleMetricSink         : AvatarSingleMetricSink         =avatarSingleMetricSink        
        member _.avatarSingleMetricSource       : AvatarSingleMetricSource       =avatarSingleMetricSource      
        member _.islandJobPurger                : IslandJobPurger                =islandJobPurger               
        member _.islandSingleJobSource          : IslandSingleJobSource          =islandSingleJobSource         
        member _.islandSingleMarketSink         : IslandSingleMarketSink         =islandSingleMarketSink        
        member _.islandSingleMarketSource       : IslandSingleMarketSource       =islandSingleMarketSource      
        member _.islandSource                   : IslandSource                   =islandSource                  
        member _.itemSource                     : ItemSource                     =itemSource                    
        member _.shipmateSingleStatisticSink    : ShipmateSingleStatisticSink    =shipmateSingleStatisticSink   
        member _.shipmateSingleStatisticSource  : ShipmateSingleStatisticSource  =shipmateSingleStatisticSource 
        member _.vesselSingleStatisticSource    : VesselSingleStatisticSource    =vesselSingleStatisticSource   

    interface JobCreationContext with
        member _.termSources: TermSources = termSources
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource

    interface IslandAddVisitContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink   
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource

    interface WorldDockContext with
        member _.avatarIslandFeatureSink : AvatarIslandFeatureSink = avatarIslandFeatureSink
        member _.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.avatarJobSource: AvatarJobSource = avatarJobSource
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        member _.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member _.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
        member _.commoditySource: CommoditySource = commoditySource
        member _.islandItemSink: IslandItemSink = islandItemSink
        member _.islandItemSource: IslandItemSource = islandItemSource
        member _.islandJobSink: IslandJobSink = islandJobSink
        member _.islandJobSource: IslandJobSource = islandJobSource
        member _.islandMarketSink: IslandMarketSink = islandMarketSink
        member _.islandMarketSource: IslandMarketSource = islandMarketSource
        member _.itemSource: ItemSource = itemSource
        member _.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarJobSink: AvatarJobSink = avatarJobSink
        member _.islandSource: IslandSource = islandSource
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink

    interface HelpRunContext with
        member _.avatarIslandFeatureSource : AvatarIslandFeatureSource = avatarIslandFeatureSource

    interface RunnerRunContext with
        member _.avatarMetricSource              : AvatarMetricSource=avatarMetricSource
        member _.switchSource                    : SwitchSource      =switchSource      

    interface IslandFeatureRunIslandContext with
        member _.islandSingleFeatureSource : IslandSingleFeatureSource = islandSingleFeatureSource
    
    interface IslandFeatureRunContext with
        member _.islandSingleNameSource    : IslandSingleNameSource = islandSingleNameSource

    interface IslandFeatureRunDarkAlleyContext with
        member _.avatarMessageSource : AvatarMessageSource = avatarMessageSource
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink
        member _.islandSingleStatisticSource   : IslandSingleStatisticSource = islandSingleStatisticSource
        member _.shipmateSingleStatisticSource : ShipmateSingleStatisticSource = shipmateSingleStatisticSource

