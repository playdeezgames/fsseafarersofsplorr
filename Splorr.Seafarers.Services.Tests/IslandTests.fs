module IslandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open IslandTestFixtures
open CommonTestFixtures

[<Test>]
let ``Create.It returns a new island.`` () =
    let actual = Island.Create()
    Assert.AreEqual("", actual.Name)

[<Test>]
let ``SetName.It sets the name of a given island to a given name.`` () =
    let name = "Uno"
    let actual = Island.Create() |> Island.SetName name
    Assert.AreEqual(name, actual.Name)

[<Test>]
let ``GetDisplayName.It returns ???? when there is no visit count.`` () =
    let actual = Island.Create() |> Island.SetName "Uno" |> Island.GetDisplayName avatarId
    Assert.AreEqual("(unknown)", actual)

[<Test>]
let ``GetDisplayName.It returns the island's name when there is a visit count.`` () =
    let name = "Uno"
    let actual = 
        {Island.Create() with 
            AvatarVisits = Map.empty |> Map.add avatarId {VisitCount=0u |> Some; LastVisit=None}
            } |> Island.SetName name |> Island.GetDisplayName avatarId
    Assert.AreEqual(name, actual)

[<Test>]
let ``AddVisit.It increases visit count to one and sets last visit to given turn when there is no last visit and no visit count.`` () =
    let turn = 100.0
    let actual =
        unvisitedIsland
        |> Island.AddVisit turn avatarId
    Assert.AreEqual(1u, actual.AvatarVisits.[avatarId].VisitCount.Value)
    Assert.AreEqual(Some turn, actual.AvatarVisits.[avatarId].LastVisit)

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when there is no last visit.`` () =
    let turn = 100.0
    let actual = 
        visitedIslandNoLastVisit
        |> Island.AddVisit turn avatarId
    Assert.AreEqual(1u, actual.AvatarVisits.[avatarId].VisitCount.Value)
    Assert.AreEqual(Some turn, actual.AvatarVisits.[avatarId].LastVisit)

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when the given turn is after the last visit.`` () =
    let turn = 100.0
    let expected = 
        (visitedIsland.AvatarVisits.[avatarId].VisitCount.Value + 1u) |> Some
    let actual = 
        visitedIsland
        |> Island.AddVisit turn avatarId
    Assert.AreEqual(expected, actual.AvatarVisits.[avatarId].VisitCount)
    Assert.AreEqual(Some turn, actual.AvatarVisits.[avatarId].LastVisit)

[<Test>]
let ``AddVisit.It does not update visit count when given turn was prior or equal to last visit.`` () =
    let turn = 0.0
    let actual = 
        visitedIsland
        |> Island.AddVisit turn avatarId
    Assert.AreEqual(visitedIsland, actual)

[<Test>]
let ``GenerateJob.It generates a job when no job is present on the island.`` () =
    let actual =
        visitedIsland
        |> Island.GenerateJobs random rewardRange singleDestination
    Assert.False(actual.Jobs.IsEmpty)

[<Test>]
let ``GenerateJob.It does nothing when no job is present on the island and no potential job destinations are given.`` () =
    let actual =
        visitedIsland
        |> Island.GenerateJobs random rewardRange Set.empty
    Assert.True(actual.Jobs.IsEmpty)

[<Test>]
let ``RemoveJob.It returns the original island and None when the given job index is 0u.`` () =
    let actual =
        jobAvailableIsland
        |> Island.RemoveJob 0u
    Assert.AreEqual((jobAvailableIsland,None),actual)

[<Test>]
let ``RemoveJob.It returns the original island and None when there are no jobs on the given island.`` () =
    let actual =
        visitedIsland
        |> Island.RemoveJob 1u
    Assert.AreEqual((visitedIsland,None),actual)

[<Test>]
let ``RemoveJob.It returns the original island and None whenthe given job index is out of range.`` () =
    let actual =
        jobAvailableIsland
        |> Island.RemoveJob 0xFFFFFFFFu
    Assert.AreEqual((jobAvailableIsland,None),actual)


[<Test>]
let ``RemoveJob.It returns the modified island and the indicated job when the given job index is in range.`` () =
    let actual =
        jobAvailableIsland
        |> Island.RemoveJob 1u
    Assert.AreEqual(({jobAvailableIsland with Jobs = []},Some jobAvailableIsland.Jobs.Head),actual)

