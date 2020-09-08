module IslandTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open IslandTestFixtures
open CommonTestFixtures
open System

type TestIslandGetDisplayNameContext
        (avatarIslandSingleMetricSource, 
        islandSingleNameSource)=
    interface IslandGetDisplayNameContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.islandSingleNameSource         : IslandSingleNameSource = islandSingleNameSource

type TestIslandAddVisitContext (avatarIslandSingleMetricSink, avatarIslandSingleMetricSource, epochSecondsSource) =
    interface IslandAddVisitContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource

[<Test>]
let ``GetDisplayName.It returns (unknown) when there is no visit count.`` () =
    let inputLocation = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandSingleNameSource (_) =
        "island name" |> Some
    let context = TestIslandGetDisplayNameContext(avatarIslandSingleMetricSource, islandSingleNameSource) :> IslandGetDisplayNameContext
    let actual = 
        inputLocation
        |> Island.GetDisplayName 
            context
            avatarId
    Assert.AreEqual("(unknown)", actual)

[<Test>]
let ``GetDisplayName.It returns the island's name when there is a visit count.`` () =
    let name = "Uno"
    let inputLocation = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            0UL |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandSingleNameSource (location:Location) =
        if location = inputLocation then
            name
            |> Some
        else
            None
    let context = TestIslandGetDisplayNameContext(avatarIslandSingleMetricSource, islandSingleNameSource) :> IslandGetDisplayNameContext
    let actual = 
        Island.GetDisplayName 
            context
            avatarId
            inputLocation
    Assert.AreEqual(name, actual)

[<Test>]
let ``AddVisit.It increases visit count to one and sets last visit to given turn when there is no last visit and no visit count.`` () =
    let turn = 100UL
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier: AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount
        | AvatarIslandMetricIdentifier.LastVisit ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier:AvatarIslandMetricIdentifier) (value: uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(1UL, value)
        | AvatarIslandMetricIdentifier.LastVisit ->
            Assert.AreEqual(turn, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let epochSecondsSource = (fun () -> turn)
    let context = TestIslandAddVisitContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource, epochSecondsSource) :> IslandAddVisitContext
    Island.AddVisit 
        context
        (fun () -> turn)
        avatarId
        location

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when there is no last visit.`` () =
    let turn = 100UL
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier: AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.LastVisit ->
            (turn + 1UL)
            |> Some
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier:AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(1UL, value)
        | AvatarIslandMetricIdentifier.LastVisit ->
            Assert.AreEqual(turn, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let epochSecondsSource = (fun () -> turn)
    let context = TestIslandAddVisitContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource, epochSecondsSource) :> IslandAddVisitContext
    Island.AddVisit 
        context
        (fun () -> turn) 
        avatarId
        location

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when the given turn is after the last visit.`` () =
    let turn = 100UL
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier: AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount -> 
            1UL 
            |> Some
        | AvatarIslandMetricIdentifier.LastVisit -> 
            0UL 
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier: AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(2UL, value)
        | AvatarIslandMetricIdentifier.LastVisit ->
            Assert.AreEqual(turn, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let epochSecondsSource = (fun () -> turn)
    let context = TestIslandAddVisitContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource, epochSecondsSource) :> IslandAddVisitContext
    Island.AddVisit 
        context
        (fun () -> turn) 
        avatarId
        location

[<Test>]
let ``AddVisit.It does not update visit count when given turn was prior or equal to last visit.`` () =
    let turn = 0UL
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier: AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount -> 
            1UL 
            |> Some
        | AvatarIslandMetricIdentifier.LastVisit -> 
            turn 
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (_) (_)= 
        Assert.Fail("avatarIslandSingleMetricSink")
    let epochSecondsSource = (fun () -> turn)
    let context = TestIslandAddVisitContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource, epochSecondsSource) :> IslandAddVisitContext
    Island.AddVisit 
        context
        (fun () -> turn) 
        avatarId
        location

type TestIslandJobsGenerationContext
        (islandJobSink             : IslandJobSink, 
        islandJobSource            : IslandJobSource,
        termSources                : TermSources,
        worldSingleStatisticSource : WorldSingleStatisticSource) =
    interface IslandJobsGenerationContext with
        member _.islandJobSink   : IslandJobSink = islandJobSink
        member _.islandJobSource : IslandJobSource = islandJobSource

    interface UtilitySortListRandomlyContext with
        member _.random : Random = random

    interface JobCreationContext with
        member _.termSources : TermSources = termSources
        member _.worldSingleStatisticSource : WorldSingleStatisticSource = worldSingleStatisticSource

