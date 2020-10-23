module Dummies

open System
open Splorr.Seafarers.Models

let ValidAvatarId = Guid.NewGuid().ToString()
let ValidJobIndex = 1u
let ValidIslandLocation = (1.0,2.0)
let ValidItemName = "valid item name"
let ValidCommodityId = 0UL
let private ValidCommodityDescriptor 
        : CommodityDescriptor =
    {
        CommodityName = "valid commodity name"
        BasePrice = 10.0
        PurchaseFactor = 0.01
        SaleFactor = 0.02
        Discount = 0.3
    }
let ValidCommodityTable =
    [
        (ValidCommodityId, ValidCommodityDescriptor)
    ]
    |> Map.ofList
let ValidItemDescription = 
    {
        ItemName = ValidItemName
        Commodities = 
            [
                (ValidCommodityId, 0.75)
            ]
            |> Map.ofList
        Occurrence = 1.0
        Tonnage = 0.1
    }
let ValidItemId = 0UL
let ValidItemTable=
    [
        (ValidItemId, 
            ValidItemDescription)
    ]
    |> Map.ofList
        
let ValidIslandName = "valid island name"
let ValidJob : Job = 
    {
        FlavorText = ""
        Destination = ValidIslandLocation
        Reward = 1.0
    }
let ValidIslandList = [ ValidIslandLocation ]
let ValidMarket : Market =
    {
        Supply = 2.0
        Demand = 3.0
    }
let ValidMarketTable : Map<uint64, Market> =
    [
        (ValidCommodityId, ValidMarket)
    ]
    |> Map.ofList
let ValidShipmates : ShipmateIdentifier list =
    [
        Primary
    ]

let InvalidIslandLocation = (-1.0, -2.0)
