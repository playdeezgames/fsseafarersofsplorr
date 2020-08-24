module IslandStatisticTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models
open System.Data.SQLite

[<Test>]
let ``GetStatisticForIsland.It gets the statistic for an existing shipmate.`` () =
    let connection = SetupConnection()
    try
        let inputIslandLocation = VisitedIslandLocation
        let inputStatisticId = IslandStatisticIdentifier.CareenDistance
        let expected : Result<Statistic option, string> =
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some |> Ok
        let actual =
            IslandStatistic.GetStatisticForIsland connection inputIslandLocation inputStatisticId
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()

[<Test>]
let ``SetStatisticForIsland.It create the given statistic for the given island when it does not already exist.`` () =
    use connection = SetupConnection()
    try
        let inputIslandLocation = InvalidIslandLocation
        let inputIdentifier = IslandStatisticIdentifier.CareenDistance
        let inputStatistic = {MaximumValue=100.0; MinimumValue=0.0; CurrentValue=50.0} |> Some

        use command = new SQLiteCommand("SELECT COUNT(1) FROM [IslandStatistics] WHERE [IslandX]=$IslandX AND [IslandY]=$islandY;", connection)
        command.Parameters.AddWithValue("$islandX", inputIslandLocation |> fst) |> ignore
        command.Parameters.AddWithValue("$islandY", inputIslandLocation |> snd) |> ignore
        let initialCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(0L, initialCount)

        let expected: Result<unit, string> = () |> Ok
        let actual =
            IslandStatistic.SetStatisticForIsland connection inputIslandLocation (inputIdentifier, inputStatistic)
        Assert.AreEqual(expected, actual)

        let finalCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(1L, finalCount)
    finally
        connection.Close()


[<Test>]
let ``SetStatisticForIsland.It removes the given statistic for the given island when it exists and None is passed.`` () =
    use connection = SetupConnection()
    try
        let inputIslandLocation = VisitedIslandLocation
        let inputIdentifier = IslandStatisticIdentifier.CareenDistance
        let inputStatistic = None

        use command = new SQLiteCommand("SELECT COUNT(1) FROM [IslandStatistics] WHERE [IslandX]=$IslandX AND [IslandY]=$islandY;", connection)
        command.Parameters.AddWithValue("$islandX", inputIslandLocation |> fst) |> ignore
        command.Parameters.AddWithValue("$islandY", inputIslandLocation |> snd) |> ignore
        let initialCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(1L, initialCount)

        let expected: Result<unit, string> = () |> Ok
        let actual =
            IslandStatistic.SetStatisticForIsland connection inputIslandLocation (inputIdentifier, inputStatistic)
        Assert.AreEqual(expected, actual)

        let finalCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(0L, finalCount)
    finally
        connection.Close()
