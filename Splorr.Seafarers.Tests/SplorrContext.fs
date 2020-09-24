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
    let context = splorrContext :?> World.UndockContext
    let context = splorrContext :?> Island.GenerateJobsContext
    let context = splorrContext :?> Island.ChangeMarketContext
    let context = splorrContext :?> Island.GetListContext
    let context = splorrContext :?> Island.GetDisplayNameContext
    let context = splorrContext :?> Vessel.TransformFoulingContext
    let context = splorrContext :?> Vessel.BefoulContext
    let context = splorrContext :?> Shipmate.GetStatusContext
    let context = splorrContext :?> Shipmate.EatContext
    let context = splorrContext :?> Avatar.GetSpeedContext
    let context = splorrContext :?> Avatar.GetHeadingContext
    let context = splorrContext :?> Avatar.SetPositionContext
    let context = splorrContext :?> Avatar.SetSpeedContext
    let context = splorrContext :?> Avatar.SetHeadingContext
    let context = splorrContext :?> Avatar.EatContext
    let context = splorrContext :?> Avatar.TransformShipmatesContext
    let context = splorrContext :?> World.AddMessagesContext
    let context = splorrContext :?> World.ClearMessagesContext
    let context = splorrContext :?> Avatar.MoveContext
    let context = splorrContext :?> World.DistanceToContext
    let context = splorrContext :?> World.HeadForContext
    let context = splorrContext :?> World.GetNearbyLocationsContext
    let context = splorrContext :?> AtSeaHandleCommandContext
    let context = splorrContext :?> Avatar.GetCurrentFoulingContext
    let context = splorrContext :?> AtSeaRunContext
    let context = splorrContext :?> Island.CreateContext
    let context = splorrContext :?> Utility.RandomContext
    let context = splorrContext :?> IslandFeature.CreateContext
    let context = splorrContext :?> World.PopulateIslandsContext
    let context = splorrContext :?> World.GenerateIslandNameContext
    let context = splorrContext :?> World.NameIslandsContext
    let context = splorrContext :?> World.GenerateIslandsContext
    let context = splorrContext :?> Vessel.CreateContext
    let context = splorrContext :?> Shipmate.CreateContext
    let context = splorrContext :?> Avatar.CreateContext
    let context = splorrContext :?> Avatar.GetPositionContext
    let context = splorrContext :?> World.CreateContext
    let context = splorrContext :?> World.UpdateChartsContext
    let context = splorrContext :?> DockedUpdateDisplayContext
    let context = splorrContext :?> IslandMarket.DeterminePriceContext
    let context = splorrContext :?> Island.MakeKnownContext
    let context = splorrContext :?> Island.UpdateMarketForItemContext
    let context = splorrContext :?> Avatar.RemoveInventoryContext
    let context = splorrContext :?> Avatar.GetUsedTonnageContext
    let context = splorrContext :?> Avatar.AbandonJobContext
    let context = splorrContext :?> Avatar.GetItemCountContext
    let context = splorrContext :?> Avatar.AddInventoryContext
    let context = splorrContext :?> World.AcceptJobContext
    let context = splorrContext :?> World.AbandonJobContext
    let context = splorrContext :?> World.BuyItemsContext
    let context = splorrContext :?> Avatar.EnterIslandFeatureContext
    let context = splorrContext :?> DockedHandleCommandContext
    let context = splorrContext :?> World.SellItemsContext
    let context = splorrContext :?> Job.CreateContext
    let context = splorrContext :?> Island.AddVisitContext
    let context = splorrContext :?> Island.GenerateCommoditiesContext
    let context = splorrContext :?> Island.GenerateItemsContext
    let context = splorrContext :?> Shipmate.TransformStatisticContext
    let context = splorrContext :?> Avatar.AddMetricContext
    let context = splorrContext :?> Avatar.AddMessagesContext
    let context = splorrContext :?> Avatar.GetPrimaryStatisticContext
    let context = splorrContext :?> Avatar.CompleteJobContext
    let context = splorrContext :?> World.DockContext
    let context = splorrContext :?> HelpRunContext
    let context = splorrContext :?> Avatar.GetMaximumFoulingContext
    let context = splorrContext :?> Avatar.CleanHullContext
    let context = splorrContext :?> IslandFeatureRunIslandContext
    let context = splorrContext :?> IslandFeatureRunContext
    let context = splorrContext :?> Avatar.GetGamblingHandContext
    let context = splorrContext :?> Avatar.DealGamblingHandContext
    let context = splorrContext :?> Avatar.FoldGamblingHandContext
    let context = splorrContext :?> World.HasDarkAlleyMinimumStakesContext
    let context = splorrContext :?> IslandFeatureRunDarkAlleyContext
    let context = splorrContext :?> Commodity.GetCommoditiesContext
    let context = splorrContext :?> GamestateCheckForAvatarDeathContext
    let context = splorrContext :?> AtSeaUpdateDisplayContext
    let context = splorrContext :?> AtSeaGetVisibleIslandsContext
    let context = splorrContext :?> AtSeaCanCareenContext
    let context = splorrContext :?> DockedRunBoilerplateContext
    let context = splorrContext :?> ItemListRunWithIslandContext
    let context = splorrContext :?> InventoryRunContext
    let context = splorrContext :?> InventoryRunWorldContext
    let context = splorrContext :?> IslandListRunWorldContext
    let context = splorrContext :?> CareenedRunContext
    let context = splorrContext :?> CareenedRunAliveContext
    let context = splorrContext :?> CareenedUpdateDisplayContext
    let context = splorrContext :?> CareenedHandleCommandContext
    let context = splorrContext :?> ChartRunContext
    let context = splorrContext :?> ChartOutputChartContext
    let context = splorrContext :?> ConfirmQuit.RunContext
    let context = splorrContext :?> Avatar.GetJobContext
    let context = splorrContext :?> Island.GetNameContext
    let context = splorrContext :?> Vessel.GetStatisticContext
    let context = splorrContext :?> Shipmate.GetStatisticContext
    let context = splorrContext :?> Avatar.GetMetricsContext
    let context = splorrContext :?> Island.GetJobsContext
    let context = splorrContext :?> Item.GetListContext
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    //let context = splorrContext :?> 
    Assert.Pass("If it makes it this far, it works!")


