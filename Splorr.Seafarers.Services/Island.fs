﻿namespace Splorr.Seafarers.Services
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

module Island =
    type CreateContext = 
        inherit OperatingContext
        abstract member islandSingleStatisticSink     : IslandSingleStatisticSink
        abstract member islandStatisticTemplateSource : IslandStatisticTemplateSource
    let  Create
            (context  : OperatingContext)
            (location : Location)
            : unit =
        let context = context :?> CreateContext
        context.islandStatisticTemplateSource()
        |> Map.map
            (fun _ template ->
                    Statistic.CreateFromTemplate template)
        |> Map.iter
            (fun identifier statistic ->
                (identifier, statistic |> Some)
                |> context.islandSingleStatisticSink location)

    type GetDisplayNameContext =
        inherit OperatingContext
        abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
        abstract member islandSingleNameSource         : IslandSingleNameSource
    let GetDisplayName 
            (context  : OperatingContext)
            (avatarId : string) 
            (location : Location)
            : string =
        let context = context :?> GetDisplayNameContext
        let visitCount = context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
        let islandName = context.islandSingleNameSource location
        match visitCount, islandName with
        | Some _, Some name ->
            name
        | None, Some _ ->
            "(unknown)"
        | _ ->
            raise (NotImplementedException "This island does not exist!")
    
    type AddVisitContext =
        inherit OperatingContext
        abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
        abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
        abstract member epochSecondsSource : EpochSecondsSource
    let AddVisit 
            (context      : OperatingContext)
            (avatarId     : string) 
            (location     : Location)
            : unit =
        let context = context :?> AddVisitContext
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

    type GenerateJobsContext =
        inherit OperatingContext
        abstract member islandJobSink              : IslandJobSink
        abstract member islandJobSource            : IslandJobSource
    let GenerateJobs 
            (context      : OperatingContext)
            (destinations : Set<Location>) 
            (location     : Location)
            : unit =
        let context = context :?> GenerateJobsContext
        if (context.islandJobSource location).IsEmpty && not destinations.IsEmpty then
            Job.Create 
                context 
                destinations
            |> context.islandJobSink location
    
    type MakeKnownContext = 
        inherit OperatingContext
        abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
        abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    let MakeKnown
            (context  : OperatingContext)
            (avatarId : string) 
            (location : Location)
            : unit =
        let context = context :?> MakeKnownContext
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

    type GenerateCommoditiesContext =
        inherit OperatingContext
        abstract member commoditySource    : CommoditySource
        abstract member islandMarketSource : IslandMarketSource
        abstract member islandMarketSink   : IslandMarketSink
        abstract member random             : Random
    let GenerateCommodities 
            (context  : OperatingContext)
            (location : Location) 
            : unit =
        let context = context :?> GenerateCommoditiesContext
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

    type GenerateItemsContext =
        inherit OperatingContext
        abstract member islandItemSink   : IslandItemSink
        abstract member islandItemSource : IslandItemSource
        abstract member itemSource       : ItemSource
        abstract member random           : Random
    let GenerateItems 
            (context  : OperatingContext)
            (location : Location) 
            : unit =
        let context = context :?> GenerateItemsContext
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

    type UpdateMarketForItemContext =
        inherit OperatingContext
        abstract member commoditySource          : CommoditySource
        abstract member islandSingleMarketSink   : IslandSingleMarketSink
        abstract member islandSingleMarketSource : IslandSingleMarketSource
    let UpdateMarketForItemSale 
            (context      : OperatingContext)
            (descriptor   : ItemDescriptor) 
            (quantitySold : uint64) 
            (location     : Location) 
            : unit =
        let context = context :?> UpdateMarketForItemContext
        let commodities = context.commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantitySold |> float) * commodities.[commodity].SaleFactor
                ChangeMarketDemand context.islandSingleMarketSource context.islandSingleMarketSink commodity totalQuantity location)

    let UpdateMarketForItemPurchase 
            (context      : OperatingContext)
            (descriptor               : ItemDescriptor) 
            (quantityPurchased        : uint64) 
            (location                 : Location) 
            : unit =
        let context = context :?> UpdateMarketForItemContext
        let commodities = context.commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantityPurchased |> float) * commodities.[commodity].PurchaseFactor
                ChangeMarketSupply context.islandSingleMarketSource context.islandSingleMarketSink commodity totalQuantity location)

    type GetStatisticContext =
        abstract member islandSingleStatisticSource : IslandSingleStatisticSource 

    let GetStatistic
            (context    : GetStatisticContext)
            (identifier : IslandStatisticIdentifier)
            (location   : Location)
            : Statistic option =
        context.islandSingleStatisticSource location identifier