namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldShipmate =
    let private GetStatus
            (context    : CommonContext)
            (avatarId   : string)
            (shipmateId : ShipmateIdentifier)
            : bool =
        let health = ShipmateStatistic.GetHealth context avatarId shipmateId
        let turn = ShipmateStatistic.GetTurn context avatarId shipmateId
        (health.CurrentValue > health.MinimumValue) && (turn.CurrentValue < turn.MaximumValue)

    let internal IsAvatarAlive
            (context  : CommonContext)
            (avatarId : string) 
            : bool =
        GetStatus 
            context
            avatarId
            Primary


