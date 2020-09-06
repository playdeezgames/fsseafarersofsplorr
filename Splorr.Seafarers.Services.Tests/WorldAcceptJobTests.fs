module WorldAcceptJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

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
    avatarId
    |> World.AcceptJob 
        avatarIslandSingleMetricSink
        avatarIslandSingleMetricSource
        avatarJobSink
        avatarJobSource
        avatarMessageSinkStub 
        avatarSingleMetricSinkExplode
        avatarSingleMetricSourceStub
        islandJobPurger
        islandSingleJobSource
        islandSource
        1u 
        inputLocation

[<Test>]
let ``AcceptJob.It adds a message to the world when given an 0 job index for the given valid island location.`` () =
    let inputWorld = avatarId
    let inputLocation = (0.0, 0.0)
    let expectedMessage = "That job is currently unavailable."
    let expected = inputWorld
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let avatarIslandSingleMetricSink (_) (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSink")
    let avatarIslandSingleMetricSource (_) (_) (_) =
        Assert.Fail("avatarIslandSingleMetricSource")
        None
    let islandJobSink (_) (_) =
        Assert.Fail("islandJobSink")
    let islandJobSource (_) =
        Assert.Fail("islandJobSource")
        []
    let islandSource () =
        [inputLocation]
    let islandJobPurger (_) (_) =
        Assert.Fail("islandJobPurger")
    let islandSingleJobSource (_) (_) =
        Assert.Fail("islandSingleJobSource")
        None
    inputWorld
    |> World.AcceptJob 
        avatarIslandSingleMetricSink
        avatarIslandSingleMetricSource
        avatarJobSink
        avatarJobSource
        (avatarExpectedMessageSink expectedMessage)
        avatarSingleMetricSinkExplode
        avatarSingleMetricSourceStub
        islandJobPurger
        islandSingleJobSource
        islandSource
        0u 
        inputLocation

[<Test>]
let ``AcceptJob.It adds a message to the world when given an invalid job index for the given valid island location.`` () =
    let inputWorld =  avatarId
    let inputLocation = (0.0, 0.0)
    let expectedMessage = "That job is currently unavailable."
    let expected =
        inputWorld
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
    inputWorld
    |> World.AcceptJob 
        avatarIslandSingleMetricSink
        avatarIslandSingleMetricSource
        avatarJobSink
        avatarJobSource
        (avatarExpectedMessageSink expectedMessage)
        avatarSingleMetricSinkExplode
        avatarSingleMetricSourceStub
        islandJobPurger
        islandSingleJobSource
        islandSource
        0xFFFFFFFFu 
        inputLocation

[<Test>]
let ``AcceptJob.It adds a message to the world when the job is valid but the avatar already has a job.`` () =
    let inputWorld = 
        avatarId
    let inputLocation = (0.0, 0.0)
    let expectedMessage = "You must complete or abandon your current job before taking on a new one."
    let expected =
        inputWorld
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
    inputWorld
    |> World.AcceptJob 
        avatarIslandSingleMetricSink
        avatarIslandSingleMetricSource
        avatarJobSink
        avatarJobSource
        (avatarExpectedMessageSink expectedMessage)
        avatarSingleMetricSinkExplode
        avatarSingleMetricSourceStub
        islandJobPurger
        islandSingleJobSource
        islandSource
        1u 
        inputLocation


[<Test>]
let ``AcceptJob.It adds the given job to the avatar and eliminates it from the island's job list when given a valid island location and a valid job index and the avatar has no current job.`` () =
    let inputWorld = avatarId
    let inputLocation = (0.0, 0.0)
    let inputJob : Job =
        { 
            FlavorText=""
            Destination = inputLocation
            Reward = 1.0
        }
    let inputDestination = (1.0, 1.0)
    let expectedMessage = "You accepted the job!"
    let expectedDestination =
        inputDestination
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
    World.AcceptJob 
        avatarIslandSingleMetricSink
        avatarIslandSingleMetricSource
        avatarJobSink
        avatarJobSource
        (avatarExpectedMessageSink expectedMessage)
        (assertAvatarSingleMetricSink [Metric.AcceptedJob, 1UL])
        avatarSingleMetricSourceStub
        islandJobPurger
        islandSingleJobSource
        islandSource
        1u 
        inputLocation
        inputWorld




