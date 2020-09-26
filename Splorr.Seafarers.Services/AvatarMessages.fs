namespace Splorr.Seafarers.Services
open System

type AvatarMessageSink = string -> string -> unit
type AvatarMessageSource = string -> string list

module AvatarMessages =
    type AddContext =
        inherit ServiceContext
        abstract member avatarMessageSink : AvatarMessageSink
    let Add
            (context : ServiceContext)
            (messages : string list) 
            (avatarId : string) 
            : unit =
        let context = context :?> AddContext
        messages
        |> List.iter (context.avatarMessageSink avatarId)

    type GetContext = 
        inherit ServiceContext
        abstract member avatarMessageSource : AvatarMessageSource
    let Get
            (context : ServiceContext)
            (avatarId: string)
            : string list =
        (context :?> GetContext).avatarMessageSource avatarId
