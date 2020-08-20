namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module AvatarJob =
    let private convertor 
            (reader : SQLiteDataReader) 
            : Job =
        {
            FlavorText=reader.GetString(0)
            Reward = reader.GetDouble(1)
            Destination = (reader.GetDouble(2), reader.GetDouble(3))
        }

    let GetForAvatar 
            (connection : SQLiteConnection) 
            (avatarId   : string) 
            : Result<Job option, string> =
        connection
        |> Utility.GetList 
            "SELECT [Description], [Reward], [DestinationX], [DestinationY] FROM [AvatarJobs] WHERE [AvatarId]=$avatarId;" 
            (fun command->
                command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore)
            convertor
        |> Result.map
            (List.tryHead)

    let SetForAvatar 
            (connection : SQLiteConnection) 
            (avatarId   : string) 
            (job        : Job option)
            : Result<unit, string> =
        match job with
        | None ->
            use command = new SQLiteCommand("DELETE FROM [AvatarJobs] WHERE [AvatarId] = $avatarId;", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.ExecuteNonQuery() |> ignore
            () |> Ok
        | Some j ->
            use command = new SQLiteCommand("REPLACE INTO [AvatarJobs] ([AvatarId], [Description], [Reward], [DestinationX], [DestinationY]) VALUES ($avatarId, $description, $reward, $destinationX, $destinationY);", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.Parameters.AddWithValue("$description", j.FlavorText) |> ignore
            command.Parameters.AddWithValue("$reward", j.Reward) |> ignore
            command.Parameters.AddWithValue("$destinationX", j.Destination |> fst) |> ignore
            command.Parameters.AddWithValue("$destinationY", j.Destination |> snd) |> ignore
            command.ExecuteNonQuery() |> ignore
            () |> Ok
