namespace Splorr.Seafarers.Persistence

open System.Data.SQLite

module AvatarInventory =
    let private convertor 
            (reader : SQLiteDataReader) 
            : uint64 * uint64 =
        (reader.GetInt64(0) |> uint64, reader.GetInt64(1) |> uint64)        

    let GetForAvatar 
            (connection : SQLiteConnection) 
            (avatarId   : string) 
            : Result<Map<uint64, uint64>, string> =
        connection
        |> Utility.GetList 
            "SELECT [ItemId], [ItemCount] FROM [AvatarInventories] WHERE [AvatarId]=$avatarId;" 
            (fun command->
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore)
            convertor
        |> Result.map
            (Map.ofList)

    let SetForAvatar
            (connection:SQLiteConnection)
            (avatarId:string)
            (inventory:Map<uint64, uint64>)
            : Result<unit, string> =
        try
            use command = new SQLiteCommand("DELETE FROM [AvatarInventories] WHERE [AvatarId] = $avatarId", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.ExecuteNonQuery() |> ignore
            inventory
            |> Map.filter 
                (fun _ itemCount -> 
                    itemCount > 0UL)
            |> Map.iter 
                (fun itemId itemCount->
                    use command = new SQLiteCommand("REPLACE INTO [AvatarInventories] ([AvatarId], [ItemId], [ItemCount]) VALUES ($avatarId, $itemId, $itemCount);", connection)
                    command.Parameters.AddWithValue("$avatarId",avatarId) |> ignore
                    command.Parameters.AddWithValue("$itemId",itemId) |> ignore
                    command.Parameters.AddWithValue("$itemCount",itemCount) |> ignore
                    command.ExecuteNonQuery() |> ignore
                    )
            () |> Ok
        with
        | ex -> 
            ex.ToString() 
            |> Error

        

