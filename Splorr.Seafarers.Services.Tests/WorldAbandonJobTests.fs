module WorldAbandonJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``AbandonJob.It add message when there is no job to abandon.`` () =
    let calledGetJob = ref false
    let calledAddMessages = ref false
    let context = Contexts.TestContext()
    (context :> AvatarJob.GetContext).avatarJobSource := Spies.Source (calledGetJob, None)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink calledAddMessages
    World.AbandonJob 
        context 
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetJob.Value)
    Assert.IsTrue(calledAddMessages.Value)

[<Test>]
let ``AbandonJob.It abandons a job and adds a message.`` () =
    let calledGetJob = ref false
    let calledAddMessages = ref false
    let calledGetShipmateStatistic = ref false
    let calledGetAvatarMetric = ref false
    let calledSetAvatarmetric = ref false
    let calledSetJob = ref false
    let calledSetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> AvatarJob.GetContext).avatarJobSource := 
        Spies.Source (calledGetJob, Some Dummies.ValidJob)
    (context :> AvatarMessages.AddContext).avatarMessageSink := 
        Spies.Sink calledAddMessages
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource := 
        Spies.Source (calledGetShipmateStatistic, Some {MaximumValue=100.0;MinimumValue=0.0;CurrentValue=50.0})
    (context :> ShipmateStatistic.PutContext).shipmateSingleStatisticSink := 
        Spies.Expect (calledSetShipmateStatistic, 
            (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Reputation, 
                Some {MaximumValue=100.0;MinimumValue=0.0;CurrentValue=49.0}))
    (context :> AvatarMetric.GetMetricContext).avatarSingleMetricSource := 
        Spies.Source (calledGetAvatarMetric, 0UL)
    (context :> AvatarMetric.SetMetricContext).avatarSingleMetricSink := 
        Spies.Expect (calledSetAvatarmetric, (Dummies.ValidAvatarId,Metric.AbandonedJob,1UL))
    (context :> Avatar.SetJobContext).avatarJobSink := 
        Spies.Expect (calledSetJob, (Dummies.ValidAvatarId, None))
    World.AbandonJob 
        context 
        Dummies.ValidAvatarId
    Assert.IsTrue(calledGetJob.Value)
    Assert.IsTrue(calledAddMessages.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)
    Assert.IsTrue(calledGetAvatarMetric.Value)
    Assert.IsTrue(calledSetAvatarmetric.Value)
    Assert.IsTrue(calledSetJob.Value)
    Assert.IsTrue(calledSetShipmateStatistic.Value)


