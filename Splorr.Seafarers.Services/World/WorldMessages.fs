namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldMessages = 
    type AvatarMessagePurger = string -> unit
    type ClearMessagesContext =
        abstract member avatarMessagePurger : AvatarMessagePurger ref
    let internal ClearMessages 
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        (context :?> ClearMessagesContext).avatarMessagePurger.Value avatarId



