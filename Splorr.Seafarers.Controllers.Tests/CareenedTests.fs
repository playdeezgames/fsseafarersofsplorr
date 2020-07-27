﻿module CareenedTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures

[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let inputWorld = deadWorld
    let inputSource(): Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let inputSide = Port
    let expected =
        inputWorld.Avatars.[avatarId].Messages
        |> Gamestate.GameOver
        |> Some
    let actual =
        inputWorld
        |> Careened.Run inputSource sinkStub inputSide
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
        |> Careened.Run inputSource sinkStub inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns InvalidInput when given invalid command.`` () =
    let inputWorld = world
    let inputSource = 
        None 
        |> toSource
    let inputSide = Port
    let expected =
        (inputSide, inputWorld)
        |> Gamestate.Careened
        |> Gamestate.InvalidInput
        |> Some
    let actual =
        inputWorld
        |> Careened.Run inputSource sinkStub inputSide
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
        |> Careened.Run inputSource sinkStub inputSide
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
        |> Careened.Run inputSource sinkStub inputSide
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
        |> Careened.Run inputSource sinkStub inputSide
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
        |> Careened.Run inputSource sinkStub inputSide
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
        |> Careened.Run inputSource sinkStub inputSide
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns Careened with a cleaned hull when given the command Clean Hull.`` () =
    let inputVessel = 
        world.Avatars.[avatarId].Vessel
        |> Vessel.TransformFouling Port (fun x -> {x with CurrentValue = x.MaximumValue})
        |> Vessel.TransformFouling Starboard (fun x -> {x with CurrentValue = x.MaximumValue})
    let inputAvatar = 
        {world.Avatars.[avatarId] with
            Vessel = 
                inputVessel}
    let inputWorld = {world with Avatars = world.Avatars |> Map.add avatarId inputAvatar}
    let inputSource = 
        Command.CleanHull
        |> Some 
        |> toSource
    let inputSide = Port
    let expectedVessel = 
        inputVessel
        |> Vessel.TransformFouling Starboard (fun x -> {x with CurrentValue = x.MinimumValue})
    let expectedTurn =
        inputAvatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn] |> Statistic.ChangeCurrentBy 1.0
    let expectedAvatar =
        {inputAvatar with 
            Vessel = expectedVessel}
        |> Avatar.TransformShipmate (Shipmate.SetStatistic AvatarStatisticIdentifier.Turn (expectedTurn |> Some)) 0u
        |> Avatar.AddMetric Metric.CleanedHull 1u
    let expected =
        (inputSide, {inputWorld with Avatars = inputWorld.Avatars |> Map.add avatarId expectedAvatar})
        |> Gamestate.Careened
        |> Some
    let actual =
        inputWorld
        |> Careened.Run inputSource sinkStub inputSide
    Assert.AreEqual(expected, actual)

//[<Test>]
//let ``Run..`` () =
//    raise (System.NotImplementedException "Not implemented")

