module GameTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models
open System

[<Test>]
let ``Save.It saves the game to an external db file.`` () =
    let connection = SetupConnection()
    let filename = Guid.NewGuid().ToString()
    let expected : Result<string, string> = filename |> sprintf "%s.db" |> Ok
    let actual = Game.Export connection filename
    Assert.AreEqual(expected, actual)