namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models
open Splorr.Common

module Avatar =
    type AvatarJobSink = string * Job option -> unit

    type SetJobContext =
        abstract member avatarJobSink : AvatarJobSink ref
    let internal SetJob
            (context : CommonContext)
            (avatarId : string)
            (job : Job option)
            : unit =
        (context :?> SetJobContext).avatarJobSink.Value (avatarId, job)

    let internal Create 
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        Vessel.Create 
            context
            avatarId
        Shipmate.Create 
            context
            avatarId 
            Primary
        SetJob context avatarId None
    
    let internal IncrementMetric 
            (context  : CommonContext)
            (metric   : Metric) 
            (avatarId : string) 
            : unit =
        let rateOfIncrement = 1UL
        avatarId
        |> AvatarMetric.Add 
            context
            metric 
            rateOfIncrement

    let private Eat
            (context : CommonContext)
            (avatarId                      : string)
            : unit =
        let inventory, eaten, starved =
            ((AvatarInventory.GetInventory context avatarId, 0UL, 0UL), AvatarShipmates.GetShipmates context avatarId)
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
        |> AvatarInventory.SetInventory context avatarId
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

    let internal Move
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        let actualSpeed = 
            avatarId 
            |> Vessel.GetEffectiveSpeed context
        let actualHeading = 
            Vessel.GetStatistic context avatarId VesselStatisticIdentifier.Heading 
            |> Option.map Statistic.GetCurrentValue 
            |> Option.get
        Vessel.Befoul 
            context
            avatarId
        let avatarPosition = 
            Vessel.GetPosition 
                context 
                avatarId 
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

    let internal CleanHull 
            (context : CommonContext)
            (side : Side) 
            (avatarId : string)
            : unit =
        Vessel.RemoveFouling
            context
            avatarId
            side
        //TODO: shipmates age, but apparently do not need to eat?
        AvatarShipmates.IncrementTurn
            context
            avatarId
        IncrementMetric
            context
            Metric.CleanedHull
            avatarId
