namespace Splorr.Seafarers.Controllers

type Hue =
    | Heading
    | Subheading
    | Label
    | Sublabel
    | Value
    | Flavor
    | Usage
    | Error
    | Warning

type Message =
    | Line of string
    | Hued of Hue * Message
    | Text of string
    | Group of Message list

type MessageSink = Message -> unit
