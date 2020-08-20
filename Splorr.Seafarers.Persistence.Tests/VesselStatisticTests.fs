module VesselStatisticTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models
open System.Data.SQLite

[<Test>]
let ``SetStatisticForAvatar.It create the given statistic for the given avatar when it does not already exist.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputIdentifier = VesselStatisticIdentifier.PortFouling
        let inputStatistic = {MaximumValue=100.0; MinimumValue=0.0; CurrentValue=50.0}

        use command = new SQLiteCommand("SELECT COUNT(1) FROM [VesselStatistics] WHERE [AvatarId]=$avatarId;", connection)
        command.Parameters.AddWithValue("$avatarId", inputAvatarId) |> ignore
        let initialCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(0L, initialCount)

        let expected: Result<unit, string> = () |> Ok
        let actual =
            VesselStatistic.SetStatisticForAvatar connection inputAvatarId (inputIdentifier, inputStatistic) 
        Assert.AreEqual(expected, actual)

        let finalCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(1L, finalCount)
    finally
        connection.Close()

[<Test>]
let ``SetStatisticForAvatar.It replaces the given statistic for the given avatar when it already exists.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let inputIdentifier = VesselStatisticIdentifier.PortFouling
        let inputStatistic = {MaximumValue=100.0; MinimumValue=0.0; CurrentValue=50.0}

        use command = new SQLiteCommand("SELECT COUNT(1) FROM [VesselStatistics] WHERE [AvatarId]=$avatarId;", connection)
        command.Parameters.AddWithValue("$avatarId", inputAvatarId) |> ignore
        let initialCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(10L, initialCount)

        let expected: Result<unit, string> = () |> Ok
        let actual =
            VesselStatistic.SetStatisticForAvatar connection inputAvatarId (inputIdentifier, inputStatistic) 
        Assert.AreEqual(expected, actual)

        let finalCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(10L, finalCount)
    finally
        connection.Close()

[<Test>]
let ``GetStatisticForAvatar.It returns the given statistic for the given avatar when it exists.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let inputIdentifier = VesselStatisticIdentifier.PortFouling
        let expectedStatistic = 
            {MinimumValue=0.0;MaximumValue=100.0;CurrentValue=50.0} |> Some
        let expected : Result<Statistic option, string> =
            expectedStatistic |> Ok
        let actual =
            VesselStatistic.GetStatisticForAvatar connection inputAvatarId inputIdentifier 
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()

[<Test>]
let ``GetStatisticForAvatar.It returns none for the given avatar when the statistic does not exist.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputIdentifier = VesselStatisticIdentifier.PortFouling
        let expectedStatistic = 
            None
        let expected : Result<Statistic option, string> =
            expectedStatistic |> Ok
        let actual =
            VesselStatistic.GetStatisticForAvatar connection inputAvatarId inputIdentifier 
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()

[<Test>]
let ``GetForAvatar.It returns a list of statistics for a given avatar.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        match VesselStatistic.GetForAvatar connection inputAvatarId with
        | Ok actual ->
            let expectedCount = System.Enum.GetValues(typedefof<VesselStatisticIdentifier>).Length
            Assert.AreEqual(expectedCount, actual.Count)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetForAvatar.It creates the vessel statistics for a given avatar when that avatar does not exist.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputVesselStatistics :Map<VesselStatisticIdentifier, Statistic> = 
            Map.empty
            |> Map.add VesselStatisticIdentifier.PortFouling ({MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
            |> Map.add VesselStatisticIdentifier.StarboardFouling ({MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
            |> Map.add VesselStatisticIdentifier.FoulRate ({MinimumValue=0.001; MaximumValue=0.001; CurrentValue=0.001})
            |> Map.add VesselStatisticIdentifier.Tonnage ({MinimumValue=100.0; MaximumValue=100.0; CurrentValue=100.0})
            |> Map.add VesselStatisticIdentifier.PositionX ({MinimumValue=100.0; MaximumValue=100.0; CurrentValue=100.0})
            |> Map.add VesselStatisticIdentifier.PositionY ({MinimumValue=100.0; MaximumValue=100.0; CurrentValue=100.0})
            |> Map.add VesselStatisticIdentifier.Speed ({MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0})
            |> Map.add VesselStatisticIdentifier.Heading ({MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0})
            |> Map.add VesselStatisticIdentifier.ViewDistance ({MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0})
            |> Map.add VesselStatisticIdentifier.DockDistance ({MinimumValue=1.0; MaximumValue=1.0; CurrentValue=1.0})
        match VesselStatistic.GetForAvatar connection inputAvatarId with
        | Ok x ->
            Assert.AreEqual(0, x.Count)
        | Error message ->  Assert.Fail message
        match VesselStatistic.SetForAvatar connection inputAvatarId inputVesselStatistics with
        | Ok () ->
            match VesselStatistic.GetForAvatar connection inputAvatarId with
            | Ok x ->
                Assert.AreEqual(System.Enum.GetValues(typedefof<VesselStatisticIdentifier>).Length, x.Count)
            | Error message ->  Assert.Fail message
        | Error message -> Assert.Fail message
    finally
        connection.Close()
    
