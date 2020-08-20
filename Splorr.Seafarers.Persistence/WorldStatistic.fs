namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module WorldStatistic =
    let private convertor 
            (reader : SQLiteDataReader) 
            : WorldStatisticIdentifier * Statistic =
        (reader.GetInt32(0) |> enum<WorldStatisticIdentifier>, 
            {
                MinimumValue = reader.GetDouble(1)
                MaximumValue = reader.GetDouble(2)
                CurrentValue = reader.GetDouble(3)
            })

    let Get
            (connection : SQLiteConnection) 
            (identifier : WorldStatisticIdentifier) 
            : Result<Statistic,string> =
        let commandFilter (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [MinimumValue], [MaximumValue], [CurrentValue] FROM [WorldStatistics] WHERE [StatisticId]=$statisticId;" commandFilter convertor
        |> Result.map
            (fun items ->
                items
                |> List.map snd
                |> List.head)


