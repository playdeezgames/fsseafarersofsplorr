namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module Commodity =
    let rec private ReadEntities (reader:SQLiteDataReader) (previous:CommodityDescriptor list) : Map<uint64, CommodityDescriptor> =
        if reader.Read() then
            let next = 
                [{
                    CommodityId = reader.GetInt64(0) |> uint64
                    CommodityName = reader.GetString(1)
                    BasePrice = reader.GetDouble(2)
                    SaleFactor = reader.GetDouble(3)
                    PurchaseFactor = reader.GetDouble(4)
                    Discount = reader.GetDouble(5)
                }] 
                |> List.append previous
            ReadEntities reader next
        else    
            previous
            |> List.map (fun i -> (i.CommodityId, i))
            |> Map.ofList

    let GetList (connection:SQLiteConnection) : Map<uint64, CommodityDescriptor> =
        use command = new SQLiteCommand("SELECT [CommodityId], [CommodityName], [BasePrice], [SaleFactor], [PurchaseFactor], [Discount] FROM [Commodities];",connection)
        ReadEntities (command.ExecuteReader()) []

