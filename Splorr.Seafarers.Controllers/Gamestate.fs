namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

(*
type Suit =
    | Wands
    | Cups
    | Swords
    | Pentacles

type Rank =
    | Ace
    | Deuce
    | Three
    | Four
    | Five
    | Six
    | Seven
    | Eight
    | Nine
    | Ten
    | Page
    | Knight
    | Queen
    | King

type Arcana =
    | Fool
    | Magician
    | HighPriestess
    | Empress
    | Emperor
    | Hierophant
    | Lovers
    | Chariot
    | Strength
    | Hermit
    | WheelOfFortune
    | Justice
    | HangedMan
    | Death
    | Temperance
    | Devil
    | Tower
    | Star
    | Moon
    | Sun
    | Judgement
    | World

type Card = 
    | Minor of Suit * Rank
    | Major of Arcana

type Hand = Card * Card * Card

type GamblingState = Card * Card * Card
*)

type DockedState =
    | Jobs //view state
    | Feature of IslandFeatureIdentifier //game state
    //|Gambling of GamblingState

type AvatarMessageSource = string -> string list

[<RequireQualifiedAccess>]
type Gamestate = 
    | AtSea        of string
    | Careened     of Side * string //persist to db
    | Chart        of string * string
    | ConfirmQuit  of Gamestate
    | Docked       of DockedState * Location *  string
    | ErrorMessage of string * Gamestate 
    | GameOver     of string list
    | Help         of Gamestate
    | Inventory    of Gamestate
    | IslandList   of uint32 * Gamestate
    | ItemList     of Gamestate
    | MainMenu     of string option
    | Metrics      of Gamestate
    | Status       of Gamestate

module Gamestate =
    let rec GetWorld 
            (gamestate:Gamestate) 
            : string option =
        match gamestate with
        | Gamestate.AtSea w            -> w |> Some
        | Gamestate.Careened (_, w)    -> w |> Some
        | Gamestate.Chart (_,w)        -> w |> Some
        | Gamestate.ConfirmQuit g      -> GetWorld g
        | Gamestate.Docked (_,_,w)     -> w |> Some
        | Gamestate.ErrorMessage (_,g) -> GetWorld g
        | Gamestate.Help g             -> GetWorld g
        | Gamestate.Inventory g        -> GetWorld g
        | Gamestate.IslandList (_,g)   -> GetWorld g
        | Gamestate.ItemList g         -> GetWorld g
        | Gamestate.MainMenu w         -> w
        | Gamestate.Metrics g          -> GetWorld g
        | Gamestate.Status g           -> GetWorld g
        | _ -> None

    let CheckForAvatarDeath 
            (avatarMessageSource           : AvatarMessageSource)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (gamestate                     : Gamestate option) 
            : Gamestate option =
        gamestate
        |> Option.bind
            (GetWorld)
        |> Option.fold 
            (fun g w -> 
                if w |> World.IsAvatarAlive shipmateSingleStatisticSource then
                    g
                else
                    w
                    |> avatarMessageSource
                    |> Gamestate.GameOver
                    |> Some) gamestate
