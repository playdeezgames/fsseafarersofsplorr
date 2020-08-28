namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type DockedState =
    | Dock
    | Jobs
    | ItemList

type AvatarMessageSource = string -> string list

[<RequireQualifiedAccess>]
type Gamestate = 
    | AtSea        of string
    | Careened     of Side * string
    | Chart        of string * string
    | ConfirmQuit  of Gamestate
    | Docked       of DockedState * Location *  string
    | ErrorMessage of string * Gamestate 
    | GameOver     of string list
    | Help         of Gamestate
    | Inventory    of Gamestate
    | IslandList   of uint32 * Gamestate
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
