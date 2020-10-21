namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module IslandStatisticTemplate =
    let private convertor 
            (reader : SQLiteDataReader) 
            : IslandStatisticIdentifier * StatisticTemplate =
        (reader.GetInt32(0) |> enum<IslandStatisticIdentifier>,
            {
                StatisticName = reader.GetString(1)
                MinimumValue = reader.GetDouble(2)
                MaximumValue = reader.GetDouble(3)
                CurrentValue = reader.GetDouble(4)
            })

    let internal GetList 
            (connection : SQLiteConnection) 
            : Result<Map<IslandStatisticIdentifier, StatisticTemplate>, string> =
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [StatisticName], [MinimumValue], [MaximumValue], [CurrentValue] FROM [IslandStatisticTemplates];" (fun _->()) convertor
        |> Result.map
            (Map.ofList)
