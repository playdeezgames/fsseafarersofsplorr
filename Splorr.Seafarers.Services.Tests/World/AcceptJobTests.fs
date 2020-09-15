module WorldAcceptJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldAcceptJobContext
        (avatarIslandSingleMetricSink, 
        avatarIslandSingleMetricSource,
        avatarJobSink,
        avatarJobSource,
        avatarMessageSink,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        islandJobPurger,
        islandSingleJobSource,
        islandSource) =
    interface AvatarAddMessagesContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface WorldAddMessagesContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
    interface IslandMakeKnownContext with
        member _.avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink = avatarIslandSingleMetricSink
        member _.avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource = avatarIslandSingleMetricSource
    interface WorldAcceptJobContext with
        member _.avatarJobSink         : AvatarJobSink = avatarJobSink
        member _.avatarJobSource       : AvatarJobSource = avatarJobSource
        member _.islandJobPurger       : IslandJobPurger = islandJobPurger
        member _.islandSingleJobSource : IslandSingleJobSource = islandSingleJobSource
        member _.islandSource          : IslandSource = islandSource
    interface AvatarAddMetricContext with
        member _.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member _.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource

[<Test>]
let ``AcceptJob.It does nothing when given an invalid island location.`` () =
    let inputLocation = (-1.0, -1.0)
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSink")
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandSource () =
        []
    let islandJobPurger (_) (_) =
        Assert.Fail("islandJobPurger")
    let islandSingleJobSource (_) (_) =
        Assert.Fail("islandSingleJobSource")
        None
    let context = 
        TestWorldAcceptJobContext
            (avatarIslandSingleMetricSink, 
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            Fixtures.Common.Fake.AvatarMessageSink,
            Fixtures.Common.Fake.AvatarSingleMetricSink,
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            islandJobPurger,
            islandSingleJobSource,
            islandSource) :> WorldAcceptJobContext
    Fixtures.Common.Dummy.AvatarId
    |> World.AcceptJob 
        context
        1u 
        inputLocation

[<Test>]
let ``AcceptJob.It adds a message to the world when given an 0 job index for the given valid island location.`` () =
    let inputWorld = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSink")
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandSource () =
        [inputLocation]
    let islandJobPurger (_) (_) =
        Assert.Fail("islandJobPurger")
    let islandSingleJobSource (_) (_) =
        Assert.Fail("islandSingleJobSource")
        None
    let context = 
        TestWorldAcceptJobContext
            (avatarIslandSingleMetricSink, 
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            Fixtures.Common.Mock.AvatarMessageSink "That job is currently unavailable.",
            Fixtures.Common.Fake.AvatarSingleMetricSink,
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            islandJobPurger,
            islandSingleJobSource,
            islandSource) :> WorldAcceptJobContext
    inputWorld
    |> World.AcceptJob 
        context
        0u 
        inputLocation

[<Test>]
let ``AcceptJob.It adds a message to the world when given an invalid job index for the given valid island location.`` () =
    let inputWorld =  Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSink")
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandJobPurger (_) (_) =
        Assert.Fail("islandJobPurger")
    let islandSingleJobSource (_) (_) = 
        None
    let islandSource() =
        [inputLocation]
    let context = 
        TestWorldAcceptJobContext
            (avatarIslandSingleMetricSink, 
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            Fixtures.Common.Mock.AvatarMessageSink "That job is currently unavailable.",
            Fixtures.Common.Fake.AvatarSingleMetricSink,
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            islandJobPurger,
            islandSingleJobSource,
            islandSource) :> WorldAcceptJobContext
    inputWorld
    |> World.AcceptJob 
        context
        0xFFFFFFFFu 
        inputLocation

[<Test>]
let ``AcceptJob.It adds a message to the world when the job is valid but the avatar already has a job.`` () =
    let inputWorld = 
        Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        {
            FlavorText  = ""
            Reward      = 0.0
            Destination = (0.0, 0.0)
        }
        |> Some
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSink")
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandJobPurger (_) (_) =
        Assert.Fail("islandJobPurger")
    let islandSingleJobSource (_) (_) = 
        Assert.Fail("islandSingleJobSource")
        None
    let islandSource() =
        [inputLocation]
    let context = 
        TestWorldAcceptJobContext
            (avatarIslandSingleMetricSink, 
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            Fixtures.Common.Mock.AvatarMessageSink "You must complete or abandon your current job before taking on a new one.",
            Fixtures.Common.Fake.AvatarSingleMetricSink,
            Fixtures.Common.Fake.AvatarSingleMetricSource,
            islandJobPurger,
            islandSingleJobSource,
            islandSource) :> WorldAcceptJobContext
    inputWorld
    |> World.AcceptJob 
        context
        1u 
        inputLocation


[<Test>]
let ``AcceptJob.It adds the given job to the avatar and eliminates it from the island's job list when given a valid island location and a valid job index and the avatar has no current job.`` () =
    let inputWorld = Fixtures.Common.Dummy.AvatarId
    let inputLocation = (0.0, 0.0)
    let inputDestination = (1.0, 1.0)
    let avatarJobSink (_) (actual: Job option) =
        Assert.True(actual.IsSome)
    let avatarJobSource (_) =
        None
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        ()
    let avatarIslandSingleMetricSource(_) (_) (identifier:AvatarIslandMetricIdentifier) = 
        match identifier with
        | _ ->
            None
    let islandJobPurger (_) (_) =
        ()
    let islandSingleJobSource (_) (_) = 
        {
            FlavorText=""
            Reward=0.0
            Destination=inputDestination
        }
        |> Some
    let islandSource() =
        [
            inputLocation
            inputDestination
        ]
    let avatarSingleMetricSource (_) (metric:Metric) : uint64 =
        match metric with 
        | Metric.AcceptedJob ->
            0UL
        | _ ->
            Assert.Fail(metric.ToString() |> sprintf "avatarSingleMetricSource - %s")
            0UL
    let context = 
        TestWorldAcceptJobContext
            (avatarIslandSingleMetricSink, 
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            Fixtures.Common.Mock.AvatarMessageSink "You accepted the job!",
            (Fixtures.Common.Mock.AvatarSingleMetricSink [Metric.AcceptedJob, 1UL]),
            avatarSingleMetricSource,
            islandJobPurger,
            islandSingleJobSource,
            islandSource) :> WorldAcceptJobContext
    World.AcceptJob 
        context
        1u 
        inputLocation
        inputWorld




