namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open System

module MessageSink =
    let private colorMap 
            : Map<Hue, ConsoleColor> =
        [
            Hue.Heading,    ConsoleColor.Cyan
            Hue.Subheading, ConsoleColor.DarkCyan
            Hue.Label,      ConsoleColor.Magenta
            Hue.Sublabel,   ConsoleColor.DarkMagenta
            Hue.Value,      ConsoleColor.Green
            Hue.Warning,    ConsoleColor.DarkYellow
            Hue.Flavor ,    ConsoleColor.DarkBlue
            Hue.Usage,      ConsoleColor.DarkGray
            Hue.Error,      ConsoleColor.DarkRed
        ]
        |> Map.ofList

    let rec Write 
            (message:Message) 
            : unit = 
        match message with
        | Hued (hue, message) ->
            let old = Console.ForegroundColor
            Console.ForegroundColor<-colorMap.[hue]
            message 
            |> Write
            Console.ForegroundColor <- old

        | Group messages ->
            messages
            |> List.iter Write

        | Line line ->
            Console.WriteLine line

        | Text text ->
            Console.Write text
            

