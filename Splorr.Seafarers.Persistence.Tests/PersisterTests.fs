module PersisterTests

open NUnit.Framework
open Splorr.Seafarers.Models
open System.Data.SQLite
open Splorr.Seafarers.Persistence
open CommonTestFixtures


[<Test>]
let ``Save.It persist the given world to the given database.`` () =
    let inputName = "name"
    let inputWorld = world
    use inputConnection = new SQLiteConnection(connectionString)
    let expected : Result<bool, exn> = true |> Ok
    let actual = 
        (inputName, inputWorld)
        ||> Persister.Save inputConnection
    Assert.AreEqual(expected, actual)
