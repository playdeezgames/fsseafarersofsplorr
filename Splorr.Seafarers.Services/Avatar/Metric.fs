namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models
open Splorr.Common

type AvatarMetrics = Map<Metric, uint64>

module AvatarMetric = 
    
    type AvatarSingleMetricSink = string * Metric * uint64 -> unit
    type SetMetricContext =
        abstract member avatarSingleMetricSink   : AvatarSingleMetricSink ref
    let private SetMetric
            (context : CommonContext)
            (avatarId : string)
            (metric : Metric, value : uint64)
            : unit =
        (context :?> SetMetricContext).avatarSingleMetricSink.Value (avatarId, metric, value)

    type AvatarSingleMetricSource = string * Metric -> uint64
    type GetMetricContext =
        abstract member avatarSingleMetricSource : AvatarSingleMetricSource ref
    let private GetMetric
            (context : CommonContext)
            (avatarId : string)
            (metric : Metric)
            : uint64 =
        (context :?> GetMetricContext).avatarSingleMetricSource.Value (avatarId, metric)

    let internal Add 
            (context  : CommonContext)
            (metric   : Metric) 
            (amount   : uint64) 
            (avatarId : string)
            : unit =
        (metric, (GetMetric context avatarId metric) + amount)
        |> SetMetric context avatarId 

    type AvatarMetricSource = string -> AvatarMetrics
    type GetContext = 
        abstract member avatarMetricSource : AvatarMetricSource ref
    let internal Get
            (context : CommonContext)
            (avatarId: string)
            : AvatarMetrics =
        (context :?> GetContext).avatarMetricSource.Value avatarId
