namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type IslandSingleNameSource = Location -> string option

module IslandName = 
    type GetNameContext =
        inherit ServiceContext
        abstract member islandSingleNameSource : IslandSingleNameSource
    let GetName
            (context : ServiceContext)
            (location : Location) 
            : string option =
        (context :?> GetNameContext).islandSingleNameSource location

    let GetDisplayName 
            (context  : ServiceContext)
            (avatarId : string) 
            (location : Location)
            : string =
        let visitCount = AvatarIslandMetric.Get context avatarId location AvatarIslandMetricIdentifier.VisitCount
        let islandName = GetName context location
        match visitCount, islandName with
        | Some _, Some name ->
            name
        | None, Some _ ->
            "(unknown)"
        | _ ->
            raise (NotImplementedException "This island does not exist!")

