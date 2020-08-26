module IslandTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

let ``GetList.It retrieves a list of locations for islands.`` () =
    let connection = SetupConnection()
    let expected = 
        [
            (0.0, 0.0)
        ]
    let actual = Island.GetList connection
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetName.It retrieves a name for a given location where an island exists.`` () =
    let connection = SetupConnection()
    let input = VisitedIslandLocation
    let expected : Result<string option, string> = 
        VisitedIslandName 
        |> Some 
        |> Ok
    let actual =
        Island.GetName connection input
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetName.It retrieves None for a given location where an island does not exist.`` () =
    let connection = SetupConnection()
    let input = InvalidIslandLocation
    let expected : Result<string option, string> = 
        None 
        |> Ok
    let actual =
        Island.GetName connection input
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetName.It sets a name for a given location.`` () =
    let connection = SetupConnection()
    let inputLocation = InvalidIslandLocation
    let inputName = "yermom" |> Some
    let expectedInitial : Result<string option, string> = 
        None
        |> Ok
    let actualInitial =
        Island.GetName connection inputLocation
    Assert.AreEqual(expectedInitial, actualInitial)
    let expected : Result<unit, string> = Ok ()
    let actual =
        Island.SetName connection inputLocation inputName
    Assert.AreEqual(expected, actual)
    let expectedFinal : Result<string option, string> = 
        inputName
        |> Ok
    let actualFinal =
        Island.GetName connection inputLocation
    Assert.AreEqual(expectedFinal, actualFinal)


[<Test>]
let ``SetName.It clears a name for a given location.`` () =
    let connection = SetupConnection()
    let inputLocation = VisitedIslandLocation
    let inputName = None
    let expectedInitial : Result<string option, string> = 
        VisitedIslandName
        |> Some
        |> Ok
    let actualInitial =
        Island.GetName connection inputLocation
    Assert.AreEqual(expectedInitial, actualInitial)
    let expected : Result<unit, string> = Ok ()
    let actual =
        Island.SetName connection inputLocation inputName
    Assert.AreEqual(expected, actual)
    let expectedFinal : Result<string option, string> = 
        None |> Ok
    let actualFinal =
        Island.GetName connection inputLocation
    Assert.AreEqual(expectedFinal, actualFinal)

[<Test>]
let ``GetByName.It retrieves an island's location if the name exists.`` () =
    let connection = SetupConnection()
    let input = VisitedIslandName
    let expected : Result<Location option, string> = 
        VisitedIslandLocation 
        |> Some 
        |> Ok
    let actual =
        Island.GetByName connection input
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetByName.It retrieves none if the name does not exist.`` () =
    let connection = SetupConnection()
    let input = ""
    let expected : Result<Location option, string> = 
        None
        |> Ok
    let actual =
        Island.GetByName connection input
    Assert.AreEqual(expected, actual)