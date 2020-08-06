﻿namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module ShipmateStatisticTemplate =
    let private convertor 
            (reader : SQLiteDataReader) 
            : ShipmateStatisticTemplate =
        {
            StatisticId = reader.GetInt32(0) |> enum<ShipmateStatisticIdentifier>
            StatisticName = reader.GetString(1)
            MinimumValue = reader.GetDouble(2)
            MaximumValue = reader.GetDouble(3)
            CurrentValue = reader.GetDouble(4)
        }

    let GetList 
            (connection : SQLiteConnection) 
            : Result<Map<ShipmateStatisticIdentifier, ShipmateStatisticTemplate>, string> =
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [StatisticName], [MinimumValue], [MaximumValue], [CurrentValue] FROM [ShipmateStatisticTemplates];" (fun _->()) convertor
        |> Result.map
            (fun items ->
                items
                |> List.map (fun item -> (item.StatisticId, item))
                |> Map.ofList)