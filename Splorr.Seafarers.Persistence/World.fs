namespace Splorr.Seafarers.Persistence

module World =

    open System.Data.SQLite
    open Splorr.Seafarers.Models

    let private createTableCommand =
        "CREATE TABLE IF NOT EXISTS [Worlds] (
            [WorldId] INTEGER PRIMARY KEY AUTOINCREMENT,
            [WorldName] TEXT NOT NULL,
            [RewardMinimum] REAL NOT NULL,
            [RewardMaximum] REAL NOT NULL,
            [Timestamp] TEXT NOT NULL
        );"
    let private EnsureWorldTableExists (connection:SQLiteConnection) : unit =
        use command = new SQLiteCommand(createTableCommand, connection)
        command.ExecuteNonQuery() |> ignore

    let Save (connection:SQLiteConnection) (name:string) (world:World) : Result<int, exn> =
        try
            connection
            |>  EnsureWorldTableExists 

            use command = new SQLiteCommand("INSERT INTO [Worlds]([WorldName],[RewardMinimum],[RewardMaximum],[Timestamp]) VALUES($WorldName,$RewardMinimum,$RewardMaximum,$Timestamp)", connection)
            command.Parameters.AddWithValue("$WorldName",name) |> ignore
            command.Parameters.AddWithValue("$RewardMinimum",world.RewardRange |> fst) |> ignore
            command.Parameters.AddWithValue("$RewardMaximum",world.RewardRange |> snd) |> ignore
            command.Parameters.AddWithValue("$Timestamp",System.DateTimeOffset.Now) |> ignore
            command.ExecuteNonQuery() |> ignore

            use command = new SQLiteCommand("SELECT last_insert_rowid();",connection)
            command.ExecuteScalar() :?> int64 |> int32 |> Ok
        with
        | ex -> ex |> Error
