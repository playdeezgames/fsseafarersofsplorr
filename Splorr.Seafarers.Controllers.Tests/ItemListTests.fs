module ItemListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Services

type TestItemListRunContext
        (commoditySource,
        islandMarketSource,
        itemSingleSource,
        shipmateSingleStatisticSource) =
    interface ItemListRunContext
    interface Avatar.GetPrimaryStatisticContext with
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
            (atSeaCommoditySource, 
            atSeaIslandMarketSource,
            (fun x -> atSeaItemSource() |> Map.tryFind x ),
            shipmateSingleStatisticSourceStub) 
        :> ItemListRunContext
    let actual = 
        (inputLocation, inputWorld)
        ||> ItemList.Run 
            context
            avatarMessageSourceDummy
            atSeaIslandItemSource 
            islandSourceStub
            atSeaItemSource 
            sinkDummy
    Assert.AreEqual(expected, actual)