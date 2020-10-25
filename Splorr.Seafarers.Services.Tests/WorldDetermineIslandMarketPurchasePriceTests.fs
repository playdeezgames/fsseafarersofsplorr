module WorldDetermineIslandMarketPurchasePriceTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``DetermineIslandMarketPurchasePrice.It determines an island market purchase price.`` () =
    let calledGetItem = ref false
    let calledGetCommodities = ref false
    let calledIslandMarketSource = ref false
    let context = Contexts.TestContext()
    (context :> Item.GetContext).itemSingleSource := Spies.Source(calledGetItem, Some Dummies.ValidItemDescription)
    (context :> Commodity.GetCommoditiesContext).commoditySource := Spies.Source(calledGetCommodities, Dummies.ValidCommodityTable)
    (context :> IslandMarket.DeterminePriceContext).islandMarketSource := Spies.Source(calledIslandMarketSource, Dummies.ValidMarketTable)
    let actual = 
        World.DetermineIslandMarketPurchasePrice
            context
            0UL
            Dummies.ValidIslandLocation
    Assert.AreEqual(7.875, actual)
    Assert.IsTrue(calledGetItem.Value)
    Assert.IsTrue(calledGetCommodities.Value)
    Assert.IsTrue(calledIslandMarketSource.Value)


