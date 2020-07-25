namespace Splorr.Seafarers.Controllers

type Hue =
    | Error      = 1
    | Flavor     = 2
    | Heading    = 3
    | Label      = 4
    | Subheading = 5
    | Sublabel   = 6 
    | Usage      = 7
    | Value      = 8
    | Warning    = 9

type Message =
    | Group of Message list
    | Hued of Hue * Message
    | Line of string
    | Text of string

type MessageSink = Message -> unit
