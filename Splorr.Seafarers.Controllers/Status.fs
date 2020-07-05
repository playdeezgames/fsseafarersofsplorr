namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Status =
    let private RunWorld (sink:MessageSink) (world:World) : unit =
        [
            "" |> Line
            (Heading, "Status:" |> Line) |> Hued
            world.Avatar.Money |> sprintf "\tMoney: %f" |> Line
            world.Avatar.Reputation |> sprintf "\tReputation: %f" |> Line
        ]
        |> List.iter sink
        world.Avatar.Job
        |> Option.iter
            (fun job ->
                let island = 
                    world.Islands.[job.Destination]
                [
                    "Current Job:" |> Line
                    job.FlavorText |> sprintf "\tDescription: %s" |> Line
                    island.Name |> sprintf "\tDestination: %s" |> Line
                    job.Reward |> sprintf "\tReward: %f" |> Line
                ]
                |> List.iter sink)

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink)
        gamestate
        |> Some


