namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module AvatarMetric =
    let private convertor 
            (reader : SQLiteDataReader) 
            : Metric * uint64 =
        (reader.GetInt32(0) |> enum<Metric>, reader.GetInt64(1) |> uint64)

    let GetForAvatar 
            (avatarId   : string)
            (connection : SQLiteConnection) 
            : Result<Map<Metric, uint64>, string> =
        connection
        |> Utility.GetList 
            "SELECT [MetricId], [MetricValue] FROM [AvatarMetrics] WHERE [AvatarId]=$avatarId;" 
            (fun command->
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore) 
            convertor
        |> Result.map
            Map.ofList

    let SetMetricForAvatar
            (avatarId   : string)
            (metric     : Metric,
                value   : uint64)
            (connection : SQLiteConnection)
            : Result<unit, string> =
        try
            if value>0UL then
                use command = new SQLiteCommand("REPLACE INTO [AvatarMetrics] ([AvatarId],[MetricId],[MetricValue]) VALUES ($avatarId,$metricId,$metricValue);", connection)
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                command.Parameters.AddWithValue("$metricId", metric |> uint) |> ignore
                command.Parameters.AddWithValue("$metricValue", value) |> ignore
                command.ExecuteNonQuery() |> ignore
            else
                use command = new SQLiteCommand("DELETE FROM [AvatarMetrics] WHERE [AvatarId] = $avatarId AND [MetricId] = $metricId;", connection)                
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                command.Parameters.AddWithValue("$metricId", metric |> uint) |> ignore
                command.ExecuteNonQuery() |> ignore
            ()
            |> Ok
        with
        | ex ->
            ex.ToString() |> Error

    let GetMetricForAvatar
            (avatarId: string)
            (metric: Metric)
            (connection : SQLiteConnection)
            : Result<uint64, string> =
        try
            use command = new SQLiteCommand("SELECT [MetricValue] FROM [AvatarMetrics] WHERE [AvatarId]=$avatarId AND [MetricId]=$metricId", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.Parameters.AddWithValue("$metricId", metric |> uint) |> ignore
            let reader = command.ExecuteReader()
            if reader.Read() then
                reader.GetInt64(0) 
                |> uint64
                |> Ok
            else
                0UL
                |> Ok
        with
        | ex ->
            ex.ToString() |> Error