[<Test>]
let ``MakeKnown.It does nothing when the given island is already known.`` () =
    let input = visitedIsland
    let expected = visitedIsland
    let actual =
        input
        |> Island.MakeKnown avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``MakeKnown.It mutates the island's visit count to Some 0 when the given island is not known.`` () =
    let input = unvisitedIsland
    let expected = 
        {input with
            AvatarVisits = Map.empty |> Map.add avatarId {VisitCount=0u |> Some;LastVisit=None}}
    let actual =
        input
        |> Island.MakeKnown avatarId
    Assert.AreEqual(0u, actual.AvatarVisits.[avatarId].VisitCount.Value)
    Assert.AreEqual(expected.AvatarVisits.[avatarId].LastVisit, actual.AvatarVisits.[avatarId].LastVisit)

[<Test>]
let ``MakeSeen.It does nothing when the given island is already seen.`` () =
    let input = seenIsland
    let expected = seenIsland
    let actual =
        input
        |> Island.MakeSeen avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``MakeSeen.It does nothing when the given island is already known.`` () =
    let input = visitedIsland
    let expected = visitedIsland
    let actual =
        input
        |> Island.MakeSeen avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``MakeSeen.It mutates the island's visit count to None when the given island is not known or seen.`` () =
    let input = unvisitedIsland
    let expected = 
        {input with
            AvatarVisits = Map.empty |> Map.add avatarId {VisitCount=None;LastVisit=None}}
    let actual =
        input
        |> Island.MakeSeen avatarId
    Assert.AreEqual(None, actual.AvatarVisits.[avatarId].VisitCount)
    Assert.AreEqual(expected.AvatarVisits.[avatarId].LastVisit, actual.AvatarVisits.[avatarId].LastVisit)

[<Test>]
let ``GenerateCommodities.It does nothing when commodities already exists for the given island.`` () =
    let input = (0.0, 0.0)
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL { Supply=1.0; Demand=1.0 }
    let islandMarketSink (_) (_) =
        Assert.Fail("This should not be called.")
    input
    |> Island.GenerateCommodities commoditySource islandMarketSource islandMarketSink random

[<Test>]
let ``GenerateCommodities.It generates commodities when the given island has no commodities.`` () =
    let input = (0.0, 0.0)
    let islandMarketSource (_) =
        Map.empty
    let islandMarketSink (_) (markets:Map<uint64, Market>) =
        Assert.AreEqual(commodities.Count, markets.Count)
    input
    |> Island.GenerateCommodities commoditySource islandMarketSource islandMarketSink random


[<Test>]
let ``GenerateItems.It has no effect when the given island already has items in the shop.`` () =
    let input = (0.0,0.0)
    let islandItemSource (l:Location) = [1UL] |> Set.ofList
    let islandItemSink (_) (_) =
        Assert.Fail("This should not be called")
    input
    |> Island.GenerateItems islandItemSource islandItemSink random items


[<Test>]
let ``GenerateItems.It generates the shop when the given island has no items in the shop.`` () =
    let mutable counter = 0
    let input = (0.0,0.0)
    let islandItemSource (l:Location) = Set.empty
    let islandItemSink (_) (_) =
        counter <- counter + 1
    input
    |> Island.GenerateItems islandItemSource islandItemSink random items
    Assert.AreEqual(1,counter)

[<Test>]
let ``UpdateMarketForItemSale.It updates market commodity demands based on the given number of units sold.`` () =
    let input = (0.0, 0.0)
    let inputQuantity = 1u
    let inputDescriptor = items.[1UL]
    let market = {Supply=1.0;Demand=1.0}
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL market
    let expectedMarket = 
        {Supply=1.0;Demand=market.Demand+1.0}
    let islandSingleMarketSink (_) (commodityId, market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(expectedMarket, market)
    input
    |> Island.UpdateMarketForItemSale islandMarketSource islandSingleMarketSink commodities inputDescriptor inputQuantity


[<Test>]
let ``UpdateMarketForItemPurchase.It updates market commodity supply based on the given number of units purchased.`` () =
    let input = (0.0, 0.0)
    let inputQuantity = 1u
    let inputDescriptor = items.[1UL]
    let market = {Supply=1.0; Demand=1.0}
    let islandMarketSource (_) =
        Map.empty
        |> Map.add 1UL market
    let islandSingleMarketSource location commodity =
        islandMarketSource location
        |> Map.tryFind commodity

    let expectedMarket = 
        {market with
            Supply = 2.0}
    let islandMarketSink (_) (commodityId: uint64,market:Market) =
        Assert.AreEqual(1UL, commodityId)
        Assert.AreEqual(expectedMarket, market)
    input
    |> Island.UpdateMarketForItemPurchase islandSingleMarketSource islandMarketSink commodities inputDescriptor inputQuantity

//[<Test>]
//let ``AddVisit..`` () =
//    raise (System.NotImplementedException "Not Implemented")
