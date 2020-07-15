namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Docked = 
    let private RunWithIsland (commodities:Map<uint64, CommodityDescriptor>) (source:CommandSource) (sink:MessageSink) (location:Location) (island:Island) (avatarId:string) (world: World) : Gamestate option =
        "" |> Line |> sink
        world.Avatars.[avatarId].Messages
        |> Utility.DumpMessages sink
        [
            (Flavor, sprintf "You have visited %u times." (island.AvatarVisits |> Map.tryFind avatarId |> Option.map (fun x->x.VisitCount) |> Option.defaultValue 0u) |> Line) |> Hued
            (Heading, sprintf "You are docked at '%s':" island.Name |> Line) |> Hued
        ]
        |> List.iter sink

        let world =
            world
            |> World.ClearMessages avatarId

        match source() with
        | Some (Command.AcceptJob index) ->
            (Dock, location, world |> World.AcceptJob index location avatarId)
            |> Gamestate.Docked
            |> Some

        | Some (Command.Buy (quantity, itemName))->
            (Dock, location, world |> World.BuyItems commodities location quantity itemName avatarId) 
            |> Gamestate.Docked
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            (Dock, location, world |> World.SellItems commodities location quantity itemName avatarId) 
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
            (Dock, location, world |> World.AbandonJob avatarId)
            |> Gamestate.Docked
            |> Some

        | Some Command.Undock ->
            world 
            |> World.AddMessages avatarId [ "You undock." ]
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
            (Error, "Maybe try 'help'?" |> Line) |> Hued |> sink
            (Dock, location, world) 
            |> Gamestate.Docked 
            |> Some

    let internal RunBoilerplate (func:Location -> Island -> string -> World->(Gamestate option)) (location:Location) (avatarId:string) (world: World) : Gamestate option =
        if world |> World.IsAvatarAlive avatarId then
            match world.Islands |> Map.tryFind location with
            | Some island ->
                func location island avatarId world
            | None ->
                world
                |> Gamestate.AtSea
                |> Some
        else
            world.Avatars.[avatarId].Messages
            |> Gamestate.GameOver
            |> Some

    let Run (commodities:Map<uint64, CommodityDescriptor>) (source:CommandSource) (sink:MessageSink) =
        RunBoilerplate (RunWithIsland commodities source sink)
