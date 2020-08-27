namespace Splorr.Seafarers.Persistence.Schema

module Views =
    let IslandList = "CREATE VIEW IF NOT EXISTS [IslandList] AS
        SELECT DISTINCT [IslandX], [IslandY] FROM [IslandStatistics]"

