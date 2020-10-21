namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module AvatarIslandMetric =

    type AvatarIslandSingleMetricSource = (string * Location * AvatarIslandMetricIdentifier) -> uint64 option
    type GetContext =
        abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource ref
    let internal Get
            (context : CommonContext)
            (avatarId: string)
            (location: Location)
            (identifier: AvatarIslandMetricIdentifier)
            : uint64 option =
        (context :?> GetContext).avatarIslandSingleMetricSource.Value (avatarId, location, identifier)

    type AvatarIslandSingleMetricSink = (string * Location * AvatarIslandMetricIdentifier * uint64) -> unit
    type PutContext =
        abstract member avatarIslandSingleMetricSink:AvatarIslandSingleMetricSink ref
    let internal Put
            (context : CommonContext)
            (avatarId : string)
            (location : Location)
            (identifier: AvatarIslandMetricIdentifier)
            (value : uint64) 
            : unit =
        (context :?> PutContext).avatarIslandSingleMetricSink.Value (avatarId, location, identifier, value)
            

