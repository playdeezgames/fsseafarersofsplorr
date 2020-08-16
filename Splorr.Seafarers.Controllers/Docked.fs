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
            (commoditySource               : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource                    : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource            : Location -> Map<uint64, Market>) 
            (islandSingleMarketSource      : Location -> uint64 -> Market option) 
            (islandSingleMarketSink        : Location -> uint64 * Market -> unit) 
            (vesselSingleStatisticSource   : string -> VesselStatisticIdentifier -> Statistic option)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarMessagePurger           : AvatarMessagePurger)
            (command                       : Command option) 
            (location                      : Location) 
            (world                         : World) 
            : Gamestate option =
        world
        |> World.ClearMessages avatarMessagePurger

        match command with
        | Some (Command.AcceptJob index) ->
            (Dock, location, world |> World.AcceptJob avatarMessageSink index location)
            |> Gamestate.Docked
            |> Some

        | Some (Command.Buy (quantity, itemName))->
            (Dock, 
                location, 
                    world 
                    |> World.BuyItems 
                        islandMarketSource 
                        islandSingleMarketSource 
                        islandSingleMarketSink 
                        vesselSingleStatisticSource 
                        shipmateSingleStatisticSource
                        shipmateSingleStatisticSink
                        avatarMessageSink 
                        commoditySource 
                        (itemSource()) 
                        location 
                        quantity 
                        itemName) 
            |> Gamestate.Docked
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            (Dock, 
                location, 
                    world 
                    |> World.SellItems 
                        islandMarketSource 
                        islandSingleMarketSource 
                        islandSingleMarketSink 
                        shipmateSingleStatisticSource
                        shipmateSingleStatisticSink
                        avatarMessageSink
                        commoditySource 
                        (itemSource()) 
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
                    shipmateSingleStatisticSource
                    shipmateSingleStatisticSink
                    avatarMessageSink)
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
            (commoditySource               : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource                    : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource            : Location -> Map<uint64, Market>) 
            (islandSingleMarketSource      : Location -> uint64 -> Market option) 
            (islandSingleMarketSink        : Location -> uint64 * Market -> unit) 
            (vesselSingleStatisticSource   : string -> VesselStatisticIdentifier -> Statistic option)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarMessageSource           : AvatarMessageSource)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarMessagePurger           : AvatarMessagePurger)
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
            commoditySource 
            itemSource 
            islandMarketSource 
            islandSingleMarketSource
            islandSingleMarketSink 
            vesselSingleStatisticSource
            shipmateSingleStatisticSource
            shipmateSingleStatisticSink
            avatarMessageSink
            avatarMessagePurger
            (commandSource()) 
            location 

    let internal RunBoilerplate 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (avatarMessageSource           : AvatarMessageSource)
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
            (commoditySource               : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource                    : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource            : Location -> Map<uint64,Market>) 
            (islandSingleMarketSource      : Location -> uint64 -> Market option) 
            (islandSingleMarketSink        : Location -> uint64 * Market -> unit) 
            (vesselSingleStatisticSource   : string -> VesselStatisticIdentifier -> Statistic option)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarMessageSource           : AvatarMessageSource)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarMessagePurger           : AvatarMessagePurger)
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) =
        RunBoilerplate 
            shipmateSingleStatisticSource
            avatarMessageSource
            (RunWithIsland 
                commoditySource              
                itemSource                   
                islandMarketSource           
                islandSingleMarketSource     
                islandSingleMarketSink       
                vesselSingleStatisticSource  
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink  
                avatarMessageSource          
                avatarMessageSink            
                avatarMessagePurger          
                commandSource                
                messageSink)                 
