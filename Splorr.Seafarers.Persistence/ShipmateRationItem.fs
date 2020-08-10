namespace Splorr.Seafarers.Persistence

open System.Data.SQLite

module ShipmateRationItem =
    let GetForShipmate
            (avatarId   : string)
            (shipmateId : string)
            (connection : SQLiteConnection) 
            : Result<uint64 list, string> =
        let commandSideEffect (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.Parameters.AddWithValue("$shipmateId", shipmateId) |> ignore
        let convertor (reader:SQLiteDataReader) =
            reader.GetInt64(0) |> uint64
        connection
        |> Utility.GetList "SELECT [ItemId] FROM [ShipmateRationItems] WHERE [AvatarId]= $avatarId AND [ShipmateId]= $shipmateId ORDER BY [Order];" commandSideEffect convertor
        
    let private SetEntryForShipmate
            (avatarId    : string)
            (shipmateId  : string)
            (itemId      : uint64,
             order       : int)
            (connection  : SQLiteConnection)
            : unit =
        use command = new SQLiteCommand("REPLACE INTO [ShipmateRationItems] ([AvatarId], [ShipmateId], [ItemId], [Order]) VALUES ($avatarId, $shipmateId, $itemId, $order);", connection)
        command.Parameters.AddWithValue("$avatarId",avatarId) |> ignore
        command.Parameters.AddWithValue("$shipmateId",shipmateId) |> ignore
        command.Parameters.AddWithValue("$itemId",itemId) |> ignore
        command.Parameters.AddWithValue("$order",order) |> ignore
        command.ExecuteNonQuery() |> ignore
        ()

    let SetForShipmate
            (avatarId    : string)
            (shipmateId  : string)
            (rationItems : uint64 list)
            (connection  : SQLiteConnection)
            : Result<unit, string> =
        use command = new SQLiteCommand("DELETE FROM [ShipmateRationItems] WHERE [AvatarId]=$avatarId and [ShipmateId]=$shipmateId;", connection)
        command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
        command.Parameters.AddWithValue("$shipmateId", shipmateId) |> ignore
        command.ExecuteNonQuery() |> ignore
        [1..(rationItems.Length)]
        |> List.zip rationItems
        |> List.iter
            (fun entry -> SetEntryForShipmate avatarId shipmateId entry connection)
        () |> Ok
