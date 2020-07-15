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
    Assert.AreEqual("????", actual)

[<Test>]
let ``GetDisplayName.It returns the island's name when there is a visit count.`` () =
    let name = "Uno"
    let actual = 
        {Island.Create() with 
            AvatarVisits = Map.empty |> Map.add avatarId {VisitCount=0u; LastVisit=None}
            } |> Island.SetName name |> Island.GetDisplayName avatarId
    Assert.AreEqual(name, actual)

[<Test>]
let ``AddVisit.It increases visit count to one and sets last visit to given turn when there is no last visit and no visit count.`` () =
    let turn = 100.0
    let actual =
        unvisitedIsland
        |> Island.AddVisit turn avatarId
    Assert.AreEqual(1u, actual.AvatarVisits.[avatarId].VisitCount)
    Assert.AreEqual(Some turn, actual.AvatarVisits.[avatarId].LastVisit)

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when there is no last visit.`` () =
    let turn = 100.0
    let actual = 
        visitedIslandNoLastVisit
        |> Island.AddVisit turn avatarId
    Assert.AreEqual(1u, actual.AvatarVisits.[avatarId].VisitCount)
    Assert.AreEqual(Some turn, actual.AvatarVisits.[avatarId].LastVisit)

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when the given turn is after the last visit.`` () =
    let turn = 100.0
    let actual = 
        visitedIsland
        |> Island.AddVisit turn avatarId
    Assert.AreEqual(visitedIsland.AvatarVisits.[avatarId].VisitCount + 1u, actual.AvatarVisits.[avatarId].VisitCount)
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
            AvatarVisits = Map.empty |> Map.add avatarId {VisitCount=0u;LastVisit=None}}
    let actual =
        input
        |> Island.MakeKnown avatarId
    Assert.AreEqual(0u, actual.AvatarVisits.[avatarId].VisitCount)
    Assert.AreEqual(expected.AvatarVisits.[avatarId].LastVisit, actual.AvatarVisits.[avatarId].LastVisit)

[<Test>]
let ``GenerateCommodities.It does nothing when commodities already exists for the given island.`` () =
    let input = commodityIsland
    let expected = input
    let actual = 
        input
        |> Island.GenerateCommodities random commodities
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GenerateCommodities.It generates commodities when the given island has no commodities.`` () =
    let input = noCommodityIsland
    let actual = 
        input
        |> Island.GenerateCommodities random commodities
    Assert.AreEqual(commodities.Count, actual.Markets.Count)

[<Test>]
let ``GenerateItems.It has no effect when the given island already has items in the shop.`` () =
    let input = shopIsland
    let expected = shopIsland
    let actual = 
        input
        |> Island.GenerateItems random items
    Assert.AreEqual(expected, actual)


[<Test>]
let ``GenerateItems.It generates the shop when the given island has not items in the shop.`` () =
    let input = noShopIsland
    let expected = {noShopIsland with Items = noShopIsland.Items |> Set.add 1UL}
    let actual = 
        input
        |> Island.GenerateItems random items
    Assert.AreEqual(expected, actual)

[<Test>]
let ``UpdateMarketForItemSale.It updates market commodity demands based on the given number of units sold.`` () =
    let input = shopIsland
    let inputQuantity = 1u
    let inputDescriptor = items.[1UL]

    let expectedMarket = 
        {input.Markets.[1UL] with
            Demand = 2.0}
    let expectedMarkets = 
        input.Markets
        |> Map.add 1UL expectedMarket
    let expected = 
        {input with
            Markets = expectedMarkets}

    let actual = 
        input
        |> Island.UpdateMarketForItemSale commodities inputDescriptor inputQuantity

    Assert.AreEqual(expected, actual)


[<Test>]
let ``UpdateMarketForItemPurchase.It updates market commodity supply based on the given number of units purchased.`` () =
    let input = shopIsland
    let inputQuantity = 1u
    let inputDescriptor = items.[1UL]

    let expectedMarket = 
        {input.Markets.[1UL] with
            Supply = 2.0}
    let expectedMarkets = 
        input.Markets
        |> Map.add 1UL expectedMarket
    let expected = 
        {input with
            Markets = expectedMarkets}

    let actual = 
        input
        |> Island.UpdateMarketForItemPurchase commodities inputDescriptor inputQuantity

    Assert.AreEqual(expected, actual)

//[<Test>]
//let ``AddVisit..`` () =
//    raise (System.NotImplementedException "Not Implemented")
