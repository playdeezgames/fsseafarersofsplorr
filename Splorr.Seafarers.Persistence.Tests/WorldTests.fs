module WorldTests

open NUnit.Framework
open System.Data.SQLite
open Splorr.Seafarers.Persistence
open CommonTestFixtures

[<Test>]
let ``Save.It persists the given world's data to the database.`` () =
    let inputSaveName = "test"
    let inputWorld = world
    use inputConnection = new SQLiteConnection(connectionString)
    inputConnection.Open()
    match World.Save inputConnection inputSaveName inputWorld with
    | Ok worldId ->
        use command = new SQLiteCommand("SELECT COUNT(1) FROM [Worlds] WHERE [WorldId]=$WorldId", inputConnection)
        command.Parameters.AddWithValue("$WorldId",worldId) |> ignore
        Assert.AreEqual(1, command.ExecuteScalar())
    | Error ex ->
        raise ex
