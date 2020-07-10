namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module Commodity =
    let private createTableCommand =
        "CREATE TABLE IF NOT EXISTS [Commodities] (
        	[WorldId]	INTEGER NOT NULL,
        	[CommodityId]	INTEGER NOT NULL,
        	[CommodityName]	TEXT NOT NULL,
        	[BasePrice]	REAL NOT NULL,
        	[PurchaseFactor]	REAL NOT NULL,
        	[SaleFactor]	REAL NOT NULL,
        	[Discount]	REAL NOT NULL,
        	[Occurrence]	REAL NOT NULL,
        	PRIMARY KEY([CommodityId],[WorldId])
        );"
    let private EnsureTableExists (connection:SQLiteConnection) : unit =
        use command = new SQLiteCommand(createTableCommand, connection)
        command.ExecuteNonQuery() |> ignore

    let private createRecordCommand =
        "INSERT INTO [Commodities]
            ([WorldId], 
            [CommodityId], 
            [CommodityName], 
            [BasePrice], 
            [PurchaseFactor], 
            [SaleFactor], 
            [Discount], 
            [Occurrence]) 
        VALUES 
            ($WorldId, 
            $CommodityId, 
            $CommodityName, 
            $BasePrice, 
            $PurchaseFactor, 
            $SaleFactor, 
            $Discount, 
            $Occurrence);"

    let Save (connection:SQLiteConnection) (worldId:int) (commodities:Map<Commodity, CommodityDescriptor>): Result<int, exn> =
        try
            connection
            |>  EnsureTableExists 

            commodities
            |> Map.iter (fun commodity descriptor ->
                use command = new SQLiteCommand(createRecordCommand,connection)
                command.Parameters.AddWithValue("$WorldId", worldId) |> ignore
                command.Parameters.AddWithValue("$CommodityId", commodity) |> ignore
                command.Parameters.AddWithValue("$CommodityName", descriptor.Name) |> ignore
                command.Parameters.AddWithValue("$BasePrice", descriptor.BasePrice) |> ignore
                command.Parameters.AddWithValue("$PurchaseFactor", descriptor.PurchaseFactor) |> ignore
                command.Parameters.AddWithValue("$SaleFactor", descriptor.SaleFactor) |> ignore
                command.Parameters.AddWithValue("$Discount", descriptor.Discount) |> ignore
                command.Parameters.AddWithValue("$Occurrence", descriptor.Occurrence) |> ignore
                command.ExecuteNonQuery() |> ignore)
            worldId |> Ok
        with
        | ex -> ex |> Error


