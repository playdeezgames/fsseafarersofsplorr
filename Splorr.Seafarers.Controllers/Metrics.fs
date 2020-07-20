namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Metrics = 
    let private GetMetricDisplayName (metric:Metric) : string =
        match metric with
        | Metric.Moved         -> "moved"
        | Metric.Ate           -> "ate"
        | Metric.VisitedIsland -> "visited an island"
        | Metric.CompletedJob  -> "completed a job"
        | Metric.AbandonedJob  -> "abandoned a job"
        | Metric.AcceptedJob   -> "accepted a job"
        | Metric.CleanedHull   -> "cleaned a hull"
        | _ -> raise (System.NotImplementedException (metric.ToString() |> sprintf "'%s' is a metric with no name!"))

    let private RunWorld (sink:MessageSink) (avatar:Avatar) : unit = 
        [
            "" |> Line
            (Heading, "Metric Name" |> sprintf "%-24s" |> Text) |> Hued
            " | " |> Text
            (Heading, "Count" |> sprintf "%6s" |> Line) |> Hued
            "-------------------------+-------" |> Line
        ]
        |> List.iter sink
        if avatar.Metrics.IsEmpty then
            (Usage, "(none)" |> Line) |> Hued |> sink
        else
            avatar.Metrics
            |> Map.iter
                (fun k v -> 
                    [
                        (Label, k |> GetMetricDisplayName |> sprintf "%-24s" |> Text) |> Hued
                        " | " |> Text
                        (Value, v |> sprintf "%6u" |> Line) |> Hued
                    ]
                    |> List.iter sink)

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        match gamestate |> Gamestate.GetWorld |> Option.bind (fun w->w.Avatars |> Map.tryFind w.AvatarId) with
        | Some avatar ->
            RunWorld sink avatar
        | None -> 
            ()
        gamestate
        |> Some

