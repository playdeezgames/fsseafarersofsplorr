module PersisterTests

open NUnit.Framework
open Splorr.Seafarers.Models
open System.Data.SQLite
open Splorr.Seafarers.Persistence
open CommonTestFixtures

let world : World =
    {
        Turn = 0u
        Messages = []
        Avatar = 
            {
                Position = (1.0, 2.0)
                Heading = 3.0
                Speed = 4.0
                ViewDistance = 5.0
                DockDistance = 6.0
                Money = 7.0
                Reputation= 8.0
                Job = None
                Inventory = Map.empty
                Satiety = {MinimumValue=9.0; CurrentValue=10.0; MaximumValue=11.0}
                Health = {MinimumValue=12.0; CurrentValue=13.0; MaximumValue=14.0}
            }
        Islands = Map.empty
        RewardRange = (1.0, 10.0)
        Commodities= Map.empty
        Items = Map.empty
    }

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
