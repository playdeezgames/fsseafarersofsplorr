namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module AvatarIslandMetric = 
    let GetMetricForAvatarIsland
            (connection : SQLiteConnection)
            (avatarId    : string) 
            (location    : Location) 
            (metric      : AvatarIslandMetricIdentifier) 
            : Result<uint64 option, string> =
        try
            use command = new SQLiteCommand("SELECT [MetricValue] FROM [AvatarIslandMetrics] WHERE [AvatarId]=$avatarId AND [IslandX]=$islandX AND [IslandY]=$islandY AND [MetricId]=$metricId", connection)
            command.Parameters.AddWithValue("$avatarId",avatarId) |> ignore
            command.Parameters.AddWithValue("$islandX",location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY",location |> snd) |> ignore
            command.Parameters.AddWithValue("$metricId",metric |> int) |> ignore
            let reader = command.ExecuteReader()
            if reader.Read() then
                reader.GetInt64(0)
                |> uint64
                |> Some
                |> Ok
            else
                None
                |> Ok
        with
        | ex -> 
            ex.ToString() |> Error

    let SetMetricForAvatarIsland
            (connection : SQLiteConnection)
            (avatarId    : string) 
            (location    : Location) 
            (metric      : AvatarIslandMetricIdentifier) 
            (value       : uint64 option)
            : Result<unit, string> =
        try
            match value with
            | None ->
                use command = new SQLiteCommand("DELETE FROM [AvatarIslandMetrics] WHERE [AvatarId]=$avatarId AND [IslandX]=$islandX AND [IslandY]=$islandY AND [MetricId]=$metricId;",connection)
                command.Parameters.AddWithValue("$avatarId",avatarId) |> ignore
                command.Parameters.AddWithValue("$islandX",location |> fst) |> ignore
                command.Parameters.AddWithValue("$islandY",location |> snd) |> ignore
                command.Parameters.AddWithValue("$metricId",metric |> int) |> ignore
                command.ExecuteNonQuery() |> ignore
            | Some v ->
                use command = new SQLiteCommand("REPLACE INTO [AvatarIslandMetrics] ([AvatarId],[IslandX],[IslandY],[MetricId],[MetricValue]) VALUES ($avatarId,$islandX,$islandY,$metricId,$value);",connection)
                command.Parameters.AddWithValue("$avatarId",avatarId) |> ignore
                command.Parameters.AddWithValue("$islandX",location |> fst) |> ignore
                command.Parameters.AddWithValue("$islandY",location |> snd) |> ignore
                command.Parameters.AddWithValue("$metricId",metric |> int) |> ignore
                command.Parameters.AddWithValue("$value",v) |> ignore
                command.ExecuteNonQuery() |> ignore
            |> Ok
        with
        | ex ->
            ex.ToString() |> Error

