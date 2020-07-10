﻿module ShopTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures

[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let input = deadDockWorld   
    let inputLocation= deadDockLocation
    let inputSource(): Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let expected =
        input.Avatars.[avatarId].Messages
        |> Gamestate.GameOver
        |> Some
    let actual =
        (avatarId, inputLocation, input)
        |||> Shop.Run inputSource sinkStub 
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and returns Docked (at Shop) gamestate when given an invalid command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let input =
        (avatarId, inputLocation, inputWorld)
    let inputSource = 
        None 
        |> toSource
    let expectedWorld = 
        inputWorld 
        |> World.AddMessages avatarId ["Maybe try 'help'?"]
    let expected = 
        (Shop, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        input
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at Dock) gamestate when given the Dock command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = 
        Command.Dock 
        |> Some 
        |> toSource
    let expected = 
        (Dock, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at ItemList) gamestate when given the Items command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = 
        Command.Items 
        |> Some 
        |> toSource
    let expected = 
        (ItemList, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Help gamestate when given the Help command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = Command.Help |> Some |> toSource
    let expected = 
        (Shop, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Gamestate.Help
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Inventory gamestate when given the Inventory command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = Command.Inventory |> Some |> toSource
    let expected = 
        (Shop, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Gamestate.Inventory
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status gamestate when given the Status command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = Command.Status |> Some |> toSource
    let expected = 
        (Shop, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Gamestate.Status
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit gamestate when given the Quit command.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let inputSource = Command.Quit |> Some |> toSource
    let expected = 
        (Shop, inputLocation, inputWorld)
        |> Gamestate.Docked
        |> Gamestate.ConfirmQuit
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Buy command for a non-existent item.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (1u, "non existent item") |> Command.Buy |> Some |> toSource
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["Round these parts, we don't sell things like that."]
    let expected = 
        (Shop, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Buy command and the avatar does not have enough money to complete the purchase.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = smallWorldDocked |> World.ClearMessages avatarId
    let inputSource = (1u, "item under test") |> Command.Buy |> Some |> toSource
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["You don't have enough money to buy those."]
    let expected = 
        (Shop, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and completes the purchase when given the Buy command and the avatar has enough money.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Money = 1000000.0}
    let inputWorld = {shopWorld with Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
    let inputSource = (1u, "item under test") |> Command.Buy |> Some |> toSource
    let expectedPrice = Item.DetermineSalePrice inputWorld.Commodities inputWorld.Islands.[inputLocation].Markets inputWorld.Items.[Item.Ration]
    let expectedDemand = 
        inputWorld.Islands.[inputLocation].Markets.[Commodity.Grain].Demand + inputWorld.Commodities.[Commodity.Grain].SaleFactor
    let expectedMarket = 
        {inputWorld.Islands.[inputLocation].Markets.[Commodity.Grain] with 
            Demand= expectedDemand}
    let expectedMarkets = 
        inputWorld.Islands.[inputLocation].Markets
        |> Map.add Commodity.Grain expectedMarket
    let expectedIsland = 
        {inputWorld.Islands.[inputLocation] with 
            Markets = expectedMarkets}
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["You complete the purchase."]
        |> World.TransformIsland inputLocation (fun _ -> expectedIsland |> Some)
        |> World.TransformAvatar avatarId (Avatar.AddInventory Item.Ration 1u >> Some)
        |> World.TransformAvatar avatarId (Avatar.SpendMoney expectedPrice >> Some)
    let expected = 
        (Shop, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Sell command for a non-existent item.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (1u, "non existent item") |> Command.Sell |> Some |> toSource
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["Round these parts, we don't buy things like that."]
    let expected = 
        (Shop, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given the Sell command and the avatar does not sufficient items to sell.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputWorld = shopWorld
    let inputSource = (1u, "item under test") |> Command.Sell |> Some |> toSource
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["You don't have enough of those to sell."]
    let expected = 
        (Shop, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message and completes the sale when given the Sell command and the avatar sufficient items to sell.`` () =
    let inputLocation = smallWorldIslandLocation
    let inputItems = [(Item.Ration, 1u)] |> Map.ofList
    let inputAvatar = {shopWorld.Avatars.[avatarId] with Inventory = inputItems}
    let inputWorld = 
        {shopWorld with 
            Avatars = shopWorld.Avatars |> Map.add avatarId inputAvatar}
        |> World.TransformIsland smallWorldIslandLocation 
            (fun i -> 
                {i with Markets = i.Markets |> Map.add Commodity.Grain {Supply = 5.0; Demand =5.0; Traded=true}}
                |> Some)
    let inputSource = (1u, "item under test") |> Command.Sell |> Some |> toSource
    let expectedSupply = 
        inputWorld.Islands.[inputLocation].Markets.[Commodity.Grain].Supply + inputWorld.Commodities.[Commodity.Grain].PurchaseFactor
    let expectedMarket = 
        {inputWorld.Islands.[inputLocation].Markets.[Commodity.Grain] with 
            Supply= expectedSupply}
    let expectedMarkets = 
        inputWorld.Islands.[inputLocation].Markets
        |> Map.add Commodity.Grain expectedMarket
    let expectedIsland = 
        {inputWorld.Islands.[inputLocation] with 
            Markets = expectedMarkets}
    let expectedAvatar = 
        {inputAvatar with 
            Inventory = Map.empty
            Money = 0.5
            Messages = ["You complete the sale."]}
    let expectedWorld =
        inputWorld
        |> World.AddMessages avatarId ["You complete the sale."]
        |> World.TransformIsland inputLocation (fun _ -> expectedIsland |> Some)
        |> World.TransformAvatar avatarId (fun _ -> expectedAvatar |> Some)
    let expected = 
        (Shop, inputLocation, expectedWorld)
        |> Gamestate.Docked
        |> Some
    let actual =
        (avatarId, inputLocation, inputWorld)
        |||> Shop.Run inputSource (sinkStub)
    Assert.AreEqual(expected, actual)
