module VesselTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let private inputAvatarId = "avatar"

type TestVesselCreateContext(vesselStatisticSink, vesselStatisticTemplateSource) =
    interface Vessel.CreateContext with
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource

type TestVesselTransformFoulingContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface Vessel.TransformFoulingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestVesselBefoulContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface Vessel.TransformFoulingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Vessel.BefoulContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

[<Test>]
let ``Create.It creates a vessel.`` () =
    let inputAvatarId = "avatar"
    let inputTemplates : Map<VesselStatisticIdentifier, StatisticTemplate> =
        Map.empty
        |> Map.add VesselStatisticIdentifier.Tonnage {StatisticName=""; MinimumValue=0.0; MaximumValue=0.0; CurrentValue=0.0}
    let expectedStatistics =
        inputTemplates
        |> Map.map
            (fun _ template -> { MinimumValue=template.MinimumValue; MaximumValue=template.MaximumValue; CurrentValue=template.CurrentValue})
    let vesselStatisticTemplateSource() : Map<VesselStatisticIdentifier, StatisticTemplate> = 
        inputTemplates
    let vesselStatisticSink (avatarId:string) (statistics:Map<VesselStatisticIdentifier, Statistic>) : unit =
        Assert.AreEqual(inputAvatarId, avatarId)
        Assert.AreEqual(expectedStatistics, statistics)
    let context = TestVesselCreateContext(vesselStatisticSink, vesselStatisticTemplateSource) :> Vessel.CreateContext
    Vessel.Create 
        context
        inputAvatarId

type TestVesselGetStatisticContext
        (vesselSingleStatisticSource) =
    interface ServiceContext
    interface Vessel.GetStatisticContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
[<Test>]
let ``GetStatistic.It calls XXXX in the context.`` () =
    let mutable called = false
    let vesselSingleStatisticSource (_) (_) =
        called <- true
        None
    let context = TestVesselGetStatisticContext(vesselSingleStatisticSource) :> ServiceContext
    let expected = None
    let actual = 
        Vessel.GetStatistic
            context
            Fixtures.Common.Dummy.AvatarId
            VesselStatisticIdentifier.Heading
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
    

[<Test>]
let ``TransformFouling.It transforms fouling on the port side when the port side is specified.`` () =
    let inputSide = Port
    let inputAvatarId = "avatar"
    let vesselSingleStatisticSource (_:string) (_:VesselStatisticIdentifier) : Statistic option =
        None
    let vesselSingleStatisticSink  (_:string) (identifier:VesselStatisticIdentifier, _: Statistic) : unit = 
        Assert.AreEqual(VesselStatisticIdentifier.PortFouling, identifier)
    let context = TestVesselTransformFoulingContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.TransformFoulingContext
    Vessel.TransformFouling 
        context
        inputAvatarId 
        inputSide 
        (Statistic.ChangeCurrentBy 0.25)

[<Test>]
let ``TransformFouling.It transforms fouling on the starboard side when the starboard side is specified.`` () =
    let inputValue = 0.25
    let inputSide = Starboard
    let vesselSingleStatisticSource (_) (_) = 
        {MaximumValue=0.25;MinimumValue=0.0;CurrentValue=0.0} |> Some
    let expectedValue = inputValue
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier,statistic:Statistic) =
        Assert.AreEqual(VesselStatisticIdentifier.StarboardFouling, identifier)
        Assert.AreEqual(expectedValue, statistic.CurrentValue)
    let context = TestVesselTransformFoulingContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.TransformFoulingContext
    Vessel.TransformFouling 
        context
        inputAvatarId 
        inputSide 
        (Statistic.ChangeCurrentBy inputValue)

[<Test>]
let ``Befoul.It increases how fouled the vessel's hull is by the vessel's foul rate.`` () =
    let inputFoulRate = 0.005
    let expectedFoulage = inputFoulRate
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with 
        | VesselStatisticIdentifier.PortFouling
        | VesselStatisticIdentifier.StarboardFouling ->
            {MaximumValue=0.25; MinimumValue=0.0; CurrentValue=0.0} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MaximumValue=0.01; MinimumValue=0.01; CurrentValue=0.01} |> Some
        | _ ->
            Assert.Fail("Don't call me.")
            None
    let vesselSingleStatisticSink (_) (_:VesselStatisticIdentifier, statistic:Statistic):unit =
        Assert.AreEqual(expectedFoulage, statistic.CurrentValue)
    let context = TestVesselBefoulContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.BefoulContext
    Vessel.Befoul 
        context
        inputAvatarId

[<Test>]
let ``Befoul.It will not increase the vessel's fouling when the vessel is already at maximum fouling.`` () =
    let expectedFoulage = 0.25
    let vesselSingleStatisticSource (_) (_) = 
        {MaximumValue=0.25; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let vesselSingleStatisticSink (_) (_:VesselStatisticIdentifier, statistic:Statistic):unit =
        Assert.AreEqual(expectedFoulage, statistic.CurrentValue)
    let context = TestVesselBefoulContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.BefoulContext
    Vessel.Befoul 
        context
        inputAvatarId
