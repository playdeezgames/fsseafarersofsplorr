namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

type DockedState =
    | Dock
    | Jobs
    | ItemList
    | PriceList
    | Shop

[<RequireQualifiedAccess>]
type Gamestate = 
    | AtSea of World
    | ConfirmQuit of Gamestate
    | Docked of DockedState * Location *  World
    | GameOver of string list
    | Help of Gamestate
    | Inventory of Gamestate
    | IslandList of uint32 * Gamestate
    | MainMenu of World option
    | Status of Gamestate
    | SaveGame of string * World

module Gamestate =
    let rec GetWorld (gamestate:Gamestate) : World option =
        match gamestate with
        | Gamestate.AtSea w -> w |> Some
        | Gamestate.ConfirmQuit g -> GetWorld g
        | Gamestate.Docked (_,_,w) -> w |> Some
        | Gamestate.Help g -> GetWorld g
        | Gamestate.Inventory g -> GetWorld g
        | Gamestate.IslandList (_,g) -> GetWorld g
        | Gamestate.MainMenu w -> w
        | Gamestate.Status g -> GetWorld g
        | Gamestate.SaveGame (_,w) -> w |> Some
        | _ -> None

