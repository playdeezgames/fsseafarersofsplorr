module VesselTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let vessel =
    {
        Tonnage = 100.0
        FoulRate = 0.01
    }

[<Test>]
let ``Create.It creates a vessel.`` () =
    let inputTonnage = 1.0
    let inputAvatarId = "avatar"
    let expected =
        {
            Tonnage = inputTonnage
            FoulRate=0.001
        }
    let vesselStatisticTemplateSource() : Map<VesselStatisticIdentifier, VesselStatisticTemplate> = Map.empty
    let vesselStatisticSink (avatarId:string) (statistics:Map<VesselStatisticIdentifier, Statistic>) : unit =
        Assert.AreEqual(inputAvatarId, avatarId)
        Assert.AreEqual(Map.empty, statistics)
    let actual =
        inputTonnage
        |> Vessel.Create vesselStatisticTemplateSource vesselStatisticSink inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``TransformFouling.It transforms fouling on the port side when the port side is specified.`` () =
    let input = vessel
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
    let input = vessel
    let inputFoulRate = input.FoulRate/2.0
    let expectedFoulage = inputFoulRate
    let vesselSingleStatisticSource (_) (_) = 
        {MaximumValue=0.25; MinimumValue=0.0; CurrentValue=0.0} |> Some
    let vesselSingleStatisticSink (_) (_:VesselStatisticIdentifier, statistic:Statistic):unit =
        Assert.AreEqual(expectedFoulage, statistic.CurrentValue)
    input
    |> Vessel.Befoul vesselSingleStatisticSource vesselSingleStatisticSink inputAvatarId

[<Test>]
let ``Befoul.It will not increase the vessel's fouling when the vessel is already at maximum fouling.`` () =
    let input = 
        vessel
    let expectedFoulage = 0.25
    let vesselSingleStatisticSource (_) (_) = 
        {MaximumValue=0.25; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let vesselSingleStatisticSink (_) (_:VesselStatisticIdentifier, statistic:Statistic):unit =
        Assert.AreEqual(expectedFoulage, statistic.CurrentValue)
    input
    |> Vessel.Befoul vesselSingleStatisticSource vesselSingleStatisticSink inputAvatarId
