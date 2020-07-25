open Splorr.Seafarers
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence

let sourceConnectionString = "Data Source=seafarers.db;Version=3;"
let connectionString = "Data Source=:memory:;Version=3;"
let bootstrapConnection () : SQLiteConnection =
    use source = new SQLiteConnection(sourceConnectionString)
    let destination = new SQLiteConnection(connectionString)
    source.Open()
    destination.Open()
    source.BackupDatabase(destination, "main", "main", -1, null, 0)
    source.Close()
    destination

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
    let islandMarketSink (location:Location) (markets:Map<uint64, Market>)=
        IslandMarket.CreateForIsland connection location markets
        |> ignore
    let islandSingleMarketSink (location:Location) (data:uint64 * Market)=
        IslandMarket.SetForIsland connection location data
        |> ignore
    try
        Runner.Run switches islandMarketSource islandMarketSink islandSingleMarketSink islandItemSource islandItemSink connection
    finally
        connection.Close()
    0
