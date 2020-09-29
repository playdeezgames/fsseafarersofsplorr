namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module DarkAlleyGamblingHand =
    let internal Run
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (avatarId      : string)
            (hand          : AvatarGamblingHand)
            : Gamestate option =
        "" |> Line |> messageSink
        avatarId
        |> AvatarMessages.Get context
        |> Utility.DumpMessages messageSink

        avatarId
        |> World.ClearMessages context

        let (first, second, final) = hand
        [
            (Hue.Heading, Line "The cards that you've been dealt:") |> Hued
            (Hue.Cards, Cards [ first; second ]) |> Hued
            Line "What is your bet?"
        ]
        |> Group
        |> messageSink
        match commandSource() with
        | Some (Command.Status) ->
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.Status
            |> Some

        | Some (Command.Bet None) ->
            avatarId
            |> World.FoldGamblingHand context
            avatarId
            |> Gamestate.InPlay
            |> Some   

        | Some (Command.Bet (Some amount)) ->
            if World.BetOnGamblingHand context amount avatarId then
                [
                    (Hue.Heading, Line "The final card:") |> Hued
                    (Hue.Cards, Cards [ final ]) |> Hued
                ]
                |> Group
                |> messageSink
            avatarId
            |> Gamestate.InPlay
            |> Some   
        | _ ->
            ("Maybe try 'help'?",
                        avatarId
                        |> Gamestate.InPlay)
                    |> Gamestate.ErrorMessage
                    |> Some
