namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models
open System

module Game =
    let Export 
            (connection : SQLiteConnection)
            (filename: string)
            : Result<string, string> =
        try
            let result = sprintf "%s.db" filename
            let connectionString = result |> sprintf "Data Source=%s;Version=3;"
            let destination = new SQLiteConnection(connectionString)
            destination.Open()
            connection.BackupDatabase(destination, "main", "main", -1, null, 0)
            destination.Close()
            result |> Ok
        with
        | ex ->
            ex.ToString() |> Error


