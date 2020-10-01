namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type AvatarSingleMetricSource = string -> Metric -> uint64
type AvatarSingleMetricSink = string -> Metric * uint64 -> unit
type AvatarMetrics = Map<Metric, uint64>
type AvatarMetricSource = string -> AvatarMetrics

module AvatarMetric = 
    type AddContext =
        inherit ServiceContext
        abstract member avatarSingleMetricSink   : AvatarSingleMetricSink
        abstract member avatarSingleMetricSource : AvatarSingleMetricSource
    let Add 
            (context  : ServiceContext)
            (metric   : Metric) 
            (amount   : uint64) 
            (avatarId : string)
            : unit =
        let context = context :?> AddContext
        context.avatarSingleMetricSink avatarId (metric, (context.avatarSingleMetricSource avatarId metric) + amount)

    type GetContext = 
        inherit ServiceContext
        abstract member avatarMetricSource : AvatarMetricSource
    let Get
            (context : ServiceContext)
            (avatarId: string)
            : AvatarMetrics =
        (context :?> GetContext).avatarMetricSource avatarId
