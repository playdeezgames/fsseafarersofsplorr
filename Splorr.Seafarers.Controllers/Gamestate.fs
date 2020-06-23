namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

type Gamestate = 
    | AtSea of World
    | Docked of Location *  World
    | ConfirmQuit of Gamestate
    | Help of Gamestate
    | MainMenu of World option
    | IslandList of uint32 * Gamestate

module Gamestate =
    let rec GetWorld (gamestate:Gamestate) : World option =
        match gamestate with
        | AtSea w -> w |> Some
        | Docked (_,w) -> w |> Some
        | MainMenu w -> w
        | ConfirmQuit g -> GetWorld g
        | Help g -> GetWorld g
        | IslandList (_,g) -> GetWorld g
