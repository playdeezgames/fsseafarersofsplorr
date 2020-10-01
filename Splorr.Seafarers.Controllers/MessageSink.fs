namespace Splorr.Seafarers.Controllers

open Tarot

type Hue =
    | Error      =  1
    | Flavor     =  2
    | Heading    =  3
    | Label      =  4
    | Subheading =  5
    | Sublabel   =  6 
    | Usage      =  7
    | Value      =  8
    | Warning    =  9
    | Cards      = 10

type Message =
    | Group of Message list
    | Hued  of Hue * Message
    | Line  of string
    | Text  of string
    | Cards of Card list

type MessageSink = 
    Message -> unit
