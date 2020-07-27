namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type DockedState =
    | Dock
    | Jobs
    | ItemList

[<RequireQualifiedAccess>]
type Gamestate = 
    | AtSea        of World
    | Careened     of Side * World
    | Chart        of string * World
    | ConfirmQuit  of Gamestate
    | Docked       of DockedState * Location *  World
    | GameOver     of string list
    | Help         of Gamestate
    | InvalidInput of Gamestate
    | Inventory    of Gamestate
    | IslandList   of uint32 * Gamestate
    | MainMenu     of World option
    | Metrics      of Gamestate
    | Status       of Gamestate

module Gamestate =
    let rec GetWorld (gamestate:Gamestate) : World option =
        match gamestate with
        | Gamestate.AtSea w          -> w |> Some
        | Gamestate.Careened (_, w)  -> w |> Some
        | Gamestate.Chart (_,w)      -> w |> Some
        | Gamestate.ConfirmQuit g    -> GetWorld g
        | Gamestate.Docked (_,_,w)   -> w |> Some
        | Gamestate.Help g           -> GetWorld g
        | Gamestate.InvalidInput g   -> GetWorld g
        | Gamestate.Inventory g      -> GetWorld g
        | Gamestate.IslandList (_,g) -> GetWorld g
        | Gamestate.MainMenu w       -> w
        | Gamestate.Metrics g        -> GetWorld g
        | Gamestate.Status g         -> GetWorld g
        | _ -> None

    let CheckForAvatarDeath (gamestate:Gamestate option) : Gamestate option =
        gamestate
        |> Option.bind
            (GetWorld)
        |> Option.fold 
            (fun g w -> 
                if w |> World.IsAvatarAlive w.AvatarId then
                    g
                else
                    w.Avatars.[w.AvatarId].Messages
                    |> Gamestate.GameOver
                    |> Some) gamestate
