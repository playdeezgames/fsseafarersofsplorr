namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module ShipmateStatistic = 
    //SELECT DISTINCT [ShipmateId] FROM [ShipmateStatistics] WHERE [AvatarId]=$avatarid;
    let private statisticConvertor 
            (reader : SQLiteDataReader) 
            : ShipmateStatisticIdentifier * Statistic =
        (reader.GetInt32(0) |> enum<ShipmateStatisticIdentifier>, 
            {
                MinimumValue = reader.GetDouble(1)
                MaximumValue = reader.GetDouble(2)
                CurrentValue = reader.GetDouble(3)
            })

    let GetShipmatesForAvatar 
            (avatarId   : string) 
            (connection : SQLiteConnection) 
            : Result<string list, string> =
        connection
        |> Utility.GetList 
            "SELECT DISTINCT [ShipmateId] FROM [ShipmateStatistics] WHERE [AvatarId]=$avatarid;" 
            (fun command->command.Parameters.AddWithValue("$avatarId",avatarId) |> ignore) 
            (fun reader -> reader.GetString(0))

    let GetStatisticForShipmate
            (avatarId   : string) 
            (shipmateId : string)
            (identifier : ShipmateStatisticIdentifier) 
            (connection : SQLiteConnection) 
            : Result<Statistic option,string> =
        let commandFilter (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.Parameters.AddWithValue("$shipmateId", shipmateId) |> ignore
            command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [MinimumValue], [MaximumValue], [CurrentValue] FROM [ShipmateStatistics] WHERE [AvatarId]=$avatarId AND [StatisticId]=$statisticId AND [ShipmateId] = $shipmateId;" commandFilter statisticConvertor
        |> Result.map
            (fun items ->
                items
                |> List.map snd
                |> List.tryHead)

    let SetStatisticForShipmate
            (avatarId: string)
            (shipmateId: string)
            (identifier: ShipmateStatisticIdentifier, statistic:Statistic option)
            (connection: SQLiteConnection) 
            : Result<unit, string> =
        try
            use command = new SQLiteCommand ("DELETE FROM [ShipmateStatistics] WHERE [AvatarId] = $avatarId AND [ShipmateId] = $shipmateId AND [StatisticId] = $statisticId;", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.Parameters.AddWithValue("$shipmateId", shipmateId) |> ignore
            command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
            command.ExecuteNonQuery() |> ignore
            statistic
            |> Option.iter
                (fun stat ->
                    use command = new SQLiteCommand("
                        REPLACE INTO [ShipmateStatistics] 
                            ([AvatarId],[ShipmateId],[StatisticId],[MinimumValue],[CurrentValue],[MaximumValue]) 
                            VALUES 
                            ($avatarId, $shipmateId, $statisticId, $minimumValue, $currentValue, $maximumValue);", connection)
                    command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                    command.Parameters.AddWithValue("$shipmateId", shipmateId) |> ignore
                    command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
                    command.Parameters.AddWithValue("$minimumValue", stat.MinimumValue) |> ignore
                    command.Parameters.AddWithValue("$currentValue", stat.CurrentValue) |> ignore
                    command.Parameters.AddWithValue("$maximumValue", stat.MaximumValue) |> ignore
                    command.ExecuteNonQuery() |> ignore)
            () |> Ok
        with
        | ex -> ex.ToString() |> Error


