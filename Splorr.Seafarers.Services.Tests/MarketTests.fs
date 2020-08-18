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
        CommodityName = ""
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

[<Test>]
let ``ChangeDemand.It adds the given demand to the given markets demand and returns the resulting market.`` () =
    let input = market
    let inputChange = 1.0
    let expected =
        {input with 
            Demand = input.Demand + inputChange}
    let actual =
        input
        |> Market.ChangeDemand inputChange
    Assert.AreEqual(expected, actual)

[<Test>]
let ``ChangeSupply.It adds the given supply to the given markets supply and returns the resulting market.`` () =
    let input = market
    let inputChange = 1.0
    let expected =
        {input with 
            Supply = input.Supply + inputChange}
    let actual =
        input
        |> Market.ChangeSupply inputChange
    Assert.AreEqual(expected, actual)
