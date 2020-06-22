namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

type Gamestate = 
    | AtSea of World
    | Docked of Location *  World
    | ConfirmQuit of Gamestate
    | Help of Gamestate
    | MainMenu of World option
    | IslandList of uint32 * Gamestate
