﻿module ItemTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

let internal commodities =
    [(1UL, {CommodityName=""; BasePrice=10.0; PurchaseFactor=0.0; SaleFactor=0.0; Discount=0.1})] |> Map.ofList
let internal markets =
    [(1UL,{Demand=3.0;Supply=2.0})] |> Map.ofList
let internal itemDescriptor = 
    {
        ItemName=""
        Commodities = [(1UL, 1.0)]|>Map.ofList
        Occurrence=0.0
        Tonnage = 1.0
    }
type TestItemDetermineSalePriceContext
        (commoditySource,
        islandMarketSource,
        itemSingleSource) =
    interface ItemDetermineSalePriceContext with
        member _.commoditySource                : CommoditySource               = commoditySource  
        member _.islandMarketSource             : IslandMarketSource            = islandMarketSource            
        member _.itemSingleSource               : ItemSingleSource              = itemSingleSource

type TestItemDeterminePurchasePriceContext
        (commoditySource,
        islandMarketSource,
        itemSingleSource) = 
    interface ItemDeterminePurchasePriceContext with
        member _.commoditySource                : CommoditySource               =commoditySource  
        member _.islandMarketSource             : IslandMarketSource            =islandMarketSource  
        member _.itemSingleSource               : ItemSingleSource               = itemSingleSource

[<Test>]
let ``DetermineSalePrice.It calculates the sale price of an item in a given set of markets with given commodities.`` () =
    let input = itemDescriptor
    let inputCommodities = commodities
    let inputMarkets = markets
    let givenLocation = (0.0, 0.0)
    let expected = 15.0
    let context : ItemDetermineSalePriceContext = 
        TestItemDetermineSalePriceContext
            ((fun () -> inputCommodities),
            (fun(_) -> inputMarkets),
            (fun(_)->input |> Some)) 
        :> ItemDetermineSalePriceContext
    let actual = 
        (0UL, givenLocation)
        ||> Item.DetermineSalePrice 
            context
    Assert.AreEqual(expected, actual)

[<Test>]
let ``DeterminePurchasePrice.It calculates the purchase price of an item in a given set of markets with given commodities.`` () =
    let input = itemDescriptor
    let inputCommodities = commodities
    let inputMarkets = markets
    let givenLocation = (0.0, 0.0)
    let expected = 13.5
    let context = 
        TestItemDeterminePurchasePriceContext
            ((fun () -> inputCommodities),
            (fun(_) -> inputMarkets),
            (fun(_)->input |> Some))
        :> ItemDeterminePurchasePriceContext
    let actual = 
        (0UL, givenLocation)
        ||> Item.DeterminePurchasePrice 
            context
    Assert.AreEqual(expected, actual)
