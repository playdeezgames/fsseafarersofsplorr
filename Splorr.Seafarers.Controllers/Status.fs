namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Status =
    let private RunWorld 
            (context : ServiceContext)
            (messageSink                   : MessageSink) 
            (avatarId                      : string) 
            : unit =
        let portFouling = Vessel.GetStatistic context avatarId VesselStatisticIdentifier.PortFouling |> Option.get
        let satiety = ShipmateStatistic.Get context avatarId Primary ShipmateStatisticIdentifier.Satiety |> Option.get
        let health = ShipmateStatistic.Get context avatarId Primary ShipmateStatisticIdentifier.Health |> Option.get
        let starboardFouling = Vessel.GetStatistic context avatarId VesselStatisticIdentifier.StarboardFouling |> Option.get
        [
            "" |> Line
            (Hue.Heading, "Status:" |> Line) |> Hued
            (Hue.Label, "Money: " |> Text) |> Hued
            (Hue.Value, avatarId |> AvatarShipmates.GetMoney context |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Reputation: " |> Text) |> Hued
            (Hue.Value, avatarId |> AvatarShipmates.GetReputation context |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Satiety: " |> Text) |> Hued
            (Hue.Value, (satiety.CurrentValue, satiety.MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Hue.Label, "Health: " |> Text) |> Hued
            (Hue.Value, (health.CurrentValue, health.MaximumValue) ||> sprintf "%.0f/%.0f" |> Line) |> Hued
            (Hue.Label, "Port Fouling: " |> Text) |> Hued
            (Hue.Value, (portFouling.CurrentValue, portFouling.MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
            (Hue.Label, "Starboard Fouling: " |> Text) |> Hued
            (Hue.Value, (starboardFouling.CurrentValue, starboardFouling.MaximumValue) ||> sprintf "%.2f/%.2f" |> Line) |> Hued
        ]
        |> List.iter messageSink
        avatarId
        |> AvatarJob.Get context
        |> Option.iter
            (fun job ->
                [
                    (Hue.Subheading, "Current Job:" |> Line) |> Hued
                    (Hue.Sublabel, "Description: " |> Text) |> Hued
                    (Hue.Flavor, job.FlavorText |> sprintf "%s" |> Line) |> Hued
                    (Hue.Sublabel, "Destination: " |> Text) |> Hued
                    (Hue.Value, job.Destination |> IslandName.GetName context |> Option.get |> sprintf "%s" |> Line) |> Hued
                    (Hue.Sublabel, "Reward: " |> Text) |> Hued
                    (Hue.Value, job.Reward |> sprintf "%f" |> Line) |> Hued
                ]
                |> List.iter messageSink)

    let Run 
            (context : ServiceContext)
            (messageSink                   : MessageSink) 
            (gamestate                     : Gamestate) 
            : Gamestate option =
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter 
            (RunWorld 
                context
                messageSink)
        gamestate
        |> Some


