namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module IslandFeature =
    let private convertor 
            (reader : SQLiteDataReader) 
            : IslandFeatureIdentifier * IslandFeatureGenerator =
        (reader.GetInt32(0) |> enum<IslandFeatureIdentifier>,
            {
                FeatureWeight = reader.GetDouble(1)
                FeaturelessWeight = reader.GetDouble(2)
            })

    let GetGenerators
            (connection : SQLiteConnection)
            : Result<Map<IslandFeatureIdentifier, IslandFeatureGenerator>, string> =
        connection
        |> Utility.GetList 
            "SELECT [FeatureId], [FeatureWeight], [FeaturelessWeight] FROM [IslandFeatureGenerators];" 
            (fun _->()) 
            convertor
        |> Result.map
            (Map.ofList)

    let AddToIsland
            (connection : SQLiteConnection)
            (location   : Location)
            (identifier : IslandFeatureIdentifier)
            : Result<unit, string> =
        use command = new SQLiteCommand("REPLACE INTO [IslandFeatures] ([IslandX],[IslandY],[FeatureId]) VALUES ($islandX,$islandY,$featureId);", connection)
        command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
        command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
        command.Parameters.AddWithValue("$featureId", identifier |> uint) |> ignore
        command.ExecuteNonQuery() 
        |> ignore
        |> Ok

