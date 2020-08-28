namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Docked = 
    let private UpdateDisplay 
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarMessageSource            : AvatarMessageSource)
            (islandSingleNameSource         : IslandSingleNameSource)
            (messageSink                    : MessageSink) 
            (location                       : Location) 
            (avatarId                       : string) 
            : unit =
        "" |> Line |> messageSink
        avatarId
        |> avatarMessageSource
        |> Utility.DumpMessages messageSink
        [
            (Hue.Flavor, sprintf "You have visited %u times." (avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount |> Option.defaultValue 0UL) |> Line) |> Hued
            (Hue.Heading, sprintf "You are docked at '%s':" (islandSingleNameSource location |> Option.get) |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private HandleCommand
            (avatarInventorySink            : AvatarInventorySink)
            (avatarInventorySource          : AvatarInventorySource)
            (avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarJobSink                  : AvatarJobSink)
            (avatarJobSource                : AvatarJobSource)
            (avatarMessagePurger            : AvatarMessagePurger)
            (avatarMessageSink              : AvatarMessageSink)
            (avatarSingleMetricSink         : AvatarSingleMetricSink)
            (avatarSingleMetricSource       : AvatarSingleMetricSource)
            (commoditySource                : CommoditySource) 
            (islandJobPurger                : IslandJobPurger)
            (islandMarketSource             : IslandMarketSource) 
            (islandSingleJobSource          : IslandSingleJobSource)
            (islandSingleMarketSink         : IslandSingleMarketSink) 
            (islandSingleMarketSource       : IslandSingleMarketSource) 
            (islandSource                   : IslandSource)
            (itemSource                     : ItemSource) 
            (shipmateSingleStatisticSink    : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource  : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (command                        : Command option) 
            (location                       : Location) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages avatarMessagePurger

        match command with
        | Some (Command.AcceptJob index) ->
            avatarId 
            |> World.AcceptJob
                avatarIslandSingleMetricSink
                avatarIslandSingleMetricSource
                avatarJobSink
                avatarJobSource
                avatarMessageSink 
                avatarSingleMetricSink
                avatarSingleMetricSource
                islandJobPurger
                islandSingleJobSource
                islandSource
                index 
                location
            (Dock, 
                location, 
                avatarId)
            |> Gamestate.Docked
            |> Some

        | Some (Command.Buy (quantity, itemName))->
            avatarId 
            |> World.BuyItems 
                avatarInventorySink
                avatarInventorySource
                avatarMessageSink 
                commoditySource 
                islandMarketSource 
                islandSingleMarketSink 
                islandSingleMarketSource 
                islandSource
                itemSource 
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
                vesselSingleStatisticSource 
                location 
                quantity 
                itemName
            (Dock, 
                location, 
                    avatarId) 
            |> Gamestate.Docked
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            avatarId 
            |> World.SellItems 
                avatarInventorySink
                avatarInventorySource
                avatarMessageSink
                commoditySource 
                islandMarketSource 
                islandSingleMarketSink 
                islandSingleMarketSource 
                islandSource
                itemSource 
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
                location 
                quantity 
                itemName
            (Dock, 
                location, 
                    avatarId) 
            |> Gamestate.Docked
            |> Some            

        | Some Command.Items ->
            (ItemList, location, avatarId) 
            |> Gamestate.Docked
            |> Some

        | Some Command.Jobs ->
            (Jobs, location, avatarId)
            |> Gamestate.Docked
            |> Some

        | Some Command.Status ->
            (Dock, location, avatarId)
            |> Gamestate.Docked
            |> Gamestate.Status
            |> Some

        | Some (Command.Abandon Job) ->
            avatarId 
            |> World.AbandonJob 
                avatarJobSink
                avatarJobSource
                avatarMessageSink
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
            (Dock, 
                location, 
                    avatarId)
            |> Gamestate.Docked
            |> Some

        | Some Command.Undock ->
            avatarId 
            |> World.AddMessages  avatarMessageSink [ "You undock." ]
            avatarId
            |> Gamestate.AtSea 
            |> Some

        | Some Command.Quit ->
            (Dock, location, avatarId) 
            |> Gamestate.Docked 
            |> Gamestate.ConfirmQuit 
            |> Some

        | Some Command.Inventory ->
            (Dock, location, avatarId) 
            |> Gamestate.Docked 
            |> Gamestate.Inventory 
            |> Some

        | Some Command.Help ->
            (Dock, location, avatarId) 
            |> Gamestate.Docked 
            |> Gamestate.Help 
            |> Some

        | Some Command.Metrics ->
            (Dock, location, avatarId) 
            |> Gamestate.Docked 
            |> Gamestate.Metrics 
            |> Some

        | _ -> 
            ("Maybe try 'help'?",(Dock, location, avatarId) 
            |> Gamestate.Docked)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunWithIsland 
            (avatarInventorySink            : AvatarInventorySink)
            (avatarInventorySource          : AvatarInventorySource)
            (avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarJobSink                  : AvatarJobSink)
            (avatarJobSource                : AvatarJobSource)
            (avatarMessagePurger            : AvatarMessagePurger)
            (avatarMessageSink              : AvatarMessageSink)
            (avatarMessageSource            : AvatarMessageSource)
            (avatarSingleMetricSink         : AvatarSingleMetricSink)
            (avatarSingleMetricSource       : AvatarSingleMetricSource)
            (commoditySource                : CommoditySource) 
            (islandJobPurger                : IslandJobPurger)
            (islandMarketSource             : IslandMarketSource) 
            (islandSingleJobSource          : IslandSingleJobSource)
            (islandSingleMarketSink         : IslandSingleMarketSink) 
            (islandSingleMarketSource       : IslandSingleMarketSource) 
            (islandSingleNameSource         : IslandSingleNameSource)
            (islandSource                   : IslandSource)
            (itemSource                     : ItemSource) 
            (shipmateSingleStatisticSink    : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource  : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (commandSource                  : CommandSource) 
            (messageSink                    : MessageSink) 
            (location                       : Location) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> UpdateDisplay 
            avatarIslandSingleMetricSource
            avatarMessageSource
            islandSingleNameSource
            messageSink 
            location 
        
        avatarId   
        |> HandleCommand 
            avatarInventorySink
            avatarInventorySource
            avatarIslandSingleMetricSink
            avatarIslandSingleMetricSource
            avatarJobSink
            avatarJobSource
            avatarMessagePurger
            avatarMessageSink
            avatarSingleMetricSink
            avatarSingleMetricSource
            commoditySource 
            islandJobPurger
            islandMarketSource 
            islandSingleJobSource
            islandSingleMarketSink 
            islandSingleMarketSource
            islandSource
            itemSource 
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSource
            (commandSource()) 
            location 

    let internal RunBoilerplate 
            (avatarMessageSource           : AvatarMessageSource)
            (islandSource                  : IslandSource)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (func                          : Location -> string -> Gamestate option) 
            (location                      : Location) 
            (avatarId                      : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive shipmateSingleStatisticSource then
            if islandSource() |> List.exists (fun x->x= location) then
                func location avatarId
            else
                avatarId
                |> Gamestate.AtSea
                |> Some
        else
            avatarId
            |> avatarMessageSource
            |> Gamestate.GameOver
            |> Some

    let Run 
            (avatarInventorySink            : AvatarInventorySink)
            (avatarInventorySource          : AvatarInventorySource)
            (avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarJobSink                  : AvatarJobSink)
            (avatarJobSource                : AvatarJobSource)
            (avatarMessagePurger            : AvatarMessagePurger)
            (avatarMessageSink              : AvatarMessageSink)
            (avatarMessageSource            : AvatarMessageSource)
            (avatarSingleMetricSink         : AvatarSingleMetricSink)
            (avatarSingleMetricSource       : AvatarSingleMetricSource)
            (commoditySource                : CommoditySource) 
            (islandJobPurger                : IslandJobPurger)
            (islandMarketSource             : IslandMarketSource) 
            (islandSingleJobSource          : IslandSingleJobSource)
            (islandSingleMarketSink         : IslandSingleMarketSink) 
            (islandSingleMarketSource       : IslandSingleMarketSource) 
            (islandSingleNameSource         : IslandSingleNameSource)
            (islandSource                   : IslandSource)
            (itemSource                     : ItemSource) 
            (shipmateSingleStatisticSink    : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource  : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (commandSource                  : CommandSource) 
            (messageSink                    : MessageSink) =
        RunBoilerplate 
            avatarMessageSource
            islandSource
            shipmateSingleStatisticSource
            (RunWithIsland 
                avatarInventorySink
                avatarInventorySource
                avatarIslandSingleMetricSink
                avatarIslandSingleMetricSource
                avatarJobSink
                avatarJobSource
                avatarMessagePurger          
                avatarMessageSink            
                avatarMessageSource          
                avatarSingleMetricSink
                avatarSingleMetricSource
                commoditySource         
                islandJobPurger
                islandMarketSource           
                islandSingleJobSource
                islandSingleMarketSink       
                islandSingleMarketSource
                islandSingleNameSource
                islandSource
                itemSource                   
                shipmateSingleStatisticSink  
                shipmateSingleStatisticSource
                vesselSingleStatisticSource  
                commandSource                
                messageSink)                 
