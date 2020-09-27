namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Tarot

module IslandFeature =
    let private RunDarkAlleyGamblingHand
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (avatarId      : string)
            (hand          : AvatarGamblingHand)
            : Gamestate option =
        let (first, second, _) = hand
        [
            Line "The cards that you've been dealt:"
            Cards [ first; second ]
        ]
        |> Group
        |> messageSink
        match commandSource() with
        | Some (Command.Bet None) ->
            avatarId
            |> AvatarGamblingHand.Fold
                context 
            avatarId
            |> Gamestate.InPlay
            |> Some   
        | Some (Command.Bet amount) ->
            //does avatar have enough money? - error if no
            //did avatar make minimum stakes bet? - error if no
            //deduct money
            avatarId
            |> Gamestate.InPlay
            |> Some   
        | _ ->
            ("Maybe try 'help'?",
                        avatarId
                        |> Gamestate.InPlay)
                    |> Gamestate.ErrorMessage
                    |> Some
    let private RunDarkAlley
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (avatarId      : string)
            : Gamestate option =
        match AvatarGamblingHand.Get context avatarId with
        | Some hand ->
            RunDarkAlleyGamblingHand
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


    let private RunFeature
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (feature       : IslandFeatureIdentifier)
            (avatarId      : string)
            : Gamestate option = 
        match feature with
        | IslandFeatureIdentifier.DarkAlley ->
            RunDarkAlley
                context
                commandSource
                messageSink
                location
                avatarId
        | _ ->
            avatarId
            |> Gamestate.InPlay
            |> Some
            

    let private RunIsland
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (feature       : IslandFeatureIdentifier)
            (avatarId      : string)
            : Gamestate option =
        if Island.HasFeature context feature location then
            RunFeature
                context
                commandSource
                messageSink
                location
                feature
                avatarId
        else
            avatarId
            |> Gamestate.InPlay
            |> Some

    let Run 
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (feature       : IslandFeatureIdentifier)
            (avatarId      : string)
            : Gamestate option =
        match Island.GetName context location with
        | Some _ ->
            RunIsland
                context
                commandSource
                messageSink
                location
                feature
                avatarId
        | _ ->
            avatarId
            |> Gamestate.InPlay
            |> Some

