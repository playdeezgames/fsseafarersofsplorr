namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type VesselStatisticTemplateSource = unit -> Map<VesselStatisticIdentifier, StatisticTemplate>
type VesselStatisticSink           = string -> Map<VesselStatisticIdentifier, Statistic> -> unit
type VesselSingleStatisticSource   = string->VesselStatisticIdentifier->Statistic option
type VesselSingleStatisticSink     = string->VesselStatisticIdentifier*Statistic->unit

type VesselCreateContext =
    abstract member vesselStatisticSink           : VesselStatisticSink
    abstract member vesselStatisticTemplateSource : VesselStatisticTemplateSource

type VesselTransformFoulingContext =
    abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

type VesselBefoulContext =
    inherit VesselTransformFoulingContext
    abstract member vesselSingleStatisticSource : VesselSingleStatisticSource

module Vessel =
    let Create
            (context  : VesselCreateContext)
            (avatarId : string)
            : unit =
        context.vesselStatisticTemplateSource()
        |> Map.map
            (fun _ template ->
                {MinimumValue = template.MinimumValue; MaximumValue=template.MaximumValue; CurrentValue = template.CurrentValue})
        |> context.vesselStatisticSink avatarId

    let TransformFouling 
            (context : VesselTransformFoulingContext)
            (avatarId                    : string)
            (side                        : Side) 
            (transform                   : Statistic -> Statistic)
            : unit =
        let statisticIdentifier =
            match side with
            | Port -> VesselStatisticIdentifier.PortFouling
            | Starboard -> VesselStatisticIdentifier.StarboardFouling
        context.vesselSingleStatisticSource avatarId statisticIdentifier
        |> Option.iter (fun s -> (statisticIdentifier, s |> transform) |> context.vesselSingleStatisticSink avatarId)
    
    let Befoul 
            (context  : VesselBefoulContext)
            (avatarId : string)
            : unit =
        let foulRate = 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.FoulRate
            |> Option.map (fun x->x.CurrentValue)
            |> Option.get
        [
            Port
            Starboard
        ]
        |> List. iter
            (fun side ->
                TransformFouling 
                    context
                    avatarId 
                    side 
                    (Statistic.ChangeCurrentBy (foulRate/2.0)))

