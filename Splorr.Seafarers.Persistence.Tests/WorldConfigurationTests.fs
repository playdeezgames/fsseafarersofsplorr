module WorldConfigurationTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``Get.It retrieves the global world configuration.`` () =
    use connection = SetupConnection()
    try
        match connection |> WorldConfiguration.Get with
        | Ok actual ->
            Assert.AreEqual(100.0, actual.WorldSize |> fst)
            Assert.AreEqual(100.0, actual.WorldSize |> snd)
            Assert.AreEqual(2, actual.RationItems.Length)
            Assert.AreEqual(5, actual.StatisticDescriptors.Length)
        | Error message ->
            Assert.Fail(message)
    finally
        connection.Close()