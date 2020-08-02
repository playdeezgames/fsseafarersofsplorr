namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Metrics = 
    let private GetMetricDisplayName 
            (metric : Metric) 
            : string =
        match metric with
        | Metric.Moved         -> "moved"
        | Metric.Ate           -> "ate"
        | Metric.VisitedIsland -> "visited an island"
        | Metric.CompletedJob  -> "completed a job"
        | Metric.AbandonedJob  -> "abandoned a job"
        | Metric.AcceptedJob   -> "accepted a job"
        | Metric.CleanedHull   -> "cleaned a hull"
        | _ -> raise (System.NotImplementedException (metric.ToString() |> sprintf "'%s' is a metric with no name!"))

    let private RunWorld 
            (messageSink : MessageSink) 
            (avatar      : Avatar) 
            : unit = 
        [
            "" |> Line
            (Hue.Heading, "Metric Name" |> sprintf "%-24s" |> Text) |> Hued
            " | " |> Text
            (Hue.Heading, "Count" |> sprintf "%6s" |> Line) |> Hued
            "-------------------------+-------" |> Line
        ]
        |> List.iter messageSink
        if avatar.Metrics.IsEmpty then
            (Hue.Usage, "(none)" |> Line) |> Hued |> messageSink
        else
            avatar.Metrics
            |> Map.iter
                (fun metric value -> 
                    [
                        (Hue.Label, metric |> GetMetricDisplayName |> sprintf "%-24s" |> Text) |> Hued
                        " | " |> Text
                        (Hue.Value, value |> sprintf "%6u" |> Line) |> Hued
                    ]
                    |> List.iter messageSink)

    let Run 
            (messageSink : MessageSink) 
            (gamestate   : Gamestate) 
            : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld 
        |> Option.bind (fun w->w.Avatars |> Map.tryFind w.AvatarId)
        |> Option.iter (RunWorld messageSink)
        gamestate
        |> Some

