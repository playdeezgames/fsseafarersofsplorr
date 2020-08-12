module RationItemTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetRationItemss.It retrieves the ration items configured for the world.`` () =
    use connection = SetupConnection()
    try
        match connection |> RationItem.GetRationItems with
        | Ok actual ->
            Assert.AreEqual(2, actual.Length)
        | Error message ->
            Assert.Fail(message)
    finally
        connection.Close()
