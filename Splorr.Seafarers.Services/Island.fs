namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type ItemSource = unit -> Map<uint64, ItemDescriptor>
type IslandMarketSink = Location -> Map<uint64, Market> -> unit
type IslandItemSource = Location -> Set<uint64>
type IslandItemSink   = Location -> Set<uint64>->unit
type IslandSingleMarketSource = Location -> uint64 -> Market option
type IslandSingleMarketSink = Location -> (uint64 * Market) -> unit
type AvatarIslandSingleMetricSource = string -> Location -> AvatarIslandMetricIdentifier -> uint64 option
type AvatarIslandSingleMetricSink = string -> Location -> AvatarIslandMetricIdentifier -> uint64 -> unit
type IslandSingleNameSource = Location -> string option
type IslandSingleNameSink = Location -> string option -> unit
type IslandStatisticTemplateSource = unit -> Map<IslandStatisticIdentifier, StatisticTemplate>
type IslandSingleStatisticSink = Location->IslandStatisticIdentifier*Statistic option->unit
type IslandSingleStatisticSource = Location->IslandStatisticIdentifier->Statistic option
type IslandJobSource = Location -> Job list
type IslandJobSink = Location -> Job -> unit
type IslandSingleJobSource = Location -> uint32 -> Job option
type IslandJobPurger = Location -> uint32 -> unit
type EpochSecondsSource = unit -> uint64

type IslandJobsGenerationContext =
    inherit JobCreationContext
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

type IslandUpdateMarketForItemSaleContext =
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
            raise (System.NotImplementedException "This island does not exist!")
    
    let AddVisit 
            (context      : IslandAddVisitContext)
            (avatarId     : string) 
            (location     : Location)
            : unit =
        let visitCount = context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
        let lastVisit = context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.LastVisit
        match visitCount, lastVisit with
        | None, _ ->
            context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.VisitCount 1UL
            context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.LastVisit <| context.epochSecondsSource()
        | Some x, None ->
            context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.VisitCount (x+1UL)
            context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.LastVisit <| context.epochSecondsSource()
        | Some x, Some y when y < context.epochSecondsSource() ->
            context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.VisitCount (x+1UL)
            context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.LastVisit <| context.epochSecondsSource()
        | _ -> 
            ()

    let GenerateJobs 
            (context      : IslandJobsGenerationContext)
            (destinations : Set<Location>) 
            (location     : Location)
            : unit =
        let jobs = 
            context.islandJobSource location
        if jobs.IsEmpty && not destinations.IsEmpty then
            Job.Create 
                context 
                destinations
            |> context.islandJobSink location

    let MakeKnown
            (context  : IslandMakeKnownContext)
            (avatarId : string) 
            (location : Location)
            : unit =
        let visitCount = context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
        if visitCount.IsNone then
            context.avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.VisitCount 0UL
        
    let private SupplyDemandGenerator 
            (random:Random) 
            : float = //TODO: move this function out!
        (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + 3.0

    let GenerateCommodities 
            (context  : IslandGenerateCommoditiesContext)
            (location : Location) 
            : unit =
        let islandMarkets = context.islandMarketSource location
        if islandMarkets.IsEmpty then
            let commodities =
                context.commoditySource()
            commodities
            |> Map.fold
                (fun a commodity _->
                    let market = 
                        {
                            Supply=context.random |> SupplyDemandGenerator
                            Demand=context.random |> SupplyDemandGenerator
                        }
                    a
                    |> Map.add commodity market) islandMarkets
            |> context.islandMarketSink location

    let GenerateItems 
            (context  : IslandGenerateItemsContext)
            (location : Location) 
            : unit =
        let items = context.itemSource()
        let islandItems = context.islandItemSource location
        if islandItems.IsEmpty then
            items
            |> Map.fold 
                (fun a k v -> 
                    if context.random.NextDouble() < v.Occurrence then
                        a |> Set.add k
                    else
                        a) islandItems
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
            (context : IslandUpdateMarketForItemSaleContext)
            (descriptor               : ItemDescriptor) 
            (quantitySold             : uint64) 
            (location                 : Location) 
            : unit =
        let commodities = context.commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantitySold |> float) * commodities.[commodity].SaleFactor
                ChangeMarketDemand context.islandSingleMarketSource context.islandSingleMarketSink commodity totalQuantity location)

    let UpdateMarketForItemPurchase 
            (islandSingleMarketSource : IslandSingleMarketSource) 
            (islandSingleMarketSink   : IslandSingleMarketSink) 
            (commoditySource          : CommoditySource)
            (descriptor               : ItemDescriptor) 
            (quantityPurchased        : uint64) 
            (location                 : Location) 
            : unit =
        let commodities = commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantityPurchased |> float) * commodities.[commodity].PurchaseFactor
                ChangeMarketSupply islandSingleMarketSource islandSingleMarketSink commodity totalQuantity location)
