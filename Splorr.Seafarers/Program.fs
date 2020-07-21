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
    try
        connection.Open()
        Runner.Run switches islandItemSource islandItemSink connection
    finally
        connection.Close()
    0
