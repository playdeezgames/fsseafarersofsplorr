namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type AvatarIslandSingleMetricSource = string -> Location -> AvatarIslandMetricIdentifier -> uint64 option
type AvatarIslandSingleMetricSink = string -> Location -> AvatarIslandMetricIdentifier -> uint64 -> unit
type EpochSecondsSource = unit -> uint64

module IslandVisit =
    type AddContext =
        inherit ServiceContext
        abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
        abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
        abstract member epochSecondsSource : EpochSecondsSource
    let Add 
            (context      : ServiceContext)
            (avatarId     : string) 
            (location     : Location)
            : unit =
        let context = context :?> AddContext
        let metricSource = context.avatarIslandSingleMetricSource avatarId location
        let metricSink = context.avatarIslandSingleMetricSink avatarId location
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

    type MakeKnownContext = 
        inherit ServiceContext
        abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
        abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    let MakeKnown
            (context  : ServiceContext)
            (avatarId : string) 
            (location : Location)
            : unit =
        let context = context :?> MakeKnownContext
        let visitCount = 
            context.avatarIslandSingleMetricSource 
                avatarId 
                location 
                AvatarIslandMetricIdentifier.VisitCount
        if visitCount.IsNone then
            context.avatarIslandSingleMetricSink 
                avatarId 
                location 
                AvatarIslandMetricIdentifier.VisitCount 
                0UL


