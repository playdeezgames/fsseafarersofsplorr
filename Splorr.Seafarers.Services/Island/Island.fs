namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type IslandItemSink   = Location -> Set<uint64>->unit
type IslandItemSource = Location -> Set<uint64>
type IslandMarketSink = Location -> Map<uint64, Market> -> unit
type IslandSingleJobSource = Location -> uint32 -> Job option
type IslandSingleMarketSink = Location -> (uint64 * Market) -> unit
type IslandSingleMarketSource = Location -> uint64 -> Market option
type IslandSingleNameSink = Location -> string option -> unit
type IslandSingleStatisticSink = Location->IslandStatisticIdentifier*Statistic option->unit
type IslandSingleStatisticSource = Location->IslandStatisticIdentifier->Statistic option
type IslandStatisticTemplateSource = unit -> Map<IslandStatisticIdentifier, StatisticTemplate>
type IslandSingleFeatureSource = Location -> IslandFeatureIdentifier -> bool
type IslandSource = unit -> Location list
type IslandFeatureSource = Location -> IslandFeatureIdentifier list

module Island =
    type CreateContext = 
        inherit ServiceContext
        abstract member islandSingleStatisticSink     : IslandSingleStatisticSink
        abstract member islandStatisticTemplateSource : IslandStatisticTemplateSource
    let  Create
            (context  : ServiceContext)
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
        
    type GenerateCommoditiesContext =
        inherit ServiceContext
        abstract member islandMarketSource : IslandMarketSource
        abstract member islandMarketSink   : IslandMarketSink
        abstract member random             : Random
    let GenerateCommodities 
            (context  : ServiceContext)
            (location : Location) 
            : unit =
        let context = context :?> GenerateCommoditiesContext
        if (context.islandMarketSource location).IsEmpty then
            (Map.empty, Commodity.GetCommodities context)
            ||> Map.fold
                (fun markets commodity _->
                    markets
                    |> Map.add 
                        commodity 
                        {
                            Supply = context.random |> Utility.SupplyDemandGenerator
                            Demand = context.random |> Utility.SupplyDemandGenerator
                        })
            |> context.islandMarketSink location

    type GenerateItemsContext =
        inherit ServiceContext
        abstract member islandItemSink   : IslandItemSink
        abstract member islandItemSource : IslandItemSource
        abstract member itemSource       : ItemSource
        abstract member random           : Random
    let GenerateItems 
            (context  : ServiceContext)
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

    type ChangeMarketTransform = float * Market -> Market
    type ChangeMarketContext =
        inherit ServiceContext
        abstract member islandSingleMarketSource : IslandSingleMarketSource
        abstract member islandSingleMarketSink   : IslandSingleMarketSink
    let private ChangeMarket
            (transform: ChangeMarketTransform)
            (context : ServiceContext)
            (commodity                : uint64) 
            (change                   : float) 
            (location                 : Location) 
            : unit =
        let context = context :?> ChangeMarketContext
        commodity
        |> context.islandSingleMarketSource location
        |> Option.map (fun m -> transform (change, m))
        |> Option.iter (fun market -> context.islandSingleMarketSink location (commodity, market))

    let private ChangeMarketDemand (context:ServiceContext) =
        ChangeMarket Market.ChangeDemand context

    let private ChangeMarketSupply (context:ServiceContext) = 
        ChangeMarket Market.ChangeSupply context

    type UpdateMarketForItemContext =
        inherit ServiceContext
    let UpdateMarketForItemSale 
            (context      : ServiceContext)
            (descriptor   : ItemDescriptor) 
            (quantitySold : uint64) 
            (location     : Location) 
            : unit =
        let context = context :?> UpdateMarketForItemContext
        let commodities = Commodity.GetCommodities context
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantitySold |> float) * commodities.[commodity].SaleFactor
                ChangeMarketDemand context commodity totalQuantity location)

    let UpdateMarketForItemPurchase 
            (context      : ServiceContext)
            (descriptor               : ItemDescriptor) 
            (quantityPurchased        : uint64) 
            (location                 : Location) 
            : unit =
        let context = context :?> UpdateMarketForItemContext
        let commodities = Commodity.GetCommodities context
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantityPurchased |> float) * commodities.[commodity].PurchaseFactor
                ChangeMarketSupply context commodity totalQuantity location)

    type GetStatisticContext =
        inherit ServiceContext
        abstract member islandSingleStatisticSource : IslandSingleStatisticSource 
    let GetStatistic
            (context    : ServiceContext)
            (identifier : IslandStatisticIdentifier)
            (location   : Location)
            : Statistic option =
        let context = context :?> GetStatisticContext
        context.islandSingleStatisticSource location identifier

    type GetListContext =
        inherit ServiceContext
        abstract member islandSource           : IslandSource
    let GetList
            (context : ServiceContext)
            : Location list =
        (context :?> GetListContext).islandSource()

    type GetItemsContext =
        inherit ServiceContext
        abstract member islandItemSource   : IslandItemSource
    let GetItems
            (context : ServiceContext)
            (location : Location)
            : Set<uint64> =
        (context :?> GetItemsContext).islandItemSource location

    type HasFeatureContext =
        inherit ServiceContext
        abstract member islandSingleFeatureSource : IslandSingleFeatureSource
    let HasFeature
            (context    : ServiceContext)
            (identifier : IslandFeatureIdentifier)
            (location   : Location)
            : bool =
        (context :?> HasFeatureContext).islandSingleFeatureSource location identifier

    type GetFeaturesContext = 
        inherit ServiceContext
        abstract member islandFeatureSource            : IslandFeatureSource
    let GetFeatures
            (context : ServiceContext)
            (location : Location)
            : IslandFeatureIdentifier list =
        (context :?> GetFeaturesContext).islandFeatureSource location

