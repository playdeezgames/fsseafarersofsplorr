namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type VesselStatisticTemplateSource = unit -> Map<VesselStatisticIdentifier, StatisticTemplate>
type VesselStatisticSink           = string -> Map<VesselStatisticIdentifier, Statistic> -> unit
type VesselSingleStatisticSource   = string->VesselStatisticIdentifier->Statistic option
type VesselSingleStatisticSink     = string->VesselStatisticIdentifier*Statistic->unit

module Vessel =
    let Create
            (vesselStatisticTemplateSource : VesselStatisticTemplateSource)
            (vesselStatisticSink           : VesselStatisticSink)
            (avatarId                      : string)
            : unit =
        vesselStatisticTemplateSource()
        |> Map.map
            (fun _ template ->
                {MinimumValue = template.MinimumValue; MaximumValue=template.MaximumValue; CurrentValue = template.CurrentValue})
        |> vesselStatisticSink avatarId

    let TransformFouling 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarId                    : string)
            (side                        : Side) 
            (transform                   : Statistic -> Statistic)
            : unit =
        let statisticIdentifier =
            match side with
            | Port -> VesselStatisticIdentifier.PortFouling
            | Starboard -> VesselStatisticIdentifier.StarboardFouling
        vesselSingleStatisticSource avatarId statisticIdentifier
        |> Option.iter (fun s -> (statisticIdentifier, s |> transform) |> vesselSingleStatisticSink avatarId)
    
    let Befoul 
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (vesselSingleStatisticSink   : VesselSingleStatisticSink)
            (avatarId                    : string)
            : unit =
        let foulRate = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.FoulRate
            |> Option.map (fun x->x.CurrentValue)
            |> Option.get
        TransformFouling vesselSingleStatisticSource vesselSingleStatisticSink avatarId Port (Statistic.ChangeCurrentBy (foulRate/2.0))
        TransformFouling vesselSingleStatisticSource vesselSingleStatisticSink avatarId Starboard (Statistic.ChangeCurrentBy (foulRate/2.0))

