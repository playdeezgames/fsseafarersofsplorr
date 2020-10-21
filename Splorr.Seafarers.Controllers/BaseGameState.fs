namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Services
open Splorr.Seafarers.Persistence
open System
open Splorr.Common

module BaseGameState =
    let internal HandleCommand
            (context  : CommonContext)
            (command  : Command option) 
            (avatarId : string) 
            : Gamestate option =
        match command with
        | Some (Command.Save filename) ->
            World.Save 
                context 
                (filename |> Option.defaultValue (Guid.NewGuid().ToString())) 
                avatarId
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some (Command.Islands page) ->
            (page, avatarId |> Gamestate.InPlay)
            |> Gamestate.IslandList
            |> Some

        | Some (Command.Abandon Job) ->
            avatarId
            |> World.AbandonJob 
                context
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some Command.Metrics ->
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.Metrics
            |> Some

        | Some Command.Quit ->
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.ConfirmQuit
            |> Some

        | Some Command.Status ->
            (avatarId)
            |> Gamestate.InPlay
            |> Gamestate.Status
            |> Some

        | Some Command.Inventory ->
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.Inventory
            |> Some

        | None ->
            ("Maybe try 'help'?",avatarId
            |> Gamestate.InPlay)
            |> Gamestate.ErrorMessage
            |> Some

        | Some Command.Help ->
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.Help
            |> Some

        | Some Command.Menu ->
            avatarId
            |> Some
            |> Gamestate.MainMenu
            |> Some

        | _ ->
            None
