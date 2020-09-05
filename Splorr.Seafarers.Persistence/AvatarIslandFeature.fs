namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module AvatarIslandFeature =
    let SetFeatureForAvatar 
            (connection : SQLiteConnection)
            (feature: IslandFeatureIdentifier option, avatarId: string) 
            : Result<unit, string>=
        try
            match feature with
            | Some identfier ->
                use command = new SQLiteCommand("REPLACE INTO [AvatarIslandFeatures] ([AvatarId], [FeatureId]) VALUES ($avatarId, $featureId);", connection)
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                command.Parameters.AddWithValue("$featureId", identfier |> int32) |> ignore
                command.ExecuteNonQuery() |> ignore
            | None ->
                use command = new SQLiteCommand("DELETE FROM [AvatarIslandFeatures] WHERE [AvatarId] = $avatarId;", connection)
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                command.ExecuteNonQuery() |> ignore
            |> Ok
        with
        | ex ->
            ex.ToString() |> Error

    let GetFeatureForAvatar 
            (connection : SQLiteConnection)
            (avatarId : string)
            : Result<IslandFeatureIdentifier option, string> =
        try
            use command = new SQLiteCommand("SELECT [FeatureId] FROM [AvatarIslandFeatures] WHERE [AvatarId] = $avatarId;", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            let reader = command.ExecuteReader()
            if reader.Read() then
                reader.GetInt32(0) 
                |> enum<IslandFeatureIdentifier> 
                |> Some
            else
                None
            |> Ok
        with
        | ex ->
            ex.ToString() |> Error

