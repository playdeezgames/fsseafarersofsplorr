namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Docked = 
    let private RunWithIsland  (source:CommandSource) (sink:MessageSink) (location:Location) (island:Island) (world: World) : Gamestate option =
        [
            sprintf "You are docked at '%s'" island.Name
            sprintf "You have visited %u times." (island.VisitCount |> Option.defaultValue 0u)
        ]
        |> List.append
            world.Messages
            |> List.append 
                [
                    ""
                ]
        |> List.iter sink

        let world =
            world
            |> World.ClearMessages

        match source() with
        | Some (Command.AcceptJob index) ->
            (location, world |> World.AcceptJob index location)
            |> Gamestate.Docked
            |> Some

        | Some Command.Jobs ->
            (location, world)
            |> Gamestate.Jobs
            |> Some

        | Some Command.Status ->
            (location, world)
            |> Gamestate.Docked
            |> Gamestate.Status
            |> Some

        | Some (Command.Abandon Job) ->
            (location, world |> World.AbandonJob)
            |> Gamestate.Docked
            |> Some

        | Some Command.Undock ->
            world 
            |> World.AddMessages [ "You undock." ]
            |> Gamestate.AtSea 
            |> Some

        | Some Command.Quit ->
            (location, world) 
            |> Gamestate.Docked 
            |> Gamestate.ConfirmQuit 
            |> Some

        | Some Command.Help ->
            (location, world) 
            |> Gamestate.Docked 
            |> Gamestate.Help 
            |> Some

        | _ -> 
            "Maybe try 'help'?" |> sink
            (location, world) 
            |> Gamestate.Docked 
            |> Some

    let Run (source:CommandSource) (sink:MessageSink) (location:Location) (world: World) : Gamestate option =
        match world.Islands |> Map.tryFind location with
        | Some island ->
            RunWithIsland source sink location island world
        | None ->
            world
            |> Gamestate.AtSea
            |> Some
