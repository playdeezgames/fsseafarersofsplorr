namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Docked = 
    let private UpdateDisplay 
            (messageSink:MessageSink) 
            (location:Location) 
            (world: World) =
        "" |> Line |> messageSink
        world.Avatars.[world.AvatarId].Messages
        |> Utility.DumpMessages messageSink
        let island =
            world.Islands.[location]
        [
            (Hue.Flavor, sprintf "You have visited %u times." (island.AvatarVisits |> Map.tryFind world.AvatarId |> Option.bind (fun x->x.VisitCount) |> Option.defaultValue 0u) |> Line) |> Hued
            (Hue.Heading, sprintf "You are docked at '%s':" island.Name |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private HandleCommand
            (commoditySource:unit->Map<uint64, CommodityDescriptor>) 
            (itemSource:unit->Map<uint64, ItemDescriptor>) 
            (islandMarketSource:Location->Map<uint64, Market>) 
            (islandSingleMarketSource:Location->uint64->Market option) 
            (islandSingleMarketSink:Location->uint64 * Market -> unit) 
            (command:Command option) 
            (location:Location) 
            (world: World) =
        let world =
            world
            |> World.ClearMessages world.AvatarId

        match command with
        | Some (Command.AcceptJob index) ->
            (Dock, location, world |> World.AcceptJob index location world.AvatarId)
            |> Gamestate.Docked
            |> Some

        | Some (Command.Buy (quantity, itemName))->
            (Dock, location, world |> World.BuyItems islandMarketSource islandSingleMarketSink (commoditySource()) (itemSource()) location quantity itemName world.AvatarId) 
            |> Gamestate.Docked
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            (Dock, location, world |> World.SellItems islandMarketSource islandSingleMarketSource islandSingleMarketSink (commoditySource()) (itemSource()) location quantity itemName world.AvatarId) 
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
            (Dock, location, world |> World.AbandonJob world.AvatarId)
            |> Gamestate.Docked
            |> Some

        | Some Command.Undock ->
            world 
            |> World.AddMessages world.AvatarId [ "You undock." ]
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
            (commoditySource          : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource               : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource       : Location -> Map<uint64, Market>) 
            (islandSingleMarketSource : Location -> uint64 -> Market option) 
            (islandSingleMarketSink   : Location -> uint64 * Market -> unit) 
            (commandSource            : CommandSource) 
            (messageSink              : MessageSink) 
            (location                 : Location) 
            (world                    : World) 
            : Gamestate option =
        world
        |> UpdateDisplay 
            messageSink 
            location 
        
        world   
        |> HandleCommand 
            commoditySource 
            itemSource 
            islandMarketSource 
            islandSingleMarketSource
            islandSingleMarketSink 
            (commandSource()) 
            location 

    let internal RunBoilerplate 
            (func     : Location -> World->(Gamestate option)) 
            (location : Location) 
            (world    : World) 
            : Gamestate option =
        if world |> World.IsAvatarAlive world.AvatarId then
            if world.Islands |> Map.containsKey location then
                func location world
            else
                world
                |> Gamestate.AtSea
                |> Some
        else
            world.Avatars.[world.AvatarId].Messages
            |> Gamestate.GameOver
            |> Some

    let Run 
            (commoditySource          : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource               : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource       : Location -> Map<uint64,Market>) 
            (islandSingleMarketSource : Location -> uint64 -> Market option) 
            (islandSingleMarketSink   : Location -> uint64 * Market -> unit) 
            (commandSource            : CommandSource) 
            (messageSink              : MessageSink) =
        RunBoilerplate 
            (RunWithIsland 
                commoditySource 
                itemSource 
                islandMarketSource 
                islandSingleMarketSource
                islandSingleMarketSink 
                commandSource 
                messageSink)
