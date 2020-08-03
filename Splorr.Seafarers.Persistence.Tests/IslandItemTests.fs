module IslandItemTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence

[<Test>]
let ``GetForIsland.It retrieves the items sold on a particular island.`` () =
    use connection = 
        SetupConnection()
    let input = 
        (0.0, 0.0)
    let expected : Result<Set<uint64>,string>  =
        [
            1UL
            2UL
            3UL
        ] 
        |> Set.ofList 
        |> Ok
    let actual =
        input
        |> IslandItem.GetForIsland connection
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetForIsland.It returns an error when an island is not found at the location.`` () =
    use connection = 
        SetupConnection()
    let input = 
        (10.0, 10.0)
    let expected : Result<Set<uint64>,string> =
        Set.empty 
        |> Ok
    let actual =
        input
        |> IslandItem.GetForIsland connection
    Assert.AreEqual(expected, actual)

[<Test>]
let ``ExistForIsland.It returns true when items are found for a given island location.`` () =
    use connection = 
        SetupConnection()
    let input = 
        (0.0, 0.0)
    let expected : Result<bool, string> = 
        true 
        |> Ok
    let actual =
        input
        |> IslandItem.ExistForIsland connection
    Assert.AreEqual(expected, actual)

[<Test>]
let ``ExistForIsland.It returns false when items are not found for a given island location.`` () =
    use connection = 
        SetupConnection()
    let input = 
        (10.0, 10.0)
    let expected : Result<bool, string> = 
        false 
        |> Ok
    let actual =
        input
        |> IslandItem.ExistForIsland connection
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CreateForIsland.It makes new records when no previous records exist.`` () =
    use connection = 
        SetupConnection()
    let input = 
        [ 1UL ] 
        |> Set.ofList
    let inputLocation = 
        (10.0, 10.0)
    let expected : Result<unit, string> = 
        () 
        |> Ok
    let actualInitialCount = 
        match IslandItem.GetForIsland connection inputLocation with
        | Ok x -> 
            x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "IslandItem.GetForIsland returned an error '%s'")
            0
    Assert.AreEqual(0, actualInitialCount)
    let actual =
        input
        |> IslandItem.CreateForIsland connection inputLocation
    Assert.AreEqual(expected, actual)
    let actualFinalCount = 
        match IslandItem.GetForIsland connection inputLocation with
        | Ok x -> 
            x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "IslandItem.GetForIsland returned an error '%s'")
            0
    Assert.AreEqual(input.Count, actualFinalCount)
    
[<Test>]
let ``CreateForIsland.It removes old records when previous records exist.`` () =
    use connection = SetupConnection()
    let input = 
        Set.empty
    let inputLocation = 
        (0.0, 0.0)
    let expected : Result<unit, string> = 
        () 
        |> Ok
    let actualInitialCount = 
        match IslandItem.GetForIsland connection inputLocation with
        | Ok x -> 
            x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "IslandItem.GetForIsland returned an error '%s'")
            0
    Assert.AreEqual(3, actualInitialCount)
    let actual =
        input
        |> IslandItem.CreateForIsland connection inputLocation
    Assert.AreEqual(expected, actual)
    let actualFinalCount = 
        match IslandItem.GetForIsland connection inputLocation with
        | Ok x -> 
            x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "IslandItem.GetForIsland returned an error '%s'")
            0
    Assert.AreEqual(input.Count, actualFinalCount)

[<Test>]
let ``CreateForIsland.It replaces old records when previous records exist.`` () =
    use connection = 
        SetupConnection()
    let input = 
        [ 1UL ] 
        |> Set.ofList;
    let inputLocation = 
        (0.0, 0.0)
    let expected : Result<unit, string> = 
        () 
        |> Ok
    let actualInitialCount = 
        match IslandItem.GetForIsland connection inputLocation with
        | Ok x -> 
            x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "IslandItem.GetForIsland returned an error '%s'")
            0
    Assert.AreEqual(3, actualInitialCount)
    let actual =
        input
        |> IslandItem.CreateForIsland connection inputLocation
    Assert.AreEqual(expected, actual)
    let actualFinalCount = 
        match IslandItem.GetForIsland connection inputLocation with
        | Ok x -> 
            x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "IslandItem.GetForIsland returned an error '%s'")
            0
    Assert.AreEqual(input.Count, actualFinalCount)
