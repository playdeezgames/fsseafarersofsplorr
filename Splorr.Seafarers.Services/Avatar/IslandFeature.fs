namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models
open Splorr.Common

module AvatarIslandFeature =
    
    
    type AvatarIslandFeatureSink = AvatarIslandFeature option * string -> unit
    type SetFeatureContext =
        abstract member avatarIslandFeatureSink : AvatarIslandFeatureSink ref
    let internal SetFeature
            (context : CommonContext)
            (feature : AvatarIslandFeature option, avatarId : string)
            : unit =
        (context :?> SetFeatureContext).avatarIslandFeatureSink.Value (feature, avatarId)

    let internal Enter
            (context  : CommonContext)
            (avatarId : string)
            (location : Location)
            (feature  : IslandFeatureIdentifier)
            : unit =
        if Island.HasFeature context feature location then
            SetFeature context 
                ({featureId = feature; location = location} |> Some, 
                    avatarId)

    type AvatarIslandFeatureSource = string -> AvatarIslandFeature option
    type GetFeatureContext =
        abstract member avatarIslandFeatureSource : AvatarIslandFeatureSource ref
    let internal Get
            (context : CommonContext)
            (avatarId: string)
            : AvatarIslandFeature option =
        (context :?> GetFeatureContext).avatarIslandFeatureSource.Value avatarId

