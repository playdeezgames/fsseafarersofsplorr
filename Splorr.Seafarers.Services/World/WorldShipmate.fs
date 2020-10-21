namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldShipmate =
    let internal IsAvatarAlive
            (context  : CommonContext)
            (avatarId : string) 
            : bool =
        (Shipmate.GetStatus 
            context
            avatarId
            Primary) = Alive


