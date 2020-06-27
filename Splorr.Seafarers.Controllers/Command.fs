namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

type SetCommand =
    | Speed of float
    | Heading of Dms

type AbandonCommand =
    | Game
    | Job

[<RequireQualifiedAccess>]
type Command =
    | Quit
    | Yes
    | No
    | Set of SetCommand
    | Move of uint32
    | Help
    | Start
    | Menu
    | Resume
    | Abandon of AbandonCommand
    | Dock
    | Undock
    | Islands of uint32
    | HeadFor of string
    | Status
    | Jobs
    | AcceptJob of uint32

type CommandSource = unit -> (Command option)

type MessageSink = string -> unit
