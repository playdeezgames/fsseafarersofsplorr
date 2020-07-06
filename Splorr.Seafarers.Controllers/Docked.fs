namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Docked = 
    let private RunWithIsland  (source:CommandSource) (sink:MessageSink) (location:Location) (island:Island) (world: World) : Gamestate option =
        "" |> Line |> sink
        world.Messages
        |> Utility.DumpMessages sink
        [
            (Flavor, sprintf "You have visited %u times." (island.VisitCount |> Option.defaultValue 0u) |> Line) |> Hued
            (Heading, sprintf "You are docked at '%s'" island.Name |> Line) |> Hued
        ]
        |> List.iter sink

        let world =
            world
            |> World.ClearMessages

        match source() with
        | Some (Command.AcceptJob index) ->
            (Dock, location, world |> World.AcceptJob index location)
            |> Gamestate.Docked
            |> Some

        | Some Command.Shop ->
            (Shop, location, world)
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
            (Dock, location, world |> World.AbandonJob)
            |> Gamestate.Docked
            |> Some

        | Some Command.Undock ->
            world 
            |> World.AddMessages [ "You undock." ]
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

        | Some Command.Prices ->
            (PriceList, location, world) 
            |> Gamestate.Docked
            |> Some

        | Some Command.Help ->
            (Dock, location, world) 
            |> Gamestate.Docked 
            |> Gamestate.Help 
            |> Some

        | _ -> 
            "Maybe try 'help'?" |> Line |> sink
            (Dock, location, world) 
            |> Gamestate.Docked 
            |> Some

    let internal RunBoilerplate (func:Location -> Island -> World->(Gamestate option)) (location:Location) (world: World) : Gamestate option =
        match world with
        | World.AVATAR_ALIVE ->
            match world.Islands |> Map.tryFind location with
            | Some island ->
                func location island world
            | None ->
                world
                |> Gamestate.AtSea
                |> Some
        | _ ->
            world.Messages
            |> Gamestate.GameOver
            |> Some

    let Run (source:CommandSource) (sink:MessageSink) =
        RunBoilerplate (RunWithIsland source sink)

