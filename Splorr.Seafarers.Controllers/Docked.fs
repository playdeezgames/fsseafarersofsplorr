namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Docked = 
    let private RunWithIsland (islandMarketSource) (islandSingleMarketSink) (commodities:Map<uint64, CommodityDescriptor>) (items:Map<uint64, ItemDescriptor>) (source:CommandSource) (sink:MessageSink) (location:Location) (island:Island) (world: World) : Gamestate option =
        "" |> Line |> sink
        world.Avatars.[world.AvatarId].Messages
        |> Utility.DumpMessages sink
        [
            (Hue.Flavor, sprintf "You have visited %u times." (island.AvatarVisits |> Map.tryFind world.AvatarId |> Option.bind (fun x->x.VisitCount) |> Option.defaultValue 0u) |> Line) |> Hued
            (Hue.Heading, sprintf "You are docked at '%s':" island.Name |> Line) |> Hued
        ]
        |> List.iter sink

        let world =
            world
            |> World.ClearMessages world.AvatarId

        match source() with
        | Some (Command.AcceptJob index) ->
            (Dock, location, world |> World.AcceptJob index location world.AvatarId)
            |> Gamestate.Docked
            |> Some

        | Some (Command.Buy (quantity, itemName))->
            (Dock, location, world |> World.BuyItems islandMarketSource islandSingleMarketSink commodities items location quantity itemName world.AvatarId) 
            |> Gamestate.Docked
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            (Dock, location, world |> World.SellItems islandMarketSource islandSingleMarketSink commodities items location quantity itemName world.AvatarId) 
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
            (Dock, location, world) 
            |> Gamestate.Docked 
            |> Gamestate.InvalidInput
            |> Some

    let internal RunBoilerplate (func:Location -> Island -> World->(Gamestate option)) (location:Location) (world: World) : Gamestate option =
        if world |> World.IsAvatarAlive world.AvatarId then
            match world.Islands |> Map.tryFind location with
            | Some island ->
                func location island world
            | None ->
                world
                |> Gamestate.AtSea
                |> Some
        else
            world.Avatars.[world.AvatarId].Messages
            |> Gamestate.GameOver
            |> Some

    let Run (islandMarketSource) (islandSingleMarketSink) (commoditySource:unit -> Map<uint64, CommodityDescriptor>) (itemSource:unit->Map<uint64, ItemDescriptor>) (source:CommandSource) (sink:MessageSink) =
        RunBoilerplate (RunWithIsland  islandMarketSource islandSingleMarketSink (commoditySource()) (itemSource()) source sink)
