namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Common

module Metrics = 
    let private GetMetricDisplayName 
            (metric : Metric) 
            : string =
        match metric with
        | Metric.Moved         -> "moved"
        | Metric.Ate           -> "shipmates ate"
        | Metric.VisitedIsland -> "visited an island"
        | Metric.CompletedJob  -> "completed a job"
        | Metric.AbandonedJob  -> "abandoned a job"
        | Metric.AcceptedJob   -> "accepted a job"
        | Metric.CleanedHull   -> "cleaned a hull"
        | Metric.Starved       -> "shipmates starved"
        | _ -> raise (System.NotImplementedException (metric.ToString() |> sprintf "'%s' is a metric with no name!"))

    let private RunWorld
            (context : CommonContext)
            (messageSink        : MessageSink) 
            (avatarId           : string) 
            : unit = 
        [
            "" |> Line
            (Hue.Heading, "Metric Name" |> sprintf "%-24s" |> Text) |> Hued
            " | " |> Text
            (Hue.Heading, "Count" |> sprintf "%6s" |> Line) |> Hued
            "-------------------------+-------" |> Line
        ]
        |> List.iter messageSink
        let metrics = World.GetAvatarMetrics context avatarId
        if metrics.IsEmpty then
            (Hue.Usage, "(none)" |> Line) |> Hued |> messageSink
        else
            metrics
            |> Map.iter
                (fun metric value -> 
                    [
                        (Hue.Label, metric |> GetMetricDisplayName |> sprintf "%-24s" |> Text) |> Hued
                        " | " |> Text
                        (Hue.Value, value |> sprintf "%6u" |> Line) |> Hued
                    ]
                    |> List.iter messageSink)

    let Run 
            (context : CommonContext)
            (messageSink        : MessageSink) 
            (gamestate          : Gamestate) 
            : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld 
        |> Option.iter 
            (fun w -> 
                RunWorld 
                    context
                    messageSink 
                    w)
        gamestate
        |> Some

