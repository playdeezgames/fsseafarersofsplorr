namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

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
    | Items
    | Inventory
    | Buy of TradeQuantity * string
    | Sell of TradeQuantity * string
    | Save of string

type CommandSource = unit -> (Command option)

type Hue =
    | Heading
    | Subheading
    | Label
    | Sublabel
    | Value
    | Flavor
    | Usage

type Message =
    | Line of string
    | Hued of Hue * Message
    | Text of string

type MessageSink = Message -> unit
