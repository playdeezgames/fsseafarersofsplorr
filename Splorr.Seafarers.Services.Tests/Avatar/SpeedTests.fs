module AvatarSpeedTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models
open AvatarTestFixtures

let inputAvatarId = "avatar"

type TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface Vessel.SetSpeedContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetSpeedContext(vesselSingleStatisticSource) =
    interface Vessel.GetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetEffectiveSpeedContext(vesselSingleStatisticSource) =
    interface Vessel.GetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.GetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetCurrentFoulingContext(vesselSingleStatisticSource) = 
    interface Avatar.GetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetMaximumFoulingContext(vesselSingleStatisticSource) =
    interface Avatar.GetMaximumFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource



[<Test>]
let ``SetSpeed.It sets all stop when given less than zero.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputSpeed = -1.0
    let originalSpeed = 0.5
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.SetSpeedContext
    input
    |> Vessel.SetSpeed context inputSpeed

[<Test>]
let ``SetSpeed.It sets full speed when gives more than one.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputSpeed = 2.0
    let originalSpeed = 0.5
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.SetSpeedContext
    input
    |> Vessel.SetSpeed context inputSpeed

[<Test>]
let ``SetSpeed.It sets half speed when given half speed.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputSpeed = 0.5
    let originalSpeed = 1.0
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 0.5
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.SetSpeedContext
    input
    |> Vessel.SetSpeed context inputSpeed


[<Test>]
let ``SetSpeed.It sets full speed when given one.`` () =    
    let input = Fixtures.Common.Dummy.AvatarId
    let inputSpeed = 1.0
    let originalSpeed = 0.5
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.SetSpeedContext
    input
    |> Vessel.SetSpeed context inputSpeed


[<Test>]
let ``SetSpeed.It sets all stop when given zero.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputSpeed = 0.0
    let originalSpeed = 0.5
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> Vessel.SetSpeedContext
    input
    |> Vessel.SetSpeed context inputSpeed


[<Test>]
let ``GetSpeed.It gets the speed of an avatar.`` () =
    let actualSpeed = 
        {
            MinimumValue = 0.0
            MaximumValue = 1.0
            CurrentValue = 0.5
        }
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed -> actualSpeed |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let inputAvatarId="avatar"
    let expected = 0.5 |> Some
    let context = TestAvatarGetSpeedContext(vesselSingleStatisticSource) :> Vessel.GetSpeedContext
    let actual =
        inputAvatarId
        |> Vessel.GetSpeed context
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetEffectiveSpeed.It returns full speed when there is no fouling.`` () =
    let expected = 1.0
    let context = TestAvatarGetEffectiveSpeedContext(Fixtures.Common.Stub.VesselSingleStatisticSource) :> ServiceContext
    let actual =
        Avatar.GetEffectiveSpeed context inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetEffectiveSpeed.It returns proportionally reduced speed when there is fouling.`` () =
    let expected = 0.125
    let vesselSingleStatisticSource (_) (_) = {MinimumValue=0.0;MaximumValue=0.25;CurrentValue=0.25} |> Some
    let context = TestAvatarGetEffectiveSpeedContext(vesselSingleStatisticSource) :> ServiceContext
    let actual =
        Avatar.GetEffectiveSpeed context inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetCurrentFouling.It returns the current fouling for the Avatar.`` () =
    let vesselSingleStatisticSource (_) (_) =
        {MaximumValue=0.5; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let inputAvatarId = "avatar"
    let expected = 0.5
    let context = TestAvatarGetCurrentFoulingContext(vesselSingleStatisticSource) :> Avatar.GetCurrentFoulingContext
    let actual = 
        Avatar.GetCurrentFouling context inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetMaximumFouling.It returns the maximum fouling for the Avatar.`` () =
    let vesselSingleStatisticSource (_) (_) =
        {MaximumValue=0.5; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let inputAvatarId = "avatar"
    let expected = 1.0
    let context = TestAvatarGetMaximumFoulingContext(vesselSingleStatisticSource) :> Avatar.GetMaximumFoulingContext
    let actual = 
        Avatar.GetMaximumFouling context inputAvatarId
    Assert.AreEqual(expected, actual)

    



