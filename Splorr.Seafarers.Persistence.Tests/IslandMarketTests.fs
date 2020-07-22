module IslandMarketTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

[<Test>]
let ``GetForIsland.It returns nothing when there are no markets at the given location.`` () =
    let connection = SetupConnection()
    let input = (10.0, 10.0)
    let expected : Result<Map<uint64, Market>,string> =
        Map.empty |> Ok
    let actual =
        input 
        |> IslandMarket.GetForIsland connection
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetForIsland.It returns the map of markets when there are markets at the given location.`` () =
    let connection = SetupConnection()
    let input = (0.0, 0.0)
    let expected : Result<Map<uint64, Market>,string> =
        Map.empty
        |> Map.add 1UL {Supply=1.0; Demand=1.0}
        |> Map.add 2UL {Supply=2.0; Demand=2.0}
        |> Map.add 3UL {Supply=3.0; Demand=3.0}
        |> Ok
    let actual =
        input 
        |> IslandMarket.GetForIsland connection
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CreateForIsland.It adds markets when there are no markets at the given location and given new markets to add.`` () =
    let connection = SetupConnection()
    let inputLocation = (10.0, 10.0)
    let inputMarkets = 
        Map.empty
        |> Map.add 1UL {Supply=1.0; Demand=1.0}
    let expected : Result<unit, string> =
        () |> Ok
    let actual =
        inputMarkets
        |> IslandMarket.CreateForIsland connection inputLocation 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CreateForIsland.It removes markets when there are markets at the given location and given no new markets.`` () =
    let connection = SetupConnection()
    let inputLocation = (0.0, 0.0)
    let inputMarkets = 
        Map.empty
    let expected : Result<unit, string> =
        () |> Ok
    let actualInitialCount = 
        match IslandMarket.GetForIsland connection inputLocation with
        | Ok x -> x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "An error happened: '%s'")
            0
    Assert.AreEqual(3, actualInitialCount);
    let actual =
        inputMarkets
        |> IslandMarket.CreateForIsland connection inputLocation 
    Assert.AreEqual(expected, actual)
    let actualFinalCount = 
        match IslandMarket.GetForIsland connection inputLocation with
        | Ok x -> x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "An error happened: '%s'")
            0
    Assert.AreEqual(0, actualFinalCount);

[<Test>]
let ``CreateForIsland.It replaces markets when there are markets at the given location when given new markets.`` () =
    let connection = SetupConnection()
    let inputLocation = (0.0, 0.0)
    let inputMarkets = 
        Map.empty
        |> Map.add 1UL {Supply=1.0; Demand=1.0}
    let expected : Result<unit, string> =
        () |> Ok
    let actualInitialCount = 
        match IslandMarket.GetForIsland connection inputLocation with
        | Ok x -> x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "An error happened: '%s'")
            0
    Assert.AreEqual(3, actualInitialCount);
    let actual =
        inputMarkets
        |> IslandMarket.CreateForIsland connection inputLocation 
    Assert.AreEqual(expected, actual)
    let actualFinalCount = 
        match IslandMarket.GetForIsland connection inputLocation with
        | Ok x -> x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "An error happened: '%s'")
            0
    Assert.AreEqual(1, actualFinalCount);

[<Test>]
let ``SetForIsland.It creates a market if one does not already exist.`` () =
    let connection = SetupConnection()
    let inputLocation = (10.0, 10.0)
    let inputCommodityId = 1UL
    let inputMarket = {Supply=1.0; Demand=1.0}
    let actualInitialCount = 
        match IslandMarket.GetForIsland connection inputLocation with
        | Ok x -> x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "An error happened: '%s'")
            0
    Assert.AreEqual(0, actualInitialCount);
    let expected : Result<unit, string> =
        () |> Ok
    let actual =
        IslandMarket.SetForIsland connection inputLocation (inputCommodityId, inputMarket)
    Assert.AreEqual(expected, actual)
    let actualFinalCount = 
        match IslandMarket.GetForIsland connection inputLocation with
        | Ok x -> x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "An error happened: '%s'")
            0
    Assert.AreEqual(1, actualFinalCount);

[<Test>]
let ``SetForIsland.It replaces a market if one does already exists.`` () =
    let connection = SetupConnection()
    let inputLocation = (0.0, 0.0)
    let inputCommodityId = 1UL
    let inputMarket = {Supply=1.0; Demand=1.0}
    let actualInitialCount = 
        match IslandMarket.GetForIsland connection inputLocation with
        | Ok x -> x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "An error happened: '%s'")
            0
    Assert.AreEqual(3, actualInitialCount);
    let expected : Result<unit, string> =
        () |> Ok
    let actual =
        IslandMarket.SetForIsland connection inputLocation (inputCommodityId, inputMarket)
    Assert.AreEqual(expected, actual)
    let actualFinalCount = 
        match IslandMarket.GetForIsland connection inputLocation with
        | Ok x -> x.Count
        | Error message -> 
            Assert.Fail (message |> sprintf "An error happened: '%s'")
            0
    Assert.AreEqual(3, actualFinalCount);
