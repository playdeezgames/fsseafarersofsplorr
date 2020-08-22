namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Jobs = 
    let private RunIsland 
            (islandSingleNameSource : IslandSingleNameSource)
            (messageSink            : MessageSink) 
            (location               : Location) 
            (islands                : Map<Location,Island>) 
            (island                 : Island) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "Jobs Available:" |> Line) |> Hued
        ]
        |> List.iter messageSink
        island.Jobs
        |> List.zip [1..island.Jobs.Length]
        |> List.map (fun (index, job) -> (index, job, islands |> Map.find job.Destination))
        |> List.iter 
            (fun (index, job, island) ->
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
        if island.Jobs.IsEmpty then
            "(none available)" |> Line |> messageSink

        
    let Run  
            (islandSingleNameSource : IslandSingleNameSource)
            (messageSink            : MessageSink) 
            (location               : Location)
            (world                  : World) 
            : Gamestate option =
        world.Islands 
        |> Map.tryFind location
        |> Option.iter 
            (RunIsland 
                islandSingleNameSource
                messageSink 
                location 
                world.Islands)
        (Dock, location, world)
        |> Gamestate.Docked
        |> Some



