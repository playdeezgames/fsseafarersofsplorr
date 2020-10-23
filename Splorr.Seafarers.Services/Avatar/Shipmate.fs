namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models
open Splorr.Common


module AvatarShipmates =
    type AvatarShipmateSource = string -> ShipmateIdentifier list

    type GetShipmatesContext =
        abstract member avatarShipmateSource : AvatarShipmateSource ref
    let internal GetShipmates
            (context : CommonContext)
            (avatarId : string)
            : ShipmateIdentifier list =
        (context :?> GetShipmatesContext).avatarShipmateSource.Value avatarId

    let internal Transform 
            (context   : CommonContext)
            (transform : ShipmateIdentifier -> unit) 
            (avatarId  : string) 
            : unit =
        avatarId
        |> GetShipmates context
        |> List.iter transform

    let internal IncrementTurn
            (context : CommonContext)
            (avatarId : string)
            : unit =
        Transform 
            context
            (ShipmateStatistic.Transform
                context
                ShipmateStatisticIdentifier.Turn 
                (Statistic.ChangeCurrentBy 1.0 >> Some)
                avatarId)
            avatarId


    let private SetPrimaryStatistic
            (context    : CommonContext)
            (identifier : ShipmateStatisticIdentifier)
            (amount     : float) 
            (avatarId   : string)
            : unit =
        ShipmateStatistic.Transform 
            context
            identifier 
            (Statistic.SetCurrentValue amount >> Some) 
            avatarId
            Primary

    let internal SetMoney (context : CommonContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Money 

    let internal SetReputation (context : CommonContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation 

    let private GetPrimaryStatistic 
            (context : CommonContext)
            (identifier : ShipmateStatisticIdentifier) 
            (avatarId     : string) 
            : float =
        ShipmateStatistic.Get
            context
            avatarId 
            Primary 
            identifier
        |> Option.map (fun statistic -> statistic.CurrentValue)
        |> Option.defaultValue 0.0

    let internal GetMoney (context:CommonContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Money

    let internal GetReputation (context:CommonContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation

    let internal EarnMoney 
            (context : CommonContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) + amount)
                avatarId

    let internal SpendMoney 
            (context : CommonContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) - amount)
                avatarId
