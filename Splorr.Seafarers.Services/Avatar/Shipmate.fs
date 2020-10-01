namespace Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type AvatarShipmateSource = string -> ShipmateIdentifier list

module AvatarShipmates =
    type TransformContext =
        inherit ServiceContext
        abstract avatarShipmateSource : AvatarShipmateSource
    let Transform 
            (context   : ServiceContext)
            (transform : ShipmateIdentifier -> unit) 
            (avatarId  : string) 
            : unit =
        let context = context :?> TransformContext
        avatarId
        |> context.avatarShipmateSource
        |> List.iter transform

    let private SetPrimaryStatistic
            (context    : ServiceContext)
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

    let SetMoney (context : ServiceContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Money 

    let SetReputation (context : ServiceContext) = SetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation 

    type GetPrimaryStatisticContext =
        inherit ServiceContext
        abstract member shipmateSingleStatisticSource : ShipmateSingleStatisticSource
    let private GetPrimaryStatistic 
            (context : ServiceContext)
            (identifier : ShipmateStatisticIdentifier) 
            (avatarId     : string) 
            : float =
        let context = context :?> GetPrimaryStatisticContext
        context.shipmateSingleStatisticSource 
            avatarId 
            Primary 
            identifier
        |> Option.map (fun statistic -> statistic.CurrentValue)
        |> Option.defaultValue 0.0

    let GetMoney (context:ServiceContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Money

    let GetReputation (context:ServiceContext) = GetPrimaryStatistic context ShipmateStatisticIdentifier.Reputation

    let EarnMoney 
            (context : ServiceContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) + amount)
                avatarId

    let SpendMoney 
            (context : ServiceContext)
            (amount                        : float) 
            (avatarId                      : string)
            : unit =
        if amount > 0.0 then
            SetMoney 
                context
                ((GetMoney context avatarId) - amount)
                avatarId
