namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Common

module IslandFeature =
    let private RunFeature
            (context       : CommonContext)
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
            (context       : CommonContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (feature       : IslandFeatureIdentifier)
            (avatarId      : string)
            : Gamestate option =
        if World.HasIslandFeature context feature location then
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
            (context       : CommonContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (location      : Location)
            (feature       : IslandFeatureIdentifier)
            (avatarId      : string)
            : Gamestate option =
        match World.GetIslandName context location with
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

