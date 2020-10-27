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
            (avatarId : string) 
            (job      : Job) 
            : unit = 
        if location = job.Destination then
            AvatarJob.Complete
                context
                avatarId
            avatarId
            |> AvatarMessages.Add context  [ "You complete your job." ]

    let private DockWhenIslandDoesNotExist
            (context : CommonContext) =
        AvatarMessages.Add context [ "There is no place to dock there." ]

    let private IncrementVisitCountAndVisitMetric
            (context : CommonContext)
            (location : Location)
            (avatarId : string)
            : unit = 
        let oldVisitCount =
            AvatarIslandMetric.GetVisitCount context avatarId location
        IslandVisit.Add context avatarId location
        let newVisitCount =
            AvatarIslandMetric.GetVisitCount context avatarId location
        if newVisitCount > oldVisitCount then
            avatarId
            |> AvatarMetric.IncrementVisitedIslands context 

    let private CompleteJobs
            (context : CommonContext)
            (location : Location)
            (avatarId : string)
            : unit =
        AvatarJob.Get context avatarId
        |> Option.iter
            (DoJobCompletion context location avatarId)


    let private UpdateAvatarWhenDocking
            (context : CommonContext)
            (location : Location)
            (avatarId: string)
            : unit =
        IncrementVisitCountAndVisitMetric context location avatarId
        AvatarMessages.Add context [ "You dock." ] avatarId
        CompleteJobs context location avatarId
        AvatarIslandFeature.SetDockFeatureForAvatar context location avatarId 

    let private GenerateJobs
            (context : CommonContext)
            (location : Location)
            : unit =
        let destinations =
            Island.GetJobDestinations context location
        location
        |> IslandJob.Generate 
            context 
            destinations 

    let private UpdateIslandWhenDocking
            (context : CommonContext)
            (location : Location)
            : unit =
        GenerateJobs context location
        Island.GenerateCommodities context location
        Island.GenerateItems context location

    let internal DockWhenIslandExists
            (context  : CommonContext)
            (location : Location) 
            (avatarId : string)
            : unit =
        UpdateIslandWhenDocking context location
        UpdateAvatarWhenDocking context location avatarId

    let internal Dock
            (context  : CommonContext)
            (location : Location) 
            (avatarId : string)
            : unit =
        if Island.Exists context location then
            DockWhenIslandExists context location avatarId
        else
            DockWhenIslandDoesNotExist context avatarId

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
    



