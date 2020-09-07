module ItemListTests

open NUnit.Framework
open Splorr.Seafarers.Controllers
open DockedTestFixtures
open CommonTestFixtures
open AtSeaTestFixtures
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type TestItemListRunContext
        (commoditySource,
        islandMarketSource,
        itemSingleSource) =
    interface ItemListRunContext
    interface ItemDetermineSalePriceContext with
        member this.commoditySource: CommoditySource = commoditySource
        member this.islandMarketSource: IslandMarketSource = islandMarketSource
        member this.itemSingleSource: ItemSingleSource = itemSingleSource
    interface ItemDeterminePurchasePriceContext with
        member this.commoditySource: CommoditySource = commoditySource
        member this.islandMarketSource: IslandMarketSource = islandMarketSource


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
            (fun x -> atSeaItemSource() |> Map.tryFind x )) 
        :> ItemListRunContext
    let actual = 
        (inputLocation, inputWorld)
        ||> ItemList.Run 
            context
            avatarMessageSourceDummy
            atSeaCommoditySource 
            atSeaIslandItemSource 
            atSeaIslandMarketSource 
            islandSourceStub
            atSeaItemSource 
            shipmateSingleStatisticSourceStub
            sinkDummy
    Assert.AreEqual(expected, actual)