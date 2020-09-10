module WorldSetHeadingTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures
open CommonTestFixtures

type TestWorldSetHeadingContext(vesselSingleStatisticSource) =
    interface AvatarGetHeadingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface WorldSetHeadingContext

[<Test>]
let ``SetHeading.It sets a new heading when given a valid avatar id.`` () =
    let heading = 1.5
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let expectedHeading = heading |> Angle.ToRadians
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) =
        Assert.AreEqual(VesselStatisticIdentifier.Heading, identifier)
        Assert.AreEqual(expectedHeading, statistic.CurrentValue)
    let context = TestWorldSetHeadingContext(vesselSingleStatisticSource) :> WorldSetHeadingContext
    avatarId
    |> World.SetHeading 
        context
        vesselSingleStatisticSource 
        vesselSingleStatisticSink 
        avatarMessageSinkStub 
        heading
    |> ignore
    

[<Test>]
let ``SetHeading.It does nothing when given an invalid avatar id`` () =
    let input = 
        bogusAvatarId
    let vesselSingleStatisticSource (_) (_) =
        None
    let vesselSingleStatisticSink (_) (_) =
        raise (System.NotImplementedException "Kaboom set")
    let heading = 1.5
    let context = TestWorldSetHeadingContext(vesselSingleStatisticSource) :> WorldSetHeadingContext
    input
    |> World.SetHeading 
        context
        vesselSingleStatisticSource 
        vesselSingleStatisticSink 
        avatarMessageSinkStub 
        heading
    |> ignore



