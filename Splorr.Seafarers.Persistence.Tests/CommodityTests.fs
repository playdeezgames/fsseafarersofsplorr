module CommodityTests

open NUnit.Framework
open CommonTestFixtures
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence

let internal commodities:Map<Commodity, CommodityDescriptor> =
    Map.empty
    |> Map.add Commodity.Grain {BasePrice=1.0;PurchaseFactor=2.0;SaleFactor=3.0;Name="grain";Occurrence=0.5;Discount=0.1}

[<Test>]
let ``Save.It persists a given set of commodities for a given world.`` () =
    use inputConnection = new SQLiteConnection(connectionString)
    inputConnection.Open()
    let inputWorldId = 1
    match Commodity.Save inputConnection inputWorldId commodities with
    | Ok worldId ->
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [Commodities] WHERE [WorldId]=$WorldId", inputConnection)
        command.Parameters.AddWithValue("$WorldId",worldId) |> ignore
        Assert.AreEqual(commodities.Count, command.ExecuteScalar())
    | Error ex ->
        raise ex

