namespace Splorr.Seafarers.Persistence

open System.Data.SQLite

module Utility =
    let rec private readEntities 
            (convertor : SQLiteDataReader -> 'T) 
            (reader    : SQLiteDataReader) 
            (previous  : 'T list) 
            : Result<'T list, string> =
        if reader.Read() then
            let next = 
                [
                    reader |> convertor
                ] 
                |> List.append previous
            readEntities convertor reader next
        else    
            previous
            |> Ok

    let GetList 
            (commandText       : string) 
            (commandSideEffect : SQLiteCommand -> unit) 
            (convertor         : SQLiteDataReader -> 'T) 
            (connection        : SQLiteConnection) 
            : Result<'T list, string> =
        try
            use command = new SQLiteCommand(commandText, connection)
            commandSideEffect command
            readEntities convertor (command.ExecuteReader()) []
        with
        | ex ->
            ex.ToString() 
            |> Error
    

