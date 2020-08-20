namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Docked = 
    let private UpdateDisplay 
            (avatarMessageSource         : AvatarMessageSource)
            (messageSink : MessageSink) 
            (location    : Location) 
            (world       : World) 
            : unit =
        "" |> Line |> messageSink
        world.AvatarId
        |> avatarMessageSource
        |> Utility.DumpMessages messageSink
        let island =
            world.Islands.[location]
        [
            (Hue.Flavor, sprintf "You have visited %u times." (island.AvatarVisits |> Map.tryFind world.AvatarId |> Option.bind (fun x->x.VisitCount) |> Option.defaultValue 0u) |> Line) |> Hued
            (Hue.Heading, sprintf "You are docked at '%s':" island.Name |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private HandleCommand
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarJobSink                 : AvatarJobSink)
            (avatarJobSource               : AvatarJobSource)
            (avatarMessagePurger           : AvatarMessagePurger)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (commoditySource               : CommoditySource) 
            (islandMarketSource            : IslandMarketSource) 
            (islandSingleMarketSink        : IslandSingleMarketSink) 
            (islandSingleMarketSource      : IslandSingleMarketSource) 
            (itemSource                    : ItemSource) 
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (command                       : Command option) 
            (location                      : Location) 
            (world                         : World) 
            : Gamestate option =
        world
        |> World.ClearMessages avatarMessagePurger

        match command with
        | Some (Command.AcceptJob index) ->
            (Dock, 
                location, 
                world 
                |> World.AcceptJob
                    avatarJobSink
                    avatarJobSource
                    avatarMessageSink 
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    index 
                    location)
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
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarJobSink                 : AvatarJobSink)
            (avatarJobSource               : AvatarJobSource)
            (avatarMessagePurger           : AvatarMessagePurger)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarMessageSource           : AvatarMessageSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (commoditySource               : CommoditySource) 
            (islandMarketSource            : IslandMarketSource) 
            (islandSingleMarketSink        : IslandSingleMarketSink) 
            (islandSingleMarketSource      : IslandSingleMarketSource) 
            (itemSource                    : ItemSource) 
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) 
            (location                      : Location) 
            (world                         : World) 
            : Gamestate option =
        world
        |> UpdateDisplay 
            avatarMessageSource
            messageSink 
            location 
        
        world   
        |> HandleCommand 
            avatarInventorySink
            avatarInventorySource
            avatarJobSink
            avatarJobSource
            avatarMessagePurger
            avatarMessageSink
            avatarSingleMetricSink
            avatarSingleMetricSource
            commoditySource 
            islandMarketSource 
            islandSingleMarketSink 
            islandSingleMarketSource
            itemSource 
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSource
            (commandSource()) 
            location 

    let internal RunBoilerplate 
            (avatarMessageSource           : AvatarMessageSource)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (func                          : Location -> World -> Gamestate option) 
            (location                      : Location) 
            (world                         : World) 
            : Gamestate option =
        if world |> World.IsAvatarAlive shipmateSingleStatisticSource then
            if world.Islands |> Map.containsKey location then
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
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarJobSink                 : AvatarJobSink)
            (avatarJobSource               : AvatarJobSource)
            (avatarMessagePurger           : AvatarMessagePurger)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarMessageSource           : AvatarMessageSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (commoditySource               : CommoditySource) 
            (islandMarketSource            : IslandMarketSource) 
            (islandSingleMarketSink        : IslandSingleMarketSink) 
            (islandSingleMarketSource      : IslandSingleMarketSource) 
            (itemSource                    : ItemSource) 
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) =
        RunBoilerplate 
            avatarMessageSource
            shipmateSingleStatisticSource
            (RunWithIsland 
                avatarInventorySink
                avatarInventorySource
                avatarJobSink
                avatarJobSource
                avatarMessagePurger          
                avatarMessageSink            
                avatarMessageSource          
                avatarSingleMetricSink
                avatarSingleMetricSource
                commoditySource              
                islandMarketSource           
                islandSingleMarketSink       
                islandSingleMarketSource     
                itemSource                   
                shipmateSingleStatisticSink  
                shipmateSingleStatisticSource
                vesselSingleStatisticSource  
                commandSource                
                messageSink)                 
