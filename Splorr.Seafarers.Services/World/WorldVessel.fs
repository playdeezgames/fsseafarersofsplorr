namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldVessel =
    let internal SetSpeed 
            (context  : CommonContext)
            (speed    : float) 
            (avatarId : string) 
            : unit = 
        avatarId
        |> Vessel.SetSpeed 
            context
            speed 
        avatarId
        |> Vessel.GetSpeed 
            context
        |> Option.iter
            (fun newSpeed ->
                avatarId
                |> AvatarMessages.Add context [newSpeed |> sprintf "You set your speed to %.2f."])

    let internal SetHeading 
            (context  : CommonContext)
            (heading  : float) 
            (avatarId : string) 
            : unit =
        avatarId
        |> Vessel.SetHeading 
            context
            heading 
        avatarId
        |> Vessel.GetHeading 
            context
        |> Option.iter
            (fun newHeading ->
                avatarId
                |> AvatarMessages.Add context [newHeading |> Angle.ToDegrees |> Angle.ToString |> sprintf "You set your heading to %s." ])

    let rec internal Move
            (context  : CommonContext)
            (distance : uint32) 
            (avatarId : string) 
            : unit =
        match distance with
        | x when x > 0u ->
            avatarId
            |> AvatarMessages.Add context [ "Steady as she goes." ]
            Avatar.Move 
                context
                avatarId 
            avatarId
            |> WorldCreation.UpdateCharts 
                context
            if WorldShipmate.IsAvatarAlive 
                    context
                    avatarId |> not then
                avatarId
                |> AvatarMessages.Add context [ "You die of old age!" ]
            else
                Move
                    context
                    (x-1u) 
                    avatarId
        | _ -> 
            ()

    let internal GetNearbyLocations
            (context : CommonContext)
            (from                        : Location) 
            (maximumDistance             : float) 
            : Location list =
        Island.GetList context
        |> List.filter (fun i -> Location.DistanceTo from i <= maximumDistance)


    let private DoJobCompletion
            (context  : CommonContext)
            (location : Location) 
            (job      : Job) 
            (avatarId : string) 
            : unit = 
        if location = job.Destination then
            AvatarJob.Complete
                context
                avatarId
            avatarId
            |> AvatarMessages.Add context  [ "You complete your job." ]

    let internal Dock
            (context  : CommonContext)
            (location : Location) 
            (avatarId : string) 
            : unit =
        let locations = Island.GetList context
        match locations |> List.tryFind (fun x -> x = location) with
        | Some l ->
            let destinations =
                locations
                |> Set.ofList
                |> Set.remove location
            let oldVisitCount =
                AvatarIslandMetric.Get  context avatarId location AvatarIslandMetricIdentifier.VisitCount
                |> Option.defaultValue 0UL
            IslandVisit.Add
                context
                avatarId
                location
            let newVisitCount =
                AvatarIslandMetric.Get context avatarId location AvatarIslandMetricIdentifier.VisitCount
                |> Option.defaultValue 0UL
            l
            |> IslandJob.Generate 
                context 
                destinations 
            Island.GenerateCommodities 
                context
                location
            Island.GenerateItems 
                context
                location
            avatarId
            |> AvatarMessages.Add 
                context
                [ 
                    "You dock." 
                ]
            avatarId
            |> AvatarMetric.Add 
                context
                Metric.VisitedIsland 
                (if newVisitCount > oldVisitCount then 1UL else 0UL)
            avatarId
            |> Option.foldBack 
                (fun job w ->
                    DoJobCompletion
                        context
                        location
                        job
                        w
                    w) (AvatarJob.Get context avatarId)
            |> ignore
            AvatarIslandFeature.SetFeature 
                context
                ({
                    featureId = IslandFeatureIdentifier.Dock
                    location = location
                } 
                |> Some, 
                    avatarId)
        | _ -> 
            avatarId
            |> AvatarMessages.Add 
                context
                [ 
                    "There is no place to dock there." 
                ]

    let internal AbandonJob
            (context  : CommonContext)
            (avatarId : string) 
            : unit =
        match AvatarJob.Get context avatarId with
        | Some _ ->
            avatarId
            |> AvatarMessages.Add context [ "You abandon your job." ]
            avatarId
            |> AvatarJob.Abandon
                context
        | _ ->
            avatarId
            |> AvatarMessages.Add context [ "You have no job to abandon." ]

    let internal Undock
            (context : CommonContext)
            (avatarId : string)
            : unit =
        avatarId
        |> AvatarMessages.Add context [ "You undock." ]
        AvatarIslandFeature.SetFeature context (None, avatarId)
    



