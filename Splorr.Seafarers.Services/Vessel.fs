namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Vessel =
    let Create
            (vesselStatisticTemplateSource: unit -> Map<VesselStatisticIdentifier, VesselStatisticTemplate>)
            (vesselStatisticSink: string -> Map<VesselStatisticIdentifier, Statistic> -> unit)
            (avatarId: string)
            (tonnage:float) 
            : Vessel =
        vesselStatisticTemplateSource()
        |> Map.map
            (fun _ template ->
                {MinimumValue = template.MinimumValue; MaximumValue=template.MaximumValue; CurrentValue = template.CurrentValue})
        |> vesselStatisticSink avatarId
        {
            Tonnage = tonnage
            FoulRate = 0.001 //TODO: dont hardcode, and base it on vessel type
        }

    let TransformFouling 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink   : string->VesselStatisticIdentifier*Statistic->unit)
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
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink   : string->VesselStatisticIdentifier*Statistic->unit)
            (avatarId                    : string)
            (vessel                      : Vessel) 
            : unit =
        TransformFouling vesselSingleStatisticSource vesselSingleStatisticSink avatarId Port (Statistic.ChangeCurrentBy (vessel.FoulRate/2.0))
        TransformFouling vesselSingleStatisticSource vesselSingleStatisticSink avatarId Starboard (Statistic.ChangeCurrentBy (vessel.FoulRate/2.0))

