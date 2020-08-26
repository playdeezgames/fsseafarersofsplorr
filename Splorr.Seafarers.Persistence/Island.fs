namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models
open System

module Island =
    let private convertor 
            (reader : SQLiteDataReader) 
            : Location =
        (reader.GetDouble(0), reader.GetDouble(1))

    let GetList
            (connection : SQLiteConnection)
            : Result<Location list, string> =
        connection
        |> Utility.GetList 
            "" (fun _->()) convertor

    let GetName
            (connection : SQLiteConnection)
            (location   : Location)
            : Result<string option, string> =
        try
            use command = new SQLiteCommand("SELECT [IslandName] FROM [Islands] WHERE [IslandX] = $islandX AND [IslandY] = $islandY;", connection)
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            let reader = command.ExecuteReader()
            if reader.Read() then
                reader.GetString(0)
                |> Some
            else
                None
            |> Ok
        with
        | ex ->
            ex.ToString()
            |> Error

    let GetByName
            (connection : SQLiteConnection)
            (name       : string)
            : Result<Location option, string> =
        try
            use command = new SQLiteCommand("SELECT [IslandX], [IslandY] FROM [Islands] WHERE [IslandName] = $islandName;", connection)
            command.Parameters.AddWithValue("$islandName", name) |> ignore
            let reader = command.ExecuteReader()
            if reader.Read() then
                (reader.GetDouble(0), reader.GetDouble(1))
                |> Some
            else
                None
            |> Ok
        with
        | ex ->
            ex.ToString()
            |> Error

    let SetName
            (connection : SQLiteConnection)
            (location   : Location)
            (name       : string option)
            : Result<unit, string> =
        try
            match name with
            | Some n ->
                use command = new SQLiteCommand("REPLACE INTO [Islands] ([IslandX], [IslandY], [IslandName]) VALUES ($islandX, $islandY, $islandName);", connection)
                command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
                command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
                command.Parameters.AddWithValue("$islandName", n) |> ignore
                command.ExecuteNonQuery() 
                |> ignore
            | None ->
                use command = new SQLiteCommand("DELETE FROM [Islands] WHERE [IslandX]=$islandX AND [IslandY]=$islandY;", connection)
                command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
                command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
                command.ExecuteNonQuery()
                |> ignore
            |> Ok
        with
        | ex ->
            ex.ToString()
            |> Error

