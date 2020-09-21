module AvatarPositionTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestAvatarSetPositionContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface Avatar.SetPositionContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetPositionContext(vesselSingleStatisticSource) =
    interface Avatar.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
     
[<Test>]
let ``SetPosition.It sets a given position.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputPosition = (10.0, 11.0)
    let originalPosition = (8.0, 9.0)
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=originalPosition |> fst} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=originalPosition |> snd} |> Some
        | _ -> 
            raise (System.NotImplementedException "Source - Dont call me.")
            None
    let expectedPosition = inputPosition
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            Assert.AreEqual(expectedPosition |> fst, statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionY ->
            Assert.AreEqual(expectedPosition |> snd, statistic.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Sink - Dont call me.")
    let context = TestAvatarSetPositionContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Avatar.SetPositionContext
    input
    |> Avatar.SetPosition context inputPosition

[<Test>]
let ``GetPosition.It gets the position of an avatar.`` () =
    let actualX = 
        {
            MinimumValue = 0.0
            MaximumValue = 50.0
            CurrentValue = 25.0
        }
    let actualY = 
        {
            MinimumValue = 50.0
            MaximumValue = 100.0
            CurrentValue = 75.0
        }
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PositionX -> actualX |> Some
        | VesselStatisticIdentifier.PositionY -> actualY |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let inputAvatarId="avatar"
    let expected = 
        (actualX.CurrentValue, actualY.CurrentValue) 
        |> Some
    let context = TestAvatarGetPositionContext(vesselSingleStatisticSource) :> Avatar.GetPositionContext
    let actual =
        inputAvatarId
        |> Avatar.GetPosition context
    Assert.AreEqual(expected, actual)


