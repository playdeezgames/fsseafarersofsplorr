open Splorr.Seafarers
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence

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

    try
        Runner.Run 
            avatarInventorySink
            avatarInventorySource
            avatarIslandSingleMetricSink
            avatarIslandSingleMetricSource
            avatarJobSink
            avatarJobSource
            avatarMessagePurger
            avatarMessageSink
            avatarMessageSource
            avatarMetricSource
            avatarShipmateSource
            avatarSingleMetricSink
            avatarSingleMetricSource
            commoditySource
            islandItemSink 
            islandItemSource 
            islandLocationByNameSource
            islandMarketSink 
            islandMarketSource 
            islandSingleMarketSink 
            islandSingleMarketSource
            islandSingleNameSink
            islandSingleNameSource
            islandSingleStatisticSink
            islandSingleStatisticSource
            islandStatisticTemplateSource
            itemSource 
            rationItemSource
            shipmateRationItemSink
            shipmateRationItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            shipmateStatisticTemplateSource
            switchSource 
            termNameSource
            termSources
            vesselSingleStatisticSink
            vesselSingleStatisticSource
            vesselStatisticSink
            vesselStatisticTemplateSource
            worldSingleStatisticSource
    finally
        connection.Close()
    0
