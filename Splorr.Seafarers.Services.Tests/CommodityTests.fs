module CommodityTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open IslandTestFixtures
open System

type TestCommodityGetList(commoditySource) =
    interface Commodity.GetCommoditiesContext with
        member this.commoditySource: CommoditySource = commoditySource

[<Test>]
let ``GetList.It retrieves the list of commodities.`` () =
    let mutable called = false
    let commoditySource() = 
        called <- true
        Map.empty
    let context = TestCommodityGetList(commoditySource) :> OperatingContext
    let expected = 
        Map.empty
    let actual =
        Commodity.GetCommodities context
    Assert.AreEqual(expected, actual)
    Assert.True(called)
    

