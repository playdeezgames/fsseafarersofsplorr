namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module IslandJob = 
    let private convertor 
            (reader : SQLiteDataReader) 
            : uint * Job =
        (reader.GetInt64(4) |> uint, {
            FlavorText=reader.GetString(0)
            Reward = reader.GetDouble(1)
            Destination = (reader.GetDouble(2), reader.GetDouble(3))
        })

    let GetForIsland 
            (connection:SQLiteConnection) 
            (location:Location) 
            : Result<Job list, string> =
        connection
        |> Utility.GetList 
            "SELECT [Description], [Reward], [DestinationX], [DestinationY], [Order] FROM [IslandJobs] WHERE [IslandX]=$islandX AND [IslandY]=$islandY ORDER BY [Order];" 
            (fun command->
                command.Parameters.AddWithValue("$islandX",location |> fst) |> ignore
                command.Parameters.AddWithValue("$islandY",location |> snd) |> ignore) 
            (convertor >> snd)

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

    let rec private ReadOrders 
            (previous: int64 list)
            (reader:SQLiteDataReader) 
            : int64 array =
        if reader.Read() then
            reader
            |> ReadOrders (List.append previous [reader.GetInt64(0)])
        else
            previous
            |> List.toArray

    let RemoveFromIsland 
            (connection : SQLiteConnection)
            (location   : Location)
            (index      : uint)
            : Result<unit, string> =
        try
            if index>0u then
                use command = new SQLiteCommand("SELECT [Order] FROM [IslandJobs] WHERE [IslandX]=$islandX AND [IslandY]=$islandY ORDER BY [Order];",connection)
                command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
                command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
                command.ExecuteReader()
                |> ReadOrders []
                |> Array.tryItem ((index |> int) - 1)
                |> Option.iter
                    (fun order ->
                        use command = new SQLiteCommand("DELETE FROM [IslandJobs] WHERE [IslandX]=$islandX AND [IslandY]=$islandY AND [Order] = $order;", connection)
                        command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
                        command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
                        command.Parameters.AddWithValue("$order", order) |> ignore
                        command.ExecuteNonQuery() |> ignore)
                        
            Ok ()
        with
        | ex ->
            ex.ToString()
            |> Error

    let GetForIslandByIndex 
            (connection : SQLiteConnection)
            (location   : Location)
            (index      : uint)
            : Result<Job option, string> =
        try
            if index=0u then
                None 
                |> Ok
            else
                connection
                |> Utility.GetList 
                    "SELECT [Description], [Reward], [DestinationX], [DestinationY], [Order] FROM [IslandJobs] WHERE [IslandX]=$islandX AND [IslandY]=$islandY ORDER BY [Order];" 
                    (fun command->
                        command.Parameters.AddWithValue("$islandX",location |> fst) |> ignore
                        command.Parameters.AddWithValue("$islandY",location |> snd) |> ignore) 
                    convertor
                |> Result.bind
                    (Array.ofList 
                    >> Array.tryItem ((index |> int)-1) 
                    >> Option.map (snd) 
                    >> Ok)
        with
        | ex ->
            ex.ToString()
            |> Error

