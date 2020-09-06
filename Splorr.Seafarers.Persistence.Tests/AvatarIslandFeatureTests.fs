module AvatarIslandFeatureTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models
open System.Data.SQLite

[<Test>]
let ``SetFeatureForAvatar.It will set the feature of a given avatar to a given feature.`` () =
    use connection = SetupConnection()
    try
        let givenFeature = IslandFeatureIdentifier.DarkAlley
        let givenAvatarId = NewAvatarId
        let expected : Result<unit, string> =
            () |> Ok
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [AvatarIslandFeatures] WHERE [AvatarId]=$avatarId;", connection)
        command.Parameters.AddWithValue("$avatarId", givenAvatarId) |> ignore
        let actualInitial = command.ExecuteScalar() :?> int64
        let expectedInitial = 0L
        Assert.AreEqual(expectedInitial, actualInitial)
        let actual = 
            ({featureId = givenFeature; location = VisitedIslandLocation} |> Some, givenAvatarId)
            |> AvatarIslandFeature.SetFeatureForAvatar
                connection
        Assert.AreEqual(expected, actual)
        let actualFinal = command.ExecuteScalar() :?> int64
        let expectedFinal = 1L
        Assert.AreEqual(expectedFinal, actualFinal)
    finally
        connection.Close()


[<Test>]
let ``SetFeatureForAvatar.It will remove the feature of a given avatar when given a feature id of none.`` () =
    use connection = SetupConnection()
    try
        let givenFeature = None
        let givenAvatarId = DockedAvatarId
        let expected : Result<unit, string> =
            () |> Ok
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [AvatarIslandFeatures] WHERE [AvatarId]=$avatarId;", connection)
        command.Parameters.AddWithValue("$avatarId", givenAvatarId) |> ignore
        let actualInitial = command.ExecuteScalar() :?> int64
        let expectedInitial = 1L
        Assert.AreEqual(expectedInitial, actualInitial)
        let actual = 
            (givenFeature, givenAvatarId)
            |> AvatarIslandFeature.SetFeatureForAvatar
                connection
        Assert.AreEqual(expected, actual)
        let actualFinal = command.ExecuteScalar() :?> int64
        let expectedFinal = 0L
        Assert.AreEqual(expectedFinal, actualFinal)
    finally
        connection.Close()

[<Test>]
let ``GetFeatureForAvatar.It retrieves the current feature associated with a given avatar.`` () =
    use connection = SetupConnection()
    try
        let givenAvatarId = DockedAvatarId
        let expected : Result<AvatarIslandFeature option, string> =
            {featureId = IslandFeatureIdentifier.Dock; location=VisitedIslandLocation} |> Some |> Ok
        let actual = 
            givenAvatarId
            |> AvatarIslandFeature.GetFeatureForAvatar
                connection
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()
