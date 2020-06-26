﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

type SetCommand =
    | Speed of float
    | Heading of Dms

[<RequireQualifiedAccess>]
type Command =
    | Quit
    | Yes
    | No
    | Set of SetCommand
    | Move
    | Help
    | Start
    | Menu
    | Resume
    | Abandon
    | Dock
    | Undock
    | Islands of uint32
    | HeadFor of string
    | Status
    | Jobs
    | AcceptJob of uint32

type CommandSource = unit -> (Command option)

type MessageSink = string -> unit
