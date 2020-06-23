namespace Splorr.Seafarers.Controllers

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
    | Dock
    | Undock
    | Islands of uint32
    | HeadFor of string
    | Status

type CommandSource = unit -> (Command option)

type MessageSink = string -> unit
