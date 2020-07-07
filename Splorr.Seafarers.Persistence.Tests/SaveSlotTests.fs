module SaveSlotTests

open NUnit.Framework
open Splorr.Seafarers.Models
open System.Data.SQLite
open Splorr.Seafarers.Persistence
open CommonTestFixtures

[<Test>]
let ``Create.It creates a new save slot and returns the row id of the given save slot record.`` () =
    let inputName = "name"
    use inputConnection = new SQLiteConnection(connectionString)
    inputConnection.Open()
    let actual =
        inputName
        |> SaveSlot.Create inputConnection 
    match actual with
    | Ok saveSlotId ->
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [SaveSlots] WHERE [SaveSlotId]=$SaveSlotId", inputConnection)
        command.Parameters.AddWithValue("$SaveSlotId",saveSlotId) |> ignore
        Assert.AreEqual(1, command.ExecuteScalar())
    | Error ex ->
        Assert.Fail(ex.ToString() |> sprintf "An error occurred.\n%s")
