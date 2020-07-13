namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Metrics = 
    let private RunWorld (sink:MessageSink) (avatar:Avatar) : unit = 
        [
            "" |> Line
            (Heading, "Metrics:" |> Line) |> Hued
        ]
        |> List.iter sink
        if avatar.Metrics.IsEmpty then
            (Usage, "(none)" |> Line) |> Hued |> sink
        else
            avatar.Metrics
            |> Map.iter
                (fun k v -> 
                    [
                        (Label, k.ToString() |> sprintf "%s: " |> Text) |> Hued
                        (Value, v |> sprintf "%u" |> Line) |> Hued
                    ]
                    |> List.iter sink)

    let Run (sink:MessageSink) (avatarId:string) (gamestate:Gamestate) : Gamestate option =
        match gamestate |> Gamestate.GetWorld |> Option.bind (fun w->w.Avatars |> Map.tryFind avatarId) with
        | Some avatar ->
            RunWorld sink avatar
        | None -> 
            ()
        gamestate
        |> Some

