module IslandFeatureTests


open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

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
        |> Ok
    let actual =
        IslandFeature.GetGenerators connection
    Assert.AreEqual(expected, actual)

