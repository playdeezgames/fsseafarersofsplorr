open Splorr.Seafarers
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence

let bootstrapConnection () : SQLiteConnection =
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

    try
        Runner.Run 
            switches 
            configurationSource
            commoditySource
            itemSource 
            islandMarketSource 
            islandSingleMarketSource
            islandMarketSink 
            islandSingleMarketSink 
            islandItemSource 
            islandItemSink 
    finally
        connection.Close()
    0
