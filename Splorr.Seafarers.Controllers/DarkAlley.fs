namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module DarkAlley = 
    let internal Run
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (avatarId      : string)
            : Gamestate option =
        match AvatarGamblingHand.Get context avatarId with
        | Some hand ->
            DarkAlleyGamblingHand.Run
                context
                commandSource
                messageSink
                location
                avatarId
                hand
        | None ->
            //TODO : wrap this into World.EnsureDarkAlleyMinimumStakes
            if World.HasDarkAlleyMinimumStakes context location avatarId |> Option.defaultValue false |> not then
                avatarId
                |> World.AddMessages
                    context
                    [ "Come back when you've got more money!" ]
                AvatarIslandFeature.Enter 
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
                |> AvatarMessages.Get context
                |> Utility.DumpMessages messageSink
                [
                    (Hue.Heading, "You are in the dark alley." |> Line) |> Hued
                ]
                |> List.iter messageSink
                match commandSource() with
                | Some (Command.Status) ->
                    avatarId
                    |> Gamestate.InPlay
                    |> Gamestate.Status
                    |> Some

                | Some Command.Help ->
                    avatarId
                    |> Gamestate.InPlay
                    |> Gamestate.Help
                    |> Some

                | Some Command.Leave ->
                    AvatarIslandFeature.Enter 
                        context 
                        avatarId 
                        location
                        IslandFeatureIdentifier.Dock
                    avatarId
                    |> Gamestate.InPlay
                    |> Some
                | Some Command.Gamble ->
                    avatarId
                    |> AvatarGamblingHand.Deal
                        context 
                    avatarId
                    |> Gamestate.InPlay
                    |> Some
                | _ ->
                    ("Maybe try 'help'?",
                        avatarId
                        |> Gamestate.InPlay)
                    |> Gamestate.ErrorMessage
                    |> Some


