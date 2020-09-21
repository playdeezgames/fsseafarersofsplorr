module WorldSetSpeedTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldSetSpeedContext(avatarMessageSink, vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface Avatar.GetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.SetSpeedContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface WorldAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface WorldSetSpeedContext

[<Test>]
let ``SetSpeed.It produces all stop in the avatar when less than zero is passed.`` () =
    let mutable speed = 1.0
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=speed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
        speed <- statistic.CurrentValue
    let inputSpeed = -1.0
    let context = 
        TestWorldSetSpeedContext
            (Fixtures.Common.Mock.AvatarMessageSink "You set your speed to 0.00.", 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> WorldSetSpeedContext
    Fixtures.Common.Dummy.AvatarId
    |> World.SetSpeed 
        context
        inputSpeed
    |> ignore

[<Test>]
let ``SetSpeed.It produces full speed when greater than one is passed.`` () =
    let mutable speed = 1.0
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=speed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
        speed <- statistic.CurrentValue
    let inputSpeed = 2.0
    let context = 
        TestWorldSetSpeedContext
            (Fixtures.Common.Mock.AvatarMessageSink "You set your speed to 1.00.", 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> WorldSetSpeedContext
    Fixtures.Common.Dummy.AvatarId
    |> World.SetSpeed 
        context
        inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It produces half speed when one half is passed.`` () =
    let mutable speed = 0.0
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=speed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 0.5
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
        speed <- statistic.CurrentValue
    let inputSpeed = 0.5
    let context = 
        TestWorldSetSpeedContext
            (Fixtures.Common.Mock.AvatarMessageSink "You set your speed to 0.50.", 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> WorldSetSpeedContext
    Fixtures.Common.Dummy.AvatarId
    |> World.SetSpeed 
        context
        inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It does nothing when a bogus avatarid is passed.`` () =
    let inputWorld = 
        Fixtures.Common.Dummy.BogusAvatarId
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
    let context = TestWorldSetSpeedContext(avatarMessageSink, vesselSingleStatisticSink, vesselSingleStatisticSource) :> WorldSetSpeedContext
    inputWorld
    |> World.SetSpeed 
        context
        inputSpeed

[<Test>]
let ``SetSpeed.It produces full speed when one is passed.`` () =
    let mutable speed = 0.0
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0;MaximumValue=1.0; CurrentValue=speed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identfier:VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed, identfier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
        speed <- statistic.CurrentValue
    let inputSpeed = 1.0
    let context = 
        TestWorldSetSpeedContext
            (Fixtures.Common.Mock.AvatarMessageSink "You set your speed to 1.00.", 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> WorldSetSpeedContext
    Fixtures.Common.Dummy.AvatarId
    |> World.SetSpeed 
        context
        inputSpeed
    |> ignore


[<Test>]
let ``SetSpeed.It sets all stop when given zero`` () =
    let mutable speed = 1.0
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
        speed <- statistic.CurrentValue
    let inputSpeed = 0.0
    let context = 
        TestWorldSetSpeedContext
            (Fixtures.Common.Mock.AvatarMessageSink "You set your speed to 0.00.", 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> WorldSetSpeedContext
    Fixtures.Common.Dummy.AvatarId
    |> World.SetSpeed 
        context
        inputSpeed
    |> ignore




