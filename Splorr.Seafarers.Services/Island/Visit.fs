namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type EpochSecondsSource = unit -> uint64

module IslandVisit =
    type AddContext =
        inherit ServiceContext
        abstract member epochSecondsSource : EpochSecondsSource
    let Add 
            (context      : ServiceContext)
            (avatarId     : string) 
            (location     : Location)
            : unit =
        let context = context :?> AddContext
        let metricSource = AvatarIslandMetric.Get context avatarId location
        let metricSink = AvatarIslandMetric.Put context avatarId location
        let sinkMetrics(visitCount,lastVisit) =
            metricSink AvatarIslandMetricIdentifier.VisitCount visitCount
            metricSink AvatarIslandMetricIdentifier.LastVisit lastVisit
        let visitCount = metricSource AvatarIslandMetricIdentifier.VisitCount
        let lastVisit = metricSource AvatarIslandMetricIdentifier.LastVisit
        let currentEpochSeconds = context.epochSecondsSource()
        match visitCount, lastVisit with
        | None, _ ->
            sinkMetrics (1UL, currentEpochSeconds)
        | Some x, None ->
            sinkMetrics (x + 1UL, currentEpochSeconds)
        | Some x, Some y when y < currentEpochSeconds ->
            sinkMetrics (x + 1UL, currentEpochSeconds)
        | _ -> 
            ()

    let MakeKnown
            (context  : ServiceContext)
            (avatarId : string) 
            (location : Location)
            : unit =
        let visitCount = 
            AvatarIslandMetric.Get context
                avatarId 
                location 
                AvatarIslandMetricIdentifier.VisitCount
        if visitCount.IsNone then
            AvatarIslandMetric.Put context
                avatarId 
                location 
                AvatarIslandMetricIdentifier.VisitCount 
                0UL


