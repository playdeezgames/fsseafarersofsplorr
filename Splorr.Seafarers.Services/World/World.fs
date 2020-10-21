namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System
open Splorr.Common

module World =
    let AbandonJob = WorldVessel.AbandonJob
    let AcceptJob = WorldIslands.AcceptJob
    let AddMessages = AvatarMessages.Add
    let BetOnGamblingHand = WorldIslandGambling.BetOnGamblingHand
    let BuyItems = WorldIslandTrading.BuyItems
    let CleanHull = Avatar.CleanHull
    let ClearMessages = WorldMessages.ClearMessages
    let Create = WorldCreation.Create
    let DealAvatarGamblingHand = AvatarGamblingHand.Deal
    let DetermineIslandMarketPurchasePrice = IslandMarket.DeterminePurchasePrice
    let DetermineIslandMarketSalePrice = IslandMarket.DetermineSalePrice
    let DistanceTo = WorldIslands.DistanceTo
    let Dock = WorldVessel.Dock
    let EnterAvatarIslandFeature = AvatarIslandFeature.Enter
    let FoldGamblingHand = WorldIslandGambling.FoldGamblingHand
    let GetAvatarGamblingHand = AvatarGamblingHand.Get
    let GetAvatarInventory = AvatarInventory.GetInventory
    let GetAvatarIslandFeature = AvatarIslandFeature.Get
    let GetAvatarIslandMetric = AvatarIslandMetric.Get
    let GetAvatarJob = AvatarJob.Get
    let GetAvatarMessages = AvatarMessages.Get
    let GetAvatarMetrics = AvatarMetric.Get
    let GetAvatarMoney = AvatarShipmates.GetMoney
    let GetAvatarReputation = AvatarShipmates.GetReputation
    let GetIslandDisplayName = IslandName.GetDisplayName
    let GetIslandFeatures = Island.GetFeatures
    let GetIslandItems = Island.GetItems
    let GetIslandJobs = IslandJob.Get
    let GetIslandList = Island.GetList
    let GetIslandName = IslandName.GetName
    let GetIslandStatistic = Island.GetStatistic
    let GetItemList = Item.GetList
    let GetNearbyLocations = WorldVessel.GetNearbyLocations
    let GetShipmateStatistic = ShipmateStatistic.Get
    let GetStatistic = WorldStatistic.GetStatistic
    let GetVesselCurrentFouling = Vessel.GetCurrentFouling
    let GetVesselEffectiveSpeed = Vessel.GetEffectiveSpeed
    let GetVesselMaximumFouling = Vessel.GetMaximumFouling
    let GetVesselPosition = Vessel.GetPosition
    let GetVesselHeading = Vessel.GetHeading
    let GetVesselSpeed = Vessel.GetSpeed
    let GetVesselStatistic = Vessel.GetStatistic
    let GetVesselUsedTonnage = AvatarInventory.GetUsedTonnage
    let HasDarkAlleyMinimumStakes = WorldIslandGambling.HasDarkAlleyMinimumStakes
    let HasIslandFeature = Island.HasFeature
    let HeadFor = WorldIslands.HeadFor
    let Move = WorldVessel.Move
    let IsAvatarAlive = WorldShipmate.IsAvatarAlive
    let Save = WorldExport.Save
    let SellItems = WorldIslandTrading.SellItems
    let SetHeading = WorldVessel.SetHeading
    let SetSpeed = WorldVessel.SetSpeed
    let Undock = WorldVessel.Undock

            
