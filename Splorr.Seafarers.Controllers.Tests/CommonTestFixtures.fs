module CommonTestFixtures

open Splorr.Seafarers.Controllers
open System.Data.SQLite


let internal connectionString = "Data Source=:memory:;Version=3;New=True;"
let internal random = System.Random()
let internal sinkStub (_:Message) : unit = ()
let internal toSource (command:Command option) = fun () -> command
let internal createConnection() :SQLiteConnection =
    new SQLiteConnection(connectionString)
let internal avatarId:string = ""

