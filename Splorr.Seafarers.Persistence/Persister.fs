namespace Splorr.Seafarers.Persistence

open Splorr.Seafarers.Models
open System.Data.SQLite

module Persister =
    let Save (connection:SQLiteConnection) (name:string) (world:World) : Result<bool, exn> = 
        try
            try
                connection.Open()
                SaveSlot.Create connection name
                |> Result.map (fun _ -> true)
            with
            | ex -> ex |> Error
        finally
            connection.Close()

