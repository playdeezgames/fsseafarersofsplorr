namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module Vessel =
    type VesselStatisticTemplateSource = unit -> Map<VesselStatisticIdentifier, StatisticTemplate>
    type VesselSingleStatisticSource   = string * VesselStatisticIdentifier->Statistic option
    type VesselSingleStatisticSink     = string * VesselStatisticIdentifier * Statistic->unit

    type SetStatisticContext =
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink ref
    let private SetStatistic 
            (context : CommonContext)
            (avatarId : string)
            (identifier : VesselStatisticIdentifier, statistic : Statistic)
            : unit =
        (context :?> SetStatisticContext).vesselSingleStatisticSink.Value (avatarId, identifier, statistic)

    type GetStatisticTemplateContext =
        abstract member vesselStatisticTemplateSource : VesselStatisticTemplateSource ref
    let private GetStatisticTemplates
            (context : CommonContext)
            : Map<VesselStatisticIdentifier, StatisticTemplate> =
        (context :?> GetStatisticTemplateContext).vesselStatisticTemplateSource.Value()

    let internal Create
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        GetStatisticTemplates context
        |> Map.map
            (fun _ template ->
                {MinimumValue = template.MinimumValue; MaximumValue=template.MaximumValue; CurrentValue = template.CurrentValue})
        |> Map.toList
        |> List.iter (SetStatistic context avatarId)

    type GetStatisticContext =
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource ref
    let internal GetStatistic
            (context : CommonContext)
            (avatarId : string)
            (identifier: VesselStatisticIdentifier)
            : Statistic option =
        (context :?> GetStatisticContext).vesselSingleStatisticSource.Value (avatarId, identifier)

    let internal GetAvailableTonnage 
            (context : CommonContext)
            (avatarId : string)
            : float =
        GetStatistic
            context
            avatarId 
            VesselStatisticIdentifier.Tonnage 
        |> Option.map
            Statistic.GetCurrentValue 
        |> Option.get


    
    let internal TransformFouling 
            (context : CommonContext)
            (avatarId                    : string)
            (side                        : Side) 
            (transform                   : Statistic -> Statistic)
            : unit =
        let statisticIdentifier =
            match side with
            | Port -> VesselStatisticIdentifier.PortFouling
            | Starboard -> VesselStatisticIdentifier.StarboardFouling
        GetStatistic context avatarId statisticIdentifier
        |> Option.iter (fun s -> (statisticIdentifier, s |> transform) |> SetStatistic context avatarId)

    let internal RemoveFouling
            (context : CommonContext)
            (avatarId : string)
            (side : Side)
            : unit =
        TransformFouling 
            context
            avatarId 
            side 
            (fun x-> 
                {x with 
                    CurrentValue = x.MinimumValue})

    
    let internal Befoul 
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        let foulRate = 
            GetStatistic context avatarId VesselStatisticIdentifier.FoulRate
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

    let private GetCurrentValueAndAssumePresent
            (context : CommonContext)
            (identifier : VesselStatisticIdentifier)
            (avatarId: string)
            : float =
        identifier
        |> GetStatistic context avatarId
        |> Option.map Statistic.GetCurrentValue
        |> Option.get

    let internal GetPosition
            (context  : CommonContext)
            (avatarId : string)
            : Location =
        let positionX =
            GetCurrentValueAndAssumePresent context VesselStatisticIdentifier.PositionX avatarId
        let positionY = 
            GetCurrentValueAndAssumePresent context VesselStatisticIdentifier.PositionY avatarId
        (positionX, positionY)

    let internal SetPosition 
            (context  : CommonContext)
            (position : Location) 
            (avatarId : string) 
            : unit =
        match 
            GetStatistic context avatarId VesselStatisticIdentifier.PositionX, 
            GetStatistic context avatarId VesselStatisticIdentifier.PositionY 
            with
        | Some x, Some _ ->
            SetStatistic context
                avatarId 
                (VesselStatisticIdentifier.PositionX, 
                    x 
                    |> Statistic.SetCurrentValue 
                        (position 
                        |> fst))
            SetStatistic context 
                avatarId 
                (VesselStatisticIdentifier.PositionY, 
                    x 
                    |> Statistic.SetCurrentValue 
                        (position 
                        |> snd))
        | _ -> ()

    let internal GetSpeed
            (context  : CommonContext)
            (avatarId : string)
            : float option =
        VesselStatisticIdentifier.Speed
        |> GetStatistic context avatarId 
        |> Option.map Statistic.GetCurrentValue

    let internal GetHeading
            (context  : CommonContext)
            (avatarId : string)
            : float option =
        VesselStatisticIdentifier.Heading
        |> GetStatistic context avatarId 
        |> Option.map Statistic.GetCurrentValue
    
    let internal SetSpeed 
            (context  : CommonContext)
            (speed    : float) 
            (avatarId : string) 
            : unit =
        GetStatistic context avatarId VesselStatisticIdentifier.Speed
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Speed, 
                    statistic
                    |> Statistic.SetCurrentValue speed)
                |> SetStatistic context avatarId)

    let internal SetHeading 
            (context  : CommonContext)
            (heading  : float) 
            (avatarId : string) 
            : unit =
        GetStatistic context avatarId VesselStatisticIdentifier.Heading
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Heading, 
                    statistic
                    |> Statistic.SetCurrentValue (heading |> Angle.ToRadians))
                |> SetStatistic context avatarId)

    let private GetFouling
            //TODO: context me
            (context : CommonContext)
            (getter : Statistic -> float)
            (avatarId                    : string)
            :float =
        [
            VesselStatisticIdentifier.PortFouling
            VesselStatisticIdentifier.StarboardFouling
        ]
        |> List.map
            (GetStatistic context avatarId
                >> Option.map getter
                >> Option.defaultValue 0.0)
        |> List.reduce (+)

    let internal GetCurrentFouling
            (context : CommonContext) =
        GetFouling 
            context 
            Statistic.GetCurrentValue
    
    let internal GetMaximumFouling
            (context : CommonContext) =
        GetFouling 
            context
            Statistic.GetMaximumValue 

    let internal GetEffectiveSpeed 
            (context  : CommonContext)
            (avatarId : string)
            : float =
        let currentValue = GetCurrentFouling context avatarId
        let currentSpeed = GetSpeed context avatarId |> Option.get
        (currentSpeed * (1.0 - currentValue))



