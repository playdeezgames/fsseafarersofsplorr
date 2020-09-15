namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type AvatarIslandSingleMetricSource = string -> Location -> AvatarIslandMetricIdentifier -> uint64 option
type AvatarIslandSingleMetricSink = string -> Location -> AvatarIslandMetricIdentifier -> uint64 -> unit
type EpochSecondsSource = unit -> uint64
type IslandItemSink   = Location -> Set<uint64>->unit
type IslandItemSource = Location -> Set<uint64>
type IslandJobPurger = Location -> uint32 -> unit
type IslandJobSink = Location -> Job -> unit
type IslandJobSource = Location -> Job list
type IslandMarketSink = Location -> Map<uint64, Market> -> unit
type IslandSingleJobSource = Location -> uint32 -> Job option
type IslandSingleMarketSink = Location -> (uint64 * Market) -> unit
type IslandSingleMarketSource = Location -> uint64 -> Market option
type IslandSingleNameSink = Location -> string option -> unit
type IslandSingleNameSource = Location -> string option
type IslandSingleStatisticSink = Location->IslandStatisticIdentifier*Statistic option->unit
type IslandSingleStatisticSource = Location->IslandStatisticIdentifier->Statistic option
type IslandStatisticTemplateSource = unit -> Map<IslandStatisticIdentifier, StatisticTemplate>
type ItemSource = unit -> Map<uint64, ItemDescriptor>
type IslandSingleFeatureSource = Location -> IslandFeatureIdentifier -> bool



type IslandGenerateJobsContext =
    inherit JobCreateContext
    abstract member islandJobSink              : IslandJobSink
    abstract member islandJobSource            : IslandJobSource

type IslandCreateContext = 
    abstract member islandSingleStatisticSink     : IslandSingleStatisticSink
    abstract member islandStatisticTemplateSource : IslandStatisticTemplateSource

type IslandGetDisplayNameContext =
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member islandSingleNameSource         : IslandSingleNameSource

type IslandAddVisitContext =
    abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member epochSecondsSource : EpochSecondsSource

type IslandMakeKnownContext = 
    abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource

type IslandGenerateCommoditiesContext =
    abstract member commoditySource    : CommoditySource
    abstract member islandMarketSource : IslandMarketSource
    abstract member islandMarketSink   : IslandMarketSink
    abstract member random             : Random

type IslandGenerateItemsContext =
    abstract member islandItemSink   : IslandItemSink
    abstract member islandItemSource : IslandItemSource
    abstract member itemSource       : ItemSource
    abstract member random           : Random

type IslandUpdateMarketForItemContext =
    abstract member commoditySource          : CommoditySource
    abstract member islandSingleMarketSink   : IslandSingleMarketSink
    abstract member islandSingleMarketSource : IslandSingleMarketSource


