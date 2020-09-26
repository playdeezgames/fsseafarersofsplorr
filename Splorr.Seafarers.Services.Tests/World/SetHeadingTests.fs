module WorldSetHeadingTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldSetHeadingContext(avatarMessageSink, vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface Avatar.GetHeadingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.SetHeadingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarMessages.AddContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface World.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink

[<Test>]
let ``SetHeading.It sets a new heading when given a valid avatar id.`` () =
    let heading = 1.5
    let mutable currentHeading = 0.0
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=currentHeading} |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedHeading = heading |> Angle.ToRadians
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) =
        Assert.AreEqual(VesselStatisticIdentifier.Heading, identifier)
        Assert.AreEqual(expectedHeading, statistic.CurrentValue)
        currentHeading <- statistic.CurrentValue
    let context = 
        TestWorldSetHeadingContext
            (Fixtures.Common.Mock.AvatarMessageSink "You set your heading to 1.50°.", 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> ServiceContext
    Fixtures.Common.Dummy.AvatarId
    |> World.SetHeading 
        context
        heading
    |> ignore
    

[<Test>]
let ``SetHeading.It does nothing when given an invalid avatar id`` () =
    let input = 
        Fixtures.Common.Dummy.BogusAvatarId
    let vesselSingleStatisticSource (_) (_) =
        None
    let vesselSingleStatisticSink (_) (_) =
        raise (System.NotImplementedException "Kaboom set")
    let heading = 1.5
    let context = 
        TestWorldSetHeadingContext
            (Fixtures.Common.Fake.AvatarMessageSink, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> ServiceContext
    input
    |> World.SetHeading 
        context
        heading
    |> ignore



