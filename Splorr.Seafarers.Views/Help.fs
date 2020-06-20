namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models

module Help =
    let private AtSea (sink:MessageSink) : unit =
        [
            ""
            "At Sea Commands:"
            "\tquit - quits the game"
            "\tset heading (degrees) [(minutes) [(seconds)]] - sets a new heading"
            "\tset speed (speed)"
            "\tmove"
        ]
        |> List.iter sink

    let private ConfirmQuit (sink:MessageSink) : unit =
        [
            ""
            "Confirm Quit Commands:"
            "\tyes - confirms that you want to quit"
            "\tno - cancels quitting and returns you to the game"
        ]
        |> List.iter sink

    let Run (sink:MessageSink) (viewState:ViewState) : ViewState option =
        match viewState with
        | AtSea _ ->
            sink |> AtSea    
        | ConfirmQuit _ ->
            sink |> ConfirmQuit
        | _ ->
            ()
        viewState
        |> Some