[<Test>]
let ``GenerateJob.It generates a job when no job is present on the island.`` () =
    let inputLocation = (0.0, 0.0)
    let mutable sinkCalled = false
    let islandJobSink (_) (_) =
        sinkCalled<-true
    let islandJobSource (_) =
        []
    let context : IslandJobsGenerationContext =
        TestIslandJobsGenerationContext
            (islandJobSink,
            islandJobSource,
            termSourcesStub,
            worldSingleStatisticSourceStub) 
        :> IslandJobsGenerationContext
    inputLocation
    |> Island.GenerateJobs 
        context
        singleDestination
    Assert.IsTrue(sinkCalled)

[<Test>]
let ``GenerateJob.It does nothing when no job is present on the island and no potential job destinations are given.`` () =
    let inputLocation = (0.0, 0.0)
    let islandJobSink (_) (_) =
        Assert.Fail("islandJobSink")
    let islandJobSource (_) =
        [
            {
                FlavorText=""
                Reward=0.0
                Destination=(0.0, 0.0)
            }
        ]
    let context : IslandJobsGenerationContext =
        TestIslandJobsGenerationContext
            (islandJobSink,
            islandJobSource,
            termSourcesStub,
            worldSingleStatisticSourceStub) 
        :> IslandJobsGenerationContext
    inputLocation
    |> Island.GenerateJobs 
        context 
        Set.empty

[<Test>]
let ``MakeKnown.It does nothing when the given island is already known.`` () =
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            0UL |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (_) (_)= 
        Assert.Fail("avatarIslandSingleMetricSink")
    Island.MakeKnown 
        avatarIslandSingleMetricSink
        avatarIslandSingleMetricSource
        avatarId
        location

[<Test>]
let ``MakeKnown.It mutates the island's visit count to Some 0 when the given island is not known.`` () =
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier: AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(0UL, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    Island.MakeKnown 
        avatarIslandSingleMetricSink
        avatarIslandSingleMetricSource
        avatarId
        location

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
    |> Island.GenerateItems islandItemSource islandItemSink random itemSource


[<Test>]
let ``GenerateItems.It generates the shop when the given island has no items in the shop.`` () =
    let mutable counter = 0
    let input = (0.0,0.0)
    let islandItemSource (l:Location) = Set.empty
    let islandItemSink (_) (_) =
        counter <- counter + 1
    input
    |> Island.GenerateItems islandItemSource islandItemSink random itemSource
    Assert.AreEqual(1,counter)

[<Test>]
let ``UpdateMarketForItemSale.It updates market commodity demands based on the given number of units sold.`` () =
    let input = (0.0, 0.0)
    let inputQuantity = 1UL
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
    let islandSingleMarketSource (_) (_) =
        None
    input
    |> Island.UpdateMarketForItemSale islandSingleMarketSource islandSingleMarketSink commoditySource inputDescriptor inputQuantity


[<Test>]
let ``UpdateMarketForItemPurchase.It updates market commodity supply based on the given number of units purchased.`` () =
    let input = (0.0, 0.0)
    let inputQuantity = 1UL
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
    |> Island.UpdateMarketForItemPurchase islandSingleMarketSource islandMarketSink commoditySource inputDescriptor inputQuantity

type TestIslandCreateContext
        (islandSingleStatisticSink : IslandSingleStatisticSink,
        islandStatisticTemplateSource : IslandStatisticTemplateSource)=
    interface IslandCreateContext with
        member this.islandStatisticTemplateSource: IslandStatisticTemplateSource = islandStatisticTemplateSource
        member _.islandSingleStatisticSink : IslandSingleStatisticSink = islandSingleStatisticSink

[<Test>]
let ``Create.It sets up statistics for an island.`` () =
    let givenLocation = (1.0, 2.0)
    let mutable statisticCounter : uint64 = 0UL
    let islandSingleStatisticSink (location:Location) (identifier: IslandStatisticIdentifier, statistic: Statistic option) =
        statisticCounter <- statisticCounter + 1UL
        Assert.AreEqual(givenLocation, location)
    let islandStatisticTemplateSource () =
        Map.empty
        |> Map.add 
            IslandStatisticIdentifier.CareenDistance
            {
                StatisticTemplate.StatisticName="careen distance"
                MinimumValue = 0.0
                MaximumValue = 100.0
                CurrentValue = 50.0
            }
    let context : IslandCreateContext =
        TestIslandCreateContext
            (islandSingleStatisticSink,
            islandStatisticTemplateSource) 
        :> IslandCreateContext
    Island.Create context givenLocation
    Assert.AreEqual(1UL, statisticCounter)
