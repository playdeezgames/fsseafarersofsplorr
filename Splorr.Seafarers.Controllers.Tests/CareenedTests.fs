module CareenedTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures

let private functionUnderTest 
        (avatarSingleMetricSink : AvatarSingleMetricSink)= 
    Careened.Run 
        avatarMessagePurgerStub
        avatarMessageSourceStub
        avatarShipmateSourceStub
        avatarSingleMetricSink
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSinkStub
        shipmateSingleStatisticSourceStub
        vesselSingleStatisticSinkStub
        vesselSingleStatisticSourceStub

[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let inputWorld = deadWorld
    let inputSource(): Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let inputSide = Port
    let expectedMessages = []
    let expected =
        expectedMessages
        |> Gamestate.GameOver
        |> Some
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSource - %s"))
    let actual =
        inputWorld
        |> Careened.Run 
            avatarMessagePurgerStub
            avatarMessageSourceStub
            avatarShipmateSourceStub
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            shipmateSingleStatisticSinkStub
            shipmateSingleStatisticSource
            vesselSingleStatisticSinkStub
            vesselSingleStatisticSourceStub
            inputSource 
            sinkStub 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit command.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Quit
        |> Some 
        |> toSource
    let inputSide = Port
    let expected =
        (inputSide, inputWorld)
        |> Gamestate.Careened
        |> Gamestate.ConfirmQuit
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            avatarSingleMetricSinkExplode
            inputSource 
            sinkStub 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns InvalidInput when given invalid command.`` () =
    let inputWorld = world
    let inputSource = 
        None 
        |> toSource
    let inputSide = Port
    let expected =
        ("Maybe try 'help'?",(inputSide, inputWorld)
        |> Gamestate.Careened)
        |> Gamestate.ErrorMessage
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            avatarSingleMetricSinkExplode
            inputSource 
            sinkStub 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Careened Help when given the Help command.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Help
        |> Some 
        |> toSource
    let inputSide = Port
    let expected =
        (inputSide, inputWorld)
        |> Gamestate.Careened
        |> Gamestate.Help
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            avatarSingleMetricSinkExplode
            inputSource 
            sinkStub 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Careened Metrics when given the Metrics command.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Metrics
        |> Some 
        |> toSource
    let inputSide = Port
    let expected =
        (inputSide, inputWorld)
        |> Gamestate.Careened
        |> Gamestate.Metrics
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            avatarSingleMetricSinkExplode
            inputSource 
            sinkStub 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Careened Inventory when given the Inventory command.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Inventory
        |> Some 
        |> toSource
    let inputSide = Port
    let expected =
        (inputSide, inputWorld)
        |> Gamestate.Careened
        |> Gamestate.Inventory
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            avatarSingleMetricSinkExplode
            inputSource 
            sinkStub 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Status
        |> Some 
        |> toSource
    let inputSide = Port
    let expected =
        (inputSide, inputWorld)
        |> Gamestate.Careened
        |> Gamestate.Status
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            avatarSingleMetricSinkExplode
            inputSource 
            sinkStub 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea when given the command Weigh Anchor.`` () =
    let inputWorld = world
    let inputSource = 
        Command.WeighAnchor
        |> Some 
        |> toSource
    let inputSide = Port
    let expected =
        inputWorld
        |> Gamestate.AtSea
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            avatarSingleMetricSinkExplode
            inputSource
            sinkStub
            inputSide
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Careened with a cleaned hull when given the command Clean Hull.`` () =
    let inputWorld = world
    let inputSource = 
        Command.CleanHull
        |> Some 
        |> toSource
    let inputSide = Port
    let expected =
        (inputSide, inputWorld)
        |> Gamestate.Careened
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            (assertAvatarSingleMetricSink [Metric.CleanedHull, 1UL])
            inputSource 
            sinkStub 
            inputSide
    Assert.AreEqual(expected, actual)

