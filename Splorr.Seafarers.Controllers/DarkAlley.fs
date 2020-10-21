namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Common

module DarkAlley = 
    let private RunNoGamblingHand 
            (context: CommonContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location: Location)
            (avatarId: string)
            : Gamestate option =
        if World.HasDarkAlleyMinimumStakes context avatarId |> not then
            avatarId
            |> World.AddMessages
                context
                [ "Come back when you've got more money!" ]
            World.EnterAvatarIslandFeature
                context 
                avatarId 
                location 
                IslandFeatureIdentifier.Dock
            avatarId
            |> Gamestate.InPlay
            |> Some
        else
            "" |> Line |> messageSink
            avatarId
            |> World.GetAvatarMessages context
            |> Utility.DumpMessages messageSink
            [
                (Hue.Heading, "You are in the dark alley." |> Line) |> Hued
                (Hue.Subheading ,"Off in a corner, some shady characters are playing a card game and gambling." |> Line) |> Hued
            ]
            |> List.iter messageSink

            let command = commandSource()
            match (BaseGameState.HandleCommand context command avatarId), command with
            | Some newState, _ ->
                newState
                |> Some

            | _, Some Command.Leave ->
                World.EnterAvatarIslandFeature
                    context 
                    avatarId 
                    location
                    IslandFeatureIdentifier.Dock
                avatarId
                |> Gamestate.InPlay
                |> Some
            | _, Some Command.Gamble ->
                avatarId
                |> World.DealAvatarGamblingHand
                    context 
                avatarId
                |> Gamestate.InPlay
                |> Some
            | _ ->
                BaseGameState.HandleCommand context None avatarId

    let internal Run
            (context       : CommonContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (avatarId      : string)
            : Gamestate option =
        match World.GetAvatarGamblingHand context avatarId with
        | Some hand ->
            DarkAlleyGamblingHand.Run
                context
                commandSource
                messageSink
                location
                avatarId
                hand
        | None ->
            RunNoGamblingHand
                context
                commandSource
                messageSink
                location
                avatarId
