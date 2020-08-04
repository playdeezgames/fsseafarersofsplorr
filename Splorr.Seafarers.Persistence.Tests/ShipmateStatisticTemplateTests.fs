module ShipmateStatisticTemplateTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

[<Test>]
let ``GetList.It returns a list of statistics.`` () =
    use connection = SetupConnection()
    try
        match connection |> ShipmateStatisticTemplate.GetList with
        | Ok actual ->
            let expectedCount = System.Enum.GetValues(typedefof<ShipmateStatisticIdentifier>).Length
            Assert.AreEqual(expectedCount, actual.Count)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

