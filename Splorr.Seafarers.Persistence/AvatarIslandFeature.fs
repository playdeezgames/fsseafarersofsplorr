namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module AvatarIslandFeature =
    let SetFeatureForAvatar 
            (connection : SQLiteConnection)
            (avatarIslandFeature: AvatarIslandFeature option, avatarId: string) 
            : Result<unit, string>=
        try
            match avatarIslandFeature with
            | Some feature ->
                use command = new SQLiteCommand("REPLACE INTO [AvatarIslandFeatures] ([AvatarId], [FeatureId], [IslandX], [IslandY]) VALUES ($avatarId, $featureId, $islandX, $islandY);", connection)
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                command.Parameters.AddWithValue("$featureId", feature.featureId |> int32) |> ignore
                command.Parameters.AddWithValue("$islandX", feature.location |> fst) |> ignore
                command.Parameters.AddWithValue("$islandY", feature.location |> snd) |> ignore
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
            : Result<AvatarIslandFeature option, string> =
        try
            use command = new SQLiteCommand("SELECT [FeatureId], [IslandX], [IslandY] FROM [AvatarIslandFeatures] WHERE [AvatarId] = $avatarId;", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            let reader = command.ExecuteReader()
            if reader.Read() then
                {
                    featureId = 
                        reader.GetInt32(0) 
                        |> enum<IslandFeatureIdentifier> 
                    location = 
                        (reader.GetDouble(1),reader.GetDouble(2))
                }
                |> Some
            else
                None
            |> Ok
        with
        | ex ->
            ex.ToString() |> Error

