namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common


module Island =
    type IslandItemSink   = Location -> Set<uint64>->unit
    type IslandItemSource = Location -> Set<uint64>
    type IslandMarketSink = Location -> Map<uint64, Market> -> unit
    type IslandSingleMarketSink = Location -> (uint64 * Market) -> unit
    type IslandSingleMarketSource = Location -> uint64 -> Market option
    type IslandSingleStatisticSink = Location->IslandStatisticIdentifier*Statistic option->unit
    type IslandSingleStatisticSource = Location * IslandStatisticIdentifier->Statistic option
    type IslandStatisticTemplateSource = unit -> Map<IslandStatisticIdentifier, StatisticTemplate>
    type IslandSingleFeatureSource = Location -> IslandFeatureIdentifier -> bool
    type IslandSource = unit -> Location list
    type IslandFeatureSource = Location -> IslandFeatureIdentifier list

    type GetStatisticTemplatesContext =
        abstract member islandStatisticTemplateSource : IslandStatisticTemplateSource ref
    let private GetStatisticTemplates
            (context : CommonContext)
            : Map<IslandStatisticIdentifier, StatisticTemplate> =
        (context :?> GetStatisticTemplatesContext).islandStatisticTemplateSource.Value ()

    type CreateContext = 
        abstract member islandSingleStatisticSink     : IslandSingleStatisticSink ref

    let private SetIslandStatistic
            (context : CommonContext)
            (location : Location)
            (identifier : IslandStatisticIdentifier, statistic: Statistic option)
            : unit =
        (context :?> CreateContext).islandSingleStatisticSink.Value location (identifier, statistic)
        
    let internal Create
            (context  : CommonContext)
            (location : Location)
            : unit =
        GetStatisticTemplates context
        |> Map.map
            (fun _ template ->
                    Statistic.CreateFromTemplate template)
        |> Map.iter
            (fun identifier statistic ->
                (identifier, statistic |> Some)
                |> SetIslandStatistic context location)

    type GetCommoditiesContext =
        abstract member islandMarketSource : IslandMarket.IslandMarketSource ref
    let private GetCommodities
            (context : CommonContext)
            (location : Location)
            : Map<uint64, Market> =
        (context :?> GetCommoditiesContext).islandMarketSource.Value location
    type PutCommoditiesContext =
        abstract member islandMarketSink   : IslandMarketSink ref
    let private PutCommodities
            (context : CommonContext)
            (location: Location)
            (markets : Map<uint64, Market>)
            : unit =
        (context :?> PutCommoditiesContext).islandMarketSink.Value location markets
    let internal GenerateCommodities 
            (context  : CommonContext)
            (location : Location) 
            : unit =
        if (GetCommodities context location).IsEmpty then
            (Map.empty, Commodity.GetCommodities context)
            ||> Map.fold
                (fun markets commodity _->
                    markets
                    |> Map.add 
                        commodity 
                        {
                            Supply = context |> Utility.GenerateSupplyOrDemand
                            Demand = context |> Utility.GenerateSupplyOrDemand
                        })
            |> PutCommodities context location

    type PutIslandItemsContext =
        abstract member islandItemSink   : IslandItemSink ref
    let private PutIslandItems
            (context : CommonContext)
            (location : Location)
            (items : Set<uint64>)
            : unit =
        (context :?> PutIslandItemsContext).islandItemSink.Value location items

    type GetIslandItemsContext =
        abstract member islandItemSource : IslandItemSource ref
    let private GetIslandItems
            (context : CommonContext)
            (location : Location)
            : Set<uint64> =
        (context :?> GetIslandItemsContext).islandItemSource.Value location

    let internal GenerateItems 
            (context  : CommonContext)
            (location : Location) 
            : unit =
        if (GetIslandItems context location).IsEmpty then
            (Set.empty, Item.GetList context)
            ||> Map.fold 
                (fun items item descriptor -> 
                    let table =
                        Map.empty
                        |> Map.add true descriptor.Occurrence
                        |> Map.add false (1.0 - descriptor.Occurrence)
                    if Utility.GenerateFromWeightedValues context table |> Option.defaultValue false then
                        items |> Set.add item
                    else
                        items)
            |> PutIslandItems context location

    type ChangeMarketTransform = float * Market -> Market
    type GetIslandMarketContext =
        abstract member islandSingleMarketSource : IslandSingleMarketSource ref
    let private GetIslandMarket
            (context : CommonContext)
            (location : Location)
            (itemId : uint64)
            : Market option =
        (context :?> GetIslandMarketContext).islandSingleMarketSource.Value location itemId
    type PutIslandMarketContext =
        abstract member islandSingleMarketSink   : IslandSingleMarketSink ref
    let private PutIslandMarket
            (context : CommonContext)
            (location : Location)
            (itemId:uint64, market:Market)
            : unit =
        (context :?> PutIslandMarketContext).islandSingleMarketSink.Value location (itemId, market)

    let private ChangeMarket
            (transform: ChangeMarketTransform)
            (context : CommonContext)
            (commodity                : uint64) 
            (change                   : float) 
            (location                 : Location) 
            : unit =
        commodity
        |> GetIslandMarket context location
        |> Option.map (fun m -> transform (change, m))
        |> Option.iter (fun market -> PutIslandMarket context location (commodity, market))

    let private ChangeMarketDemand (context:CommonContext) =
        ChangeMarket Market.ChangeDemand context

    let private ChangeMarketSupply (context:CommonContext) = 
        ChangeMarket Market.ChangeSupply context

    let internal UpdateMarketForItemSale 
            (context      : CommonContext)
            (descriptor   : ItemDescriptor) 
            (quantitySold : uint64) 
            (location     : Location) 
            : unit =
        let commodities = Commodity.GetCommodities context
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantitySold |> float) * commodities.[commodity].SaleFactor
                ChangeMarketDemand context commodity totalQuantity location)

    let internal UpdateMarketForItemPurchase 
            (context      : CommonContext)
            (descriptor               : ItemDescriptor) 
            (quantityPurchased        : uint64) 
            (location                 : Location) 
            : unit =
        let commodities = Commodity.GetCommodities context
        descriptor.Commodities
        |> Map.iter 
            (fun commodity quantityContained -> 
                let totalQuantity = quantityContained * (quantityPurchased |> float) * commodities.[commodity].PurchaseFactor
                ChangeMarketSupply context commodity totalQuantity location)

    type GetStatisticContext =
        abstract member islandSingleStatisticSource : IslandSingleStatisticSource ref
    let internal GetStatistic
            (context    : CommonContext)
            (identifier : IslandStatisticIdentifier)
            (location   : Location)
            : Statistic option =
        (context :?> GetStatisticContext).islandSingleStatisticSource.Value (location, identifier)

    type GetListContext =
        abstract member islandSource           : IslandSource ref
    let internal GetList
            (context : CommonContext)
            : Location list =
        (context :?> GetListContext).islandSource.Value()

    let internal Exists
            (context : CommonContext)
            (location : Location)
            : bool =
        GetList context
        |> List.tryFind (fun x -> x = location)
        |> Option.isSome

    type GetItemsContext =
        abstract member islandItemSource   : IslandItemSource ref
    let internal GetItems
            (context : CommonContext)
            (location : Location)
            : Set<uint64> =
        (context :?> GetItemsContext).islandItemSource.Value location

    type HasFeatureContext =
        abstract member islandSingleFeatureSource : IslandSingleFeatureSource ref
    let internal HasFeature
            (context    : CommonContext)
            (identifier : IslandFeatureIdentifier)
            (location   : Location)
            : bool =
        (context :?> HasFeatureContext).islandSingleFeatureSource.Value location identifier

    type GetFeaturesContext = 
        abstract member islandFeatureSource            : IslandFeatureSource ref
    let internal GetFeatures
            (context : CommonContext)
            (location : Location)
            : IslandFeatureIdentifier list =
        (context :?> GetFeaturesContext).islandFeatureSource.Value location

    let internal GetMinimumGamblingStakes
            (context : CommonContext)
            (location : Location)
            : float =
        GetStatistic 
            context 
            IslandStatisticIdentifier.MinimumGamblingStakes 
            location 
        |> Option.get
        |> Statistic.GetCurrentValue



