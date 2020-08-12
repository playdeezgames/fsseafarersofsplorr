namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module WorldConfiguration =
    let Get 
            (connection : SQLiteConnection) 
            : Result<WorldConfiguration, string> =
        try
            use command = new SQLiteCommand("SELECT [WorldConfigurationId],[RewardMinimum],[RewardMaximum],[WorldWidth],[WorldHeight],[MaximumGenerationTries],[MinimumIslandDistance],[AvatarViewDistance],[AvatarDockDistance] FROM [WorldConfiguration];", connection)
            let reader = command.ExecuteReader()
            if reader.Read() then
                {
                    WorldSize   = (reader.GetDouble(3), reader.GetDouble(4))
                }
                |> Ok
            else
                Error "Did not find WorldConfiguration record!"
        with
        | ex -> Error (ex.ToString())

