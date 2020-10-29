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

    let internal GetDisplayNameWhenIslandExists 
            (context  : CommonContext)
            (avatarId : string) 
            (location : Location)
            (islandName : string)
            : string =
        match AvatarIslandMetric.Get context avatarId location AvatarIslandMetricIdentifier.VisitCount with
        | Some _ ->
            islandName
        | None ->
            "(unknown)"

    let internal GetDisplayName 
            (context  : CommonContext)
            (avatarId : string) 
            (location : Location)
            : string =
        match GetName context location with
        | Some islandName ->
            GetDisplayNameWhenIslandExists context avatarId location islandName
        | None ->
            raise (NotImplementedException "This island does not exist!")

