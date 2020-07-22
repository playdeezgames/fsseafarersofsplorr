namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module IslandItem =
    let rec private ReadEntities (reader:SQLiteDataReader) (items:Set<uint64>) : Result<Set<uint64>, string> =
        if reader.Read() then
            items
            |> Set.add (reader.GetInt64(0) |> uint64)
            |> ReadEntities reader
        else
            items
            |> Ok

    let GetForIsland (connection:SQLiteConnection) (location:Location) : Result<Set<uint64>,string> =
        let commandSideEffect (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
        let convertor (reader:SQLiteDataReader) =
            (reader.GetInt64(0) |> uint64)
        connection
        |> Utility.GetList "SELECT [ItemId] FROM [IslandItems] WHERE [IslandX]= $islandX AND [IslandY]= $islandY;" commandSideEffect convertor
        |> Result.bind
            (Set.ofList >> Ok)

    let ExistForIsland (connection:SQLiteConnection) (location:Location) : Result<bool, string> =
        try
            use command : SQLiteCommand = new SQLiteCommand("SELECT COUNT(1) FROM [IslandItems] WHERE [IslandX]= $islandX AND [IslandY]= $islandY;", connection)
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            (command.ExecuteScalar() :?> int64) > 0L
            |> Ok
        with
        | ex -> 
            ex.ToString() 
            |> Error

    let CreateForIsland (connection:SQLiteConnection) (location:Location) (items: Set<uint64>) : Result<unit, string> =
        try
            use command : SQLiteCommand = new SQLiteCommand("DELETE FROM [IslandItems] WHERE [IslandX]= $islandX AND [IslandY]= $islandY;", connection)
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            command.ExecuteNonQuery() |> ignore
            items
            |> Set.iter
                (fun itemId ->
                    use command : SQLiteCommand = new SQLiteCommand("REPLACE INTO [IslandItems] ([IslandX],[IslandY],[ItemId]) VALUES ($islandX, $islandY, $itemId);", connection)
                    command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
                    command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
                    command.Parameters.AddWithValue("$itemId", itemId) |> ignore
                    command.ExecuteNonQuery() |> ignore)
            () |> Ok
        with
        | ex ->
            ex.ToString()
            |> Error
