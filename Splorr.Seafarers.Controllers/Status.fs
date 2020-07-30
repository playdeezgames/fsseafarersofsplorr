namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Status =
    let private RunWorld 
            (messageSink:MessageSink) 
            (world:World) 
            : unit =
        let avatar = world.Avatars.[world.AvatarId]
        let shipmate = avatar.Shipmates.[0]
        let vessel = avatar.Vessel
        [
            "" |> Line
            (Hue.Heading, "Status:" |> Line) |> Hued
            (Hue.Label, "Money: " |> Text) |> Hued
            (Hue.Value, avatar.Money |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Reputation: " |> Text) |> Hued
            (Hue.Value, avatar.Reputation |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Satiety: " |> Text) |> Hued
            (Hue.Value, (shipmate.Statistics.[AvatarStatisticIdentifier.Satiety].CurrentValue, shipmate.Statistics.[AvatarStatisticIdentifier.Satiety].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Hue.Label, "Health: " |> Text) |> Hued
            (Hue.Value, (shipmate.Statistics.[AvatarStatisticIdentifier.Health].CurrentValue, shipmate.Statistics.[AvatarStatisticIdentifier.Health].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Hue.Label, "Port Fouling: " |> Text) |> Hued
            (Hue.Value, (vessel.Fouling.[Port].CurrentValue, vessel.Fouling.[Port].MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
            (Hue.Label, "Starboard Fouling: " |> Text) |> Hued
            (Hue.Value, (vessel.Fouling.[Starboard].CurrentValue, vessel.Fouling.[Starboard].MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
        ]
        |> List.iter messageSink
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
                |> List.iter messageSink)

    let Run 
            (messageSink:MessageSink) 
            (gamestate:Gamestate) 
            : Gamestate option =
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld messageSink)
        gamestate
        |> Some


