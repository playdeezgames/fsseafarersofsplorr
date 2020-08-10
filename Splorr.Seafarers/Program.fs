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
    let parameterlessFetcher
            (connection : SQLiteConnection) 
            (fetcher    : SQLiteConnection -> Result<'T,string>) 
            : unit -> 'T =
        match connection |> fetcher with
        | Ok x -> (fun () -> x)
        | Error x ->  raise (System.InvalidOperationException x)      

[<EntryPoint>]
let main argv =
    let switches =
        argv
        |> Array.map (fun x -> x.ToLower())
        |> Set.ofArray

    use connection = bootstrapConnection()

    let islandItemSource (location:Location) =
        match location |> IslandItem.GetForIsland connection with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let islandItemSink (location:Location) (items:Set<uint64>) =
        IslandItem.CreateForIsland connection location items
        |> ignore

    let islandMarketSource (location:Location) =
        match location |> IslandMarket.GetForIsland connection with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let islandSingleMarketSource (location:Location) (itemId:uint64) =
        match location |> IslandMarket.GetMarketForIsland connection itemId with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let islandMarketSink (location:Location) (markets:Map<uint64, Market>)=
        IslandMarket.CreateForIsland connection location markets
        |> ignore

    let islandSingleMarketSink (location:Location) (data:uint64 * Market)=
        IslandMarket.SetForIsland connection location data
        |> ignore

    let commoditySource = Persister.parameterlessFetcher connection Commodity.GetList
    let itemSource = Persister.parameterlessFetcher connection Item.GetList
    let configurationSource = Persister.parameterlessFetcher connection WorldConfiguration.Get

    let vesselStatisticTemplateSource = Persister.parameterlessFetcher connection VesselStatisticTemplate.GetList
    let vesselStatisticSink (avatarId:string) (statistics:Map<VesselStatisticIdentifier, Statistic>) : unit =
        VesselStatistic.SetForAvatar avatarId statistics connection
        |> ignore

    let vesselSingleStatisticSource (avatarId:string) (identifier:VesselStatisticIdentifier) : Statistic option =
        match VesselStatistic.GetStatisticForAvatar avatarId identifier connection with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let vesselSingleStatisticSink (avatarId: string) (identifier:VesselStatisticIdentifier, statistic:Statistic) : unit =
        VesselStatistic.SetStatisticForAvatar avatarId (identifier, statistic) connection
        |> ignore

    let adverbSource() : string list =
        match connection |> Term.GetForTermType "adverb" with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let adjectiveSource() : string list =
        match connection |> Term.GetForTermType "adjective" with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let objectNameSource() : string list =
        match connection |> Term.GetForTermType "object name" with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let personNameSource() : string list =
        match connection |> Term.GetForTermType "person name" with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let personAdjectiveSource() : string list =
        match connection |> Term.GetForTermType "person adjective" with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let professionSource() : string list =
        match connection |> Term.GetForTermType "profession" with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let islandNameSource() : string list =
        match connection |> Term.GetForTermType "island name" with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let termSources = 
        (adverbSource, adjectiveSource, objectNameSource, personNameSource, personAdjectiveSource, professionSource)

    let avatarMessageSource(avatarId:string) : string list =
        match connection |> Message.GetForAvatar avatarId with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let avatarMessageSink(avatarId:string) (message:string) : unit =
        match connection |> Message.AddForAvatar (avatarId, message) with
        | Ok _ -> ()
        | Error x -> raise (System.InvalidOperationException x)

    let avatarMessagePurger(avatarId:string) : unit =
        match connection |> Message.ClearForAvatar avatarId with
        | Ok _ -> ()
        | Error x -> raise (System.InvalidOperationException x)

    let shipmateIdentifierToString =
        function
        | Primary -> "primary"

    let shipmateRationItemSource
            (avatarId   : string) 
            (shipmateId : ShipmateIdentifier) : uint64 list =
        match connection |> ShipmateRationItem.GetForShipmate avatarId (shipmateId |> shipmateIdentifierToString) with
        | Ok x -> x
        | Error x -> raise (System.InvalidOperationException x)

    let shipmateRationItemSink
            (avatarId   : string) 
            (shipmateId : ShipmateIdentifier) 
            (items      : uint64 list) 
            : unit =
        match connection |> ShipmateRationItem.SetForShipmate avatarId (shipmateId |> shipmateIdentifierToString) items with
        | Ok _ -> ()
        | Error x -> raise (System.InvalidOperationException x)

    try
        Runner.Run 
            switches 
            islandNameSource
            termSources
            configurationSource
            commoditySource
            itemSource 
            islandMarketSource 
            islandSingleMarketSource
            islandMarketSink 
            islandSingleMarketSink 
            islandItemSource 
            islandItemSink 
            vesselStatisticTemplateSource
            vesselStatisticSink
            vesselSingleStatisticSource
            vesselSingleStatisticSink
            shipmateRationItemSource
            shipmateRationItemSink
            avatarMessageSource
            avatarMessageSink
            avatarMessagePurger
    finally
        connection.Close()
    0
