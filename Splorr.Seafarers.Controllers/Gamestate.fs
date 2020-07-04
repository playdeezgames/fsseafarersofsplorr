namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

type DockedState =
    | Dock
    | Jobs
    | Shop
    | PriceList
    | ItemList

[<RequireQualifiedAccess>]
type Gamestate = 
    | AtSea of World
    | ConfirmQuit of Gamestate
    | Docked of DockedState * Location *  World
    | Help of Gamestate
    | IslandList of uint32 * Gamestate
    | MainMenu of World option
    | Status of Gamestate
    | Inventory of Gamestate

module Gamestate =
    let rec GetWorld (gamestate:Gamestate) : World option =
        match gamestate with
        | Gamestate.AtSea w -> w |> Some
        | Gamestate.Docked (_,_,w) -> w |> Some
        | Gamestate.MainMenu w -> w
        | Gamestate.ConfirmQuit g -> GetWorld g
        | Gamestate.Help g -> GetWorld g
        | Gamestate.IslandList (_,g) -> GetWorld g
        | Gamestate.Status g -> GetWorld g
        | Gamestate.Inventory g -> GetWorld g
        //| _ -> raise (System.NotImplementedException "Not Implemented")

