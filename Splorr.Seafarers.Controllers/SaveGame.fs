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
        | Ok false ->
            [
                "" |> Line
                "Saving the game failed for some reason." |> Line
            ]
            |> List.iter sink
            
        | Error ex ->
            [
                "" |> Line
                "Error occurred!" |> Line
                ex.ToString() |> Line
            ]
            |> List.iter sink

        world
        |> Some
        |> Gamestate.MainMenu
        |> Some

