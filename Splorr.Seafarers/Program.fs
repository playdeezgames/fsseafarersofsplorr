open Splorr.Seafarers
open System.Data.SQLite

let connectionString = "Data Source=seafarers.db;Version=3;"
[<EntryPoint>]
let main argv =
    let switches =
        argv
        |> Array.map (fun x -> x.ToLower())
        |> Set.ofArray
    use connection = new SQLiteConnection(connectionString)
    try
        connection.Open()
        Runner.Run switches connection
    finally
        connection.Close()
    0
