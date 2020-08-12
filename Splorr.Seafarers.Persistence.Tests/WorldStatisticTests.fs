module WorldStatisticTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

[<Test>]
let ``Get.It returns the given statistic for the world.`` () =
    use connection = SetupConnection()
    try
        let inputIdentifier = WorldStatisticIdentifier.JobReward
        let expectedStatistic = 
            {MinimumValue=0.0;MaximumValue=100.0;CurrentValue=50.0}
        let expected : Result<Statistic, string> =
            expectedStatistic |> Ok
        let actual =
            WorldStatistic.Get inputIdentifier connection
        Assert.AreEqual(expected, actual)
    finally
        connection.Close()
