namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module Statistic =
    let private convertor (reader:SQLiteDataReader) : StatisticDescriptor =
        {
            StatisticId = reader.GetInt32(0) |> enum<StatisticIdentifier>
            StatisticName = reader.GetString(1)
            MinimumValue = reader.GetDouble(2)
            MaximumValue = reader.GetDouble(3)
            CurrentValue = reader.GetDouble(4)
        }

    let GetList (connection:SQLiteConnection) : Result<Map<StatisticIdentifier, StatisticDescriptor>,string> =
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [StatisticName], [MinimumValue], [MaximumValue], [CurrentValue] FROM [Statistics] " convertor
        |> Result.map
            (fun items ->
                items
                |> List.map (fun item -> (item.StatisticId, item))
                |> Map.ofList)
