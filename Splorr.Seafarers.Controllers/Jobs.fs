namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Jobs = 
    let private RunIsland 
            (context:ServiceContext)
            (messageSink            : MessageSink) 
            (location               : Location) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "Jobs Available:" |> Line) |> Hued
        ]
        |> List.iter messageSink
        let jobs = 
            location
            |> IslandJob.Get context
        jobs
        |> List.zip [1..jobs.Length]
        |> List.iter 
            (fun (index, job) ->
                let bearing = 
                    Location.HeadingTo location job.Destination
                    |> Angle.ToDegrees
                    |> Angle.ToString
                let distance = 
                    Location.DistanceTo location job.Destination
                [
                    (Hue.Label, index |> sprintf "%d. " |> Text) |> Hued
                    (Hue.Value, job.Destination |> IslandName.GetName context |> Option.get |> sprintf "%s " |> Text) |> Hued
                    (Hue.Sublabel, "Bearing: " |> Text) |> Hued
                    (Hue.Value, bearing |> sprintf "%s " |> Text) |> Hued
                    (Hue.Sublabel, "Distance: " |> Text) |> Hued
                    (Hue.Value, distance |> sprintf "%f " |> Text) |> Hued
                    (Hue.Sublabel, "Reward: " |> Text) |> Hued
                    (Hue.Value, job.Reward |> sprintf "%f " |> Line) |> Hued
                    (Hue.Flavor, job.FlavorText |> sprintf "\t'%s'" |> Line) |> Hued
                ]
                |> List.iter messageSink)
        if jobs.IsEmpty then
            "(none available)" |> Line |> messageSink
    
    let Run  
            (context : ServiceContext)
            (messageSink            : MessageSink) 
            (location               : Location)
            (avatarId               : string) 
            : Gamestate option =
        context
        |> Island.GetList
        |> List.tryFind(fun x->x= location)
        |> Option.iter 
            (RunIsland 
                context
                messageSink)
        avatarId
        |> Gamestate.InPlay
        |> Some



