module ItemListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Services

type TestItemListRunContext
        (avatarMessageSource,
        commoditySource,
        islandItemSource,
        islandMarketSource,
        islandSource,
        itemSingleSource,
        shipmateSingleStatisticSource) =
    interface Island.GetItemsContext with
        member this.islandItemSource: IslandItemSource = islandItemSource
    interface Island.GetListContext with
        member this.islandSource: IslandSource = islandSource
    interface Item.GetListContext with
        member this.itemSource: ItemSource = itemSource
    interface AvatarShipmate.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Shipmate.GetStatusContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface IslandMarket.DeterminePriceContext with
        member this.islandMarketSource: IslandMarketSource = islandMarketSource
        member this.itemSingleSource: ItemSingleSource = itemSingleSource

[<Test>]
let ``Run.It returns Docked (at Shop) gamestate.`` () =
    let inputLocation = dockLocation
    let inputWorld = dockWorld
    let expected = 
        inputWorld
        |> Gamestate.InPlay 
        |> Some
    let context = 
        TestItemListRunContext
            (avatarMessageSourceDummy,
            atSeaCommoditySource, 
            atSeaIslandItemSource,
            atSeaIslandMarketSource,
            islandSourceStub,
            (fun x -> atSeaItemSource() |> Map.tryFind x ),
            shipmateSingleStatisticSourceStub) 
        :> ServiceContext
    let actual = 
        (inputLocation, inputWorld)
        ||> ItemList.Run 
            context
            sinkDummy
    Assert.AreEqual(expected, actual)