module Island =
    let  Create
            (context  : IslandCreateContext)
            (location : Location)
            : unit =
        context.islandStatisticTemplateSource()
        |> Map.map
            (fun _ template ->
                    Statistic.CreateFromTemplate template)
        |> Map.iter
            (fun identifier statistic ->
                (identifier, statistic |> Some)
                |> context.islandSingleStatisticSink location)

    let GetDisplayName 
            (context  : IslandGetDisplayNameContext)
            (avatarId : string) 
            (location : Location)
            : string =
        let visitCount = context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
        let islandName = context.islandSingleNameSource location
        match visitCount, islandName with
        | Some _, Some name ->
            name
        | None, Some _ ->
            "(unknown)"
        | _ ->
            raise (NotImplementedException "This island does not exist!")
    
    let AddVisit 
            (context      : IslandAddVisitContext)
            (avatarId     : string) 
            (location     : Location)
            : unit =
        let metricSource = context.avatarIslandSingleMetricSource avatarId location
        let metricSink = context.avatarIslandSingleMetricSink avatarId location
        let sinkMetrics(visitCount,lastVisit) =
            metricSink AvatarIslandMetricIdentifier.VisitCount visitCount
            metricSink AvatarIslandMetricIdentifier.LastVisit lastVisit
        let visitCount = metricSource AvatarIslandMetricIdentifier.VisitCount
        let lastVisit = metricSource AvatarIslandMetricIdentifier.LastVisit
        let currentEpochSeconds = context.epochSecondsSource()
        match visitCount, lastVisit with
        | None, _ ->
            sinkMetrics (1UL, currentEpochSeconds)
        | Some x, None ->
            sinkMetrics (x + 1UL, currentEpochSeconds)
        | Some x, Some y when y < currentEpochSeconds ->
            sinkMetrics (x + 1UL, currentEpochSeconds)
        | _ -> 
            ()

    let GenerateJobs 
            (context      : IslandGenerateJobsContext)
            (destinations : Set<Location>) 
            (location     : Location)
            : unit =
        if (context.islandJobSource location).IsEmpty && not destinations.IsEmpty then
            Job.Create 
                context 
                destinations
            |> context.islandJobSink location

    let MakeKnown
            (context  : IslandMakeKnownContext)
            (avatarId : string) 
            (location : Location)
            : unit =
        let visitCount = 
            context.avatarIslandSingleMetricSource 
                avatarId 
                location 
                AvatarIslandMetricIdentifier.VisitCount
        if visitCount.IsNone then
            context.avatarIslandSingleMetricSink 
                avatarId 
                location 
                AvatarIslandMetricIdentifier.VisitCount 
                0UL
        
    let private SupplyDemandGenerator 
            (random:Random) 
            : float = //TODO: move this function out!
        (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + 3.0

    let GenerateCommodities 
            (context  : IslandGenerateCommoditiesContext)
            (location : Location) 
            : unit =
        if (context.islandMarketSource location).IsEmpty then
            (Map.empty, context.commoditySource())
            ||> Map.fold
                (fun markets commodity _->
                    markets
                    |> Map.add 
                        commodity 
                        {
                            Supply = context.random |> SupplyDemandGenerator
                            Demand = context.random |> SupplyDemandGenerator
                        })
            |> context.islandMarketSink location

    let GenerateItems 
            (context  : IslandGenerateItemsContext)
            (location : Location) 
            : unit =
        if (context.islandItemSource location).IsEmpty then
            (Set.empty, context.itemSource())
            ||> Map.fold 
                (fun items item descriptor -> 
                    if context.random.NextDouble() < descriptor.Occurrence then
                        items |> Set.add item
                    else
                        items)
            |> context.islandItemSink location

    let private ChangeMarketDemand 
            (islandSingleMarketSource : IslandSingleMarketSource)
            (islandSingleMarketSink   : IslandSingleMarketSink) 
            (commodity                : uint64) 
            (change                   : float) 
            (location                 : Location) 
            : unit =
        commodity
        |> islandSingleMarketSource location
        |> Option.map (fun m -> Market.ChangeDemand (change, m))
        |> Option.iter (fun market -> islandSingleMarketSink location (commodity, market))

        

    let private ChangeMarketSupply 
            (islandSingleMarketSource : IslandSingleMarketSource)
            (islandSingleMarketSink   : IslandSingleMarketSink) 
            (commodity                : uint64) 
            (change                   : float) 
            (location                 : Location) 
            : unit =
        commodity
        |> islandSingleMarketSource location
        |> Option.map (fun m -> Market.ChangeSupply (change, m))
        |> Option.iter (fun market -> islandSingleMarketSink location (commodity, market))

    let UpdateMarketForItemSale 
            (context      : IslandUpdateMarketForItemContext)
            (descriptor   : ItemDescriptor) 
            (quantitySold : uint64) 
            (location     : Location) 
            : unit =
        let commodities = context.commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantitySold |> float) * commodities.[commodity].SaleFactor
                ChangeMarketDemand context.islandSingleMarketSource context.islandSingleMarketSink commodity totalQuantity location)

    let UpdateMarketForItemPurchase 
            (context      : IslandUpdateMarketForItemContext)
            (descriptor               : ItemDescriptor) 
            (quantityPurchased        : uint64) 
            (location                 : Location) 
            : unit =
        let commodities = context.commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantityPurchased |> float) * commodities.[commodity].PurchaseFactor
                ChangeMarketSupply context.islandSingleMarketSource context.islandSingleMarketSink commodity totalQuantity location)

