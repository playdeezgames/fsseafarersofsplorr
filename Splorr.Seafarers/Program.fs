open Splorr.Seafarers
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence

let connectionString = "Data Source=seafarers.db;Version=3;"
[<EntryPoint>]
let main argv =
    let switches =
        argv
        |> Array.map (fun x -> x.ToLower())
        |> Set.ofArray
    use connection = new SQLiteConnection(connectionString)
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
        connection.Open()
        Runner.Run switches islandMarketSource islandMarketSink islandSingleMarketSink islandItemSource islandItemSink connection
    finally
        connection.Close()
    0
