﻿namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models

type SetCommand =
    | Speed of float
    | Heading of Dms

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

type CommandSource = unit -> (Command option)

type MessageSink = string -> unit
