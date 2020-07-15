module ItemTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetList.It returns a list of items.`` () =
    use connection = SetupConnection()
    try
        let actual = connection |> Item.GetList 
        Assert.AreEqual(3, actual.Count)
        Assert.AreEqual(1, actual.[1UL].Commodities.Count)
        Assert.AreEqual(2, actual.[2UL].Commodities.Count)
        Assert.AreEqual(3, actual.[3UL].Commodities.Count)
    finally
        connection.Close()