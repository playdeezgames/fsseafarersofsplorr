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

    interface IslandCreateContext with 
        member this.islandStatisticTemplateSource: IslandStatisticTemplateSource = islandStatisticTemplateSource
        member this.islandSingleStatisticSink: IslandSingleStatisticSink = islandSingleStatisticSink

    interface WorldPopulateIslandsContext with
        member this.islandFeatureGeneratorSource: IslandFeatureGeneratorSource = islandFeatureGeneratorSource
        member this.islandSingleFeatureSink: IslandSingleFeatureSink = islandSingleFeatureSink
        member this.random: Random = random
        member this.islandSource: IslandSource = islandSource

    interface UtilitySortListRandomlyContext with 
        member this.random : Random = random

    interface WorldNameIslandsContext with
        member this.islandSingleNameSink: IslandSingleNameSink = islandSingleNameSink
        member this.nameSource: TermSource = termNameSource
        member this.islandSource: IslandSource = islandSource

    interface WorldGenerateIslandsContext with
        member this.termNameSource: TermSource = termNameSource
        member this.islandSingleNameSink : IslandSingleNameSink = islandSingleNameSink

    interface WorldCreateContext with
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.rationItemSource: RationItemSource = rationItemSource
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        member this.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member this.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource
        member this.worldSingleStatisticSource: WorldSingleStatisticSource = worldSingleStatisticSource
        member _.shipmateRationItemSink          : ShipmateRationItemSink = shipmateRationItemSink

let private world = 
    avatarId

[<Test>]
let ``Run.It returns Confirm Quit when given Quit command and there is no world.`` () =
    let input = 
        None
    let inputSource = 
        Command.Quit 
        |> Some 
        |> toSource
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
        |> toSource
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
        System.Guid.NewGuid().ToString() 
        |> Command.Start 
        |> Some 
        |> toSource
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
    | Some (Gamestate.AtSea _) -> true
    | _ -> false
    |> Assert.True


[<Test>]
let ``Run.It returns Main Menu when given Start command and there is a world.`` () =
    let inputSource = "" |> Command.Start |> Some |> toSource
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
    let inputSource = Game |> Command.Abandon |> Some |> toSource
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
        |> toSource
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
    let inputSource = Command.Resume |> Some |> toSource
    let expected =
        input 
        |> Gamestate.AtSea 
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
    let inputSource = Command.Resume |> Some |> toSource
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


