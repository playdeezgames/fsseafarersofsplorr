namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Help =
    let private AtSea (sink:MessageSink) : unit =
        [
            "" |> Line
            (Heading, "At Sea Commands:" |> Line) |> Hued
            (Label, "abandon job" |> Text) |> Hued
            (Usage, " - abandons your current job, if you have one" |> Line) |> Hued
            (Label, "dock" |> Text) |> Hued
            (Usage, " - docks at an island, if one is close enough" |> Line) |> Hued
            (Label, "head for (island name)" |> Text) |> Hued
            (Usage, " - heads for an island if it exists and is known" |> Line) |> Hued
            (Label, "islands [page]" |> Text) |> Hued
            (Usage, " - lists known island names, direction and distances" |> Line) |> Hued
            (Label, "menu" |> Text) |> Hued
            (Usage, " - brings up the main menu" |> Line) |> Hued
            (Label, "move (turns)" |> Text) |> Hued
            (Usage, " - moves for the given number of turns, with a default of 1 turn" |> Line) |> Hued
            (Label, "quit" |> Text) |> Hued
            (Usage, " - quits the game" |> Line) |> Hued
            (Label, "set heading (degrees) [(minutes) [(seconds)]]" |> Text) |> Hued
            (Usage, " - sets a new heading" |> Line) |> Hued
            (Label, "set speed (speed)" |> Text) |> Hued
            (Usage, " - sets a new speed" |> Line) |> Hued
            (Label, "status" |> Text) |> Hued
            (Usage, " - shows the avatar's status" |> Line) |> Hued
        ]
        |> List.iter sink

    let private ConfirmQuit (sink:MessageSink) : unit =
        [
            "" |> Line
            (Heading, "Confirm Quit Commands:" |> Line) |> Hued
            "\tno - cancels quitting and returns you to the game" |> Line
            "\tyes - confirms that you want to quit" |> Line
        ]
        |> List.iter sink

    let private Docked (sink:MessageSink) : unit =
        [
            "" |> Line
            (Heading, "Docked Commands:" |> Line) |> Hued
            "\tabandon job - abandons your current job, if you have one" |> Line
            "\taccept job (number) - accepts the offered job" |> Line
            "\tjobs - lists job offers" |> Line
            "\tprices - lists traded commodity prices for the island" |> Line
            "\tshop - goes to the shop where items may be bought and sold" |> Line
            "\tstatus - shows the avatar's status" |> Line
            "\tundock - undocks from the island" |> Line
        ]
        |> List.iter sink

    let private Shop (sink:MessageSink) : unit =
        [
            "" |> Line
            (Heading, "Shop Commands:" |> Line) |> Hued
            "\tdock - returns to the dock" |> Line
            "\titems - lists prices of items" |> Line
            "\tquit - quits the game" |> Line
            "\tstatus - shows the avatar's status" |> Line
            
        ]
        |> List.iter sink

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        match gamestate with
        | Gamestate.AtSea _ ->
            sink |> AtSea    
        | Gamestate.ConfirmQuit _ ->
            sink |> ConfirmQuit
        | Gamestate.Docked (Dock, _, _) ->
            sink |> Docked
        | Gamestate.Docked (Shop, _, _) ->
            sink |> Shop
        | _ ->
            ()
        gamestate
        |> Some

