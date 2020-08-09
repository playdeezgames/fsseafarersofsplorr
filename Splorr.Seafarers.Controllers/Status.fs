namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Status =
    let private RunWorld 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (messageSink                 : MessageSink) 
            (world                       : World) 
            : unit =
        let avatar = world.Avatars.[world.AvatarId]
        let shipmate = avatar.Shipmates.[Primary]
        let portFouling = vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.PortFouling |> Option.get
        let starboardFouling = vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.StarboardFouling |> Option.get
        [
            "" |> Line
            (Hue.Heading, "Status:" |> Line) |> Hued
            (Hue.Label, "Money: " |> Text) |> Hued
            (Hue.Value, avatar |> Avatar.GetMoney |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Reputation: " |> Text) |> Hued
            (Hue.Value, avatar |> Avatar.GetReputation |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Satiety: " |> Text) |> Hued
            (Hue.Value, (shipmate.Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue, shipmate.Statistics.[ShipmateStatisticIdentifier.Satiety].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Hue.Label, "Health: " |> Text) |> Hued
            (Hue.Value, (shipmate.Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue, shipmate.Statistics.[ShipmateStatisticIdentifier.Health].MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Hue.Label, "Port Fouling: " |> Text) |> Hued
            (Hue.Value, (portFouling.CurrentValue, portFouling.MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
            (Hue.Label, "Starboard Fouling: " |> Text) |> Hued
            (Hue.Value, (starboardFouling.CurrentValue, starboardFouling.MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
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
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (messageSink                 : MessageSink) 
            (gamestate                   : Gamestate) 
            : Gamestate option =
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld vesselSingleStatisticSource messageSink)
        gamestate
        |> Some


