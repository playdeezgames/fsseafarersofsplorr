module WorldAcceptJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``AcceptJob.It gives a message when the island does not exist.`` () =
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList )
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    World.AcceptJob 
        context 
        Dummies.ValidJobIndex 
        Dummies.InvalidIslandLocation 
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)

[<Test>]
let ``AcceptJob.It gives a message when the avatar already has a job.`` () =
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetJob = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList )
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> AvatarJob.GetContext).avatarJobSource := Spies.Source (calledGetJob, Some Dummies.ValidJob)
    World.AcceptJob 
        context 
        Dummies.ValidJobIndex 
        Dummies.ValidIslandLocation 
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetJob.Value)

[<Test>]
let ``AcceptJob.It gives a message when the avatar has no job but the job index is invalid.`` () =
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetJob = ref false
    let calledGetIslandJob = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList )
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> AvatarJob.GetContext).avatarJobSource := Spies.Source (calledGetJob, None)
    (context :> WorldIslands.GetIslandJobContext).islandSingleJobSource := Spies.Source (calledGetIslandJob, None)
    World.AcceptJob 
        context 
        Dummies.ValidJobIndex 
        Dummies.ValidIslandLocation 
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetJob.Value)
    Assert.IsTrue(calledGetIslandJob.Value)

[<Test>]
let ``AcceptJob.It accepts a job when the avatar has no job and the job index is valid and makes the destination island known if not already known.`` () =
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetJob = ref false
    let calledGetIslandJob = ref false
    let calledGetAvatarMetric = ref false
    let calledSetAvatarMetric = ref false
    let calledSetJob = ref false
    let calledGetAvatarIslandMetric = ref false
    let calledPurgeIslandJob = ref false
    let calledPutAvatarIslandMetric = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList )
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> AvatarJob.GetContext).avatarJobSource := Spies.Source (calledGetJob, None)
    (context :> WorldIslands.GetIslandJobContext).islandSingleJobSource := Spies.Source (calledGetIslandJob, Some Dummies.ValidJob)
    (context :> AvatarMetric.GetMetricContext).avatarSingleMetricSource := Spies.Source (calledGetAvatarMetric, 0UL)
    (context :> AvatarMetric.SetMetricContext).avatarSingleMetricSink := Spies.Expect (calledSetAvatarMetric, (Dummies.ValidAvatarId, Metric.AcceptedJob, 1UL))
    (context :> Avatar.SetJobContext).avatarJobSink := Spies.Sink calledSetJob
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource := Spies.Source (calledGetAvatarIslandMetric, None)
    (context :> AvatarIslandMetric.PutContext).avatarIslandSingleMetricSink := Spies.Expect (calledPutAvatarIslandMetric, (Dummies.ValidAvatarId, Dummies.ValidIslandLocation, AvatarIslandMetricIdentifier.VisitCount, 0UL))
    (context :> IslandJob.PurgeContext).islandJobPurger := Spies.Expect (calledPurgeIslandJob,(Dummies.ValidIslandLocation, Dummies.ValidJobIndex))
    World.AcceptJob 
        context 
        Dummies.ValidJobIndex 
        Dummies.ValidIslandLocation 
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetJob.Value)
    Assert.IsTrue(calledGetIslandJob.Value)
    Assert.IsTrue(calledGetAvatarMetric.Value)
    Assert.IsTrue(calledSetAvatarMetric.Value)
    Assert.IsTrue(calledSetJob.Value)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)
    Assert.IsTrue(calledPurgeIslandJob.Value)
    Assert.IsTrue(calledPutAvatarIslandMetric.Value)

[<Test>]
let ``AcceptJob.It accepts a job when the avatar has no job and the job index is valid.`` () =
    let calledGetIslandList = ref false
    let calledAddMessage = ref false
    let calledGetJob = ref false
    let calledGetIslandJob = ref false
    let calledGetAvatarMetric = ref false
    let calledSetAvatarMetric = ref false
    let calledSetJob = ref false
    let calledGetAvatarIslandMetric = ref false
    let calledPurgeIslandJob = ref false
    let context = Contexts.TestContext()
    (context :> Island.GetListContext).islandSource := Spies.Source (calledGetIslandList, Dummies.ValidIslandList )
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessage
    (context :> AvatarJob.GetContext).avatarJobSource := Spies.Source (calledGetJob, None)
    (context :> WorldIslands.GetIslandJobContext).islandSingleJobSource := Spies.Source (calledGetIslandJob, Some Dummies.ValidJob)
    (context :> AvatarMetric.GetMetricContext).avatarSingleMetricSource := Spies.Source (calledGetAvatarMetric, 0UL)
    (context :> AvatarMetric.SetMetricContext).avatarSingleMetricSink := Spies.Expect (calledSetAvatarMetric, (Dummies.ValidAvatarId, Metric.AcceptedJob, 1UL))
    (context :> Avatar.SetJobContext).avatarJobSink := Spies.Sink calledSetJob
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource := Spies.Source (calledGetAvatarIslandMetric, Some 0UL)
    (context :> IslandJob.PurgeContext).islandJobPurger := Spies.Expect (calledPurgeIslandJob,(Dummies.ValidIslandLocation, Dummies.ValidJobIndex))
    World.AcceptJob 
        context 
        Dummies.ValidJobIndex 
        Dummies.ValidIslandLocation 
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetIslandList.Value)
    Assert.IsTrue(calledAddMessage.Value)
    Assert.IsTrue(calledGetJob.Value)
    Assert.IsTrue(calledGetIslandJob.Value)
    Assert.IsTrue(calledGetAvatarMetric.Value)
    Assert.IsTrue(calledSetAvatarMetric.Value)
    Assert.IsTrue(calledSetJob.Value)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)
    Assert.IsTrue(calledPurgeIslandJob.Value)
