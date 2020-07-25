namespace Splorr.Seafarers.Controllers

type Hue =
    | Heading    = 1
    | Subheading = 2
    | Label      = 3
    | Sublabel   = 4 
    | Value      = 5
    | Flavor     = 6
    | Usage      = 7
    | Error      = 8
    | Warning    = 9

type Message =
    | Line of string
    | Hued of Hue * Message
    | Text of string
    | Group of Message list

type MessageSink = Message -> unit
