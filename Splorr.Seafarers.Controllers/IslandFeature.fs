namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Tarot

type IslandSingleFeatureSource = Location -> IslandFeatureIdentifier -> bool

type IslandFeatureRunDarkAlleyGamblingHand =
    interface
    end

type IslandFeatureRunDarkAlleyContext =
    inherit IslandFeatureRunDarkAlleyGamblingHand
    inherit WorldAddMessagesContext
    inherit AvatarGetGamblingHandContext
    inherit AvatarDealGamblingHandContext
    abstract member avatarMessageSource           : AvatarMessageSource
    abstract member avatarMessageSink             : AvatarMessageSink
    abstract member islandSingleStatisticSource   : IslandSingleStatisticSource
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource

type IslandFeatureRunFeatureContext =
    inherit IslandFeatureRunDarkAlleyContext
    

type IslandFeatureRunIslandContext = 
    inherit IslandFeatureRunFeatureContext
    abstract member islandSingleFeatureSource : IslandSingleFeatureSource

type IslandFeatureRunContext =
    inherit IslandFeatureRunIslandContext
    abstract member islandSingleNameSource    : IslandSingleNameSource

module IslandFeature =
    let private RunDarkAlleyGamblingHand
            (context       : IslandFeatureRunDarkAlleyGamblingHand)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (avatarId      : string)
            (hand          : AvatarGamblingHand)
            : Gamestate option =
        //display the hand
        let (first, second, _) = hand
        [
            Line "The cards that you've been dealt:"
            Cards [ first; second ]
        ]
        |> Group
        |> messageSink
        //get input
        match commandSource() with
        
        | _ ->
            ("Maybe try 'help'?",
                        avatarId
                        |> Gamestate.InPlay)
                    |> Gamestate.ErrorMessage
                    |> Some
    let private RunDarkAlley
            (context       : IslandFeatureRunDarkAlleyContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (avatarId      : string)
            : Gamestate option =
        match Avatar.GetGamblingHand context avatarId with
        | Some hand ->
            RunDarkAlleyGamblingHand
                context
                commandSource
                messageSink
                location
                avatarId
                hand
        | None ->
            let minimumBet = 
                context.islandSingleStatisticSource 
                    location 
                    IslandStatisticIdentifier.MinimumGamblingStakes
                |> Option.get
                |> Statistic.GetCurrentValue
            let money =
                context.shipmateSingleStatisticSource 
                    avatarId 
                    ShipmateIdentifier.Primary 
                    ShipmateStatisticIdentifier.Money
                |> Option.get
                |> Statistic.GetCurrentValue
            if money < minimumBet then
                avatarId
                |> World.AddMessages
                    context
                    [ "Come back when you've got more money!" ]
                avatarId
                |> Gamestate.InPlay
                |> Some
            else
                "" |> Line |> messageSink
                avatarId
                |> context.avatarMessageSource
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
                    avatarId
                    |> Gamestate.InPlay
                    |> Some
                | Some Command.Gamble ->
                    avatarId
                    |> Avatar.DealGamblingHand
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
            (context       : IslandFeatureRunFeatureContext)
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
            (context       : IslandFeatureRunIslandContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (feature       : IslandFeatureIdentifier)
            (avatarId      : string)
            : Gamestate option =
        if context.islandSingleFeatureSource location feature then
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
            (context       : IslandFeatureRunContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (feature       : IslandFeatureIdentifier)
            (avatarId      : string)
            : Gamestate option =
        match context.islandSingleNameSource location with
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

