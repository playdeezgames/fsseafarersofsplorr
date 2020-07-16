namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module WorldConfiguration =
    let Get (connection:SQLiteConnection) : Result<WorldConfiguration,string> =
        try
            use command = new SQLiteCommand("SELECT [WorldConfigurationId],[RewardMinimum],[RewardMaximum],[WorldWidth],[WorldHeight],[MaximumGenerationTries],[MinimumIslandDistance] FROM [WorldConfiguration];", connection)
            let reader = command.ExecuteReader()
            if reader.Read() then
                {
                    WorldSize = (reader.GetDouble(3), reader.GetDouble(4))
                    MinimumIslandDistance = reader.GetDouble(6)
                    MaximumGenerationTries =  reader.GetInt32(5) |> uint32
                    RewardRange = (reader.GetDouble(1), reader.GetDouble(2))
                }
                |> Ok
            else
                Error "Did not find WorldConfiguration record!"
        with
        | ex -> Error (ex.ToString())

