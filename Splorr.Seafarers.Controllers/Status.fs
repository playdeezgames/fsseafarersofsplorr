namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Status =
    let private RunWorld (sink:MessageSink) (avatarId:string) (world:World) : unit =
        [
            "" |> Line
            (Heading, "Status:" |> Line) |> Hued
            (Label, "Money: " |> Text) |> Hued
            (Value, world.Avatars.[avatarId].Money |> sprintf "%f" |> Line) |> Hued
            (Label, "Reputation: " |> Text) |> Hued
            (Value, world.Avatars.[avatarId].Reputation |> sprintf "%f" |> Line) |> Hued
            (Label, "Satiety: " |> Text) |> Hued
            (Value, (world.Avatars.[avatarId].Shipmates.[0].Statistics.[StatisticIdentifier.Satiety].CurrentValue, world.Avatars.[avatarId].Shipmates.[0].Statistics.[StatisticIdentifier.Satiety].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Label, "Health: " |> Text) |> Hued
            (Value, (world.Avatars.[avatarId].Shipmates.[0].Statistics.[StatisticIdentifier.Health].CurrentValue, world.Avatars.[avatarId].Shipmates.[0].Statistics.[StatisticIdentifier.Health].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Label, "Port Fouling: " |> Text) |> Hued
            (Value, (world.Avatars.[avatarId].Vessel.Fouling.[Port].CurrentValue, world.Avatars.[avatarId].Vessel.Fouling.[Port].MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
            (Label, "Starboard Fouling: " |> Text) |> Hued
            (Value, (world.Avatars.[avatarId].Vessel.Fouling.[Starboard].CurrentValue, world.Avatars.[avatarId].Vessel.Fouling.[Starboard].MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
        ]
        |> List.iter sink
        world.Avatars.[avatarId].Job
        |> Option.iter
            (fun job ->
                let island = 
                    world.Islands.[job.Destination]
                [
                    (Subheading, "Current Job:" |> Line) |> Hued
                    (Sublabel, "Description: " |> Text) |> Hued
                    (Flavor, job.FlavorText |> sprintf "%s" |> Line) |> Hued
                    (Sublabel, "Destination: " |> Text) |> Hued
                    (Value, island.Name |> sprintf "%s" |> Line) |> Hued
                    (Sublabel, "Reward: " |> Text) |> Hued
                    (Value, job.Reward |> sprintf "%f" |> Line) |> Hued
                ]
                |> List.iter sink)

    let Run (sink:MessageSink) (avatarId:string) (gamestate:Gamestate) : Gamestate option =
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink avatarId)
        gamestate
        |> Some


