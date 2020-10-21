namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

type EpochSecondsSource = unit -> uint64

module IslandVisit =
    type EpochSecondsSourceContext =
        abstract member epochSecondsSource : EpochSecondsSource ref
    let private GetEpochSeconds
            (context : CommonContext)
            : uint64 =
        (context :?> EpochSecondsSourceContext).epochSecondsSource.Value()

    let internal Add 
            (context      : CommonContext)
            (avatarId     : string) 
            (location     : Location)
            : unit =
        let metricSource = AvatarIslandMetric.Get context avatarId location
        let metricSink = AvatarIslandMetric.Put context avatarId location
        let sinkMetrics(visitCount,lastVisit) =
            metricSink AvatarIslandMetricIdentifier.VisitCount visitCount
            metricSink AvatarIslandMetricIdentifier.LastVisit lastVisit
        let visitCount = metricSource AvatarIslandMetricIdentifier.VisitCount
        let lastVisit = metricSource AvatarIslandMetricIdentifier.LastVisit
        let currentEpochSeconds = GetEpochSeconds context
        match visitCount, lastVisit with
        | None, _ ->
            sinkMetrics (1UL, currentEpochSeconds)
        | Some x, None ->
            sinkMetrics (x + 1UL, currentEpochSeconds)
        | Some x, Some y when y < currentEpochSeconds ->
            sinkMetrics (x + 1UL, currentEpochSeconds)
        | _ -> 
            ()

    let internal MakeKnown
            (context  : CommonContext)
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


