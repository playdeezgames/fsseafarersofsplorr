module MarketTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

let private market =
    {
        Supply = 1.0
        Demand = 1.0
    }

let private descriptor =
    {
        Name = ""
        BasePrice = 10.0
        PurchaseFactor = 0.0
        SaleFactor = 0.0
        Discount = 0.1
    }

[<Test>]
let ``DetermineSalePrice.It calculates the selling price for the the described commodity in the given market.`` () =
    let input = market
    let expected = descriptor.BasePrice * market.Demand / market.Supply
    let actual =
        input
        |> Market.DetermineSalePrice descriptor
    Assert.AreEqual(expected, actual)

[<Test>]
let ``DeterminePurchasePrice.It calculates the buying price for the described commodity in the given market.`` () =
    let input = market
    let expected = descriptor.BasePrice * market.Demand / market.Supply * (1.0 - descriptor.Discount)
    let actual =
        input
        |> Market.DeterminePurchasePrice descriptor
    Assert.AreEqual(expected, actual)
