module AvatarStatisticTemplateTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

[<Test>]
let ``GetList.It returns a list of statistics.`` () =
    use connection = SetupConnection()
    try
        match connection |> AvatarStatisticTemplate.GetList with
        | Ok actual ->
            let expectedCount = System.Enum.GetValues(typedefof<AvatarStatisticIdentifier>).Length
            Assert.AreEqual(expectedCount, actual.Count)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

