namespace Splorr.Seafarers.Views

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
        world
        |> World.GetNearbyLocations world.Avatar.Position world.Avatar.ViewDistance
        |> List.map
            (fun i -> 
                (Location.HeadingTo world.Avatar.Position i |> Dms.ToDms |> Dms.ToString, Location.DistanceTo world.Avatar.Position i, world.Islands.[i].Name))
        |> List.sortBy (fun (_,d,_)->d)
        |> List.iter
            (fun (heading, distance, name) -> 
                ((if name="" then "????" else name), heading, distance)
                |||> sprintf "Name: %s Bearing: %s Distance: %f"
                |> sink)

        match source() with
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

