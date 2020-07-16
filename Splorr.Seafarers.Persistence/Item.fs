namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module Item = 
    let rec private ReadEntities (reader:SQLiteDataReader) (previous:Map<uint64, ItemDescriptor>) : Result<Map<uint64, ItemDescriptor>,string> =
        if reader.Read() then
            let itemId = reader.GetInt64(0) |> uint64
            let item = 
                match previous |> Map.tryFind itemId with
                | Some descriptor ->
                    {descriptor with
                        Commodities = 
                            descriptor.Commodities
                            |> Map.add (reader.GetInt64(4) |> uint64) (reader.GetDouble(5))}
                | None ->
                    {
                        ItemId = itemId
                        ItemName = reader.GetString(1)
                        Occurrence = reader.GetDouble(2)
                        Tonnage = reader.GetDouble(3)
                        Commodities = 
                            Map.empty
                            |> Map.add (reader.GetInt64(4) |> uint64) (reader.GetDouble(5))
                    }
            previous
            |> Map.add itemId item
            |> ReadEntities reader
        else
            previous
            |> Ok

    let GetList (connection:SQLiteConnection) : Result<Map<uint64, ItemDescriptor>,string> =
        try
            use command = new SQLiteCommand("SELECT
        	    i.[ItemId],
        	    i.[ItemName],
        	    i.[Occurrence],
        	    i.[Tonnage],
        	    ci.[CommodityId],
        	    ci.[Quantity]
            FROM
        	    [Items] i 
        	    JOIN [CommodityItems] ci ON i.[ItemId]=ci.[ItemId]",connection)
            ReadEntities (command.ExecuteReader()) Map.empty
        with
        | ex -> ex.ToString() |> Error


