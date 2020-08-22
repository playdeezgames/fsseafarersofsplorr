module MainMenuTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open CommonTestFixtures
open AtSeaTestFixtures

let private world = 
    World.Create 
        avatarIslandSingleMetricSinkStub
        avatarJobSinkStub
        termNameSource
        worldSingleStatisticSourceStub
        shipmateStatisticTemplateSourceStub
        shipmateSingleStatisticSinkStub
        rationItemSourceStub
        vesselStatisticTemplateSourceStub
        vesselStatisticSinkStub
        vesselSingleStatisticSourceStub
        shipmateRationItemSinkStub
        random 
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
    let actual =
        input
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            inputSource 
            sinkStub
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
    let actual =
        input
        |> Some
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            inputSource 
            sinkStub 
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
    let actual =
        input
        |> MainMenu.Run
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            (fun()->None) 
            sinkStub 
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
    let actual =
        input
        |> Some
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            (fun()->None) 
            sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea when given Start command and there is no world.`` () =
    let input = None
    let inputSource = System.Guid.NewGuid().ToString() |> Command.Start |> Some |> toSource
    let avatarJobSink (_) (actual:Job option) = 
        Assert.AreEqual(None, actual)
    let actual =
        input
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            inputSource 
            sinkStub 
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
    let actual =
        input
        |> Some
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            inputSource 
            sinkStub 
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
    let actual =
        input
        |> Some
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            inputSource 
            sinkStub 
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
    let actual =
        input
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            inputSource 
            sinkStub 
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
    let actual =
        input
        |> Some
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            inputSource 
            sinkStub
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
    let actual =
        input
        |> MainMenu.Run 
            avatarIslandSingleMetricSinkStub
            avatarJobSink
            rationItemSourceStub
            shipmateRationItemSinkStub
            shipmateSingleStatisticSinkStub
            shipmateStatisticTemplateSourceStub
            termNameSource
            vesselSingleStatisticSourceStub
            vesselStatisticSinkStub
            vesselStatisticTemplateSourceStub
            worldSingleStatisticSourceStub
            inputSource 
            sinkStub 
    Assert.AreEqual(expected, actual)


