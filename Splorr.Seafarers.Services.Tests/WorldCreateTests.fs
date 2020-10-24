module WorldCreateTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``Create.It creates a world.`` () =
    let calledGetVesselStatisticTemplates = ref false
    let calledGetGlobalRationItems = ref false
    let calledSetShipmateRationItems = ref false
    let calledGetShipmateStatisticTemplates = ref false
    let calledGetIslandStatisticTemplates = ref false
    let calledSetAvatarJob = ref false
    let calledGetTerms = ref false
    let calledGenerateIslandFeature = ref false

    let callsForGetWorldStatistic = ref 0UL
    let callsForSetVesselStatistic = ref 0UL
    let callsForSetShipmateStatistic = ref 0UL
    let callsForGetIslandList = ref 0UL
    let callsForSetIslandStatistics = ref 0UL
    let callsForSetIslandName = ref 0UL
    let callsForSetIslandFeature = ref 0UL
    let calledGetVesselStatistic = ref 0UL
    let calledSetAvatarIslandMetric = ref 0UL

    let islandList : Location list ref = ref []
    let islandNames : Map<Location, string option> ref = ref Map.empty
    let islandFeatureSet:Set<Location * IslandFeatureIdentifier> ref = ref Set.empty

    let context = Contexts.TestContext()
    (context :> WorldStatistic.GetStatisticContext).worldSingleStatisticSource := 
        Spies.SourceTable(callsForGetWorldStatistic, 
            Dummies.ValidWorldStatistics)
    (context :> Vessel.GetStatisticTemplateContext).vesselStatisticTemplateSource := 
        Spies.Source(calledGetVesselStatisticTemplates, 
            Dummies.ValidVesselStatisticTemplates)
    (context :> Vessel.SetStatisticContext).vesselSingleStatisticSink :=
        Spies.ExpectSet(callsForSetVesselStatistic, 
            Dummies.ValidVesselStatisticSet)
    (context :> Shipmate.GetGlobalRationItemsContext).rationItemSource := Spies.Source(calledGetGlobalRationItems, Dummies.ValidGlobalRationItems)
    (context :> Shipmate.SetRationItemsContext).shipmateRationItemSink := Spies.Expect(calledSetShipmateRationItems, (Dummies.ValidAvatarId, Primary, Dummies.ValidGlobalRationItems))
    (context :> Shipmate.GetStatisticTemplatesContext).shipmateStatisticTemplateSource := Spies.Source(calledGetShipmateStatisticTemplates, Dummies.ValidShipmateStatisticTemplates)
    (context :> ShipmateStatistic.PutContext).shipmateSingleStatisticSink := Spies.ExpectSet(callsForSetShipmateStatistic, Dummies.ValidShipmateStatisticSet)
    (context :> Avatar.SetJobContext).avatarJobSink := Spies.Expect(calledSetAvatarJob, (Dummies.ValidAvatarId, None))
    (context :> Island.GetListContext).islandSource := Spies.SourceHook(callsForGetIslandList, fun ()->islandList.Value)
    (context :> Island.GetStatisticTemplatesContext).islandStatisticTemplateSource := Spies.Source(calledGetIslandStatisticTemplates, Dummies.ValidIslandStatisticTemplates)
    (context :> Island.SetIslandStatisticContext).islandSingleStatisticSink := 
        Spies.Validate(callsForSetIslandStatistics, 
            fun sunk ->
                let location, _, _ = sunk
                islandList := 
                    islandList.Value 
                    |> List.append [ location ] 
                    |> Set.ofList 
                    |> Set.toList
                (Dummies.ValidIslandStatisticSet location).Contains sunk)
    (context :> Utility.GetTermsContext).termListSource :=
        Spies.Source(calledGetTerms, ["antwerp"])
    (context :> WorldCreation.SetIslandNameContext).islandSingleNameSink :=
        Spies.SinkHook(callsForSetIslandName, 
            fun (location, name) ->
                islandNames:= 
                    islandNames.Value 
                    |> Map.add location name)
    (context :> WorldCreation.GenerateIslandFeatureContext).islandFeatureGeneratorSource :=
        Spies.Source(calledGenerateIslandFeature, 
            Dummies.ValidIslandFeatureGenerators)
    (context :> WorldCreation.SetIslandFeatureContext).islandSingleFeatureSink := 
        Spies.SinkHook(callsForSetIslandFeature,
            fun sunk->
                islandFeatureSet :=
                    islandFeatureSet.Value
                    |> Set.add sunk)
    (context :> Vessel.GetStatisticContext).vesselSingleStatisticSource :=
        Spies.SourceTable(calledGetVesselStatistic, Dummies.ValidDefaultVesselStatisticTable)
    (context :> AvatarIslandMetric.PutContext).avatarIslandSingleMetricSink :=
        Spies.SinkCounter(calledSetAvatarIslandMetric)
        
    World.Create
        context
        Dummies.ValidAvatarId

    Assert.IsTrue(calledGetVesselStatisticTemplates.Value)
    Assert.IsTrue(calledGetGlobalRationItems.Value)
    Assert.IsTrue(calledSetShipmateRationItems.Value)
    Assert.IsTrue(calledGetShipmateStatisticTemplates.Value)
    Assert.IsTrue(calledSetAvatarJob.Value)
    Assert.IsTrue(calledGetIslandStatisticTemplates.Value)
    Assert.IsTrue(calledGetTerms.Value)
    Assert.IsTrue(calledGenerateIslandFeature.Value)

    Assert.AreEqual(4UL, callsForGetWorldStatistic.Value)
    Assert.AreEqual(10UL, callsForSetVesselStatistic.Value)
    Assert.AreEqual(5UL, callsForSetShipmateStatistic.Value)
    Assert.AreEqual(10UL, callsForGetIslandList.Value)
    Assert.AreEqual(8UL, callsForSetIslandStatistics.Value)
    Assert.AreEqual(4UL, callsForSetIslandName.Value)
    Assert.AreEqual(6UL, callsForSetIslandFeature.Value)
    Assert.AreEqual(3UL, calledGetVesselStatistic.Value)
    Assert.AreEqual(3UL, calledSetAvatarIslandMetric.Value)

    Assert.AreEqual(6UL, islandFeatureSet.Value.Count)
    Assert.AreEqual(4UL, islandNames.Value.Count)
    Assert.AreEqual(4UL, islandList.Value.Length)

