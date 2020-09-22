module WorldDistanceToTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldDistanceToContext
        (avatarIslandSingleMetricSource,
        avatarMessageSink,
        islandLocationByNameSource,
        vesselSingleStatisticSource) =
    interface Avatar.GetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface Avatar.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface World.AddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface World.DistanceToContext with
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
        member _.islandLocationByNameSource     : IslandLocationByNameSource = islandLocationByNameSource

[<Test>]
let ``DistanceTo.It adds a 'unknown island' message when given a bogus island name.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputName = "$$$$$$$"
    let expectedMessage = inputName |> sprintf "I don't know how to get to `%s`."
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandLocationByNameSource (_) =
        None
    let context = TestWorldDistanceToContext(avatarIslandSingleMetricSource, (Fixtures.Common.Mock.AvatarMessageSink expectedMessage), islandLocationByNameSource, vesselSingleStatisticSource) :> World.DistanceToContext
    input
    |> World.DistanceTo 
        context
        inputName

[<Test>]
let ``DistanceTo.It adds a 'unknown island' message when given a valid island name that is not known.`` () =
    let inputName = "yermom"
    let input = Fixtures.Common.Dummy.AvatarId
    let expectedMessage = inputName |> sprintf "I don't know how to get to `%s`."
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandLocationByNameSource (name:string) =
        if name = inputName then
            (0.0, 0.0)
            |> Some
        else
            None
    let context = TestWorldDistanceToContext(avatarIslandSingleMetricSource, (Fixtures.Common.Mock.AvatarMessageSink expectedMessage), islandLocationByNameSource, vesselSingleStatisticSource) :> World.DistanceToContext
    input
    |> World.DistanceTo 
        context
        inputName

[<Test>]
let ``DistanceTo.It adds a 'distance to island' message when given a valid island name that is known.`` () =
    let inputLocation = (0.0, 0.0)
    let inputName = "yermom"
    let input = 
        Fixtures.Common.Dummy.AvatarId
    let avatarPosition = (0.0, 0.0)
    let expectedMessage = (inputName, Location.DistanceTo inputLocation avatarPosition) ||> sprintf "Distance to `%s` is %f."
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=avatarPosition |> fst} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue = 0.0; MaximumValue=100.0; CurrentValue=avatarPosition |> snd} |> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            0UL |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandLocationByNameSource (name:string) =
        if name = inputName then
            inputLocation
            |> Some
        else
            None
    let context = TestWorldDistanceToContext(avatarIslandSingleMetricSource, (Fixtures.Common.Mock.AvatarMessageSink expectedMessage), islandLocationByNameSource, vesselSingleStatisticSource) :> World.DistanceToContext
    input
    |> World.DistanceTo 
        context
        inputName

