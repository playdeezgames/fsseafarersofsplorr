namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type IslandFeatureSource = Location -> IslandFeatureIdentifier list

type DockedUpdateDisplayContext =
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member avatarMessageSource            : AvatarMessageSource
    abstract member islandSingleNameSource         : IslandSingleNameSource
    abstract member islandFeatureSource            : IslandFeatureSource

type DockedHandleCommandContext = 
    inherit World.AcceptJobContext
    inherit World.UndockContext
    inherit World.BuyItemsContext
    inherit World.SellItemsContext
    inherit World.AbandonJobContext
    inherit World.ClearMessagesContext
    inherit Avatar.EnterIslandFeatureContext
    abstract member avatarInventorySink : AvatarInventorySink
    abstract member avatarInventorySource : AvatarInventorySource
    abstract member avatarIslandSingleMetricSink : AvatarIslandSingleMetricSink
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member avatarJobSink : AvatarJobSink
    abstract member avatarJobSource : AvatarJobSource
    abstract member avatarMessagePurger : AvatarMessagePurger
    abstract member avatarMessageSink : AvatarMessageSink
    abstract member avatarSingleMetricSink : AvatarSingleMetricSink
    abstract member avatarSingleMetricSource : AvatarSingleMetricSource
    abstract member islandJobPurger : IslandJobPurger
    abstract member islandMarketSource : IslandMarketSource 
    abstract member islandSingleJobSource : IslandSingleJobSource
    abstract member islandSingleMarketSink : IslandSingleMarketSink 
    abstract member islandSingleMarketSource : IslandSingleMarketSource 
    abstract member islandSource : IslandSource
    abstract member itemSource : ItemSource 
    abstract member shipmateSingleStatisticSink : ShipmateSingleStatisticSink
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type DockedRunBoilerplateContext =
    inherit ServiceContext
    abstract member avatarMessageSource           : AvatarMessageSource
    abstract member islandSource                  : IslandSource


type DockedRunContext =
    inherit DockedUpdateDisplayContext
    inherit DockedHandleCommandContext
    inherit DockedRunBoilerplateContext

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
        let context = context :?> DockedUpdateDisplayContext
        "" |> Line |> messageSink
        avatarId
        |> context.avatarMessageSource
        |> Utility.DumpMessages messageSink
        context.islandFeatureSource location
        |> List.map
            getFeatureDisplayName
        |> List.append
            [
                (Hue.Heading, sprintf "You are docked at '%s':" (context.islandSingleNameSource location |> Option.get) |> Line) |> Hued
                (Hue.Flavor, sprintf "You have visited %u times." (context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount |> Option.defaultValue 0UL) |> Line) |> Hued
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
            //enter the feature if the island has it
            Avatar.EnterIslandFeature 
                context 
                avatarId 
                location 
                feature
            //context.avatarIslandFeatureSink ({featureId = feature; location = location} |> Some, avatarId)//TODO: this should become an avatar module function
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
        let context = context :?> DockedRunBoilerplateContext
        if avatarId |> World.IsAvatarAlive context then
            if context.islandSource() |> List.exists (fun x->x= location) then
                func location avatarId
            else
                avatarId
                |> Gamestate.InPlay
                |> Some
        else
            avatarId
            |> context.avatarMessageSource
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
