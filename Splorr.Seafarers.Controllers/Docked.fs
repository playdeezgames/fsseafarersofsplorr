namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

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
            (context     : ServiceContext)
            (messageSink : MessageSink) 
            (location    : Location) 
            (avatarId    : string) 
            : unit =
        "" |> Line |> messageSink
        avatarId
        |> AvatarMessages.Get context
        |> Utility.DumpMessages messageSink
        location
        |> Island.GetFeatures context 
        |> List.map
            getFeatureDisplayName
        |> List.append
            [
                (Hue.Heading, sprintf "You are docked at '%s':" (Island.GetName context location |> Option.get) |> Line) |> Hued
                (Hue.Flavor, sprintf "You have visited %u times." (AvatarMetric.GetForIsland context location AvatarIslandMetricIdentifier.VisitCount avatarId |> Option.defaultValue 0UL) |> Line) |> Hued
            ]
        |> List.iter messageSink

    let private HandleCommand
            (context : ServiceContext)
            (command                        : Command option) 
            (location                       : Location) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages context

        match command with
        | Some (Command.GoTo feature) ->
            Avatar.EnterIslandFeature 
                context 
                avatarId 
                location 
                feature
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some (Command.AcceptJob index) ->
            avatarId 
            |> World.AcceptJob
                context
                index 
                location
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some (Command.Buy (quantity, itemName))->
            avatarId 
            |> World.BuyItems 
                context
                location 
                quantity 
                itemName
            avatarId 
            |> Gamestate.InPlay
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            avatarId 
            |> World.SellItems 
                context
                location 
                quantity 
                itemName
            avatarId 
            |> Gamestate.InPlay
            |> Some            

        | Some Command.Items ->
            avatarId 
            |> Gamestate.InPlay
            |> Gamestate.ItemList
            |> Some

        | Some Command.Jobs ->
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.Jobs
            |> Some

        | Some Command.Status ->
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.Status
            |> Some

        | Some (Command.Abandon Job) ->
            avatarId 
            |> World.AbandonJob 
                context
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some Command.Undock ->
            avatarId 
            |> World.Undock context
            avatarId
            |> Gamestate.InPlay 
            |> Some

        | Some Command.Quit ->
            avatarId 
            |> Gamestate.InPlay 
            |> Gamestate.ConfirmQuit 
            |> Some

        | Some Command.Inventory ->
            avatarId 
            |> Gamestate.InPlay 
            |> Gamestate.Inventory 
            |> Some

        | Some Command.Help ->
            avatarId 
            |> Gamestate.InPlay 
            |> Gamestate.Help 
            |> Some

        | Some Command.Metrics ->
            avatarId 
            |> Gamestate.InPlay 
            |> Gamestate.Metrics 
            |> Some

        | _ -> 
            ("Maybe try 'help'?",avatarId 
            |> Gamestate.InPlay)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunWithIsland 
            (context                        : ServiceContext)
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
            (context : ServiceContext)
            (func                          : Location -> string -> Gamestate option) 
            (location                      : Location) 
            (avatarId                      : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive context then
            if context |> Island.GetList |> List.exists (fun x->x= location) then
                func location avatarId
            else
                avatarId
                |> Gamestate.InPlay
                |> Some
        else
            avatarId
            |> AvatarMessages.Get context
            |> Gamestate.GameOver
            |> Some

    let Run 
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) =
        RunBoilerplate 
            context
            (RunWithIsland 
                context
                commandSource                
                messageSink)                 
