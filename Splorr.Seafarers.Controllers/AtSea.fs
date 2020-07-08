namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module AtSea =
    let private RunAlive (random:System.Random) (source:CommandSource) (sink:MessageSink) (avatarId:string) (world:World) : Gamestate option =

        "" |> Line |> sink
        world.Messages
        |> Utility.DumpMessages sink
        let world =
            world
            |> World.ClearMessages
        [
            (Heading, "At Sea:" |> Line) |> Hued
            (Label, "Turn: " |> Text) |> Hued
            (Value, world.Turn |> sprintf "%u" |> Line) |> Hued
            (Label, "Heading: " |> Text) |> Hued
            (Value, world.Avatars.[avatarId].Heading |> Dms.ToDms |> Dms.ToString |> sprintf "%s" |> Line) |> Hued
            (Label, "Speed: " |> Text) |> Hued
            (Value, world.Avatars.[avatarId].Speed |> sprintf "%f" |> Line) |> Hued
            (Subheading, "Nearby:" |> Line) |> Hued
        ]
        |> List.iter sink

        let dockTarget = 
            world
            |> World.GetNearbyLocations world.Avatars.[avatarId].Position world.Avatars.[avatarId].ViewDistance
            |> List.map
                (fun location -> 
                    (location, Location.HeadingTo world.Avatars.[avatarId].Position location |> Dms.ToDms |> Dms.ToString, Location.DistanceTo world.Avatars.[avatarId].Position location, (world.Islands.[location] |> Island.GetDisplayName)))
            |> List.sortBy (fun (_,_,d,_)->d)
            |> List.fold
                (fun target (location, heading, distance, name) -> 
                    (Sublabel, "Name: " |> Text) |> Hued |> sink
                    (Value, sprintf "%s " (if name="" then "????" else name) |> Text) |> Hued |> sink
                    (Sublabel, "Bearing: " |> Text) |> Hued |> sink
                    (Value, sprintf "%s " heading |> Text) |> Hued |> sink
                    (Sublabel, "Distance: " |> Text) |> Hued |> sink
                    (Value, sprintf "%f" distance |> Text) |> Hued |> sink
                    (Flavor, sprintf "%s" (if distance<world.Avatars.[avatarId].DockDistance then " (Can Dock)" else "") |> Line) |> Hued |> sink
                    (if distance<world.Avatars.[avatarId].DockDistance then (Some location) else target)) None

        match source() with
        | Some Command.Status ->
            world 
            |> Gamestate.AtSea
            |> Gamestate.Status
            |> Some

        | Some (Command.HeadFor name) ->
            world
            |> World.HeadFor avatarId name
            |> Gamestate.AtSea
            |> Some

        | Some Command.Dock ->
            match dockTarget with
            | Some location ->
                (Dock, location, world |> World.Dock random location avatarId)
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
            |> World.AbandonJob avatarId
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
            |> World.Move distance avatarId
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (SetCommand.Heading heading)) ->
            world
            |> World.SetHeading heading avatarId
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (Speed speed)) ->
            world
            |> World.SetSpeed speed avatarId
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

    let Run (random:System.Random) (source:CommandSource) (sink:MessageSink) (avatarId:string) (world:World) : Gamestate option =
        if world |> World.IsAvatarAlive avatarId then
            RunAlive random source sink avatarId world
        else
            world.Messages
            |> Gamestate.GameOver
            |> Some
