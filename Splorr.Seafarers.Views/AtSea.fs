namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers

module AtSea =
    let rec Run (source:CommandSource) (sink:MessageSink) (world:World) : ViewState option =
        "" |> sink
        world.Messages
        |> Utility.DumpMessages sink
        let world =
            world
            |> World.ClearMessages
        "At Sea:" |> sink
        world.Avatar.X |> sprintf "X: %f" |> sink
        world.Avatar.Y |> sprintf "Y: %f" |> sink
        world.Avatar.Heading |> Dms.ToDms |> Dms.ToString |> sprintf "Heading: %s" |> sink
        world.Avatar.Speed |> sprintf "Speed: %f" |> sink
        match source() with
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
            world
            |> AtSea
            |> Some

