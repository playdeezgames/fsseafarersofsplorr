﻿namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers

module AtSea =
    let Run (source:CommandSource) (sink:MessageSink) (world:World) : ViewState option =
        "" |> sink
        world.Messages
        |> Utility.DumpMessages sink
        let world =
            world
            |> World.ClearMessages
        "At Sea:" |> sink
        world.Turn |> sprintf "Turn: %u" |> sink
        world.Avatar.Heading |> Dms.ToDms |> Dms.ToString |> sprintf "Heading: %s" |> sink
        world.Avatar.Speed |> sprintf "Speed: %f" |> sink

        "Nearby:" |> sink
        let dockTarget = 
            world
            |> World.GetNearbyLocations world.Avatar.Position world.Avatar.ViewDistance
            |> List.map
                (fun location -> 
                    (location, Location.HeadingTo world.Avatar.Position location |> Dms.ToDms |> Dms.ToString, Location.DistanceTo world.Avatar.Position location, world.Islands.[location].Name))
            |> List.sortBy (fun (_,_,d,_)->d)
            |> List.fold
                (fun target (location, heading, distance, name) -> 
                    sprintf "Name: %s Bearing: %s Distance: %f%s" (if name="" then "????" else name) heading distance (if distance<world.Avatar.DockDistance then " (Can Dock)" else "")
                    |> sink
                    (if distance<world.Avatar.DockDistance then (Some location) else target)) None

        match source() with
        | Some Dock ->
            match dockTarget with
            | Some location ->
                (location, world |> World.AddMessages [ "You dock." ])
                |> Docked
                |> Some
            | None ->
                world
                |> World.AddMessages [ "There is no place to dock." ]
                |> AtSea
                |> Some
        | Some Menu ->
            world
            |> Some
            |> MainMenu
            |> Some
        | Some Help ->
            world
            |> AtSea
            |> ViewState.Help
            |> Some
        | Some Move ->
            world
            |> World.Move
            |> AtSea
            |> Some
        | Some (Set (Heading heading)) ->
            world
            |> World.SetHeading heading
            |> AtSea
            |> Some
        | Some (Set (Speed speed)) ->
            world
            |> World.SetSpeed speed
            |> AtSea
            |> Some
        | Some Quit -> 
            world
            |> AtSea
            |> ConfirmQuit
            |> Some
        | _ ->
            "Maybe try 'help'?" |> sink
            world
            |> AtSea
            |> Some

