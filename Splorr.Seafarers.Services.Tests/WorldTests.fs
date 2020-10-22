﻿module WorldTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

[<Test>]
let ``CleanHull..`` () = 
    let context = Contexts.TestContext()
    World.CleanHull
        context
        Side.Port
        Dummies.ValidAvatarId

[<Test>]
let ``HasDarkAlleyMinimumStakes..`` () = 
    let context = Contexts.TestContext()
    World.HasDarkAlleyMinimumStakes
        context
        Dummies.ValidAvatarId

[<Test>]
let ``FoldGamblingHand..`` () = 
    let context = Contexts.TestContext()
    World.FoldGamblingHand
        context
        Dummies.ValidAvatarId

[<Test>]
let ``Save..`` () = 
    let context = Contexts.TestContext()
    World.Save
        context
        "filename"
        Dummies.ValidAvatarId

[<Test>]
let ``GetIslandList..`` () =
    let context = Contexts.TestContext()
    let actual =
        World.GetIslandList
            context
    Assert.AreEqual([], actual)

[<Test>]
let ``GetAvatarMessages..`` () =
    let context = Contexts.TestContext()
    let actual =
        World.GetAvatarMessages
            context
    Assert.AreEqual([], actual)

[<Test>]
let ``GetAvatarIslandMetric..`` () =
    let context = Contexts.TestContext()
    let actual =
        World.GetAvatarIslandMetric
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
            AvatarIslandMetricIdentifier.LastVisit
    Assert.AreEqual(None, actual)

[<Test>]
let ``EnterAvatarIslandFeature..`` () =
    let context = Contexts.TestContext()
    let actual =
        World.EnterAvatarIslandFeature
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
            IslandFeatureIdentifier.Dock
    Assert.AreEqual(None, actual)

[<Test>]
let ``DealAvatarGamblingHand..`` () =
    let context = Contexts.TestContext()
    let actual =
        World.DealAvatarGamblingHand
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetAvatarGamblingHand..`` () =
    let context = Contexts.TestContext()
    let actual =
        World.GetAvatarGamblingHand
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetItemList..`` () =
    let context = Contexts.TestContext()
    let actual =
        World.GetItemList
            context
    Assert.AreEqual(Map.empty, actual)

[<Test>]
let ``GetAvatarIslandFeature..`` () =
    let context = Contexts.TestContext()
    let actual =
        World.GetAvatarIslandFeature
            context
    Assert.AreEqual(Map.empty, actual)

[<Test>]
let ``SellItems..`` () =
    let context = Contexts.TestContext()
    World.SellItems
        context
        Dummies.ValidIslandLocation
        (TradeQuantity.Specific 1UL)
        Dummies.ValidItemName
        Dummies.ValidAvatarId

[<Test>]
let ``GetVesselStatistic..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselStatistic
            context
            Dummies.ValidAvatarId
            VesselStatisticIdentifier.Heading
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetVesselCurrentFouling..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselCurrentFouling
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(0.0, actual)

[<Test>]
let ``GetVesselMaximumFouling..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselMaximumFouling
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)

[<Test>]
let ``IsAvatarAlive..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.IsAvatarAlive
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(true, actual)

[<Test>]
let ``GetIslandFeatures..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandFeatures
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual([], actual)

[<Test>]
let ``GetIslandName..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandName
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual([], actual)
    
[<Test>]
let ``GetAvatarInventory..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarInventory
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(Map.empty, actual)

[<Test>]
let ``Undock..`` () =
    let context = Contexts.TestContext()
    World.Undock
        context
        Dummies.ValidAvatarId

        
[<Test>]
let ``Dock..`` () =
    let context = Contexts.TestContext()
    World.Dock
        context
        Dummies.ValidIslandLocation
        Dummies.ValidAvatarId
        
[<Test>]
let ``HasIslandFeature..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.HasIslandFeature
            context
            IslandFeatureIdentifier.DarkAlley
            Dummies.ValidIslandLocation
    Assert.AreEqual(false, actual)

[<Test>]
let ``GetVesselPosition..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselPosition
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetVesselSpeed..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselSpeed
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetVesselHeading..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselHeading
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetIslandItems..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandItems
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual(Set.empty, actual)

[<Test>]
let ``GetVesselUsedTonnage..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselUsedTonnage
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)

[<Test>]
let ``GetIslandJobs..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandJobs
            context
            Dummies.ValidIslandLocation
    Assert.AreEqual([], actual)

[<Test>]
let ``Create..`` () =
    let context = Contexts.TestContext()
    World.Create
        context
        Dummies.ValidAvatarId

[<Test>]
let ``GetAvatarMetrics..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarMetrics
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(Map.empty, actual)

[<Test>]
let ``GetShipmateStatistic..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetShipmateStatistic
            context
            Dummies.ValidAvatarId
            Primary
            ShipmateStatisticIdentifier.Health
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetIslandStatistic..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandStatistic
            context
            IslandStatisticIdentifier.CareenDistance
            Dummies.ValidIslandLocation
    Assert.AreEqual(None, actual)

[<Test>]
let ``GetNearbyLocations..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetNearbyLocations
            context
            Dummies.ValidIslandLocation
            10.0
    Assert.AreEqual([], actual)

[<Test>]
let ``GetIslandDisplayName..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetIslandDisplayName
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
    Assert.AreEqual("", actual)

[<Test>]
let ``GetVesselEffectiveSpeed..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetVesselEffectiveSpeed
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)

[<Test>]
let ``GetAvatarJob..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarJob
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)


[<Test>]
let ``HeadFor..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.HeadFor
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)

[<Test>]
let ``DistanceTo..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.DistanceTo
            context
            Dummies.ValidIslandName
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)

[<Test>]
let ``Move..`` () =
    let context = Contexts.TestContext()
    World.Move
        context
        1u
        Dummies.ValidAvatarId

[<Test>]
let ``SetHeading..`` () =
    let context = Contexts.TestContext()
    World.SetHeading
        context
        1.0
        Dummies.ValidAvatarId

[<Test>]
let ``SetSpeed..`` () =
    let context = Contexts.TestContext()
    World.SetSpeed
        context
        0.5
        Dummies.ValidAvatarId

[<Test>]
let ``GetAvatarMoney..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarMoney
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)

[<Test>]
let ``GetAvatarReputation..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.GetAvatarReputation
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(1.0, actual)

[<Test>]
let ``DetermineIslandMarketSalePrice..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.DetermineIslandMarketSalePrice
            context
            0UL
            Dummies.ValidIslandLocation
    Assert.AreEqual(1.0, actual)

[<Test>]
let ``DetermineIslandMarketPurchasePrice..`` () =
    let context = Contexts.TestContext()
    let actual = 
        World.DetermineIslandMarketPurchasePrice
            context
            0UL
            Dummies.ValidIslandLocation
    Assert.AreEqual(1.0, actual)

