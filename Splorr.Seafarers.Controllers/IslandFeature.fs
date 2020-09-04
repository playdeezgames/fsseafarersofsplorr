namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type IslandSingleFeatureSource = Location -> IslandFeatureIdentifier -> bool

type IslandFeatureRunFeatureContext =
    abstract member avatarMessageSource : AvatarMessageSource

type IslandFeatureRunIslandContext = 
    inherit IslandFeatureRunFeatureContext
    abstract member islandSingleFeatureSource : IslandSingleFeatureSource

type IslandFeatureRunContext =
    inherit IslandFeatureRunIslandContext
    abstract member islandSingleNameSource    : IslandSingleNameSource

module IslandFeature =
    let private RunFeature
            (context       : IslandFeatureRunFeatureContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (feature       : IslandFeatureIdentifier)
            (avatarId      : string)
            : Gamestate option = 
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
            (Feature feature, location, avatarId)
            |> Gamestate.Docked
            |> Gamestate.Help
            |> Some
        | Some Command.Leave ->
            (Dock, location, avatarId)
            |> Gamestate.Docked
            |> Some
        | _ ->
            ("Maybe try 'help'?",
                (Feature feature, location, avatarId)
                |> Gamestate.Docked)
            |> Gamestate.ErrorMessage
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
            (Dock, location, avatarId)
            |> Gamestate.Docked
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
            |> Gamestate.AtSea
            |> Some

