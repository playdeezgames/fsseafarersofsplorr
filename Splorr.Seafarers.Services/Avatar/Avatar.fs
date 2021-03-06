namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type AvatarJobSink = string -> Job option -> unit

module Avatar =
    type CreateContext =
        inherit ServiceContext
        abstract member avatarJobSink : AvatarJobSink
    let Create 
            (context  : ServiceContext)
            (avatarId : string)
            : unit =
        let context = context :?> CreateContext
        Vessel.Create 
            context
            avatarId
        Shipmate.Create 
            context
            avatarId 
            Primary
        context.avatarJobSink avatarId None
    
    let internal IncrementMetric 
            (context  : ServiceContext)
            (metric   : Metric) 
            (avatarId : string) 
            : unit =
        let rateOfIncrement = 1UL
        avatarId
        |> AvatarMetric.Add 
            context
            metric 
            rateOfIncrement

    type EatContext =
        inherit ServiceContext
        abstract member avatarInventorySink           : AvatarInventorySink
        abstract member avatarInventorySource         : AvatarInventorySource
        abstract member avatarShipmateSource          : AvatarShipmateSource
    let private Eat
            (context : ServiceContext)
            (avatarId                      : string)
            : unit =
        let context = context :?> EatContext
        let inventory, eaten, starved =
            ((context.avatarInventorySource avatarId, 0UL, 0UL), context.avatarShipmateSource avatarId)
            ||> List.fold 
                (fun (inventory,eatMetric, starveMetric) identifier -> 
                    let updateInventory, ate, starved =
                        Shipmate.Eat
                            context
                            inventory 
                            avatarId 
                            identifier
                    (updateInventory, 
                        (if ate then eatMetric+1UL else eatMetric), 
                            (if starved then starveMetric+1UL else starveMetric))) 
        inventory
        |> context.avatarInventorySink avatarId
        if eaten > 0UL then
            avatarId
            |> AvatarMetric.Add 
                context
                Metric.Ate 
                eaten
        if starved > 0UL then
            avatarId
            |> AvatarMetric.Add 
                context
                Metric.Starved 
                starved


    type MoveContext =
        inherit ServiceContext
        abstract member vesselSingleStatisticSource   : VesselSingleStatisticSource
    let Move
            (context  : ServiceContext)
            (avatarId : string)
            : unit =
        let context = context :?> MoveContext
        let actualSpeed = 
            avatarId 
            |> Vessel.GetEffectiveSpeed context
        let actualHeading = 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.Heading 
            |> Option.map Statistic.GetCurrentValue 
            |> Option.get
        Vessel.Befoul 
            context
            avatarId
        let avatarPosition = 
            Vessel.GetPosition 
                context 
                avatarId 
            |> Option.get
        let newPosition = ((avatarPosition |> fst) + System.Math.Cos(actualHeading) * actualSpeed, (avatarPosition |> snd) + System.Math.Sin(actualHeading) * actualSpeed)
        Vessel.SetPosition context newPosition avatarId
        AvatarShipmates.Transform
            context
            (fun identifier -> 
                ShipmateStatistic.Transform 
                    context
                    ShipmateStatisticIdentifier.Turn 
                    (Statistic.ChangeCurrentBy 1.0 >> Some)
                    avatarId
                    identifier)
            avatarId
        avatarId
        |> AvatarMetric.Add 
            context
            Metric.Moved 
            1UL
        avatarId
        |> Eat 
            context

    type CleanHullContext =
        inherit ServiceContext
        abstract member avatarShipmateSource          : AvatarShipmateSource
    let CleanHull 
            (context : ServiceContext)
            (side : Side) 
            (avatarId : string)
            : unit =
        let context = context :?> CleanHullContext
        Vessel.TransformFouling 
            context
            avatarId 
            side 
            (fun x-> 
                {x with 
                    CurrentValue = x.MinimumValue})
        AvatarShipmates.Transform 
            context
            (ShipmateStatistic.Transform
                context
                ShipmateStatisticIdentifier.Turn 
                (Statistic.ChangeCurrentBy 1.0 >> Some)
                avatarId)
            avatarId
        avatarId
        |> IncrementMetric
            context
            Metric.CleanedHull
