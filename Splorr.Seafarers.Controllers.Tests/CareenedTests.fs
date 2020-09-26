module CareenedTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures

type TestCareenedRunContext 
        (avatarMessagePurger,
        avatarMessageSource,
        avatarShipmateSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink,
        shipmateSingleStatisticSource,
        vesselSingleStatisticSink, 
        vesselSingleStatisticSource) =
    interface AvatarMessages.GetContext with
        member this.avatarMessageSource: AvatarMessageSource = avatarMessageSource
    interface Shipmate.GetStatusContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Avatar.AddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface Avatar.GetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.GetMaximumFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.TransformShipmatesContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
    interface Avatar.CleanHullContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
    interface World.ClearMessagesContext with
        member _.avatarMessagePurger : AvatarMessagePurger = avatarMessagePurger
    interface Vessel.TransformFoulingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

let private functionUnderTest 
        (avatarSingleMetricSink : AvatarSingleMetricSink)= 
    let context = 
        TestCareenedRunContext 
            (avatarMessagePurgerStub,
            avatarMessageSourceDummy,
            avatarShipmateSourceStub,
            avatarSingleMetricSink,
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSinkStub, 
            shipmateSingleStatisticSourceStub, 
            vesselSingleStatisticSinkStub, 
            vesselSingleStatisticSourceStub) :> ServiceContext
    Careened.Run 
        context

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
    let context = 
        TestCareenedRunContext 
            (avatarMessagePurgerStub,
            avatarMessageSourceDummy,
            avatarShipmateSourceStub,
            avatarSingleMetricSinkExplode,
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSinkStub, 
            vesselSingleStatisticSourceStub) :> ServiceContext
    let actual =
        inputWorld
        |> Careened.Run 
            context
            inputSource 
            sinkDummy 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit command.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Quit
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
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
            sinkDummy 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns InvalidInput when given invalid command.`` () =
    let inputWorld = world
    let inputSource = 
        None 
        |> Fixtures.Common.Mock.CommandSource
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
            sinkDummy 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Careened Help when given the Help command.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Help
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
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
            sinkDummy 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Careened Metrics when given the Metrics command.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Metrics
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
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
            sinkDummy 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Careened Inventory when given the Inventory command.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Inventory
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
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
            sinkDummy 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let inputWorld = world
    let inputSource = 
        Command.Status
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
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
            sinkDummy 
            inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea when given the command Weigh Anchor.`` () =
    let inputWorld = world
    let inputSource = 
        Command.WeighAnchor
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let inputSide = Port
    let expected =
        inputWorld
        |> Gamestate.InPlay
        |> Some
    let actual =
        inputWorld
        |> functionUnderTest
            avatarSingleMetricSinkExplode
            inputSource
            sinkDummy
            inputSide
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Careened with a cleaned hull when given the command Clean Hull.`` () =
    let inputWorld = world
    let inputSource = 
        Command.CleanHull
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
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
            sinkDummy 
            inputSide
    Assert.AreEqual(expected, actual)

