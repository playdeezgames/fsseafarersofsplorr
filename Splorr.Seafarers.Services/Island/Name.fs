namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common


module IslandName = 
    type IslandSingleNameSource = Location -> string option

    type GetNameContext =
        abstract member islandSingleNameSource : IslandSingleNameSource ref
    let internal GetName
            (context : CommonContext)
            (location : Location) 
            : string option =
        (context :?> GetNameContext).islandSingleNameSource.Value location

    let internal GetDisplayName 
            (context  : CommonContext)
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

