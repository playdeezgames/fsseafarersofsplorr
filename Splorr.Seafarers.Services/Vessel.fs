namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type VesselStatisticTemplateSource = unit -> Map<VesselStatisticIdentifier, StatisticTemplate>
type VesselStatisticSink           = string -> Map<VesselStatisticIdentifier, Statistic> -> unit
type VesselSingleStatisticSource   = string->VesselStatisticIdentifier->Statistic option
type VesselSingleStatisticSink     = string->VesselStatisticIdentifier*Statistic->unit

module Vessel =
    type CreateContext =
        inherit OperatingContext
        abstract member vesselStatisticSink           : VesselStatisticSink
        abstract member vesselStatisticTemplateSource : VesselStatisticTemplateSource
    let Create
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> CreateContext
        context.vesselStatisticTemplateSource()
        |> Map.map
            (fun _ template ->
                {MinimumValue = template.MinimumValue; MaximumValue=template.MaximumValue; CurrentValue = template.CurrentValue})
        |> context.vesselStatisticSink avatarId

    type GetStatisticContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetStatistic
            (context : OperatingContext)
            (avatarId : string)
            (identifier: VesselStatisticIdentifier)
            : Statistic option =
        (context :?> GetStatisticContext).vesselSingleStatisticSource avatarId identifier
    
    type TransformFoulingContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let TransformFouling 
            (context : OperatingContext)
            (avatarId                    : string)
            (side                        : Side) 
            (transform                   : Statistic -> Statistic)
            : unit =
        let context = context :?> TransformFoulingContext
        let statisticIdentifier =
            match side with
            | Port -> VesselStatisticIdentifier.PortFouling
            | Starboard -> VesselStatisticIdentifier.StarboardFouling
        context.vesselSingleStatisticSource avatarId statisticIdentifier
        |> Option.iter (fun s -> (statisticIdentifier, s |> transform) |> context.vesselSingleStatisticSink avatarId)
    
    type BefoulContext =
        inherit OperatingContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let Befoul 
            (context  : OperatingContext)
            (avatarId : string)
            : unit =
        let context = context :?> BefoulContext
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

