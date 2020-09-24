namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type GamestateCheckForAvatarDeathContext =
    inherit ServiceContext
    abstract member avatarMessageSource           : AvatarMessageSource


[<RequireQualifiedAccess>]
type Gamestate = 
    | InPlay       of string
    | Careened     of Side * string //persist to db
    | Chart        of string * string
    | ConfirmQuit  of Gamestate
    | ErrorMessage of string * Gamestate 
    | GameOver     of string list
    | Help         of Gamestate
    | Inventory    of Gamestate
    | IslandList   of uint32 * Gamestate
    | ItemList     of Gamestate
    | Jobs         of Gamestate
    | MainMenu     of string option
    | Metrics      of Gamestate
    | Status       of Gamestate

module Gamestate =
    let rec GetWorld 
            (gamestate:Gamestate) 
            : string option =
        match gamestate with
        | Gamestate.InPlay (w)         -> w |> Some
        | Gamestate.Careened (_, w)    -> w |> Some
        | Gamestate.Chart (_,w)        -> w |> Some
        | Gamestate.ConfirmQuit g      -> GetWorld g
        | Gamestate.ErrorMessage (_,g) -> GetWorld g
        | Gamestate.Help g             -> GetWorld g
        | Gamestate.Inventory g        -> GetWorld g
        | Gamestate.IslandList (_,g)   -> GetWorld g
        | Gamestate.ItemList g         -> GetWorld g
        | Gamestate.Jobs g             -> GetWorld g
        | Gamestate.MainMenu w         -> w
        | Gamestate.Metrics g          -> GetWorld g
        | Gamestate.Status g           -> GetWorld g
        | _ -> None

    let CheckForAvatarDeath 
            (context : ServiceContext)
            (gamestate                     : Gamestate option) 
            : Gamestate option =
        let context = context :?> GamestateCheckForAvatarDeathContext
        gamestate
        |> Option.bind
            (GetWorld)
        |> Option.fold 
            (fun g w -> 
                if w |> World.IsAvatarAlive context then
                    g
                else
                    w
                    |> context.avatarMessageSource
                    |> Gamestate.GameOver
                    |> Some) gamestate
