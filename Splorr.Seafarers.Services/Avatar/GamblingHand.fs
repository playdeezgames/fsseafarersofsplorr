namespace Splorr.Seafarers.Services
open System
open Tarot
open Splorr.Seafarers.Models
open Splorr.Common

module AvatarGamblingHand =
    type AvatarGamblingHandSource = string -> AvatarGamblingHand option

    let internal IsWinner
            (hand: Card * Card * Card)
            : bool =
        let first, second, final = hand
        if Card.IsFirstGreater (first, second) then
            (Card.IsFirstGreater (first, final)) && (Card.IsFirstGreater(final, second))
        elif Card.IsFirstGreater (second, first) then
            (Card.IsFirstGreater (second, final)) && (Card.IsFirstGreater(final, first))
        else
            false

    type GetContext =
        abstract member avatarGamblingHandSource : AvatarGamblingHandSource ref
    let internal Get
            (context  : CommonContext)
            (avatarId : string)
            : AvatarGamblingHand option =
        (context :?> GetContext).avatarGamblingHandSource.Value avatarId
      
    type AvatarGamblingHandSink = string * AvatarGamblingHand option -> unit
    type SetContext =
        abstract member avatarGamblingHandSink : AvatarGamblingHandSink ref
    let private Set
            (context : CommonContext)
            (avatarId : string)
            (hand : AvatarGamblingHand option)
            : unit =
        (context :?> SetContext).avatarGamblingHandSink.Value (avatarId, hand)

    let internal Deal
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        match Deck.Create()
            |> Utility.SortListRandomly context with
        | first :: second :: third :: _ ->
            (first, second, third)
            |> Some
            |> Set context avatarId
        | _ ->
            raise (NotImplementedException "DealGamblingHand did not have at least three cards")

    let internal Fold
            (context  : CommonContext)
            (avatarId : string)
            : unit =
        Set context avatarId None


