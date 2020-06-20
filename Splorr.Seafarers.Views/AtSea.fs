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
        world.Avatar.Position |> fst |> sprintf "X: %f" |> sink
        world.Avatar.Position |> snd |> sprintf "Y: %f" |> sink
        world.Avatar.Heading |> Dms.ToDms |> Dms.ToString |> sprintf "Heading: %s" |> sink
        world.Avatar.Speed |> sprintf "Speed: %f" |> sink
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

