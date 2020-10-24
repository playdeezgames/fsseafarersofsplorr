﻿namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldIslands = 
    type IslandLocationByNameSource = string -> Location option
    type GetIslandByNameContext =
        abstract member islandLocationByNameSource     : IslandLocationByNameSource ref
    let private GetIslandByName
            (context : CommonContext)
            (name : string) 
            : Location option =
        (context :?> GetIslandByNameContext).islandLocationByNameSource.Value name

    let internal DistanceTo 
            (context    : CommonContext)
            (islandName : string) 
            (avatarId   : string) 
            : unit =
        let location =
            GetIslandByName context islandName
            |> Option.bind
                (fun l ->
                    if (AvatarIslandMetric.Get context avatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
                    else
                        None)
        match location, Vessel.GetPosition context avatarId with
        | Some l, Some avatarPosition ->
            avatarId
            |> AvatarMessages.Add context [ (islandName, Location.DistanceTo l avatarPosition ) ||> sprintf "Distance to `%s` is %f." ]
        | _, Some _ ->
            avatarId
            |> AvatarMessages.Add context [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    let internal HeadFor
            (context : CommonContext)
            (islandName                     : string) 
            (avatarId                       : string) 
            : unit =
        let location =
            GetIslandByName context islandName
            |> Option.bind
                (fun l ->
                    if (AvatarIslandMetric.Get context avatarId l AvatarIslandMetricIdentifier.VisitCount).IsSome then
                        Some l
                    else
                        None)
        match location, Vessel.GetPosition context avatarId with
        | Some l, Some avatarPosition ->
            [
                AvatarMessages.Add
                    context
                    [ islandName |> sprintf "You head for `%s`." ]
                WorldVessel.SetHeading
                    context
                    (Location.HeadingTo avatarPosition l |> Angle.ToDegrees)
            ]
            |> List.iter (fun f -> f avatarId)
        | _, Some _ ->
            avatarId
            |> AvatarMessages.Add context [ islandName |> sprintf "I don't know how to get to `%s`." ]
        | _ ->
            ()

    type IslandSingleJobSource = Location * uint32 -> Job option
    type GetIslandJobContext =
        abstract member islandSingleJobSource : IslandSingleJobSource ref
    let private GetIslandJob
            (context : CommonContext)
            (location : Location)
            (index : uint32)
            : Job option =
        (context :?> GetIslandJobContext).islandSingleJobSource.Value (location, index)

    let private JobUnavailable
            (context : CommonContext) =
        AvatarMessages.Add context [ "That job is currently unavailable." ]

    let private CannotHaveMoreThanOneJob
            (context : CommonContext)=
        AvatarMessages.Add context [ "You must complete or abandon your current job before taking on a new one." ]

    let private JobAcceptance
            (context : CommonContext) = 
        AvatarMessages.Add context [ "You accepted the job!" ]

    let private IslandDoesNotExist
            (context : CommonContext) = 
        AvatarMessages.Add context [ "The island does not exist!" ]

    let private IncrementJobAcceptedMetric
            (context : CommonContext) =
        AvatarMetric.Add 
            context
            Metric.AcceptedJob 
            1UL

    let private InnerInnerAcceptJob
            (context : CommonContext)
            (location : Location) 
            (avatarId : string) 
            (job : Job)
            (jobIndex : uint32) 
            : unit =
        JobAcceptance context avatarId
        IncrementJobAcceptedMetric context avatarId
        Avatar.SetJob context avatarId (job|>Some)
        IslandVisit.MakeKnown
            context
            avatarId
            job.Destination
        IslandJob.Purge context location jobIndex


    let private InnerAcceptJob
            (context : CommonContext)
            (jobIndex : uint32) 
            (location : Location) 
            (avatarId : string) 
            : unit =
        match GetIslandJob context location jobIndex with
        | Some job ->
            InnerInnerAcceptJob context location avatarId job jobIndex
        | _ ->
            JobUnavailable context avatarId

    let internal AcceptJob 
            (context  : CommonContext)
            (jobIndex : uint32) 
            (location : Location) 
            (avatarId : string) 
            : unit =
        if Island.Exists context location then
            match AvatarJob.Get context avatarId with
            | None ->
                InnerAcceptJob context jobIndex location avatarId
            | Some _ ->
                CannotHaveMoreThanOneJob context avatarId
        else
            IslandDoesNotExist context avatarId


