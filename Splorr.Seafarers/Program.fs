open Splorr.Seafarers
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open System

let bootstrapConnection (switches:Set<string>) 
        : SQLiteConnection =
    let sourceConnectionString = 
        if switches.Contains("-dev") then
            "Data Source=seafarers-dev.db;Version=3;"
        else
            "Data Source=seafarers.db;Version=3;"
    let connectionString = "Data Source=:memory:;Version=3;"
    use source = new SQLiteConnection(sourceConnectionString)
    let destination = new SQLiteConnection(connectionString)
    source.Open()
    destination.Open()
    source.BackupDatabase(destination, "main", "main", -1, null, 0)
    source.Close()
    destination

module private Persister =
    let unpackOrThrow (result:Result<'T, string>) : 'T =
        match result with
        | Ok x -> x
        | Error x -> raise (InvalidOperationException x)

[<EntryPoint>]
let main argv =
    let switches =
        argv
        |> Array.map (fun x -> x.ToLower())
        |> Set.ofArray

    use connection = bootstrapConnection(switches)

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
        Persistence.Item.GetList connection
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

    let termListSource
            (termType:string) 
            : string list =
        Term.GetForTermType connection termType
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
        | x -> raise (NotImplementedException (x |> sprintf "stringToShipmateIdentifier %s"))

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
            : Map<ShipmateStatisticIdentifier, StatisticTemplate> =
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
        Persistence.Island.GetName connection
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
        Persistence.Island.GetList connection
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

    let islandFeatureSource =
        IslandFeature.GetForIsland connection
        >> Persister.unpackOrThrow

    let islandSingleFeatureSource (location : Location) =
        IslandFeature.ExistsForIsland connection location
        >> Persister.unpackOrThrow

    let avatarIslandFeatureSink =
        AvatarIslandFeature.SetFeatureForAvatar connection
        >> Persister.unpackOrThrow

    let avatarIslandFeatureSource =
        AvatarIslandFeature.GetFeatureForAvatar connection
        >> Persister.unpackOrThrow

    let itemSingleSource (index:uint64) : ItemDescriptor option =
        itemSource()
        |> Map.tryFind index
        
    let avatarGamblingHandSource =
        AvatarGamblingHand.GetForAvatar connection
        >> Persister.unpackOrThrow
        
    let avatarGamblingHandSink (avatarId: string) =
        AvatarGamblingHand.SetForAvatar connection avatarId
        >> Persister.unpackOrThrow

    let context : ServiceContext =
        SplorrContext
            (avatarGamblingHandSink,
            avatarGamblingHandSource,
            avatarInventorySink,
            avatarInventorySource,
            avatarIslandFeatureSink,
            avatarIslandFeatureSource,
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
            (fun () -> DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            islandFeatureGeneratorSource,
            islandFeatureSource,
            islandItemSink ,
            islandItemSource, 
            islandJobPurger,
            islandJobSink,
            islandJobSource,
            islandLocationByNameSource,
            islandMarketSink ,
            islandMarketSource, 
            islandSingleFeatureSink,
            islandSingleFeatureSource,
            islandSingleJobSource,
            islandSingleMarketSink, 
            islandSingleMarketSource,
            islandSingleNameSink,
            islandSingleNameSource,
            islandSingleStatisticSink,
            islandSingleStatisticSource,
            islandSource,
            islandStatisticTemplateSource,
            itemSingleSource,
            itemSource ,
            Random(),
            rationItemSource,
            shipmateRationItemSink,
            shipmateRationItemSource,
            shipmateSingleStatisticSink,
            shipmateSingleStatisticSource,
            shipmateStatisticTemplateSource,
            switchSource ,
            termListSource,
            termNameSource,
            termSources,
            vesselSingleStatisticSink,
            vesselSingleStatisticSource,
            vesselStatisticSink,
            vesselStatisticTemplateSource,
            worldSingleStatisticSource) 
        :> ServiceContext

    try
        Runner.Run 
            context
    finally
        connection.Close()
    0
