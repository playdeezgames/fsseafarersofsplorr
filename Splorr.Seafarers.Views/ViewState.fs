namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models

type ViewState = 
    | AtSea of World
    | ConfirmQuit of ViewState
    | Help of ViewState
