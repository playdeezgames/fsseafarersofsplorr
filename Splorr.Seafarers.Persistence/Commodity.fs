namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module Commodity =
    let private convertor 
            (reader : SQLiteDataReader) 
            : CommodityDescriptor =
        {
            CommodityId    = reader.GetInt64(0) |> uint64
            CommodityName  = reader.GetString(1)
            BasePrice      = reader.GetDouble(2)
            SaleFactor     = reader.GetDouble(3)
            PurchaseFactor = reader.GetDouble(4)
            Discount       = reader.GetDouble(5)
        }

    let GetList 
            (connection : SQLiteConnection) 
            : Result<Map<uint64, CommodityDescriptor>, string> =
        connection
        |> Utility.GetList 
            "SELECT [CommodityId], [CommodityName], [BasePrice], [SaleFactor], [PurchaseFactor], [Discount] FROM [Commodities];" (fun _->()) convertor
        |> Result.map
            (fun items ->
                items
                |> List.map (fun item -> (item.CommodityId, item))
                |> Map.ofList)

