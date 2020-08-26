namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module IslandJob = 
    let private convertor 
            (reader : SQLiteDataReader) 
            : Job =
        {
            FlavorText=reader.GetString(0)
            Reward = reader.GetDouble(1)
            Destination = (reader.GetDouble(2), reader.GetDouble(3))
        }

    let GetForIsland 
            (connection:SQLiteConnection) 
            (location:Location) 
            : Result<Job list, string> =
        connection
        |> Utility.GetList 
            "SELECT [Description], [Reward], [DestinationX], [DestinationY] FROM [IslandJobs] WHERE [IslandX]=$islandX AND [IslandY]=$islandY ORDER BY [Order];" 
            (fun command->
                command.Parameters.AddWithValue("$islandX",location |> fst) |> ignore
                command.Parameters.AddWithValue("$islandY",location |> snd) |> ignore) 
            convertor

    let AddToIsland 
            (connection : SQLiteConnection)
            (location   : Location)
            (job        : Job)
            : Result<unit, string> =
        try
            use command = new SQLiteCommand("SELECT COALESCE(MAX([Order]),0)+1 FROM [IslandJobs] WHERE [IslandX]=$islandX and [IslandY]=$islandY;", connection)
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            let nextOrder = command.ExecuteScalar() :?> int64

            use command = new SQLiteCommand("REPLACE INTO [IslandJobs] ([IslandX], [IslandY], [Order], [Description], [Reward], [DestinationX], [DestinationY]) VALUES ($islandX, $islandY, $order, $description, $reward, $destinationX , $destinationY);", connection)
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            command.Parameters.AddWithValue("$order", nextOrder) |> ignore
            command.Parameters.AddWithValue("$description", job.FlavorText) |> ignore
            command.Parameters.AddWithValue("$reward", job.Reward) |> ignore
            command.Parameters.AddWithValue("$destinationX", job.Destination |> fst) |> ignore
            command.Parameters.AddWithValue("$destinationY", job.Destination |> snd) |> ignore
            command.ExecuteNonQuery() 
            |> ignore
            |> Ok
        with
        | ex ->
            ex.ToString()
            |> Error

