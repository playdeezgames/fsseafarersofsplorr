namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Common

module Docked = 
    let private getFeatureDisplayName (feature:IslandFeatureIdentifier) : Message =
        match feature with
        | IslandFeatureIdentifier.Dock ->
            "" |> Text
        | IslandFeatureIdentifier.DarkAlley ->
            (Hue.Flavor, "There is a dark alley here." |> Line) |> Hued
        | _ ->
            raise (System.NotImplementedException (sprintf "%s" (feature.ToString())))

    let private UpdateDisplay 
            (context     : CommonContext)
            (messageSink : MessageSink) 
            (location    : Location) 
            (avatarId    : string) 
            : unit =
        "" |> Line |> messageSink
        avatarId
        |> World.GetAvatarMessages context
        |> Utility.DumpMessages messageSink
        location
        |> World.GetIslandFeatures context 
        |> List.map
            getFeatureDisplayName
        |> List.append
            [
                (Hue.Heading, sprintf "You are docked at '%s':" (World.GetIslandName context location |> Option.get) |> Line) |> Hued
                (Hue.Flavor, sprintf "You have visited %u times." (World.GetAvatarIslandMetric context avatarId location AvatarIslandMetricIdentifier.VisitCount |> Option.defaultValue 0UL) |> Line) |> Hued
            ]
        |> List.iter messageSink

    let private HandleCommand
            (context : CommonContext)
            (command                        : Command option) 
            (location                       : Location) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages context

        match (BaseGameState.HandleCommand context command avatarId), command with
        | Some newState, _ ->
            newState
            |> Some

        | _, Some (Command.GoTo feature) ->
            World.EnterAvatarIslandFeature
                context 
                avatarId 
                location 
                feature
            avatarId
            |> Gamestate.InPlay
            |> Some

        | _, Some (Command.AcceptJob index) ->
            avatarId 
            |> World.AcceptJob
                context
                index 
                location
            avatarId
            |> Gamestate.InPlay
            |> Some

        | _, Some (Command.Buy (quantity, itemName))->
            avatarId 
            |> World.BuyItems 
                context
                location 
                quantity 
                itemName
            avatarId 
            |> Gamestate.InPlay
            |> Some            

        | _, Some (Command.Sell (quantity, itemName))->
            avatarId 
            |> World.SellItems 
                context
                location 
                quantity 
                itemName
            avatarId 
            |> Gamestate.InPlay
            |> Some            

        | _, Some Command.Items ->
            avatarId 
            |> Gamestate.InPlay
            |> Gamestate.ItemList
            |> Some

        | _, Some Command.Jobs ->
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.Jobs
            |> Some

        | _, Some Command.Undock ->
            avatarId 
            |> World.Undock context
            avatarId
            |> Gamestate.InPlay 
            |> Some

        | _ -> 
            BaseGameState.HandleCommand context None avatarId

    let private RunWithIsland 
            (context                        : CommonContext)
            (commandSource                  : CommandSource) 
            (messageSink                    : MessageSink) 
            (location                       : Location) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> UpdateDisplay 
            context
            messageSink 
            location 
        
        avatarId   
        |> HandleCommand 
            context
            (commandSource()) 
            location 

    let internal RunBoilerplate 
            (context : CommonContext)
            (func                          : Location -> string -> Gamestate option) 
            (location                      : Location) 
            (avatarId                      : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive context then
            if context |> World.GetIslandList |> List.exists (fun x->x= location) then
                func location avatarId
            else
                avatarId
                |> Gamestate.InPlay
                |> Some
        else
            avatarId
            |> World.GetAvatarMessages context
            |> Gamestate.GameOver
            |> Some

    let Run 
            (context       : CommonContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) =
        RunBoilerplate 
            context
            (RunWithIsland 
                context
                commandSource                
                messageSink)                 
