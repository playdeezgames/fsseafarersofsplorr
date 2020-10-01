namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module IslandFeature =
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
            DarkAlley.Run
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
        match IslandName.GetName context location with
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

