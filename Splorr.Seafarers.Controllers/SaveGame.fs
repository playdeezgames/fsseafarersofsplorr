namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module SaveGame =
    let Run (sink:MessageSink) (name:string) (world:World) : Gamestate option =
        [
            "" |> Line
            "You saved the game." |> Line
        ]
        |> List.iter sink

        world
        |> Some
        |> Gamestate.MainMenu
        |> Some

