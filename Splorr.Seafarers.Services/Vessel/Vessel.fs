namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type VesselStatisticTemplateSource = unit -> Map<VesselStatisticIdentifier, StatisticTemplate>
type VesselStatisticSink           = string -> Map<VesselStatisticIdentifier, Statistic> -> unit
type VesselSingleStatisticSource   = string->VesselStatisticIdentifier->Statistic option
type VesselSingleStatisticSink     = string->VesselStatisticIdentifier*Statistic->unit

module Vessel =
    type CreateContext =
        inherit ServiceContext
        abstract member vesselStatisticSink           : VesselStatisticSink
        abstract member vesselStatisticTemplateSource : VesselStatisticTemplateSource
    let Create
            (context  : ServiceContext)
            (avatarId : string)
            : unit =
        let context = context :?> CreateContext
        context.vesselStatisticTemplateSource()
        |> Map.map
            (fun _ template ->
                {MinimumValue = template.MinimumValue; MaximumValue=template.MaximumValue; CurrentValue = template.CurrentValue})
        |> context.vesselStatisticSink avatarId

    type GetStatisticContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetStatistic
            (context : ServiceContext)
            (avatarId : string)
            (identifier: VesselStatisticIdentifier)
            : Statistic option =
        (context :?> GetStatisticContext).vesselSingleStatisticSource avatarId identifier
    
    type TransformFoulingContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    let TransformFouling 
            (context : ServiceContext)
            (avatarId                    : string)
            (side                        : Side) 
            (transform                   : Statistic -> Statistic)
            : unit =
        let context = context :?> TransformFoulingContext
        let statisticIdentifier =
            match side with
            | Port -> VesselStatisticIdentifier.PortFouling
            | Starboard -> VesselStatisticIdentifier.StarboardFouling
        GetStatistic context avatarId statisticIdentifier
        |> Option.iter (fun s -> (statisticIdentifier, s |> transform) |> context.vesselSingleStatisticSink avatarId)
    
    let Befoul 
            (context  : ServiceContext)
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

    type GetPositionContext = 
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetPosition
            (context  : ServiceContext)
            (avatarId : string)
            : Location option =
        let context = context :?> GetPositionContext
        let positionX =
            VesselStatisticIdentifier.PositionX
            |> context.vesselSingleStatisticSource avatarId
            |> Option.map Statistic.GetCurrentValue
        let positionY = 
            VesselStatisticIdentifier.PositionY
            |> context.vesselSingleStatisticSource avatarId
            |> Option.map Statistic.GetCurrentValue
        match positionX, positionY with
        | Some x, Some y ->
            (x,y) |> Some
        | _ ->
            None

    type SetPositionContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let SetPosition 
            (context  : ServiceContext)
            (position : Location) 
            (avatarId : string) 
            : unit =
        let context = context :?> SetPositionContext
        match 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PositionX, 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.PositionY 
            with
        | Some x, Some _ ->
            context.vesselSingleStatisticSink 
                avatarId 
                (VesselStatisticIdentifier.PositionX, 
                    x 
                    |> Statistic.SetCurrentValue 
                        (position 
                        |> fst))
            context.vesselSingleStatisticSink 
                avatarId 
                (VesselStatisticIdentifier.PositionY, 
                    x 
                    |> Statistic.SetCurrentValue 
                        (position 
                        |> snd))
        | _ -> ()

    type GetSpeedContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetSpeed
            (context  : ServiceContext)
            (avatarId : string)
            : float option =
        let context = context :?> GetSpeedContext
        VesselStatisticIdentifier.Speed
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue

    type GetHeadingContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetHeading
            (context  : ServiceContext)
            (avatarId : string)
            : float option =
        let context = context :?> GetHeadingContext
        VesselStatisticIdentifier.Heading
        |> context.vesselSingleStatisticSource avatarId 
        |> Option.map Statistic.GetCurrentValue
    
    type SetSpeedContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    let SetSpeed 
            (context  : ServiceContext)
            (speed    : float) 
            (avatarId : string) 
            : unit =
        let context = context :?> SetSpeedContext
        context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Speed
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Speed, 
                    statistic
                    |> Statistic.SetCurrentValue speed)
                |> context.vesselSingleStatisticSink avatarId)

    type SetHeadingContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
        abstract member vesselSingleStatisticSink   : VesselSingleStatisticSink
    let SetHeading 
            (context  : ServiceContext)
            (heading  : float) 
            (avatarId : string) 
            : unit =
        let context = context :?> SetHeadingContext
        context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading
        |> Option.iter
            (fun statistic ->
                (VesselStatisticIdentifier.Heading, 
                    statistic
                    |> Statistic.SetCurrentValue (heading |> Angle.ToRadians))
                |> context.vesselSingleStatisticSink avatarId)

    let private GetFouling
            //TODO: context me
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (getter : Statistic -> float)
            (avatarId                    : string)
            :float =
        [
            VesselStatisticIdentifier.PortFouling
            VesselStatisticIdentifier.StarboardFouling
        ]
        |> List.map
            (vesselSingleStatisticSource avatarId
                >> Option.map getter
                >> Option.defaultValue 0.0)
        |> List.reduce (+)

    type GetCurrentFoulingContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetCurrentFouling
            (context : ServiceContext) =
        let context = context :?> GetCurrentFoulingContext
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetCurrentValue
    
    type GetMaximumFoulingContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource : VesselSingleStatisticSource
    let GetMaximumFouling
            (context : ServiceContext) =
        let context = context :?> GetMaximumFoulingContext
        GetFouling 
            context.vesselSingleStatisticSource 
            Statistic.GetMaximumValue 

    let GetEffectiveSpeed 
            (context  : ServiceContext)
            (avatarId : string)
            : float =
        let currentValue = GetCurrentFouling context avatarId
        let currentSpeed = GetSpeed context avatarId |> Option.get
        (currentSpeed * (1.0 - currentValue))



