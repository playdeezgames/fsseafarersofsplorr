namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence
open System.Data.SQLite

module SaveGame =
    let Run (connection:SQLiteConnection) (sink:MessageSink) (name:string) (world:World) : Gamestate option =
        match (name, world) ||> Persister.Save connection with
        | Ok true ->
            [
                "" |> Line
                "You saved the game." |> Line
            ]
            |> List.iter sink
        | _ ->
            [
                "" |> Line
                "Error occurred!" |> Line
            ]
            |> List.iter sink

        world
        |> Some
        |> Gamestate.MainMenu
        |> Some

