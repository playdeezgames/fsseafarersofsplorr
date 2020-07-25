namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Status =
    let private RunWorld (sink:MessageSink) (world:World) : unit =
        [
            "" |> Line
            (Hue.Heading, "Status:" |> Line) |> Hued
            (Hue.Label, "Money: " |> Text) |> Hued
            (Hue.Value, world.Avatars.[world.AvatarId].Money |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Reputation: " |> Text) |> Hued
            (Hue.Value, world.Avatars.[world.AvatarId].Reputation |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Satiety: " |> Text) |> Hued
            (Hue.Value, (world.Avatars.[world.AvatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Satiety].CurrentValue, world.Avatars.[world.AvatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Satiety].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Hue.Label, "Health: " |> Text) |> Hued
            (Hue.Value, (world.Avatars.[world.AvatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Health].CurrentValue, world.Avatars.[world.AvatarId].Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Health].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Hue.Label, "Port Fouling: " |> Text) |> Hued
            (Hue.Value, (world.Avatars.[world.AvatarId].Vessel.Fouling.[Port].CurrentValue, world.Avatars.[world.AvatarId].Vessel.Fouling.[Port].MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
            (Hue.Label, "Starboard Fouling: " |> Text) |> Hued
            (Hue.Value, (world.Avatars.[world.AvatarId].Vessel.Fouling.[Starboard].CurrentValue, world.Avatars.[world.AvatarId].Vessel.Fouling.[Starboard].MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
        ]
        |> List.iter sink
        world.Avatars.[world.AvatarId].Job
        |> Option.iter
            (fun job ->
                let island = 
                    world.Islands.[job.Destination]
                [
                    (Hue.Subheading, "Current Job:" |> Line) |> Hued
                    (Hue.Sublabel, "Description: " |> Text) |> Hued
                    (Hue.Flavor, job.FlavorText |> sprintf "%s" |> Line) |> Hued
                    (Hue.Sublabel, "Destination: " |> Text) |> Hued
                    (Hue.Value, island.Name |> sprintf "%s" |> Line) |> Hued
                    (Hue.Sublabel, "Reward: " |> Text) |> Hued
                    (Hue.Value, job.Reward |> sprintf "%f" |> Line) |> Hued
                ]
                |> List.iter sink)

    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink)
        gamestate
        |> Some


