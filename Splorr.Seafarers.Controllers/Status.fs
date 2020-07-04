namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Status =
    let private RunWorld (sink:MessageSink) (world:World) : unit =
        [
            ""
            "Status:"
            world.Avatar.Money |> sprintf "\tMoney: %f"
            world.Avatar.Reputation |> sprintf "\tReputation: %f"
        ]
        |> List.iter sink
        world.Avatar.Job
        |> Option.iter
            (fun job ->
                let island = 
                    world.Islands.[job.Destination]
                [
                    "Current Job:"
                    job.FlavorText |> sprintf "\tDescription: %s"
                    island.Name |> sprintf "\tDestination: %s"
                    job.Reward |> sprintf "\tReward: %f"
                ]
                |> List.iter sink)

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink)
        gamestate
        |> Some


