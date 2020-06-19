namespace Splorr.Seafarers.Views

module ConfirmQuit = 
    let Run (source:CommandSource) (sink:MessageSink) (state:ViewState) : ViewState option =
        "" |> sink
        "Are you sure you want to quit?" |> sink
        match source() with
        | Some Yes -> None
        | Some No -> state |> Some
        | _ -> state |> ConfirmQuit |> Some

