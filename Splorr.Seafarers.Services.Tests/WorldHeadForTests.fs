module WorldHeadForTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open WorldTestFixtures
open CommonTestFixtures

type TestWorldHeadForContext(vesselSingleStatisticSource) =
    interface AvatarGetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface WorldHeadForContext

[<Test>]
let ``HeadFor.It adds a message when the island name does not exist.`` () =
    let inputWorld = avatarId
    let expectedMessage = "I don't know how to get to `yermom`."
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PositionX 
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0}|> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let vesselSingleStatisticSink (_) (_) =
        raise (System.NotImplementedException "Kaboom set")
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandLocationByNameSource (_) =
        None
    let context = TestWorldHeadForContext(vesselSingleStatisticSource) :> WorldHeadForContext
    inputWorld
    |> World.HeadFor 
        context
        avatarIslandSingleMetricSource
        (avatarExpectedMessageSink expectedMessage)
        islandLocationByNameSource
        vesselSingleStatisticSource 
        vesselSingleStatisticSink 
        "yermom"

[<Test>]
let ``HeadFor.It adds a message when the island name exists but is not known.`` () =
    let inputWorld =  avatarId
    let expectedMessage = "I don't know how to get to `Uno`."
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PositionX 
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0}|> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let vesselSingleStatisticSink (_) (_) =
        raise (System.NotImplementedException "Kaboom set")
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandLocationByNameSource (_) =
        []
        |> List.tryHead
    let context = TestWorldHeadForContext(vesselSingleStatisticSource) :> WorldHeadForContext
    inputWorld
    |> World.HeadFor 
        context
        avatarIslandSingleMetricSource
        (avatarExpectedMessageSink expectedMessage)
        islandLocationByNameSource
        vesselSingleStatisticSource 
        vesselSingleStatisticSink 
        "Uno"

[<Test>]
let ``HeadFor.It sets the heading when the island name exists and is known.`` () =
    let inputWorld =
         avatarId
    let firstExpectedMessage = "You set your heading to 0.00°." //note - value for heading not actually stored, but is really 180
    let secondExpectedMessage = "You head for `Uno`."
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=1.0}|> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0}|> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let expectedHeading = System.Math.PI
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic: Statistic) =
        Assert.AreEqual(VesselStatisticIdentifier.Heading, identifier)
        Assert.AreEqual(expectedHeading, statistic.CurrentValue)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            0UL |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandLocationByNameSource (_) =
        [(0.0, 0.0)]
        |> List.tryHead
    let context = TestWorldHeadForContext(vesselSingleStatisticSource) :> WorldHeadForContext
    inputWorld
    |> World.HeadFor 
        context
        avatarIslandSingleMetricSource
        (avatarMessagesSinkFake [firstExpectedMessage; secondExpectedMessage])
        islandLocationByNameSource
        vesselSingleStatisticSource 
        vesselSingleStatisticSink 
        "Uno"




