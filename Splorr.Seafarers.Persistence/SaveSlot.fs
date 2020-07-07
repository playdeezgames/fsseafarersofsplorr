namespace Splorr.Seafarers.Persistence

open System.Data.SQLite

//type SaveSlot =
//    {
//        SaveSlotId: int
//        SaveSlotName: string
//        SaveSlotTimestamp: System.DateTimeOffset
//    }
module SaveSlot =
    let private createTableCommand =
        "CREATE TABLE  [SaveSlots] (
	        [SaveSlotId]	INTEGER PRIMARY KEY AUTOINCREMENT,
	        [SaveSlotName]	TEXT NOT NULL,
	        [SaveSlotTimestamp]	TEXT NOT NULL
        );"
    let private EnsureSaveSlotTableExists (connection:SQLiteConnection) : unit =
        use command = new SQLiteCommand(createTableCommand, connection)
        command.ExecuteNonQuery() |> ignore

    let Create (connection:SQLiteConnection) (name:string) : Result<int,exn> =
        try
            connection
            |> EnsureSaveSlotTableExists

            use command = new SQLiteCommand("INSERT INTO [SaveSlots]([SaveSlotName],[SaveSlotTimestamp]) VALUES($SaveSlotName,$SaveSlotTimestamp)", connection)
            command.Parameters.AddWithValue("$SaveSlotName", name) |> ignore
            command.Parameters.AddWithValue("$SaveSlotTimestamp", System.DateTimeOffset.Now) |> ignore
            command.ExecuteNonQuery() |> ignore

            use command = new SQLiteCommand("SELECT last_insert_rowid();", connection)
            let result = command.ExecuteScalar() 
            result :?> int64 |> int32 |> Ok        
        with
        | ex -> ex |> Error
