namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

[<RequireQualifiedAccess>]
type Gamestate = 
    | AtSea of World
    | Docked of Location *  World
    | ConfirmQuit of Gamestate
    | Help of Gamestate
    | MainMenu of World option
    | IslandList of uint32 * Gamestate
    | Status of Gamestate
    | Jobs of Location * World

module Gamestate =
    let rec GetWorld (gamestate:Gamestate) : World option =
        match gamestate with
        | Gamestate.AtSea w -> w |> Some
        | Gamestate.Docked (_,w) -> w |> Some
        | Gamestate.MainMenu w -> w
        | Gamestate.ConfirmQuit g -> GetWorld g
        | Gamestate.Help g -> GetWorld g
        | Gamestate.IslandList (_,g) -> GetWorld g
        | Gamestate.Status g -> GetWorld g
        | Gamestate.Jobs (_,w) -> w |> Some
        //| _ -> raise (System.NotImplementedException "Not Implemented")

