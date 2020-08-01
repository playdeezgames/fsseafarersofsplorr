namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module MainMenu =
    let private UpdateDisplayHeader 
            (messageSink : MessageSink) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "Main Menu Commands:" |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private UpdateDisplayInGame 
            (messageSink : MessageSink) 
            : unit =
        UpdateDisplayHeader messageSink
        [
            (Hue.Label, "resume" |> Text) |> Hued
            (Hue.Usage, " - resume game" |> Line) |> Hued
            (Hue.Label, "abandon game" |> Text) |> Hued
            (Hue.Usage, " - abandon game" |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private UpdateDisplayNoGame 
            (messageSink : MessageSink) 
            : unit =
        UpdateDisplayHeader messageSink
        [
            (Hue.Label, "start" |> Text) |> Hued
            (Hue.Usage, " - starts a new world" |> Line) |> Hued
            (Hue.Label, "quit" |> Text) |> Hued
            (Hue.Usage, " - quits the game" |> Line) |> Hued
        ]
        |> List.iter messageSink
    
    let private UpdateDisplay 
            (messageSink : MessageSink) 
            (inGame      : bool) 
            : unit =
        if inGame then
            UpdateDisplayInGame messageSink
        else
            UpdateDisplayNoGame messageSink

    let private HandleInvalidCommand 
            (world:World option) 
            : Gamestate option =
        ("Invalid command.", world
        |> Gamestate.MainMenu)
        |> Gamestate.ErrorMessage
        |> Some

    let private HandleCommandInGame 
            (world : World) =
        function
        | Some Command.Resume ->
            world
            |> Gamestate.AtSea
            |> Some
        | Some Command.Abandon ->
            None
            |> Gamestate.MainMenu
            |> Some
        | _ ->
            world
            |> Some
            |> HandleInvalidCommand

    let private HandleCommandNoGame 
            (vesselStatisticTemplateSource: unit -> Map<VesselStatisticIdentifier, VesselStatisticTemplate>)
            (vesselStatisticSink: string -> Map<VesselStatisticIdentifier, Statistic> -> unit)
            (configuration : WorldConfiguration) =
        function
        | Some (Command.Start avatarId)->
            World.Create 
                vesselStatisticTemplateSource
                vesselStatisticSink
                configuration
                (System.Random())
                avatarId
            |> Gamestate.AtSea
            |> Some
        | Some Command.Quit ->
            None
            |> Gamestate.MainMenu
            |> Gamestate.ConfirmQuit
            |> Some
        | _ ->
            None
            |> HandleInvalidCommand

    let private HandleCommand
            (vesselStatisticTemplateSource: unit -> Map<VesselStatisticIdentifier, VesselStatisticTemplate>)
            (vesselStatisticSink: string -> Map<VesselStatisticIdentifier, Statistic> -> unit)
            (configuration : WorldConfiguration) 
            (world         : World option) 
            (command       : Command option) 
            : Gamestate option =
        match world with
        | Some w ->
            HandleCommandInGame w command
        | _ ->
            HandleCommandNoGame 
                vesselStatisticTemplateSource
                vesselStatisticSink
                configuration 
                command

    let Run 
            (vesselStatisticTemplateSource: unit -> Map<VesselStatisticIdentifier, VesselStatisticTemplate>)
            (vesselStatisticSink: string -> Map<VesselStatisticIdentifier, Statistic> -> unit)
            (configuration : WorldConfiguration) 
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (world         : World option) 
            : Gamestate option =
        UpdateDisplay 
            messageSink 
            world.IsSome
        HandleCommand
            vesselStatisticTemplateSource
            vesselStatisticSink
            configuration
            world
            (commandSource())



