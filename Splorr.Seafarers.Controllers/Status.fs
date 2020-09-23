namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type StatusRunWorldContext =
    inherit OperatingContext
    abstract member avatarJobSource               : AvatarJobSource
    abstract member islandSingleNameSource        : IslandSingleNameSource
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource


type StatusRunContext =
    inherit OperatingContext
    abstract member avatarJobSource               : AvatarJobSource
    abstract member islandSingleNameSource        : IslandSingleNameSource
    abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource


module Status =
    let private RunWorld 
            (context : OperatingContext)
            (messageSink                   : MessageSink) 
            (avatarId                      : string) 
            : unit =
        let context = context :?> StatusRunWorldContext
        let portFouling = context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PortFouling |> Option.get
        let satiety = context.shipmateSingleStatisticSource avatarId Primary ShipmateStatisticIdentifier.Satiety |> Option.get
        let health = context.shipmateSingleStatisticSource avatarId Primary ShipmateStatisticIdentifier.Health |> Option.get
        let starboardFouling = context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.StarboardFouling |> Option.get
        [
            "" |> Line
            (Hue.Heading, "Status:" |> Line) |> Hued
            (Hue.Label, "Money: " |> Text) |> Hued
            (Hue.Value, avatarId |> Avatar.GetMoney context |> sprintf "%f" |> Line) |> Hued
            (Hue.Label, "Reputation: " |> Text) |> Hued
            (Hue.Value, avatarId |> Avatar.GetReputation context |> sprintf "%f" |> Line) |> Hued
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
        |> context.avatarJobSource
        |> Option.iter
            (fun job ->
                [
                    (Hue.Subheading, "Current Job:" |> Line) |> Hued
                    (Hue.Sublabel, "Description: " |> Text) |> Hued
                    (Hue.Flavor, job.FlavorText |> sprintf "%s" |> Line) |> Hued
                    (Hue.Sublabel, "Destination: " |> Text) |> Hued
                    (Hue.Value, job.Destination |> context.islandSingleNameSource |> Option.get |> sprintf "%s" |> Line) |> Hued
                    (Hue.Sublabel, "Reward: " |> Text) |> Hued
                    (Hue.Value, job.Reward |> sprintf "%f" |> Line) |> Hued
                ]
                |> List.iter messageSink)

    let Run 
            (context : OperatingContext)
            (messageSink                   : MessageSink) 
            (gamestate                     : Gamestate) 
            : Gamestate option =
        let context = context :?> StatusRunContext
        gamestate
        |> Gamestate.GetWorld
        |> Option.iter 
            (RunWorld 
                context
                messageSink)
        gamestate
        |> Some


