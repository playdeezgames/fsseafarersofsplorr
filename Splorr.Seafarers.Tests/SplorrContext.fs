module SplorrContext

open Splorr.Seafarers.Services
open Splorr.Seafarers
open System
open Splorr.Seafarers.Controllers
open NUnit.Framework

let splorrContext : ServiceContext =
    SplorrContext
        (Fixtures.Stub.AvatarGamblingHandSink,
        Fixtures.Stub.AvatarGamblingHandSource,
        Fixtures.Stub.AvatarInventorySink,
        Fixtures.Stub.AvatarInventorySource,
        Fixtures.Stub.AvatarIslandFeatureSink,
        Fixtures.Stub.AvatarIslandFeatureSource,
        Fixtures.Stub.AvatarIslandSingleMetricSink,
        Fixtures.Stub.AvatarIslandSingleMetricSource,
        Fixtures.Stub.AvatarJobSink,
        Fixtures.Stub.AvatarJobSource,
        Fixtures.Stub.AvatarMessagePurger,
        Fixtures.Stub.AvatarMessageSink,
        Fixtures.Stub.AvatarMessageSource,
        Fixtures.Stub.AvatarMetricSource,
        Fixtures.Stub.AvatarShipmateSource,
        Fixtures.Stub.AvatarSingleMetricSink,
        Fixtures.Stub.AvatarSingleMetricSource,
        Fixtures.Stub.CommoditySource,
        (fun () -> DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
        Fixtures.Stub.IslandFeatureGeneratorSource,
        Fixtures.Stub.IslandFeatureSource,
        Fixtures.Stub.IslandItemSink ,
        Fixtures.Stub.IslandItemSource, 
        Fixtures.Stub.IslandJobPurger,
        Fixtures.Stub.IslandJobSink,
        Fixtures.Stub.IslandJobSource,
        Fixtures.Stub.IslandLocationByNameSource,
        Fixtures.Stub.IslandMarketSink ,
        Fixtures.Stub.IslandMarketSource, 
        Fixtures.Stub.IslandSingleFeatureSink,
        Fixtures.Stub.IslandSingleFeatureSource,
        Fixtures.Stub.IslandSingleJobSource,
        Fixtures.Stub.IslandSingleMarketSink, 
        Fixtures.Stub.IslandSingleMarketSource,
        Fixtures.Stub.IslandSingleNameSink,
        Fixtures.Stub.IslandSingleNameSource,
        Fixtures.Stub.IslandSingleStatisticSink,
        Fixtures.Stub.IslandSingleStatisticSource,
        Fixtures.Stub.IslandSource,
        Fixtures.Stub.IslandStatisticTemplateSource,
        Fixtures.Stub.ItemSingleSource,
        Fixtures.Stub.ItemSource ,
        Fixtures.Dummy.Random,
        Fixtures.Stub.RationItemSource,
        Fixtures.Stub.ShipmateRationItemSink,
        Fixtures.Stub.ShipmateRationItemSource,
        Fixtures.Stub.ShipmateSingleStatisticSink,
        Fixtures.Stub.ShipmateSingleStatisticSource,
        Fixtures.Stub.ShipmateStatisticTemplateSource,
        Fixtures.Stub.SwitchSource ,
        Fixtures.Stub.TermNameSource,
        Fixtures.Stub.TermSources,
        Fixtures.Stub.VesselSingleStatisticSink,
        Fixtures.Stub.VesselSingleStatisticSource,
        Fixtures.Stub.VesselStatisticSink,
        Fixtures.Stub.VesselStatisticTemplateSource,
        Fixtures.Stub.WorldSingleStatisticSource) :> ServiceContext

[<Test>]
let ``Downcast.It downcasts to all of the interfaces that comprise a SplorrContext.`` () =
    //avatar vessel contexts
    let context = splorrContext :?> Avatar.CleanHullContext
    let context = splorrContext :?> Avatar.MoveContext
    
    //avatar inventory contexts
    //avatar shipmate contexts
    let context = splorrContext :?> Avatar.EatContext
    let context = splorrContext :?> AvatarShipmates.GetPrimaryStatisticContext
    let context = splorrContext :?> AvatarShipmates.TransformContext

    let context = splorrContext :?> Avatar.CreateContext

    let context = splorrContext :?> AvatarGamblingHand.DealContext
    let context = splorrContext :?> AvatarGamblingHand.FoldContext
    let context = splorrContext :?> AvatarGamblingHand.GetContext

    let context = splorrContext :?> AvatarInventory.AddInventoryContext
    let context = splorrContext :?> AvatarInventory.GetInventoryContext
    let context = splorrContext :?> AvatarInventory.GetUsedTonnageContext
    let context = splorrContext :?> AvatarInventory.GetItemCountContext
    let context = splorrContext :?> AvatarInventory.RemoveInventoryContext

    let context = splorrContext :?> AvatarIslandFeature.EnterContext
    let context = splorrContext :?> AvatarIslandFeature.GetContext

    let context = splorrContext :?> AvatarJob.AbandonContext
    let context = splorrContext :?> AvatarJob.CompleteContext
    let context = splorrContext :?> AvatarJob.GetContext

    let context = splorrContext :?> AvatarMessages.AddContext
    let context = splorrContext :?> AvatarMessages.GetContext

    let context = splorrContext :?> AvatarMetric.AddContext
    let context = splorrContext :?> AvatarMetric.GetContext
    let context = splorrContext :?> AvatarMetric.GetForIslandContext

    let context = splorrContext :?> Commodity.GetCommoditiesContext

    let context = splorrContext :?> ConfirmQuit.RunContext

    let context = splorrContext :?> Island.AddVisitContext
    let context = splorrContext :?> Island.ChangeMarketContext
    let context = splorrContext :?> Island.CreateContext
    let context = splorrContext :?> Island.GenerateCommoditiesContext
    let context = splorrContext :?> Island.GenerateItemsContext
    let context = splorrContext :?> Island.GenerateJobsContext
    let context = splorrContext :?> Island.GetDisplayNameContext
    let context = splorrContext :?> Island.GetItemsContext
    let context = splorrContext :?> Island.GetJobsContext
    let context = splorrContext :?> Island.GetListContext
    let context = splorrContext :?> Island.GetNameContext
    let context = splorrContext :?> Island.HasFeatureContext
    let context = splorrContext :?> Island.MakeKnownContext
    let context = splorrContext :?> Island.UpdateMarketForItemContext

    let context = splorrContext :?> IslandFeature.CreateContext

    let context = splorrContext :?> IslandMarket.DeterminePriceContext

    let context = splorrContext :?> Item.GetListContext

    let context = splorrContext :?> Job.CreateContext

    let context = splorrContext :?> Shipmate.CreateContext
    let context = splorrContext :?> Shipmate.EatContext
    let context = splorrContext :?> Shipmate.GetStatisticContext
    let context = splorrContext :?> Shipmate.GetStatusContext
    let context = splorrContext :?> Shipmate.TransformStatisticContext

    let context = splorrContext :?> Vessel.TransformFoulingContext

    let context = splorrContext :?> Vessel.CreateContext
    let context = splorrContext :?> Vessel.GetStatisticContext
    let context = splorrContext :?> Vessel.GetHeadingContext
    let context = splorrContext :?> Vessel.GetPositionContext
    let context = splorrContext :?> Vessel.GetSpeedContext
    let context = splorrContext :?> Vessel.SetHeadingContext
    let context = splorrContext :?> Vessel.SetPositionContext
    let context = splorrContext :?> Vessel.SetSpeedContext
    let context = splorrContext :?> Vessel.GetCurrentFoulingContext
    let context = splorrContext :?> Vessel.GetMaximumFoulingContext

    let context = splorrContext :?> Utility.RandomContext

    let context = splorrContext :?> World.AbandonJobContext
    let context = splorrContext :?> World.AcceptJobContext
    let context = splorrContext :?> World.AddMessagesContext
    let context = splorrContext :?> World.BuyItemsContext
    let context = splorrContext :?> World.ClearMessagesContext
    let context = splorrContext :?> World.CreateContext
    let context = splorrContext :?> World.DistanceToContext
    let context = splorrContext :?> World.DockContext
    let context = splorrContext :?> World.GenerateIslandNameContext
    let context = splorrContext :?> World.GenerateIslandsContext
    let context = splorrContext :?> World.GetNearbyLocationsContext
    let context = splorrContext :?> World.GetStatisticContext
    let context = splorrContext :?> World.HasDarkAlleyMinimumStakesContext
    let context = splorrContext :?> World.HeadForContext
    let context = splorrContext :?> World.NameIslandsContext
    let context = splorrContext :?> World.PopulateIslandsContext
    let context = splorrContext :?> World.SellItemsContext
    let context = splorrContext :?> World.UndockContext
    let context = splorrContext :?> World.UpdateChartsContext

    let context = splorrContext :?> Island.GetStatisticContext
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    Assert.Pass("If it makes it this far, it works!")


