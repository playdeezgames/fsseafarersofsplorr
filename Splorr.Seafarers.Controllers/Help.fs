namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Help =
    let private AtSea (sink:MessageSink) : unit =
        [
            ""
            "At Sea Commands:"
            "\tquit - quits the game"
            "\tset heading (degrees) [(minutes) [(seconds)]] - sets a new heading"
            "\tset speed (speed) - sets a new speed"
            "\tmove - moves for one turn"
            "\tmenu - brings up the main menu"
            "\tdock - docks at an island, if one is close enough"
            "\tislands [page] - lists known island names, direction and distances"
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

    let private Docked (sink:MessageSink) : unit =
        [
            ""
            "Docked Commands:"
            "\tundock - undocks from the island"
            "\tquit - quits the game"
        ]
        |> List.iter sink

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        match gamestate with
        | AtSea _ ->
            sink |> AtSea    
        | ConfirmQuit _ ->
            sink |> ConfirmQuit
        | Docked _ ->
            sink |> Docked
        | _ ->
            ()
        gamestate
        |> Some

