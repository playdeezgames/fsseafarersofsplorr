open Splorr.Seafarers
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers

let bootstrapConnection () 
        : SQLiteConnection =
    let sourceConnectionString = "Data Source=seafarers.db;Version=3;"
    let connectionString = "Data Source=:memory:;Version=3;"
    use source = new SQLiteConnection(sourceConnectionString)
    let destination = new SQLiteConnection(connectionString)
    source.Open()
    destination.Open()
    source.BackupDatabase(destination, "main", "main", -1, null, 0)
    source.Close()
    destination

module private Persister =
    let unpackOrThrow (result:Result<'T,string>) : 'T =
        match result with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

type SplorrContext 
        (avatarInventorySink: AvatarInventorySink ,
        avatarInventorySource: AvatarInventorySource ,
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
        islandFeatureGeneratorSource: IslandFeatureGeneratorSource ,
        islandItemSink: IslandItemSink ,
        islandItemSource: IslandItemSource ,
        islandJobPurger: IslandJobPurger ,
        islandJobSink: IslandJobSink ,
        islandJobSource: IslandJobSource ,
        islandLocationByNameSource: IslandLocationByNameSource ,
        islandMarketSink: IslandMarketSink ,
        islandMarketSource: IslandMarketSource ,
        islandSingleFeatureSink : IslandSingleFeatureSink,
        islandSingleJobSource: IslandSingleJobSource ,
        islandSingleMarketSink: IslandSingleMarketSink ,
        islandSingleMarketSource: IslandSingleMarketSource ,
        islandSingleNameSink: IslandSingleNameSink ,
        islandSingleNameSource: IslandSingleNameSource ,
        islandSingleStatisticSink: IslandSingleStatisticSink ,
        islandSingleStatisticSource: IslandSingleStatisticSource ,
        islandSource: IslandSource ,
        islandStatisticTemplateSource: IslandStatisticTemplateSource ,
        itemSource: ItemSource ,
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
    interface RunnerRunContext with
        member _.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member _.avatarMessagePurger: AvatarMessagePurger = avatarMessagePurger
        member _.avatarMessageSource: AvatarMessageSource = avatarMessageSource
        member _.avatarMetricSource: AvatarMetricSource = avatarMetricSource
        member _.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
        member _.islandJobPurger: IslandJobPurger = islandJobPurger
        member _.islandLocationByNameSource: IslandLocationByNameSource = islandLocationByNameSource
        member _.islandSingleJobSource: IslandSingleJobSource = islandSingleJobSource
        member _.islandSingleMarketSink: IslandSingleMarketSink = islandSingleMarketSink
        member _.islandSingleMarketSource: IslandSingleMarketSource = islandSingleMarketSource
        member _.islandSingleNameSource: IslandSingleNameSource = islandSingleNameSource
        member _.islandSingleStatisticSource: IslandSingleStatisticSource = islandSingleStatisticSource
        member _.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource
        member _.switchSource: SwitchSource = switchSource
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface WorldDockContext with
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
        member _.termSources: TermSources = termSources
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarJobSink: AvatarJobSink = avatarJobSink
        member _.islandSource: IslandSource = islandSource
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource

    interface WorldCreateContext with
        member _.islandSingleFeatureSink : IslandSingleFeatureSink = islandSingleFeatureSink
        member _.islandFeatureGeneratorSource: IslandFeatureGeneratorSource = islandFeatureGeneratorSource
        member _.islandSingleNameSink: IslandSingleNameSink = islandSingleNameSink
        member _.islandSingleStatisticSink: IslandSingleStatisticSink = islandSingleStatisticSink
        member _.islandStatisticTemplateSource: IslandStatisticTemplateSource = islandStatisticTemplateSource
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource
        member _.termNameSource: TermSource = termNameSource
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarJobSink: AvatarJobSink = avatarJobSink
        member _.islandSource: IslandSource = islandSource
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource




[<EntryPoint>]
let main argv =
    let switches =
        argv
        |> Array.map (fun x -> x.ToLower())
        |> Set.ofArray

    use connection = bootstrapConnection()

    let islandItemSource = 
        IslandItem.GetForIsland connection
        >> Persister.unpackOrThrow

    let islandItemSink 
            (location:Location) 
            =
        IslandItem.CreateForIsland connection location
        >> Persister.unpackOrThrow

    let islandMarketSource =
        IslandMarket.GetForIsland connection
        >> Persister.unpackOrThrow

    let islandSingleMarketSource 
            (location: Location)=
        IslandMarket.GetMarketForIsland connection location
        >> Persister.unpackOrThrow

    let islandMarketSink 
            (location:Location)=
        IslandMarket.CreateForIsland connection location
        >> Persister.unpackOrThrow

    let islandSingleMarketSink 
            (location:Location) =
        IslandMarket.SetForIsland connection location
        >> Persister.unpackOrThrow

    let commoditySource () = 
        Commodity.GetList connection
        |> Persister.unpackOrThrow

    let itemSource () = 
        Item.GetList connection
        |> Persister.unpackOrThrow

    let vesselStatisticTemplateSource () = 
        VesselStatisticTemplate.GetList connection
        |> Persister.unpackOrThrow

    let vesselStatisticSink 
            (avatarId:string) =
        VesselStatistic.SetForAvatar connection avatarId
        >> Persister.unpackOrThrow

    let vesselSingleStatisticSource 
            (avatarId:string)=
        VesselStatistic.GetStatisticForAvatar connection avatarId
        >> Persister.unpackOrThrow

    let vesselSingleStatisticSink 
            (avatarId: string)=
        VesselStatistic.SetStatisticForAvatar connection avatarId
        >> Persister.unpackOrThrow

    let adverbSource () =
        Term.GetForTermType connection "adverb"
        |> Persister.unpackOrThrow

    let adjectiveSource() =
        Term.GetForTermType connection "adjective"
        |> Persister.unpackOrThrow

    let objectNameSource() : string list =
        Term.GetForTermType connection "object name"
        |> Persister.unpackOrThrow

    let personNameSource() : string list =
        Term.GetForTermType connection "person name"
        |> Persister.unpackOrThrow

    let personAdjectiveSource() : string list =
        Term.GetForTermType connection "person adjective"
        |> Persister.unpackOrThrow

    let professionSource() : string list =
        Term.GetForTermType connection "profession"
        |> Persister.unpackOrThrow

    let termNameSource() : string list =
        Term.GetForTermType connection "island name"
        |> Persister.unpackOrThrow

    let termSources = 
        (adverbSource, 
            adjectiveSource, 
            objectNameSource, 
            personNameSource, 
            personAdjectiveSource, 
            professionSource)

    let avatarMessageSource =
        Message.GetForAvatar connection
        >> Persister.unpackOrThrow

    let avatarMessageSink
            (avatarId:string) 
            (message:string )=
        Message.AddForAvatar connection (avatarId, message)
        |> Persister.unpackOrThrow

    let avatarMessagePurger=
        Message.ClearForAvatar connection
        >> Persister.unpackOrThrow

    let shipmateIdentifierToString =
        function
        | Primary -> "primary"

    let stringToShipmateIdentifier =
        function
        | "primary" -> Primary
        | x -> raise (System.NotImplementedException (x |> sprintf "stringToShipmateIdentifier %s"))

    let shipmateRationItemSource
            (avatarId   : string) =
        shipmateIdentifierToString
        >> ShipmateRationItem.GetForShipmate connection avatarId
        >> Persister.unpackOrThrow

    let shipmateRationItemSink
            (avatarId   : string) 
            (shipmateId : ShipmateIdentifier)=
        ShipmateRationItem.SetForShipmate connection avatarId (shipmateId |> shipmateIdentifierToString)
        >> Persister.unpackOrThrow

    let rationItemSource () 
            : uint64 list =
        connection |> RationItem.GetRationItems
        |> Persister.unpackOrThrow

    let shipmateStatisticTemplateSource () 
            : Map<ShipmateStatisticIdentifier, ShipmateStatisticTemplate> =
        connection |> ShipmateStatisticTemplate.GetList
        |> Persister.unpackOrThrow

    let worldSingleStatisticSource =
        WorldStatistic.Get connection
        >> Persister.unpackOrThrow

    let avatarShipmateSource =
        ShipmateStatistic.GetShipmatesForAvatar connection
        >> Persister.unpackOrThrow
        >> List.map stringToShipmateIdentifier

    let shipmateSingleStatisticSource 
            (avatarId: string) 
            (shipmateId:ShipmateIdentifier)=
        ShipmateStatistic.GetStatisticForShipmate connection avatarId (shipmateId |> shipmateIdentifierToString)
        >> Persister.unpackOrThrow

    let shipmateSingleStatisticSink 
            (avatarId: string) 
            (shipmateId:ShipmateIdentifier)=
        ShipmateStatistic.SetStatisticForShipmate connection avatarId (shipmateId |> shipmateIdentifierToString)
        >> Persister.unpackOrThrow

    let avatarInventorySource =
        AvatarInventory.GetForAvatar connection
        >> Persister.unpackOrThrow

    let avatarInventorySink (avatarId:string) =
        AvatarInventory.SetForAvatar connection avatarId
        >> Persister.unpackOrThrow

    let switchSource () = switches

    let avatarSingleMetricSink
            (avatarId : string)
            =
        AvatarMetric.SetMetricForAvatar connection avatarId
        >> Persister.unpackOrThrow

    let avatarSingleMetricSource
            (avatarId : string) =
        AvatarMetric.GetMetricForAvatar connection avatarId
        >> Persister.unpackOrThrow

    let avatarMetricSource=
        AvatarMetric.GetForAvatar connection
        >> Persister.unpackOrThrow

    let avatarJobSink 
            (avatarId: string) =
        AvatarJob.SetForAvatar connection avatarId
        >> Persister.unpackOrThrow

    let avatarJobSource =
        AvatarJob.GetForAvatar connection
        >> Persister.unpackOrThrow

    let avatarIslandSingleMetricSink 
            (avatarId : string) 
            (location : Location) 
            (metric   : AvatarIslandMetricIdentifier) 
            (value    : uint64)= 
        AvatarIslandMetric.SetMetricForAvatarIsland connection avatarId location metric (value |> Some)
        |> Persister.unpackOrThrow

    let avatarIslandSingleMetricSource 
            (avatarId : string) 
            (location : Location) =
        AvatarIslandMetric.GetMetricForAvatarIsland connection avatarId location
        >> Persister.unpackOrThrow

    let islandLocationByNameSource =
        Island.GetByName connection
        >> Persister.unpackOrThrow

    let islandSingleNameSink 
            (location:Location)
            =
        Island.SetName connection location
        >> Persister.unpackOrThrow

    let islandSingleNameSource =
        Island.GetName connection
        >> Persister.unpackOrThrow

    let islandSingleStatisticSink 
            (location: Location) =
        IslandStatistic.SetStatisticForIsland connection location
        >> Persister.unpackOrThrow

    let islandSingleStatisticSource 
            (location: Location) =
        IslandStatistic.GetStatisticForIsland connection location
        >> Persister.unpackOrThrow

    let islandStatisticTemplateSource () = 
        IslandStatisticTemplate.GetList connection
        |> Persister.unpackOrThrow

    let islandJobSource = 
        IslandJob.GetForIsland connection
        >> Persister.unpackOrThrow

    let islandSource () : Location list = 
        Island.GetList connection
        |> Persister.unpackOrThrow

    let islandJobSink (location:Location) = 
        IslandJob.AddToIsland connection location
        >> Persister.unpackOrThrow

    let islandJobPurger(location:Location)= 
        IslandJob.RemoveFromIsland connection location
        >> Persister.unpackOrThrow

    let islandSingleJobSource(location:Location) = 
        IslandJob.GetForIslandByIndex connection location
        >> Persister.unpackOrThrow

    let islandFeatureGeneratorSource() =
        IslandFeature.GetGenerators connection
        |> Persister.unpackOrThrow

    let islandSingleFeatureSink (location:Location) =
        IslandFeature.AddToIsland connection location
        >> Persister.unpackOrThrow

    let context : RunnerRunContext =
        SplorrContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarIslandSingleMetricSink,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            avatarMessagePurger,
            avatarMessageSink,
            avatarMessageSource,
            avatarMetricSource,
            avatarShipmateSource,
            avatarSingleMetricSink,
            avatarSingleMetricSource,
            commoditySource,
            islandFeatureGeneratorSource,
            islandItemSink ,
            islandItemSource, 
            islandJobPurger,
            islandJobSink,
            islandJobSource,
            islandLocationByNameSource,
            islandMarketSink ,
            islandMarketSource, 
            islandSingleFeatureSink,
            islandSingleJobSource,
            islandSingleMarketSink, 
            islandSingleMarketSource,
            islandSingleNameSink,
            islandSingleNameSource,
            islandSingleStatisticSink,
            islandSingleStatisticSource,
            islandSource,
            islandStatisticTemplateSource,
            itemSource ,
            rationItemSource,
            shipmateRationItemSink,
            shipmateRationItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            shipmateStatisticTemplateSource,
            switchSource ,
            termNameSource,
            termSources,
            vesselSingleStatisticSink,
            vesselSingleStatisticSource,
            vesselStatisticSink,
            vesselStatisticTemplateSource,
            worldSingleStatisticSource) 
        :> RunnerRunContext

    try
        Runner.Run 
            context
    finally
        connection.Close()
    0
