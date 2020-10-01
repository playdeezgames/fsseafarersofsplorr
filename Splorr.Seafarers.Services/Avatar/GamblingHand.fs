namespace Splorr.Seafarers.Services
open System
open Tarot
open Splorr.Seafarers.Models

type AvatarGamblingHandSource = string -> AvatarGamblingHand option
type AvatarGamblingHandSink = string -> AvatarGamblingHand option -> unit

module AvatarGamblingHand =
    let IsWinner
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
        inherit ServiceContext
        abstract member avatarGamblingHandSource : AvatarGamblingHandSource
    let Get
            (context  : ServiceContext)
            (avatarId : string)
            : AvatarGamblingHand option =
        let context = context :?> GetContext
        context.avatarGamblingHandSource avatarId
      
    type DealContext =
        inherit ServiceContext
        abstract member avatarGamblingHandSink : AvatarGamblingHandSink
        abstract member random : Random
    let Deal
            (context  : ServiceContext)
            (avatarId : string)
            : unit =
        let context = context :?> DealContext
        match Deck.Create()
            |> List.sortBy (fun _ -> context.random.Next()) with
        | first :: second :: third :: _ ->
            (first, second, third)
            |> Some
            |> context.avatarGamblingHandSink avatarId
        | _ ->
            raise (NotImplementedException "DealGamblingHand did not have at least three cards")

    type FoldContext =
        inherit ServiceContext
        abstract member avatarGamblingHandSink : AvatarGamblingHandSink
    let Fold
            (context  : ServiceContext)
            (avatarId : string)
            : unit =
        let context = context :?> FoldContext
        context.avatarGamblingHandSink avatarId None


