module WorldDetermineIslandMarketSalePriceTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``DetermineIslandMarketSalePrice.It determines a sales price.`` () =
    let calledGetItem = ref false
    let calledGetCommodities = ref false
    let calledIslandMarketSource = ref false
    let context = Contexts.TestContext()
    (context :> Item.GetContext).itemSingleSource := Spies.Source(calledGetItem, Some Dummies.ValidItemDescription)
    (context :> Commodity.GetCommoditiesContext).commoditySource := Spies.Source(calledGetCommodities, Dummies.ValidCommodityTable)
    (context :> IslandMarket.DeterminePriceContext).islandMarketSource := Spies.Source(calledIslandMarketSource, Dummies.ValidMarketTable)
    let actual = 
        World.DetermineIslandMarketSalePrice
            context
            0UL
            Dummies.ValidIslandLocation
    Assert.AreEqual(11.25, actual)
    Assert.IsTrue(calledGetItem.Value)
    Assert.IsTrue(calledGetCommodities.Value)
    Assert.IsTrue(calledIslandMarketSource.Value)



