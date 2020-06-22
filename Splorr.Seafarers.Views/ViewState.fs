namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models

type ViewState = 
    | AtSea of World
    | Docked of Location *  World
    | ConfirmQuit of ViewState
    | Help of ViewState
    | MainMenu of World option
