namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module GameOver =
    let Run (sink:MessageSink) (messages:string list): Gamestate option =
        "" |> Line |> sink
        messages |> Utility.DumpMessages sink
        None
        |> Gamestate.MainMenu
        |> Some

