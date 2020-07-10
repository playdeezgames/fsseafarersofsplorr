module ItemTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

let internal commodities =
    [(Commodity.Grain, {Name=""; BasePrice=10.0; PurchaseFactor=0.0; SaleFactor=0.0; Discount=0.1; Occurrence=0.0})] |> Map.ofList
let internal markets =
    [(Commodity.Grain,{Demand=3.0;Supply=2.0;Traded=true})] |> Map.ofList
let internal itemDescriptor = 
    {
        DisplayName=""
        Commodities = [(Commodity.Grain, 1.0)]|>Map.ofList
        Occurrence=0.0
    }

[<Test>]
let ``DetermineSalePrice.It calculates the sale price of an item in a given set of markets with given commodities.`` () =
    let input = itemDescriptor
    let inputCommodities = commodities
    let inputMarkets = markets
    let expected = 15.0
    let actual = 
        input
        |> Item.DetermineSalePrice inputCommodities inputMarkets
    Assert.AreEqual(expected, actual)

[<Test>]
let ``DeterminePurchasePrice.It calculates the purchase price of an item in a given set of markets with given commodities.`` () =
    let input = itemDescriptor
    let inputCommodities = commodities
    let inputMarkets = markets
    let expected = 13.5
    let actual = 
        input
        |> Item.DeterminePurchasePrice inputCommodities inputMarkets
    Assert.AreEqual(expected, actual)
