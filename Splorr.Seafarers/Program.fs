open Splorr.Seafarers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System.Data.SQLite

let connectionString = "Data Source=seafarers.db;Version=3;"
let avatarId = ""
[<EntryPoint>]
let main argv =
    use connection = new SQLiteConnection(connectionString)
    try
        connection.Open()
        avatarId
        |> Runner.Run connection
    finally
        connection.Close()
    0
