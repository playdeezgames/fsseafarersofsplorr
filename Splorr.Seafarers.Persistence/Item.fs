namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module Item =
    let private createItemTableCommand =
        "CREATE TABLE IF NOT EXISTS [Items] (
	        [WorldId]	INTEGER NOT NULL,
	        [ItemId]	INTEGER NOT NULL,
	        [DisplayName]	TEXT NOT NULL,
	        [Occurrence]	REAL NOT NULL,
	        PRIMARY KEY([WorldId],[ItemId])
        );"
    let private EnsureItemTableExists (connection:SQLiteConnection) : unit =
        use command = new SQLiteCommand(createItemTableCommand, connection)
        command.ExecuteNonQuery() |> ignore

    let private createCommodityItemTableCommand =
        "CREATE TABLE IF NOT EXISTS [CommodityItems] (
        	[WorldId]	INTEGER NOT NULL,
        	[ItemId]	INTEGER NOT NULL,
        	[CommodityId]	INTEGER NOT NULL,
        	[Amount]	REAL NOT NULL,
        	PRIMARY KEY([WorldId],[ItemId])
        );"
    let private EnsureCommodityItemTableExists (connection:SQLiteConnection) : unit =
        use command = new SQLiteCommand(createCommodityItemTableCommand, connection)
        command.ExecuteNonQuery() |> ignore

    let private createItemRecordCommand =
        "INSERT INTO [Items]
            ([WorldId],
            [ItemId],
            [DisplayName],
            [Occurrence])
         VALUES
            ($WorldId,
            $ItemId,
            $DisplayName,
            $Occurrence);"
    let private createCommodityItemRecordCommand =
        "INSERT INTO [CommodityItems]
            ([WorldId],
            [ItemId],
            [CommodityId],
            [Amount])
         VALUES
            ($WorldId,
            $ItemId,
            $CommodityId,
            $Amount);"

    let Save (connection:SQLiteConnection) (worldId:int) (items:Map<uint, ItemDescriptor>): Result<int, exn> =
        try
            connection
            |> EnsureItemTableExists 
            connection
            |> EnsureCommodityItemTableExists

            items
            |> Map.iter (fun item descriptor ->
                use command = new SQLiteCommand(createItemRecordCommand,connection)
                command.Parameters.AddWithValue("$WorldId", worldId) |> ignore
                command.Parameters.AddWithValue("$ItemId", item) |> ignore
                command.Parameters.AddWithValue("$DisplayName", descriptor.DisplayName) |> ignore
                command.Parameters.AddWithValue("$Occurrence", descriptor.Occurrence) |> ignore
                command.ExecuteNonQuery() |> ignore
                descriptor.Commodities
                |> Map.iter (fun commodity amount ->
                    use command = new SQLiteCommand(createCommodityItemRecordCommand,connection)
                    command.Parameters.AddWithValue("$WorldId", worldId) |> ignore
                    command.Parameters.AddWithValue("$ItemId", item) |> ignore
                    command.Parameters.AddWithValue("$CommodityId", commodity) |> ignore
                    command.Parameters.AddWithValue("$Amount", amount) |> ignore
                    command.ExecuteNonQuery() |> ignore))
            worldId |> Ok
        with
        | ex -> ex |> Error

