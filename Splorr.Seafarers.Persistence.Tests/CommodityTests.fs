module CommodityTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetList.It returns a list of commodities.`` () =
    use connection = SetupConnection()
    try
        match connection |> Commodity.GetList with
        | Ok actual     -> 
            Assert.AreEqual(3, actual.Count)
        | Error message -> 
            Assert.Fail message
    finally
        connection.Close()


