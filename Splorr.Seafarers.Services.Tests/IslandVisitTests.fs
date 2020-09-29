module IslandVisitTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System

type TestIslandAddVisitContext (avatarIslandSingleMetricSink, avatarIslandSingleMetricSource, epochSecondsSource) =
    interface ServiceContext
    interface IslandVisit.AddContext with
        member _.epochSecondsSource : EpochSecondsSource = epochSecondsSource
    interface AvatarIslandMetric.GetContext with
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
    interface AvatarIslandMetric.PutContext with
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink

type TestIslandMakeKnownContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource) =
    interface ServiceContext
    interface AvatarIslandMetric.GetContext with
        member this.avatarIslandSingleMetricSource: AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
    interface AvatarIslandMetric.PutContext with
        member this.avatarIslandSingleMetricSink: AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink

[<Test>]
let ``AddVisit.It increases visit count to one and sets last visit to given turn when there is no last visit and no visit count.`` () =
    let turn = 100UL
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier: AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount
        | AvatarIslandMetricIdentifier.LastVisit ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier:AvatarIslandMetricIdentifier) (value: uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(1UL, value)
        | AvatarIslandMetricIdentifier.LastVisit ->
            Assert.AreEqual(turn, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let epochSecondsSource = (fun () -> turn)
    let context = 
        TestIslandAddVisitContext
            (avatarIslandSingleMetricSink, 
            avatarIslandSingleMetricSource, 
            epochSecondsSource) 
            :> ServiceContext
    IslandVisit.Add 
        context
        Fixtures.Common.Dummy.AvatarId
        location


[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when there is no last visit.`` () =
    let turn = 100UL
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier: AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.LastVisit ->
            (turn + 1UL)
            |> Some
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier:AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(1UL, value)
        | AvatarIslandMetricIdentifier.LastVisit ->
            Assert.AreEqual(turn, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let epochSecondsSource = (fun () -> turn)
    let context = 
        TestIslandAddVisitContext
            (avatarIslandSingleMetricSink, 
            avatarIslandSingleMetricSource, 
            epochSecondsSource) 
            :> ServiceContext
    IslandVisit.Add 
        context
        Fixtures.Common.Dummy.AvatarId
        location

[<Test>]
let ``AddVisit.It increases visit count by one and sets last visit to given turn when the given turn is after the last visit.`` () =
    let turn = 100UL
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier: AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount -> 
            1UL 
            |> Some
        | AvatarIslandMetricIdentifier.LastVisit -> 
            0UL 
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier: AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(2UL, value)
        | AvatarIslandMetricIdentifier.LastVisit ->
            Assert.AreEqual(turn, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let epochSecondsSource = (fun () -> turn)
    let context = TestIslandAddVisitContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource, epochSecondsSource) :> ServiceContext
    IslandVisit.Add 
        context
        Fixtures.Common.Dummy.AvatarId
        location

[<Test>]
let ``AddVisit.It does not update visit count when given turn was prior or equal to last visit.`` () =
    let turn = 0UL
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier: AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount -> 
            1UL 
            |> Some
        | AvatarIslandMetricIdentifier.LastVisit -> 
            turn 
            |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (_) (_)= 
        Assert.Fail("avatarIslandSingleMetricSink")
    let epochSecondsSource = (fun () -> turn)
    let context = TestIslandAddVisitContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource, epochSecondsSource) :> ServiceContext
    IslandVisit.Add 
        context
        Fixtures.Common.Dummy.AvatarId
        location

[<Test>]
let ``MakeKnown.It does nothing when the given island is already known.`` () =
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            0UL |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (_) (_)= 
        Assert.Fail("avatarIslandSingleMetricSink")
    let context = TestIslandMakeKnownContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource) :> ServiceContext
    IslandVisit.MakeKnown 
        context
        Fixtures.Common.Dummy.AvatarId
        location

[<Test>]
let ``MakeKnown.It mutates the island's visit count to Some 0 when the given island is not known.`` () =
    let location = (0.0, 0.0)
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let avatarIslandSingleMetricSink(_) (_) (identifier: AvatarIslandMetricIdentifier) (value:uint64)= 
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            Assert.AreEqual(0UL, value)
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSink - %s")
    let context = TestIslandMakeKnownContext(avatarIslandSingleMetricSink, avatarIslandSingleMetricSource) :> ServiceContext
    IslandVisit.MakeKnown 
        context
        Fixtures.Common.Dummy.AvatarId
        location


