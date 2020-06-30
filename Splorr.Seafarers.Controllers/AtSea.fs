namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module AtSea =
    let Run (random:System.Random) (source:CommandSource) (sink:MessageSink) (world:World) : Gamestate option =
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
                    (location, Location.HeadingTo world.Avatar.Position location |> Dms.ToDms |> Dms.ToString, Location.DistanceTo world.Avatar.Position location, (world.Islands.[location] |> Island.GetDisplayName)))
            |> List.sortBy (fun (_,_,d,_)->d)
            |> List.fold
                (fun target (location, heading, distance, name) -> 
                    sprintf "Name: %s Bearing: %s Distance: %f%s" (if name="" then "????" else name) heading distance (if distance<world.Avatar.DockDistance then " (Can Dock)" else "")
                    |> sink
                    (if distance<world.Avatar.DockDistance then (Some location) else target)) None

        match source() with
        | Some Command.Status ->
            world 
            |> Gamestate.AtSea
            |> Gamestate.Status
            |> Some
        | Some (Command.HeadFor name) ->
            world
            |> World.HeadFor name
            |> Gamestate.AtSea
            |> Some
        | Some Command.Dock ->
            match dockTarget with
            | Some location ->
                (Dock, location, world |> World.Dock random location)
                |> Gamestate.Docked
                |> Some
            | None ->
                world
                |> World.AddMessages [ "There is no place to dock." ]
                |> Gamestate.AtSea
                |> Some
        | Some (Command.Islands page) ->
            (page, world |> Gamestate.AtSea)
            |> Gamestate.IslandList
            |> Some

        | Some (Command.Abandon Job) ->
            world
            |> World.AbandonJob
            |> Gamestate.AtSea
            |> Some

        | Some Command.Menu ->
            world
            |> Some
            |> Gamestate.MainMenu
            |> Some
        | Some Command.Help ->
            world
            |> Gamestate.AtSea
            |> Gamestate.Help
            |> Some
        | Some (Command.Move distance)->
            world
            |> World.Move distance
            |> Gamestate.AtSea
            |> Some
        | Some (Command.Set (Heading heading)) ->
            world
            |> World.SetHeading heading
            |> Gamestate.AtSea
            |> Some
        | Some (Command.Set (Speed speed)) ->
            world
            |> World.SetSpeed speed
            |> Gamestate.AtSea
            |> Some
        | Some Command.Quit -> 
            world
            |> Gamestate.AtSea
            |> Gamestate.ConfirmQuit
            |> Some
        | _ ->
            "Maybe try 'help'?" |> sink
            world
            |> Gamestate.AtSea
            |> Some

