namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Help =
    let private AtSea (sink:MessageSink) : unit =
        [
            ""
            "At Sea Commands:"
            "\tabandon job - abandons your current job, if you have one"
            "\tdock - docks at an island, if one is close enough"
            "\thead for (island name) - heads for an island if it exists and is known"
            "\tislands [page] - lists known island names, direction and distances"
            "\tmenu - brings up the main menu"
            "\tmove (turns)- moves for the given number of turns, with a default of 1 turn"
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
            "\tabandon job - abandons your current job, if you have one"
            "\taccept job (number) - accepts the offered job"
            "\tjobs - lists job offers"
            "\tprices - lists traded commodity prices for the island"
            "\tshop - goes to the shop where items may be bought and sold"
            "\tstatus - shows the avatar's status"
            "\tundock - undocks from the island"
        ]
        |> List.iter sink

    let private Docked (sink:MessageSink) : unit =
        [
            ""
            "Shop Commands:"
            "\tdock - returns to the dock"
            "\tstatus - shows the avatar's status"
            
        ]
        |> List.iter sink

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        match gamestate with
        | Gamestate.AtSea _ ->
            sink |> AtSea    
        | Gamestate.ConfirmQuit _ ->
            sink |> ConfirmQuit
        | Gamestate.Docked _ ->
            sink |> Docked
        | Gamestate.Shop _ ->
            sink |> Shop
        | _ ->
            ()
        gamestate
        |> Some

