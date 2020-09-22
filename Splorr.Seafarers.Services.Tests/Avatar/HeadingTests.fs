module AvatarHeadingTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestAvatarSetHeadingContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface Avatar.SetHeadingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
type TestAvatarGetHeadingContext(vesselSingleStatisticSource) = 
    interface Avatar.GetHeadingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource


[<Test>]
let ``SetHeading.It sets a given heading.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputHeading = 2.5
    let originalHeading = 0.0
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=originalHeading} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedHeading = inputHeading |> Angle.ToRadians
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Heading,identifier)
        Assert.AreEqual(expectedHeading, statistic.CurrentValue)
    let context = TestAvatarSetHeadingContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Avatar.SetHeadingContext
    input
    |> Avatar.SetHeading 
        context 
        inputHeading

[<Test>]
let ``GetHeading.It gets the heading of an avatar.`` () =
    let actualSpeed = 
        {
            MinimumValue = 0.0
            MaximumValue = 6.3
            CurrentValue = 0.0
        }
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Heading -> actualSpeed |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let inputAvatarId="avatar"
    let expected = 0.0 |> Some
    let context = TestAvatarGetHeadingContext(vesselSingleStatisticSource) :> Avatar.GetHeadingContext
    let actual =
        inputAvatarId
        |> Avatar.GetHeading 
            context
    Assert.AreEqual(expected, actual)



