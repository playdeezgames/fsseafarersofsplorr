namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers

module Docked = 
    let private RunWithIsland  (source:CommandSource) (sink:MessageSink) (location:Location) (island:Island) (world: World) : ViewState option =
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
            |> ViewState.Help 
            |> Some
        | _ -> 
            (location, world) 
            |> Docked 
            |> Some

    let Run (source:CommandSource) (sink:MessageSink) (location:Location) (world: World) : ViewState option =
        match world.Islands |> Map.tryFind location with
        | Some island ->
            RunWithIsland source sink location island world
        | None ->
            world
            |> AtSea
            |> Some
