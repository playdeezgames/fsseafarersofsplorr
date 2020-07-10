module ItemTests

open NUnit.Framework
open CommonTestFixtures
open System.Data.SQLite
open Splorr.Seafarers.Models
open Splorr.Seafarers.Persistence

let internal items:Map<Item, ItemDescriptor> =
    Map.empty
    |> Map.add Item.Ration 
        {
            DisplayName = "waffles"
            Commodities = Map.empty |> Map.add Commodity.Grain 1.0
            Occurrence = 1.0
        }

[<Test>]
let ``Save.It persists a given set of items for a given world.`` () =
    use inputConnection = new SQLiteConnection(connectionString)
    inputConnection.Open()
    let inputWorldId = 1
    let expectedItemCount = items.Count
    let expectedCommodityItemCount =
        items
        |> Map.toList
        |> List.map (fun i -> (i |> snd).Commodities.Count)
        |> List.reduce (+)
    match Item.Save inputConnection inputWorldId items with
    | Ok worldId ->
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [Items] WHERE [WorldId]=$WorldId", inputConnection)
        command.Parameters.AddWithValue("$WorldId",worldId) |> ignore
        Assert.AreEqual(expectedItemCount, command.ExecuteScalar())
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [CommodityItems] WHERE [WorldId]=$WorldId", inputConnection)
        command.Parameters.AddWithValue("$WorldId",worldId) |> ignore
        Assert.AreEqual(expectedCommodityItemCount, command.ExecuteScalar())
    | Error ex ->
        raise ex

