namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Help =
    let private AtSea (sink:MessageSink) : unit =
        [
            ""
            "At Sea Commands:"
            "\tdock - docks at an island, if one is close enough"
            "\thead for (island name) - heads for an island if it exists and is known"
            "\tislands [page] - lists known island names, direction and distances"
            "\tmenu - brings up the main menu"
            "\tmove - moves for one turn"
            "\tquit - quits the game"
            "\tset heading (degrees) [(minutes) [(seconds)]] - sets a new heading"
            "\tset speed (speed) - sets a new speed"
            "\tstatus - shows the avatar's status"
        ]
        |> List.iter sink

    let private ConfirmQuit (sink:MessageSink) : unit =
        [
            ""
            "Confirm Quit Commands:"
            "\tno - cancels quitting and returns you to the game"
            "\tyes - confirms that you want to quit"
        ]
        |> List.iter sink

    let private Docked (sink:MessageSink) : unit =
        [
            ""
            "Docked Commands:"
            "\tquit - quits the game"
            "\tstatus - shows the avatar's status"
            "\tundock - undocks from the island"
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

