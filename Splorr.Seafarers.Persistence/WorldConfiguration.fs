namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module WorldConfiguration =
    let rec private getRationItems 
            (reader   : SQLiteDataReader) 
            (previous : uint64 list) 
            : uint64 list =
        if reader.Read() then
            [(reader.GetInt64(0) |> uint64)]
            |> List.append previous
            |> getRationItems reader
        else
            previous

    let rec private getStatisticDescriptors 
            (reader   : SQLiteDataReader) 
            (previous : ShipmateStatisticTemplate list) 
            : ShipmateStatisticTemplate list =
        if reader.Read() then
            [{
                StatisticId = reader.GetInt32(0) |> enum<ShipmateStatisticIdentifier>
                StatisticName = reader.GetString(1)
                MinimumValue = reader.GetDouble(2)
                CurrentValue = reader.GetDouble(3)
                MaximumValue = reader.GetDouble(4)
            }]
            |> List.append previous
            |> getStatisticDescriptors reader
        else
            previous


    let Get 
            (connection : SQLiteConnection) 
            : Result<WorldConfiguration, string> =
        try

            use command = new SQLiteCommand("SELECT [ItemId] FROM [RationItems] ORDER BY [DefaultOrder];", connection)
            let rationItems = getRationItems (command.ExecuteReader()) []

            use command = new SQLiteCommand("SELECT [StatisticId], [StatisticName], [MinimumValue], [CurrentValue], [MaximumValue] FROM [ShipmateStatisticTemplates];",connection)
            let statisticDescriptors = getStatisticDescriptors (command.ExecuteReader()) []

            use command = new SQLiteCommand("SELECT [WorldConfigurationId],[RewardMinimum],[RewardMaximum],[WorldWidth],[WorldHeight],[MaximumGenerationTries],[MinimumIslandDistance],[AvatarViewDistance],[AvatarDockDistance] FROM [WorldConfiguration];", connection)
            let reader = command.ExecuteReader()
            if reader.Read() then
                {
                    AvatarDistances = (reader.GetDouble(7), reader.GetDouble(8))
                    WorldSize = (reader.GetDouble(3), reader.GetDouble(4))
                    MinimumIslandDistance = reader.GetDouble(6)
                    MaximumGenerationTries =  reader.GetInt32(5) |> uint32
                    RewardRange = (reader.GetDouble(1), reader.GetDouble(2))
                    RationItems = rationItems
                    StatisticDescriptors = statisticDescriptors
                }
                |> Ok
            else
                Error "Did not find WorldConfiguration record!"
        with
        | ex -> Error (ex.ToString())

