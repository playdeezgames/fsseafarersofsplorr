namespace Splorr.Seafarers.Persistence

open Splorr.Seafarers.Models
open System.Data.SQLite

module Persister =
    let Save (connection:SQLiteConnection) (name:string) (world:World) : Result<bool, exn> = 
        try
            try
                connection.Open()
                World.Save connection name world
                |> Result.map (fun worldId -> true)
            with
            | ex -> ex |> Error
        finally
            connection.Close()

