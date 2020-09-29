namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type AvatarIslandSingleMetricSource = string -> Location -> AvatarIslandMetricIdentifier -> uint64 option
type AvatarIslandSingleMetricSink = string -> Location -> AvatarIslandMetricIdentifier -> uint64 -> unit

module AvatarIslandMetric =
    type GetContext =
        inherit ServiceContext
        abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    let Get
            (context : ServiceContext)
            (avatarId: string)
            (location: Location)
            (identifier: AvatarIslandMetricIdentifier)
            : uint64 option =
        (context :?> GetContext).avatarIslandSingleMetricSource avatarId location identifier

    type PutContext =
        inherit ServiceContext
        abstract member avatarIslandSingleMetricSink:AvatarIslandSingleMetricSink
    let Put
            (context : ServiceContext)
            (avatarId : string)
            (location : Location)
            (identifier: AvatarIslandMetricIdentifier)
            (value : uint64) 
            : unit =
        (context :?> PutContext).avatarIslandSingleMetricSink avatarId location identifier value
            

