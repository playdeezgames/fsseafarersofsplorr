module WorldSetSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

[<Test>]
let ``SetSpeed.It produces all stop in the avatar when less than zero is passed.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=1.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = -1.0
    avatarId
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore

[<Test>]
let ``SetSpeed.It produces full speed when greater than one is passed.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=0.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = 2.0
    avatarId
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It produces half speed when one half is passed.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=0.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 0.5
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = 0.5
    avatarId
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It does nothing when a bogus avatarid is passed.`` () =
    let inputWorld = 
        bogusAvatarId
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            None
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let vesselSingleStatisticSink (_) (_) = 
        //assert speed being set in here
        raise (System.NotImplementedException "Kaboom set")
    let inputSpeed = 1.0
    let avatarMessageSink (_) (_) =
        Assert.Fail("Dont call me.")
    inputWorld
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink inputSpeed

[<Test>]
let ``SetSpeed.It produces full speed when one is passed.`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=0.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = 1.0
    avatarId
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It sets all stop when given zero`` () =
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=0.0} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    let inputSpeed = 0.0
    avatarId
    |> World.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSinkStub inputSpeed
    |> ignore




