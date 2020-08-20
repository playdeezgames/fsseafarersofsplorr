module ShipmateStatisticTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models
open System.Data.SQLite

[<Test>]
let ``GetShipmatesForAvatar.It retrieves a list of shipmates for an existing avatar.`` () =
    let connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let expected : Result<string list, string> =
            [ PrimaryShipmateId ] |> Ok
        let actual =
            ShipmateStatistic.GetShipmatesForAvatar connection inputAvatarId
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()
    

[<Test>]
let ``GetStatisticForShipmate.It gets the statistic for an existing shipmate.`` () =
    let connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let inputShipmateId = PrimaryShipmateId
        let inputStatisticId = ShipmateStatisticIdentifier.Health
        let expected : Result<Statistic option, string> =
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some |> Ok
        let actual =
            ShipmateStatistic.GetStatisticForShipmate connection inputAvatarId inputShipmateId inputStatisticId
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()

[<Test>]
let ``SetStatisticForShipmate.It create the given statistic for the given shipmate when it does not already exist.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = NewAvatarId
        let inputShipmateId = PrimaryShipmateId
        let inputIdentifier = ShipmateStatisticIdentifier.Health
        let inputStatistic = {MaximumValue=100.0; MinimumValue=0.0; CurrentValue=50.0} |> Some

        use command = new SQLiteCommand("SELECT COUNT(1) FROM [ShipmateStatistics] WHERE [AvatarId]=$avatarId AND [ShipmateId]=$shipmateId;", connection)
        command.Parameters.AddWithValue("$avatarId", inputAvatarId) |> ignore
        command.Parameters.AddWithValue("$shipmateId", inputShipmateId) |> ignore
        let initialCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(0L, initialCount)

        let expected: Result<unit, string> = () |> Ok
        let actual =
            ShipmateStatistic.SetStatisticForShipmate connection inputAvatarId inputShipmateId (inputIdentifier, inputStatistic)
        Assert.AreEqual(expected, actual)

        let finalCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(1L, finalCount)
    finally
        connection.Close()


[<Test>]
let ``SetStatisticForShipmate.It removes the given statistic for the given shipmate when it exists and None is passed.`` () =
    use connection = SetupConnection()
    try
        let inputAvatarId = ExistingAvatarId
        let inputShipmateId = PrimaryShipmateId
        let inputIdentifier = ShipmateStatisticIdentifier.Health
        let inputStatistic = None

        use command = new SQLiteCommand("SELECT COUNT(1) FROM [ShipmateStatistics] WHERE [AvatarId]=$avatarId AND [ShipmateId]=$shipmateId;", connection)
        command.Parameters.AddWithValue("$avatarId", inputAvatarId) |> ignore
        command.Parameters.AddWithValue("$shipmateId", inputShipmateId) |> ignore
        let initialCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(5L, initialCount)

        let expected: Result<unit, string> = () |> Ok
        let actual =
            ShipmateStatistic.SetStatisticForShipmate connection inputAvatarId inputShipmateId (inputIdentifier, inputStatistic)
        Assert.AreEqual(expected, actual)

        let finalCount = command.ExecuteScalar() :?> int64
        Assert.AreEqual(4L, finalCount)
    finally
        connection.Close()
