namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type AvatarIslandFeatureSink = AvatarIslandFeature option * string -> unit
type AvatarIslandFeatureSource = string -> AvatarIslandFeature option

module AvatarIslandFeature =
    type EnterContext =
        inherit ServiceContext
        abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink
        abstract member islandSingleFeatureSource : IslandSingleFeatureSource
    let Enter
            (context  : ServiceContext)
            (avatarId : string)
            (location : Location)
            (feature  : IslandFeatureIdentifier)
            : unit =
        let context = context :?> EnterContext
        if context.islandSingleFeatureSource location feature then
            context.avatarIslandFeatureSink 
                ({featureId = feature; location = location} |> Some, 
                    avatarId)

    type GetContext =
        inherit ServiceContext
        abstract member avatarIslandFeatureSource : AvatarIslandFeatureSource
    let Get
            (context : ServiceContext)
            (avatarId: string)
            : AvatarIslandFeature option =
        let context = context :?> GetContext
        context.avatarIslandFeatureSource avatarId

