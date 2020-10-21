namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Common

module DarkAlleyGamblingHand =
    let internal Run
            (context       : CommonContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (avatarId      : string)
            (hand          : AvatarGamblingHand)
            : Gamestate option =
        "" |> Line |> messageSink
        avatarId
        |> World.GetAvatarMessages context
        |> Utility.DumpMessages messageSink

        avatarId
        |> World.ClearMessages context

        let (first, second, final) = hand
        [
            (Hue.Heading, Line "The cards that you've been dealt:") |> Hued
            (Hue.Cards, Cards [ first; second ]) |> Hued
            (Hue.Subheading , Line "What is your bet?") |> Hued
        ]
        |> Group
        |> messageSink
        let command = commandSource()
        match (BaseGameState.HandleCommand context command avatarId), command with
        | Some newState, _ ->
            newState
            |> Some

        | _, Some (Command.Bet None) ->
            avatarId
            |> World.FoldGamblingHand context
            avatarId
            |> Gamestate.InPlay
            |> Some   

        | _, Some (Command.Bet (Some amount)) ->
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
            BaseGameState.HandleCommand context None avatarId
