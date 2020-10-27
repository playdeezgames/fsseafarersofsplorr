module Dummies

open System
open Splorr.Seafarers.Models

let ValidAvatarId = Guid.NewGuid().ToString()
let ValidJobIndex = 1u
let ValidIslandLocation = (1.0,2.0)
let OtherValidIslandLocation = (10.0, 20.0)
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
let ValidIslandList = [ ValidIslandLocation; OtherValidIslandLocation ]
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
let ValidWorldStatistics =
    Map.empty
    |> Map.add WorldStatisticIdentifier.IslandGenerationRetries {MinimumValue = 0.0; MaximumValue = 5.0; CurrentValue = 2.5}
    |> Map.add WorldStatisticIdentifier.IslandDistance {MinimumValue = 0.0; MaximumValue = 10.0; CurrentValue = 5.0}
    |> Map.add WorldStatisticIdentifier.PositionX {MinimumValue = 0.0; MaximumValue = 20.0; CurrentValue = 10.0}
    |> Map.add WorldStatisticIdentifier.PositionY {MinimumValue = 0.0; MaximumValue = 25.0; CurrentValue = 12.5}
let ValidVesselStatisticTemplates : Map<VesselStatisticIdentifier, StatisticTemplate> =
    [
        VesselStatisticIdentifier.PortFouling, {MinimumValue = 0.0; MaximumValue=0.25; CurrentValue=0.0; StatisticName=""}//TODO: why do i have a statistic name in a statistic template when the name has no bearing on anything?
        VesselStatisticIdentifier.StarboardFouling, {MinimumValue = 0.0; MaximumValue=0.25; CurrentValue=0.0; StatisticName=""}
        VesselStatisticIdentifier.FoulRate, {MinimumValue = 0.001; MaximumValue=0.001; CurrentValue=0.001; StatisticName=""}
        VesselStatisticIdentifier.Tonnage, {MinimumValue = 100.0; MaximumValue=100.0; CurrentValue=100.0; StatisticName=""}
        VesselStatisticIdentifier.Speed, {MinimumValue = 0.0; MaximumValue=1.0; CurrentValue=1.0; StatisticName=""}
        VesselStatisticIdentifier.Heading, {MinimumValue = 0.0; MaximumValue=6.3; CurrentValue=0.0; StatisticName=""}
        VesselStatisticIdentifier.ViewDistance, {MinimumValue = 10.0; MaximumValue=10.0; CurrentValue=10.0; StatisticName=""}
        VesselStatisticIdentifier.DockDistance, {MinimumValue = 1.0; MaximumValue=1.0; CurrentValue=1.0; StatisticName=""}
        VesselStatisticIdentifier.PositionX, {MinimumValue = 0.0; MaximumValue=20.0; CurrentValue=10.0; StatisticName=""}
        VesselStatisticIdentifier.PositionY, {MinimumValue = 0.0; MaximumValue=25.0; CurrentValue=12.5; StatisticName=""}
    ]
    |> Map.ofList
let ValidVesselStatisticSet: Set<string * VesselStatisticIdentifier * Statistic> =
    ValidVesselStatisticTemplates
    |> Map.toList
    |> List.map
        (fun (identifier, statisticTemplate) ->
            (ValidAvatarId, identifier, {MinimumValue = statisticTemplate.MinimumValue; MaximumValue = statisticTemplate.MaximumValue; CurrentValue=statisticTemplate.CurrentValue}))
    |> Set.ofList
let ValidDefaultVesselStatisticTable : Map<string * VesselStatisticIdentifier, Statistic option> =
    ValidVesselStatisticSet
    |> Set.toList
    |> List.map
        (fun (a,b,c) -> ((a,b),Some c))
    |> Map.ofList
let ValidGlobalRationItems =
    [
        ValidItemId
    ]
let ValidShipmateStatisticTemplates : Map<ShipmateStatisticIdentifier, StatisticTemplate> =
    [
        ShipmateStatisticIdentifier.Satiety, {MinimumValue =  0.0; MaximumValue= 100.0; CurrentValue= 100.0; StatisticName=""}
        ShipmateStatisticIdentifier.Health, {MinimumValue =  0.0; MaximumValue= 100.0; CurrentValue= 100.0; StatisticName=""}
        ShipmateStatisticIdentifier.Turn, {MinimumValue =  0.0; MaximumValue= 50000.0; CurrentValue= 0.0; StatisticName=""}
        ShipmateStatisticIdentifier.Money, {MinimumValue =  0.0; MaximumValue= 1000000000.0; CurrentValue= 0.0; StatisticName=""}
        ShipmateStatisticIdentifier.Reputation, {MinimumValue =  -1000000000.0; MaximumValue= 1000000000.0; CurrentValue= 0.0; StatisticName=""}
    ]
    |> Map.ofList
let ValidShipmateStatisticSet: Set<string * ShipmateIdentifier * ShipmateStatisticIdentifier * Statistic option> =
    ValidShipmateStatisticTemplates
    |> Map.toList
    |> List.map
        (fun (identifier, statisticTemplate) ->
            (ValidAvatarId, Primary, identifier, {MinimumValue = statisticTemplate.MinimumValue; MaximumValue = statisticTemplate.MaximumValue; CurrentValue=statisticTemplate.CurrentValue} |> Some))
    |> Set.ofList
let ValidIslandStatisticTemplates : Map<IslandStatisticIdentifier, StatisticTemplate> =
    [
        IslandStatisticIdentifier.CareenDistance, {MinimumValue =  0.05; MaximumValue= 0.15; CurrentValue= 0.1; StatisticName=""}
        IslandStatisticIdentifier.MinimumGamblingStakes, {MinimumValue =  5.0; MaximumValue= 5.0; CurrentValue= 5.0; StatisticName=""}
    ]
    |> Map.ofList
let ValidIslandStatisticSet (location : Location) : Set<Location * IslandStatisticIdentifier * Statistic option> =
    ValidIslandStatisticTemplates
    |> Map.toList
    |> List.map
        (fun (identifier, statisticTemplate) ->
            (location, identifier, {MinimumValue = statisticTemplate.MinimumValue; MaximumValue = statisticTemplate.MaximumValue; CurrentValue=statisticTemplate.CurrentValue} |> Some))
    |> Set.ofList
let ValidIslandFeatureGenerators =
    Map.empty
    |> Map.add IslandFeatureIdentifier.Dock {FeatureWeight=1.0; FeaturelessWeight=0.0}
    |> Map.add IslandFeatureIdentifier.DarkAlley {FeatureWeight=1.0; FeaturelessWeight=1.0}

let InvalidIslandLocation = (-1.0, -2.0)
