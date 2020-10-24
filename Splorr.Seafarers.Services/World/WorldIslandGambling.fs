namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module WorldIslandGambling =
    let internal HasDarkAlleyMinimumStakes
            (context : CommonContext)
            (avatarId : string)
            : bool =
        AvatarIslandFeature.IsAvatarAtFeature context IslandFeatureIdentifier.DarkAlley avatarId
        |> Option.fold
            (fun _ location ->
                let minimumBet = 
                    Island.GetMinimumGamblingStakes
                        context
                        location
                let money =
                    AvatarShipmates.GetMoney
                        context
                        avatarId
                money >= minimumBet) false

    let internal FoldGamblingHand
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        avatarId
        |> AvatarMessages.Add 
            context 
            ["Doesn't seem like a good wager to you!"] 
        avatarId
        |> AvatarGamblingHand.Fold
            context 

    let private NoGamblingHandToBetOn
            (context : CommonContext)
            (avatarId : string)
            : bool =
        AvatarMessages.Add context ["You aren't currently gambling!"] avatarId
        false

    let private NotGambling
            (context : CommonContext)
            (avatarId : string)
            : bool =
        AvatarMessages.Add context ["You aren't currently gambling!"] avatarId
        false

    let private InsufficientFundsForMinimumWager
            (context: CommonContext)
            (minimumWager: float)
            (avatarId: string)
            : bool =
        AvatarMessages.Add 
            context 
            [sprintf "Minimum stakes are %0.2f!" minimumWager] 
            avatarId
        false

    let private InsufficientFundsForWager
            (context : CommonContext)
            (avatarId : string)
            : bool =
        AvatarMessages.Add 
            context 
            [ sprintf "You cannot bet more than you have!" ] 
            avatarId
        false

    let private WinningHand
            (context : CommonContext)
            (amount: float)
            (avatarId: string)
            : bool =
        AvatarMessages.Add 
            context 
            [ sprintf "You win!" ] 
            avatarId
        AvatarShipmates.EarnMoney 
            context 
            amount 
            avatarId
        AvatarGamblingHand.Fold
            context
            avatarId
        true

    let private LosingHand
            (context : CommonContext)
            (amount : float)
            (avatarId : string)
            : bool =
        AvatarMessages.Add 
            context 
            [ sprintf "You lose!" ] 
            avatarId
        AvatarShipmates.SpendMoney 
            context 
            amount 
            avatarId
        AvatarGamblingHand.Fold
            context
            avatarId
        true

    let private InnerBetOnGamblingSufficientFunds
            (context  :CommonContext)
            (amount : float)
            (avatarId : string) 
            (hand: AvatarGamblingHand)
            : bool =
        if AvatarGamblingHand.IsWinner hand then
            WinningHand context amount avatarId
        else
            LosingHand context amount avatarId


    let private InnerBetOnGamblingAboveMinimumWager
            (context  :CommonContext)
            (amount : float)
            (avatarId : string) 
            (hand: AvatarGamblingHand)
            : bool =
        let money =
            AvatarShipmates.GetMoney 
                context 
                avatarId
        if money < amount then
            InsufficientFundsForWager context avatarId
        else
            InnerBetOnGamblingSufficientFunds context amount avatarId hand

    let private InnerBetOnGamblingInDarkAlley
            (context  :CommonContext)
            (amount : float)
            (avatarId : string) 
            (hand: AvatarGamblingHand)
            (feature : AvatarIslandFeature)
            : bool =
        let minimumWager = 
            Island.GetMinimumGamblingStakes
                context
                feature.location
        if amount<minimumWager then
            InsufficientFundsForMinimumWager context minimumWager avatarId
        else
            InnerBetOnGamblingAboveMinimumWager context amount avatarId hand 

    let private InnerBetOnGamblingHand
            (context  :CommonContext)
            (amount : float)
            (avatarId : string) 
            (feature : AvatarIslandFeature)
            : bool =
        match AvatarGamblingHand.Get context avatarId with
        | None ->
            NoGamblingHandToBetOn context avatarId
        | Some hand ->
            InnerBetOnGamblingInDarkAlley context amount avatarId hand feature

    let internal BetOnGamblingHand
            (context : CommonContext)
            (amount : float)
            (avatarId : string) 
            : bool =
        match AvatarIslandFeature.Get context avatarId with
        | Some feature when feature.featureId = IslandFeatureIdentifier.DarkAlley ->
            InnerBetOnGamblingHand context amount avatarId feature
        | _ -> 
            NotGambling context avatarId
