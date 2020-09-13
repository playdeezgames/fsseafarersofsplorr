namespace Splorr.Seafarers.Persistence

open System.Data.SQLite

module RationItem =
    let rec private getRationItems 
            (reader   : SQLiteDataReader) 
            (previous : uint64 list) 
            : uint64 list =
        if reader.Read() then
            [(reader.GetInt64(0) |> uint64)]
            |> List.append previous
            |> getRationItems reader
        else
            previous

    let GetRationItems
            (connection : SQLiteConnection) 
            : Result<uint64 list, string> =
        try
            use command = new SQLiteCommand("SELECT [ItemId] FROM [RationItems] ORDER BY [DefaultOrder];", connection)
            []
            |> getRationItems (command.ExecuteReader())
            |> Ok
        with
        | ex -> Error (ex.ToString())


