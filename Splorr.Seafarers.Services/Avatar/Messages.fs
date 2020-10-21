namespace Splorr.Seafarers.Services
open System
open Splorr.Common

module AvatarMessages =
    type AvatarMessageSink = (string * string) -> unit
    type AvatarMessageSource = string -> string list
    
    type AddContext =
        abstract member avatarMessageSink : AvatarMessageSink ref
    let internal AddMessage
            (context : CommonContext)
            (avatarId : string)
            (message: string)
            : unit =
        (context :?> AddContext).avatarMessageSink.Value (avatarId, message)

    let internal Add
            (context : CommonContext)
            (messages : string list) 
            (avatarId : string) 
            : unit =
        messages
        |> List.iter (AddMessage context avatarId)

    type GetContext = 
        abstract member avatarMessageSource : AvatarMessageSource ref
    let internal Get
            (context : CommonContext)
            (avatarId: string)
            : string list =
        (context :?> GetContext).avatarMessageSource.Value avatarId
