namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module Commodity =
    let rec private readEntities (reader:SQLiteDataReader) (previous:CommodityDescriptor list) : Result<Map<uint64, CommodityDescriptor>,string> =
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
            readEntities reader next
        else    
            previous
            |> List.map (fun i -> (i.CommodityId, i))
            |> Map.ofList
            |> Ok

    let GetList (connection:SQLiteConnection) : Result<Map<uint64, CommodityDescriptor>,string> =
        try
            use command = new SQLiteCommand("SELECT [CommodityId], [CommodityName], [BasePrice], [SaleFactor], [PurchaseFactor], [Discount] FROM [Commodities];",connection)
            readEntities (command.ExecuteReader()) []
        with
        | ex ->
            Error (ex.ToString())

