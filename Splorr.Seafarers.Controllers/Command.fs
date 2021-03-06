namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type SetCommand =
    | Heading of float
    | Speed of float

type AbandonCommand =
    | Game
    | Job

[<RequireQualifiedAccess>]
type Command =
    | Abandon of AbandonCommand
    | AcceptJob of uint32
    | Bet of float option
    | Buy of TradeQuantity * string
    | Careen of Side
    | Chart of string
    | CleanHull
    | DistanceTo of string
    | Dock
    | Gamble
    | GoTo of IslandFeatureIdentifier
    | Help
    | HeadFor of string
    | Inventory
    | Islands of uint32
    | Items
    | Jobs
    | Leave
    | Menu
    | Metrics
    | Move of uint32
    | No
    | Quit
    | Resume
    | Rules
    | Save of string option
    | Sell of TradeQuantity * string
    | Set of SetCommand
    | Start of string
    | Status
    | Undock
    | WeighAnchor
    | Yes

type CommandSource = 
    unit -> (Command option)

