namespace Splorr.Seafarers.Persistence

open System.Data.SQLite

module Term =
    let GetForTermType 
            (connection:SQLiteConnection) 
            (termType:string) 
            : Result<string list, string> =
        let commandSideEffect (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$termType", termType) |> ignore
        let convertor (reader:SQLiteDataReader) =
            (reader.GetString(0))
        connection
        |> Utility.GetList "SELECT [Term] FROM [Terms] WHERE [TermType]= $termType;" commandSideEffect convertor

