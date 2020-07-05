namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module AtSea =
    let Run (random:System.Random) (source:CommandSource) (sink:MessageSink) (world:World) : Gamestate option =
        "" |> Line |> sink
        world.Messages
        |> Utility.DumpMessages sink
        let world =
            world
            |> World.ClearMessages
        (Heading, "At Sea:" |> Line) |> Hued |> sink
        (Label, "Turn: " |> Text) |> Hued |> sink
        world.Turn |> sprintf "%u" |> Line |> sink
        (Label, "Heading: " |> Text) |> Hued |> sink
        world.Avatar.Heading |> Dms.ToDms |> Dms.ToString |> sprintf "%s" |> Line |> sink
        (Label, "Speed: " |> Text) |> Hued |> sink
        world.Avatar.Speed |> sprintf "%f" |> Line |> sink

        (Subheading, "Nearby:" |> Line) |> Hued |> sink
        let dockTarget = 
            world
            |> World.GetNearbyLocations world.Avatar.Position world.Avatar.ViewDistance
            |> List.map
                (fun location -> 
                    (location, Location.HeadingTo world.Avatar.Position location |> Dms.ToDms |> Dms.ToString, Location.DistanceTo world.Avatar.Position location, (world.Islands.[location] |> Island.GetDisplayName)))
            |> List.sortBy (fun (_,_,d,_)->d)
            |> List.fold
                (fun target (location, heading, distance, name) -> 
                    (Sublabel, "Name: " |> Text) |> Hued |> sink
                    (Value, sprintf "%s " (if name="" then "????" else name) |> Text) |> Hued |> sink
                    (Sublabel, "Bearing: " |> Text) |> Hued |> sink
                    (Value, sprintf "%s " heading |> Text) |> Hued |> sink
                    (Sublabel, "Distance: " |> Text) |> Hued |> sink
                    (Value, sprintf "%f" distance |> Text) |> Hued |> sink
                    (Flavor, sprintf "%s" (if distance<world.Avatar.DockDistance then " (Can Dock)" else "") |> Line) |> Hued |> sink
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

        | Some (Command.Set (SetCommand.Heading heading)) ->
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

        | Some Command.Inventory -> 
            world
            |> Gamestate.AtSea
            |> Gamestate.Inventory
            |> Some

        | _ ->
            "Maybe try 'help'?" |> Line |> sink
            world
            |> Gamestate.AtSea
            |> Some

