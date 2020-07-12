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
            (Label, "abandon job" |> Text) |> Hued
            (Usage, " - abandons your current job, if you have one" |> Line) |> Hued
            (Label, "accept job (number)" |> Text) |> Hued
            (Usage, " - accepts the offered job" |> Line) |> Hued
            (Label, "buy (amount) (item)" |> Text) |> Hued
            (Usage, " - purchases items" |> Line) |> Hued
            (Label, "buy maximum (item)" |> Text) |> Hued
            (Usage, " - purchases as many of the item as you have funds for" |> Line) |> Hued
            (Label, "jobs" |> Text) |> Hued
            (Usage, " - lists job offers" |> Line) |> Hued
            (Label, "items" |> Text) |> Hued
            (Usage, " - lists prices of items" |> Line) |> Hued
            (Label, "sell (amount) (item)" |> Text) |> Hued
            (Usage, " - sells items" |> Line) |> Hued
            (Label, "status" |> Text) |> Hued
            (Usage, " - shows the avatar's status" |> Line) |> Hued
            (Label, "undock" |> Text) |> Hued
            (Usage, " - undocks from the island" |> Line) |> Hued
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
        | _ ->
            ()
        gamestate
        |> Some

