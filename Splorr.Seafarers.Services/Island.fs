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
type AvatarIslandSingleMetricSink = string -> Location -> AvatarIslandMetricIdentifier -> uint64 -> unit //TODO:value needs to become uint64 option
type IslandSingleNameSource = Location -> string option
type IslandSingleNameSink = Location -> string option -> unit
type IslandStatisticTemplateSource = unit -> Map<IslandStatisticIdentifier, StatisticTemplate>
type IslandSingleStatisticSink = Location->IslandStatisticIdentifier*Statistic option->unit
type IslandSingleStatisticSource = Location->IslandStatisticIdentifier->Statistic option
type IslandJobSource = Location -> Job list
type IslandJobSink = Location -> Job -> unit
type IslandSingleJobSource = Location -> uint32 -> Job option
type IslandJobPurger = Location -> uint32 -> unit

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
            (context : IslandGetDisplayNameContext)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (islandSingleNameSource         : IslandSingleNameSource)
            (avatarId : string) 
            (location : Location)
            : string =
        //does this island location have a visit count for this avatar?
        let visitCount = avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
        let islandName = islandSingleNameSource location
        match visitCount, islandName with
        | Some _, Some name ->
            name
        | None, Some _ ->
            "(unknown)"
        | _ ->
            raise (System.NotImplementedException "This island does not exist!")
    
    let AddVisit 
            (avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (epochSeconds                   : uint64) //TODO: to time source(if the tests fail intermittently)?
            (avatarId                       : string) 
            (location                       : Location)
            : unit =
        let visitCount = avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
        let lastVisit = avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.LastVisit
        match visitCount, lastVisit with
        | None, _ ->
            avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.VisitCount 1UL
            avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.LastVisit epochSeconds
        | Some x, None ->
            avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.VisitCount (x+1UL)
            avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.LastVisit epochSeconds
        | Some x, Some y when y < epochSeconds ->
            avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.VisitCount (x+1UL)
            avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.LastVisit epochSeconds
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
            (avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarId                       : string) 
            (location                       : Location)
            : unit =
        let visitCount = avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount
        if visitCount.IsNone then
            avatarIslandSingleMetricSink avatarId location AvatarIslandMetricIdentifier.VisitCount 0UL
        
    let private SupplyDemandGenerator 
            (random:Random) 
            : float = //TODO: move this function out!
        (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + (random.NextDouble()) * 6.0 + 3.0

    let GenerateCommodities 
            (commoditySource    : CommoditySource)
            (islandMarketSource : IslandMarketSource) 
            (islandMarketSink   : IslandMarketSink) 
            (random             : Random) 
            (location           : Location) 
            : unit =
        let islandMarkets = islandMarketSource location
        if islandMarkets.IsEmpty then
            let commodities =
                commoditySource()
            commodities
            |> Map.fold
                (fun a commodity _->
                    let market = 
                        {
                            Supply=random |> SupplyDemandGenerator
                            Demand=random |> SupplyDemandGenerator
                        }
                    a
                    |> Map.add commodity market) islandMarkets
            |> islandMarketSink location

    let GenerateItems 
            (islandItemSource : IslandItemSource) 
            (islandItemSink   : IslandItemSink) 
            (random           : Random) 
            (itemSource       : ItemSource)
            (location         : Location) 
            : unit =
        let items = itemSource()
        let islandItems = islandItemSource location
        if islandItems.IsEmpty then
            items
            |> Map.fold 
                (fun a k v -> 
                    if random.NextDouble() < v.Occurrence then
                        a |> Set.add k
                    else
                        a) islandItems
            |> islandItemSink location

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
            (islandSingleMarketSource : IslandSingleMarketSource) 
            (islandSingleMarketSink   : IslandSingleMarketSink) 
            (commoditySource          : CommoditySource)
            (descriptor               : ItemDescriptor) 
            (quantitySold             : uint64) 
            (location                 : Location) 
            : unit =
        let commodities = commoditySource()
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantitySold |> float) * commodities.[commodity].SaleFactor
                ChangeMarketDemand islandSingleMarketSource islandSingleMarketSink commodity totalQuantity location)

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
