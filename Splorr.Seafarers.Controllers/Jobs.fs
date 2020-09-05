namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Jobs = 
    let private RunIsland 
            (islandSingleNameSource : IslandSingleNameSource)
            (islandJobSource        : IslandJobSource)
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
            |> islandJobSource
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
                    (Hue.Value, job.Destination |> islandSingleNameSource |> Option.get |> sprintf "%s " |> Text) |> Hued
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
            (islandJobSource        : IslandJobSource)
            (islandSingleNameSource : IslandSingleNameSource)
            (islandSource           : IslandSource)
            (messageSink            : MessageSink) 
            (location               : Location)
            (avatarId               : string) 
            : Gamestate option =
        islandSource()
        |> List.tryFind(fun x->x= location)
        |> Option.iter 
            (RunIsland 
                islandSingleNameSource
                islandJobSource
                messageSink)
        (Feature IslandFeatureIdentifier.Dock, location, avatarId)
        |> Gamestate.Docked
        |> Some



