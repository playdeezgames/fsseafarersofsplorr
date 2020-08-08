namespace Splorr.Seafarers.Persistence

open System
open System.Data.SQLite
open Splorr.Seafarers.Models

module Message =
    let private convertor (reader:SQLiteDataReader) : string =
        reader.GetString(0)

    let private commandSideEffect (avatarId:string) (command:SQLiteCommand) =
        command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore

    let GetForAvatar
            (avatarId   : string)
            (connection : SQLiteConnection)
            : Result<string list, string> =
        connection
        |> Utility.GetList 
            "SELECT [Message] FROM [Messages] WHERE [AvatarId] = $avatarId ORDER BY [MessageId];" 
            (commandSideEffect avatarId) 
            convertor

    let ClearForAvatar
            (avatarId   : string)
            (connection : SQLiteConnection)
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

    let AddForAvatar
            (avatarId   : string, 
             message    : string)
            (connection : SQLiteConnection)
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


