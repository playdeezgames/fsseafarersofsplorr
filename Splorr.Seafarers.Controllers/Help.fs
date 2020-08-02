﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Help =
    let private abandonJobMessage = 
        [
            (Hue.Label, "abandon job" |> Text) |> Hued
            (Hue.Usage, " - abandons your current job, if you have one" |> Line) |> Hued
        ] |> Group

    let private acceptJobMessage =
        [
            (Hue.Label, "accept job (number)" |> Text) |> Hued
            (Hue.Usage, " - accepts the offered job" |> Line) |> Hued
        ] |> Group

    let private buyMessage=
        [
            (Hue.Label, "buy (amount) (item)" |> Text) |> Hued
            (Hue.Usage, " - purchases items" |> Line) |> Hued
            (Hue.Label, "buy maximum (item)" |> Text) |> Hued
            (Hue.Usage, " - purchases as many of the item as you have funds for" |> Line) |> Hued
        ] |> Group

    let private careenMessage =
        [
            (Hue.Label, "careen to (port|starboard)" |> Text) |> Hued
            (Hue.Usage, " - careens the ship so that you can clean the hull (must be very close to an island to do this!)" |> Line) |> Hued
        ] |> Group

    let private chartMessage =
        [
            (Hue.Label, "chart [chart name]" |> Text) |> Hued
            (Hue.Usage, " - exports a png map and text file containing a legend to the map" |> Line) |> Hued
        ] |> Group

    let private cleanHullMessage =
        [
            (Hue.Label, "clean [the] hull" |> Text) |> Hued
            (Hue.Usage, " - cleans the hull" |> Line) |> Hued
        ] |> Group

    let private dockMessage=
        [
            (Hue.Label, "dock" |> Text) |> Hued
            (Hue.Usage, " - docks at an island, if one is close enough" |> Line) |> Hued
        ] |> Group

    let private headForMessage=
        [
            (Hue.Label, "head for (island name)" |> Text) |> Hued
            (Hue.Usage, " - heads for an island if it exists and is known" |> Line) |> Hued
        ] |> Group

    let private inventoryMessage=
        [
            (Hue.Label, "inventory" |> Text) |> Hued
            (Hue.Usage, " - shows you inventory" |> Line) |> Hued
        ] |> Group

    let private islandsMessage =
        [
            (Hue.Label, "islands [page]" |> Text) |> Hued
            (Hue.Usage, " - lists known island names, direction and distances" |> Line) |> Hued
        ] |> Group

    let private itemsMessage =
        [
            (Hue.Label, "items" |> Text) |> Hued
            (Hue.Usage, " - lists prices of items" |> Line) |> Hued
        ] |> Group

    let private jobsMessage =
        [
            (Hue.Label, "jobs" |> Text) |> Hued
            (Hue.Usage, " - lists job offers" |> Line) |> Hued
        ] |> Group

    let private menuMessage=
        [
            (Hue.Label, "menu" |> Text) |> Hued
            (Hue.Usage, " - brings up the main menu" |> Line) |> Hued
        ] |> Group

    let private metricsMessage =
        [
            (Hue.Label, "metrics" |> Text) |> Hued
            (Hue.Usage, " - shows metrics" |> Line) |> Hued
        ] |> Group

    let private moveMessage =
        [
            (Hue.Label, "move (turns)" |> Text) |> Hued
            (Hue.Usage, " - moves for the given number of turns, with a default of 1 turn" |> Line) |> Hued
        ] |> Group

    let private noMessage =
        [
            (Hue.Label, "no" |> Line) |> Hued
            (Hue.Usage, " - cancels the action" |> Line) |> Hued
        ] |> Group

    let private quitMessage =
        [
            (Hue.Label, "quit" |> Text) |> Hued
            (Hue.Usage, " - quits the game" |> Line) |> Hued
        ] |> Group

    let private sellMessage =
        [
            (Hue.Label, "sell (amount) (item)" |> Text) |> Hued
            (Hue.Usage, " - sells items" |> Line) |> Hued
            (Hue.Label, "sell all (item)" |> Text) |> Hued
            (Hue.Usage, " - sells all of that particular item in your inventory" |> Line) |> Hued
        ] |> Group

    let private setHeadingMessage =
        [
            (Hue.Label, "set heading (degrees) [(minutes) [(seconds)]]" |> Text) |> Hued
            (Hue.Usage, " - sets a new heading" |> Line) |> Hued
        ] |> Group

    let private setSpeedMessage =
        [
            (Hue.Label, "set speed (speed)" |> Text) |> Hued
            (Hue.Usage, " - sets a new speed" |> Line) |> Hued
        ] |> Group

    let private statusMessage =
        [
            (Hue.Label, "status" |> Text) |> Hued
            (Hue.Usage, " - shows the avatar's status" |> Line) |> Hued
        ] |> Group

    let private weighAnchorMessage =
        [
            (Hue.Label, "weigh anchor" |> Text) |> Hued
            (Hue.Usage, " - get back to sailing" |> Line) |> Hued
        ] |> Group

    let private undockMessage =
        [
            (Hue.Label, "undock" |> Text) |> Hued
            (Hue.Usage, " - undocks from the island" |> Line) |> Hued
        ] |> Group

    let private yesMessage =
        [
            (Hue.Label, "yes" |> Line) |> Hued
            (Hue.Usage, " - confirms the action" |> Line) |> Hued
        ] |> Group

    let private AtSea 
            (messageSink : MessageSink) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "At Sea Commands:" |> Line) |> Hued
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
        |> List.iter messageSink

    let private Careened 
            (messageSink : MessageSink) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "Careened Commands:" |> Line) |> Hued
            cleanHullMessage
            inventoryMessage
            metricsMessage
            quitMessage
            statusMessage
            weighAnchorMessage
        ]
        |> List.iter messageSink

    let private ConfirmQuit 
            (messageSink : MessageSink) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "Confirm Quit Commands:" |> Line) |> Hued
            noMessage
            yesMessage
        ]
        |> List.iter messageSink

    let private Docked 
            (messageSink : MessageSink) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "Docked Commands:" |> Line) |> Hued
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
        |> List.iter messageSink

    let Run 
            (messageSink : MessageSink) 
            (gamestate   : Gamestate) 
            : Gamestate option =
        match gamestate with
        | Gamestate.AtSea _ ->
            messageSink |> AtSea    
        | Gamestate.Careened _ ->
            messageSink |> Careened
        | Gamestate.ConfirmQuit _ ->
            messageSink |> ConfirmQuit
        | Gamestate.Docked (Dock, _, _) ->
            messageSink |> Docked
        | _ ->
            ()
        gamestate
        |> Some

