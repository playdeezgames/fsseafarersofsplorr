namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Jobs = 
    let private RunIsland (sink:MessageSink) (location:Location) (islands:Map<Location,Island>) (island:Island) : unit =
        [
            ""
            "Jobs Available:"
        ]
        |> List.iter sink
        island.Jobs
        |> List.zip [1..island.Jobs.Length]
        |> List.map (fun (index, job) -> (index, job, islands |> Map.find job.Destination))
        |> List.iter 
            (fun (index, job, island) ->
                let bearing = 
                    Location.HeadingTo location job.Destination
                    |> Dms.ToDms
                    |> Dms.ToString
                let distance = 
                    Location.DistanceTo location job.Destination
                sprintf "%d. %s Bearing:%s Distance:%f Reward:%f" index island.Name bearing distance job.Reward |> sink)
        if island.Jobs.IsEmpty then
            "(none available)" |> sink

        
    let Run  (sink:MessageSink) (location:Location, world: World) : Gamestate option =
        world.Islands 
        |> Map.tryFind location
        |> Option.iter (RunIsland sink location world.Islands)
        (location, world)
        |> Gamestate.Docked
        |> Some



