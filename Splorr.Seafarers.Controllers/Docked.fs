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
        | Some Status ->
            (location, world)
            |> Docked
            |> Gamestate.Status
            |> Some

        | Some Undock ->
            world 
            |> World.AddMessages [ "You undock." ]
            |> AtSea 
            |> Some

        | Some Quit ->
            (location, world) 
            |> Docked 
            |> ConfirmQuit 
            |> Some

        | Some Help ->
            (location, world) 
            |> Docked 
            |> Gamestate.Help 
            |> Some

        | _ -> 
            (location, world) 
            |> Docked 
            |> Some

    let Run (source:CommandSource) (sink:MessageSink) (location:Location) (world: World) : Gamestate option =
        match world.Islands |> Map.tryFind location with
        | Some island ->
            RunWithIsland source sink location island world
        | None ->
            world
            |> AtSea
            |> Some
