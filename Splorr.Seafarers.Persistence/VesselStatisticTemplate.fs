namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module VesselStatisticTemplate =
    let private convertor 
            (reader : SQLiteDataReader) 
            : VesselStatisticIdentifier * StatisticTemplate =
        (reader.GetInt32(0) |> enum<VesselStatisticIdentifier>,
            {
                StatisticName = reader.GetString(1)
                MinimumValue = reader.GetDouble(2)
                MaximumValue = reader.GetDouble(3)
                CurrentValue = reader.GetDouble(4)
            })

    let GetList 
            (connection : SQLiteConnection) 
            : Result<Map<VesselStatisticIdentifier, StatisticTemplate>, string> =
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [StatisticName], [MinimumValue], [MaximumValue], [CurrentValue] FROM [VesselStatisticTemplates];" (fun _->()) convertor
        |> Result.map
            (Map.ofList)
