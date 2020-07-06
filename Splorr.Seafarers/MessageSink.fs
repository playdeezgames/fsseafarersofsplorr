namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers

module MessageSink =
    let rec Write (message:Message) : unit = 
        match message with
        | Hued (hue, message) ->
            let old = System.Console.ForegroundColor
            match hue with
            | Heading -> 
                System.Console.ForegroundColor <- System.ConsoleColor.Cyan
            | Subheading ->
                System.Console.ForegroundColor <- System.ConsoleColor.DarkCyan
            | Label ->
                System.Console.ForegroundColor <- System.ConsoleColor.Magenta
            | Sublabel ->
                System.Console.ForegroundColor <- System.ConsoleColor.DarkMagenta
            | Value ->
                System.Console.ForegroundColor <- System.ConsoleColor.Green
            | Flavor ->
                System.Console.ForegroundColor <- System.ConsoleColor.DarkYellow
            | Usage ->
                System.Console.ForegroundColor <- System.ConsoleColor.DarkGray
            message |> Write
            System.Console.ForegroundColor <- old

        | Line line ->
            System.Console.WriteLine line

        | Text text ->
            System.Console.Write text
            

