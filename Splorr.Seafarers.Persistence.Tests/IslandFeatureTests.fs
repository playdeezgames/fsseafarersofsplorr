module IslandFeatureTests


open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models
open System.Data.SQLite

[<Test>]
let ``GetGenerators.It retrieves a map of island feature generators.`` () =
    let connection = SetupConnection()
    let expected : Result<Map<IslandFeatureIdentifier, IslandFeatureGenerator>, string> =
        Map.empty 
        |> Map.add 
            IslandFeatureIdentifier.DarkAlley
            {
                FeaturelessWeight = 0.5
                FeatureWeight = 0.5
            }
        |> Map.add
            IslandFeatureIdentifier.Dock
            {
                FeaturelessWeight = 0.5
                FeatureWeight = 0.5
            }
        |> Ok
    let actual =
        IslandFeature.GetGenerators connection
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AddToIsland.It adds a given feature to a given island when it does not exist.`` () =
    let connection = SetupConnection()
    let givenLocation = InvalidIslandLocation
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected : Result<unit, string> =
        ()
        |> Ok
    use command = new SQLiteCommand("SELECT COUNT(1) FROM [IslandFeatures] WHERE [IslandX]=$islandX AND [IslandY]=$islandY;", connection)
    command.Parameters.AddWithValue("$islandX", givenLocation |> fst) |> ignore
    command.Parameters.AddWithValue("$islandY", givenLocation |> snd) |> ignore
    let actualInitial = command.ExecuteScalar() :?> int64
    Assert.AreEqual(0L, actualInitial)
    let actual =
        IslandFeature.AddToIsland connection givenLocation givenFeature
    Assert.AreEqual(expected, actual)
    let actualFinal = command.ExecuteScalar() :?> int64
    Assert.AreEqual(1L, actualFinal)

[<Test>]
let ``GetForIsland.It returns a list of island features for a given island.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let expected : Result<IslandFeatureIdentifier list, string> =
        [
            IslandFeatureIdentifier.DarkAlley            
        ]
        |> Ok
    let actual = IslandFeature.GetForIsland connection givenLocation
    Assert.AreEqual(expected, actual)

//[<Test>]
//let ``GetForIsland.It returns nothing for an island that does not exist.`` () =


[<Test>]
let ``AddToIsland.It does nothing when a given feature on a given island already exists.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected : Result<unit, string> =
        ()
        |> Ok
    use command = new SQLiteCommand("SELECT COUNT(1) FROM [IslandFeatures] WHERE [IslandX]=$islandX AND [IslandY]=$islandY;", connection)
    command.Parameters.AddWithValue("$islandX", givenLocation |> fst) |> ignore
    command.Parameters.AddWithValue("$islandY", givenLocation |> snd) |> ignore
    let actualInitial = command.ExecuteScalar() :?> int64
    Assert.AreEqual(1L, actualInitial)
    let actual =
        IslandFeature.AddToIsland connection givenLocation givenFeature
    Assert.AreEqual(expected, actual)
    let actualFinal = command.ExecuteScalar() :?> int64
    Assert.AreEqual(1L, actualFinal)

[<Test>]
let ``ExistsForIsland.It returns false when the given feature does not exist for the given island.`` () =
    let connection = SetupConnection()
    let givenLocation = InvalidIslandLocation
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected : Result<bool, string> =
        false 
        |> Ok
    let actual = 
        IslandFeature.ExistsForIsland
            connection
            givenLocation
            givenFeature
    Assert.AreEqual(expected, actual)
    

[<Test>]
let ``ExistsForIsland.It returns false when the given feature exists for the given island.`` () =
    let connection = SetupConnection()
    let givenLocation = VisitedIslandLocation
    let givenFeature = IslandFeatureIdentifier.DarkAlley
    let expected : Result<bool, string> =
        true 
        |> Ok
    let actual = 
        IslandFeature.ExistsForIsland
            connection
            givenLocation
            givenFeature
    Assert.AreEqual(expected, actual)
    


