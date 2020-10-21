namespace Splorr.Seafarers.Persistence

open System.Data.SQLite

module MessageLegacy =
    let private convertor (reader:SQLiteDataReader) : string =
        reader.GetString(0)

    let private commandSideEffect (avatarId:string) (command:SQLiteCommand) =
        command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore

    let internal GetForAvatar
            (connection : SQLiteConnection)
            (avatarId   : string)
            : Result<string list, string> =
        connection
        |> Utility.GetList 
            "SELECT [Message] FROM [Messages] WHERE [AvatarId] = $avatarId ORDER BY [MessageId];" 
            (commandSideEffect avatarId) 
            convertor

    let internal ClearForAvatar
            (connection : SQLiteConnection)
            (avatarId   : string)
            : Result<unit, string> =
        try
            use command = new SQLiteCommand("DELETE FROM [Messages] WHERE [AvatarId] = $avatarId;", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.ExecuteNonQuery() 
            |> ignore
            |> Ok
        with
        | ex -> 
            ex.ToString() 
            |> Error

    let internal AddForAvatar
            (connection : SQLiteConnection)
            (avatarId   : string, 
             message    : string)
            : Result<unit, string> =
        try
            use command = new SQLiteCommand("INSERT INTO [Messages] ([AvatarId], [Message]) VALUES($avatarId, $message);", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.Parameters.AddWithValue("$message", message) |> ignore
            command.ExecuteNonQuery() 
            |> ignore
            |> Ok
        with
        | ex -> 
            ex.ToString() 
            |> Error


