module VesselTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

[<Test>]
let ``Create.It creates a vessel.`` () =
    let inputAvatarId = "avatar"
    let inputTemplates : Map<VesselStatisticIdentifier, VesselStatisticTemplate> =
        Map.empty
        |> Map.add VesselStatisticIdentifier.Tonnage {StatisticId=VesselStatisticIdentifier.Tonnage; StatisticName=""; MinimumValue=0.0; MaximumValue=0.0; CurrentValue=0.0}
    let expectedStatistics =
        inputTemplates
        |> Map.map
            (fun _ template -> { MinimumValue=template.MinimumValue; MaximumValue=template.MaximumValue; CurrentValue=template.CurrentValue})
    let vesselStatisticTemplateSource() : Map<VesselStatisticIdentifier, VesselStatisticTemplate> = 
        inputTemplates
    let vesselStatisticSink (avatarId:string) (statistics:Map<VesselStatisticIdentifier, Statistic>) : unit =
        Assert.AreEqual(inputAvatarId, avatarId)
        Assert.AreEqual(expectedStatistics, statistics)
    Vessel.Create vesselStatisticTemplateSource vesselStatisticSink inputAvatarId

[<Test>]
let ``TransformFouling.It transforms fouling on the port side when the port side is specified.`` () =
    let inputSide = Port
    let inputAvatarId = "avatar"
    let vesselSingleStatisticSource (avatarId:string) (identifier:VesselStatisticIdentifier) : Statistic option =
        None
    let vesselSingleStatisticSink  (avatarId:string) (identifier:VesselStatisticIdentifier, statistic: Statistic) : unit = 
        Assert.AreEqual(VesselStatisticIdentifier.PortFouling, identifier)
    Vessel.TransformFouling vesselSingleStatisticSource vesselSingleStatisticSink inputAvatarId inputSide (Statistic.ChangeCurrentBy 0.25)

let private inputAvatarId = "avatar"

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
    Vessel.TransformFouling vesselSingleStatisticSource vesselSingleStatisticSink inputAvatarId inputSide (Statistic.ChangeCurrentBy inputValue)

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
    Vessel.Befoul vesselSingleStatisticSource vesselSingleStatisticSink inputAvatarId

[<Test>]
let ``Befoul.It will not increase the vessel's fouling when the vessel is already at maximum fouling.`` () =
    let expectedFoulage = 0.25
    let vesselSingleStatisticSource (_) (_) = 
        {MaximumValue=0.25; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let vesselSingleStatisticSink (_) (_:VesselStatisticIdentifier, statistic:Statistic):unit =
        Assert.AreEqual(expectedFoulage, statistic.CurrentValue)
    Vessel.Befoul vesselSingleStatisticSource vesselSingleStatisticSink inputAvatarId
