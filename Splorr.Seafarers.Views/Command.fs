namespace Splorr.Seafarers.Views

type SetCommand =
    | Speed of float

type Command =
    | Quit
    | Yes
    | No
    | Set of SetCommand

type CommandSource = unit -> (Command option)

type MessageSink = string -> unit
