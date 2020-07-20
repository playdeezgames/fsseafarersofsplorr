namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Status =
    let private RunWorld (sink:MessageSink) (world:World) : unit =
        [
            "" |> Line
            (Heading, "Status:" |> Line) |> Hued
            (Label, "Money: " |> Text) |> Hued
            (Value, world.Avatars.[world.AvatarId].Money |> sprintf "%f" |> Line) |> Hued
            (Label, "Reputation: " |> Text) |> Hued
            (Value, world.Avatars.[world.AvatarId].Reputation |> sprintf "%f" |> Line) |> Hued
            (Label, "Satiety: " |> Text) |> Hued
            (Value, (world.Avatars.[world.AvatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Satiety].CurrentValue, world.Avatars.[world.AvatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Satiety].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Label, "Health: " |> Text) |> Hued
            (Value, (world.Avatars.[world.AvatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Health].CurrentValue, world.Avatars.[world.AvatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Health].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Label, "Port Fouling: " |> Text) |> Hued
            (Value, (world.Avatars.[world.AvatarId].Vessel.Fouling.[Port].CurrentValue, world.Avatars.[world.AvatarId].Vessel.Fouling.[Port].MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
            (Label, "Starboard Fouling: " |> Text) |> Hued
            (Value, (world.Avatars.[world.AvatarId].Vessel.Fouling.[Starboard].CurrentValue, world.Avatars.[world.AvatarId].Vessel.Fouling.[Starboard].MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
        ]
        |> List.iter sink
        world.Avatars.[world.AvatarId].Job
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

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink)
        gamestate
        |> Some


