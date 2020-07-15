module CommodityTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetList.It returns a list of commodities.`` () =
    use connection = SetupConnection()
    try
        let actual = connection |> Commodity.GetList 
        Assert.AreEqual(3, actual.Count)
    finally
        connection.Close()


