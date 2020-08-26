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
            (world                          : World) 
            : unit =
        "" |> Line |> messageSink
        world.AvatarId
        |> avatarMessageSource
        |> Utility.DumpMessages messageSink
        [
            (Hue.Flavor, sprintf "You have visited %u times." (avatarIslandSingleMetricSource world.AvatarId location AvatarIslandMetricIdentifier.VisitCount |> Option.defaultValue 0UL) |> Line) |> Hued
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
            (world                          : World) 
            : Gamestate option =
        world
        |> World.ClearMessages avatarMessagePurger

        match command with
        | Some (Command.AcceptJob index) ->
            world 
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
                world)
            |> Gamestate.Docked
            |> Some

        | Some (Command.Buy (quantity, itemName))->
            world 
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
                    world) 
            |> Gamestate.Docked
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            (Dock, 
                location, 
                    world 
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
                        itemName) 
            |> Gamestate.Docked
            |> Some            

        | Some Command.Items ->
            (ItemList, location, world) 
            |> Gamestate.Docked
            |> Some

        | Some Command.Jobs ->
            (Jobs, location, world)
            |> Gamestate.Docked
            |> Some

        | Some Command.Status ->
            (Dock, location, world)
            |> Gamestate.Docked
            |> Gamestate.Status
            |> Some

        | Some (Command.Abandon Job) ->
            (Dock, 
                location, 
                world 
                |> World.AbandonJob 
                    avatarJobSink
                    avatarJobSource
                    avatarMessageSink
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    shipmateSingleStatisticSink
                    shipmateSingleStatisticSource
                    )
            |> Gamestate.Docked
            |> Some

        | Some Command.Undock ->
            world 
            |> World.AddMessages  avatarMessageSink [ "You undock." ]
            world
            |> Gamestate.AtSea 
            |> Some

        | Some Command.Quit ->
            (Dock, location, world) 
            |> Gamestate.Docked 
            |> Gamestate.ConfirmQuit 
            |> Some

        | Some Command.Inventory ->
            (Dock, location, world) 
            |> Gamestate.Docked 
            |> Gamestate.Inventory 
            |> Some

        | Some Command.Help ->
            (Dock, location, world) 
            |> Gamestate.Docked 
            |> Gamestate.Help 
            |> Some

        | Some Command.Metrics ->
            (Dock, location, world) 
            |> Gamestate.Docked 
            |> Gamestate.Metrics 
            |> Some

        | _ -> 
            ("Maybe try 'help'?",(Dock, location, world) 
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
            (world                          : World) 
            : Gamestate option =
        world
        |> UpdateDisplay 
            avatarIslandSingleMetricSource
            avatarMessageSource
            islandSingleNameSource
            messageSink 
            location 
        
        world   
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
            (func                          : Location -> World -> Gamestate option) 
            (location                      : Location) 
            (world                         : World) 
            : Gamestate option =
        if world |> World.IsAvatarAlive shipmateSingleStatisticSource then
            if islandSource() |> List.exists (fun x->x= location) then
                func location world
            else
                world
                |> Gamestate.AtSea
                |> Some
        else
            world.AvatarId
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
