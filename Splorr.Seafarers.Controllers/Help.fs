namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Help =
    let private abandonJobMessage = 
        [
            (Label, "abandon job" |> Text) |> Hued
            (Usage, " - abandons your current job, if you have one" |> Line) |> Hued
        ] |> Group
    let private acceptJobMessage =
        [
            (Label, "accept job (number)" |> Text) |> Hued
            (Usage, " - accepts the offered job" |> Line) |> Hued
        ] |> Group
    let private buyMessage=
        [
            (Label, "buy (amount) (item)" |> Text) |> Hued
            (Usage, " - purchases items" |> Line) |> Hued
            (Label, "buy maximum (item)" |> Text) |> Hued
            (Usage, " - purchases as many of the item as you have funds for" |> Line) |> Hued
        ] |> Group
    let private careenMessage =
        [
            (Label, "careen to (port|starboard)" |> Text) |> Hued
            (Usage, " - careens the ship so that you can clean the hull (must be very close to an island to do this!)" |> Line) |> Hued
        ] |> Group
    let private chartMessage =
        [
            (Label, "chart [chart name]" |> Text) |> Hued
            (Usage, " - exports a png map and text file containing a legend to the map" |> Line) |> Hued
        ] |> Group
    let private cleanHullMessage =
        [
            (Label, "clean [the] hull" |> Text) |> Hued
            (Usage, " - cleans the hull" |> Line) |> Hued
        ] |> Group
    let private dockMessage=
        [
            (Label, "dock" |> Text) |> Hued
            (Usage, " - docks at an island, if one is close enough" |> Line) |> Hued
        ] |> Group
    let private headForMessage=
        [
            (Label, "head for (island name)" |> Text) |> Hued
            (Usage, " - heads for an island if it exists and is known" |> Line) |> Hued
        ] |> Group
    let private inventoryMessage=
        [
            (Label, "inventory" |> Text) |> Hued
            (Usage, " - shows you inventory" |> Line) |> Hued
        ] |> Group
    let private islandsMessage =
        [
            (Label, "islands [page]" |> Text) |> Hued
            (Usage, " - lists known island names, direction and distances" |> Line) |> Hued
        ] |> Group
    let private itemsMessage =
        [
            (Label, "items" |> Text) |> Hued
            (Usage, " - lists prices of items" |> Line) |> Hued
        ] |> Group
    let private jobsMessage =
        [
            (Label, "jobs" |> Text) |> Hued
            (Usage, " - lists job offers" |> Line) |> Hued
        ] |> Group
    let private menuMessage=
        [
            (Label, "menu" |> Text) |> Hued
            (Usage, " - brings up the main menu" |> Line) |> Hued
        ] |> Group
    let private metricsMessage =
        [
            (Label, "metrics" |> Text) |> Hued
            (Usage, " - shows metrics" |> Line) |> Hued
        ] |> Group
    let private moveMessage =
        [
            (Label, "move (turns)" |> Text) |> Hued
            (Usage, " - moves for the given number of turns, with a default of 1 turn" |> Line) |> Hued
        ] |> Group
    let private noMessage =
        [
            (Label, "no" |> Line) |> Hued
            (Usage, " - cancels the action" |> Line) |> Hued
        ] |> Group
    let private quitMessage =
        [
            (Label, "quit" |> Text) |> Hued
            (Usage, " - quits the game" |> Line) |> Hued
        ] |> Group
    let private sellMessage =
        [
            (Label, "sell (amount) (item)" |> Text) |> Hued
            (Usage, " - sells items" |> Line) |> Hued
            (Label, "sell all (item)" |> Text) |> Hued
            (Usage, " - sells all of that particular item in your inventory" |> Line) |> Hued
        ] |> Group
    let private setHeadingMessage =
        [
            (Label, "set heading (degrees) [(minutes) [(seconds)]]" |> Text) |> Hued
            (Usage, " - sets a new heading" |> Line) |> Hued
        ] |> Group
    let private setSpeedMessage =
        [
            (Label, "set speed (speed)" |> Text) |> Hued
            (Usage, " - sets a new speed" |> Line) |> Hued
        ] |> Group
    let private statusMessage =
        [
            (Label, "status" |> Text) |> Hued
            (Usage, " - shows the avatar's status" |> Line) |> Hued
        ] |> Group
    let private weighAnchorMessage =
        [
            (Label, "weigh anchor" |> Text) |> Hued
            (Usage, " - get back to sailing" |> Line) |> Hued
        ] |> Group
    let private undockMessage =
        [
            (Label, "undock" |> Text) |> Hued
            (Usage, " - undocks from the island" |> Line) |> Hued
        ] |> Group
    let private yesMessage =
        [
            (Label, "yes" |> Line) |> Hued
            (Usage, " - confirms the action" |> Line) |> Hued
        ] |> Group
    let private AtSea (sink:MessageSink) : unit =
        [
            "" |> Line
            (Heading, "At Sea Commands:" |> Line) |> Hued
            abandonJobMessage
            careenMessage
            chartMessage
            dockMessage
            headForMessage
            inventoryMessage
            islandsMessage
            menuMessage
            metricsMessage
            moveMessage
            quitMessage
            setHeadingMessage
            setSpeedMessage
            statusMessage
        ]
        |> List.iter sink

    let private Careened (sink:MessageSink) : unit =
        [
            "" |> Line
            (Heading, "Careened Commands:" |> Line) |> Hued
            cleanHullMessage
            inventoryMessage
            metricsMessage
            quitMessage
            statusMessage
            weighAnchorMessage
        ]
        |> List.iter sink

    let private ConfirmQuit (sink:MessageSink) : unit =
        [
            "" |> Line
            (Heading, "Confirm Quit Commands:" |> Line) |> Hued
            noMessage
            yesMessage
        ]
        |> List.iter sink

    let private Docked (sink:MessageSink) : unit =
        [
            "" |> Line
            (Heading, "Docked Commands:" |> Line) |> Hued
            abandonJobMessage
            acceptJobMessage 
            buyMessage
            inventoryMessage
            itemsMessage
            jobsMessage
            metricsMessage
            quitMessage
            sellMessage
            statusMessage
            undockMessage
        ]
        |> List.iter sink

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        match gamestate with
        | Gamestate.AtSea _ ->
            sink |> AtSea    
        | Gamestate.Careened _ ->
            sink |> Careened
        | Gamestate.ConfirmQuit _ ->
            sink |> ConfirmQuit
        | Gamestate.Docked (Dock, _, _) ->
            sink |> Docked
        | _ ->
            ()
        gamestate
        |> Some

