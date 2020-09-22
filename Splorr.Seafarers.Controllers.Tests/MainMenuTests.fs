module MainMenuTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open CommonTestFixtures
open AtSeaTestFixtures
open System

type TestWorldCreateContext
        (avatarIslandSingleMetricSink,
        avatarJobSink,
        islandFeatureGeneratorSource,
        islandSingleFeatureSink,
        islandSingleNameSink,
        islandSingleStatisticSink,
        islandSource,
        islandStatisticTemplateSource,
        rationItemSource,
        shipmateRationItemSink,
        shipmateSingleStatisticSink,
        shipmateStatisticTemplateSource,
        termNameSource,
        vesselSingleStatisticSource,
        vesselStatisticSink,
        vesselStatisticTemplateSource,
        worldSingleStatisticSource) =

    interface Island.CreateContext with 
        member _.islandStatisticTemplateSource: IslandStatisticTemplateSource = islandStatisticTemplateSource
        member _.islandSingleStatisticSink: IslandSingleStatisticSink = islandSingleStatisticSink
    interface WorldGenerateIslandNamesContext
    interface WorldPopulateIslandsContext with
        member _.islandFeatureGeneratorSource: IslandFeatureGeneratorSource = islandFeatureGeneratorSource
        member _.islandSingleFeatureSink: IslandSingleFeatureSink = islandSingleFeatureSink
        member _.islandSource: IslandSource = islandSource

    interface IslandFeatureGenerator.GenerateContext with
        member _.random: Random = Fixtures.Common.Dummy.Random

    interface Utility.SortListRandomlyContext with 
        member _.random : Random = Fixtures.Common.Dummy.Random

    interface WorldGenerateIslandNameContext with
        member this.random: Random = Fixtures.Common.Dummy.Random

    interface WorldNameIslandsContext with
        member _.islandSingleNameSink: IslandSingleNameSink = islandSingleNameSink
        member _.nameSource: TermSource = termNameSource
        member _.islandSource: IslandSource = islandSource

    interface WorldGenerateIslandsContext with
        member this.islandSource: IslandSource = islandSource
        member this.random: Random = Fixtures.Common.Dummy.Random
        member _.termNameSource: TermSource = termNameSource
        member _.islandSingleNameSink : IslandSingleNameSink = islandSingleNameSink

    interface Vessel.CreateContext with
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource

    interface Shipmate.CreateContext with
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource

    interface Avatar.CreateContext with
        member _.avatarJobSink: AvatarJobSink = avatarJobSink

    interface Avatar.GetPositionContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface WorldUpdateChartsContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.islandSource: IslandSource = islandSource
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

    interface WorldCreateContext with
        member _.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarJobSink: AvatarJobSink = avatarJobSink
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource
        member _.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource
        member _.shipmateRationItemSink          : ShipmateRationItemSink = shipmateRationItemSink

let private world = 
    Fixtures.Common.Dummy.AvatarId

[<Test>]
let ``Run.It returns Confirm Quit when given Quit command and there is no world.`` () =
    let input = 
        None
    let inputSource = 
        Command.Quit 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input 
        |> Gamestate.MainMenu 
        |> Gamestate.ConfirmQuit 
        |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context : WorldCreateContext =
        TestWorldCreateContext 
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext
    let actual =
        input
        |> MainMenu.Run 
            context
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Main Menu when given Quit command and there is a world.`` () =
    let input = world
    let inputSource = 
        Command.Quit 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        ("Invalid command.", 
            input 
            |> Some 
            |> Gamestate.MainMenu) 
        |> Gamestate.ErrorMessage |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext
    let actual =
        input
        |> Some
        |> MainMenu.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Main Menu when given invalid command and there is no world.`` () =
    let input = None
    let expected = 
        ("Invalid command.", 
            input 
            |> Gamestate.MainMenu) 
        |> Gamestate.ErrorMessage 
        |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext
    let actual =
        input
        |> MainMenu.Run
            context
            (fun()->None) 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Main Menu when given invalid command and there is a world.`` () =
    let input = world
    let expected = 
        ("Invalid command.", 
            input 
            |> Some 
            |> Gamestate.MainMenu) 
        |> Gamestate.ErrorMessage 
        |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext
    let actual =
        input
        |> Some
        |> MainMenu.Run 
            context
            (fun()->None) 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea when given Start command and there is no world.`` () =
    let input = 
        None
    let inputSource = 
        Guid.NewGuid().ToString() 
        |> Command.Start 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let avatarJobSink (_) (actual:Job option) = 
        Assert.AreEqual(None, actual)
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext

    let actual =
        input
        |> MainMenu.Run 
            context
            inputSource 
            sinkDummy 
    //the command creates a world, which has randomness in the generation
    //so it is very brittle to figure out what the expected would be
    match actual with
    | Some (Gamestate.InPlay _) -> true
    | _ -> false
    |> Assert.True


[<Test>]
let ``Run.It returns Main Menu when given Start command and there is a world.`` () =
    let inputSource = "" |> Command.Start |> Some |> Fixtures.Common.Mock.CommandSource
    let input = world
    let expected = 
        ("Invalid command.", 
            input 
            |> Some 
            |> Gamestate.MainMenu) 
        |> Gamestate.ErrorMessage 
        |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext

    let actual =
        input
        |> Some
        |> MainMenu.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Main Menu with no world when given Abandon Game command and there is a world.`` () =
    let input = world
    let inputSource = Game |> Command.Abandon |> Some |> Fixtures.Common.Mock.CommandSource
    let expected = 
        None 
        |> Gamestate.MainMenu 
        |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext

    let actual =
        input
        |> Some
        |> MainMenu.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Main Menu with no world when given Abandon Game command and there is no world.`` () =
    let input = 
        None
    let inputSource =
        Game 
        |> Command.Abandon 
        |> Some
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        ("Invalid command.", 
            None 
            |> Gamestate.MainMenu) 
        |> Gamestate.ErrorMessage 
        |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext

    let actual =
        input
        |> MainMenu.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns At Sea when given Resume command and there is a world.`` () =
    let input = world
    let inputSource = Command.Resume |> Some |> Fixtures.Common.Mock.CommandSource
    let expected =
        input
        |> Gamestate.InPlay 
        |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext

    let actual =
        input
        |> Some
        |> MainMenu.Run 
            context
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Main Menu with no world when given Resume command and there is no world.`` () =
    let input = None
    let inputSource = Command.Resume |> Some |> Fixtures.Common.Mock.CommandSource
    let expected = 
        ("Invalid command.", 
            input 
            |> Gamestate.MainMenu) 
        |> Gamestate.ErrorMessage 
        |> Some
    let avatarJobSink (_) (_) = Assert.Fail("avatarJobSink")
    let context = 
        TestWorldCreateContext
            (avatarIslandSingleMetricSinkStub,
            avatarJobSink,
            islandFeatureGeneratorSourceStub,
            islandSingleFeatureSinkStub,
            islandSingleNameSinkStub,
            islandSingleStatisticSinkStub,
            islandSourceStub,
            islandStatisticTemplateSourceStub,
            rationItemSourceStub,
            shipmateRationItemSinkStub,
            shipmateSingleStatisticSinkStub,
            shipmateStatisticTemplateSourceStub,
            termNameSource,
            vesselSingleStatisticSourceStub,
            vesselStatisticSinkDummy,
            vesselStatisticTemplateSourceDummy,
            worldSingleStatisticSourceStub) :> WorldCreateContext

    let actual =
        input
        |> MainMenu.Run 
            context
            inputSource 
            sinkDummy 
    Assert.AreEqual(expected, actual)


