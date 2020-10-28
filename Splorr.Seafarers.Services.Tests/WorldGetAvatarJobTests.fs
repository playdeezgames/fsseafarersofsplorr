module WorldGetAvatarJobTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarJob.It gets the avatar's current job.`` () =
    let calledAvatarGetJob = ref false
    let context = Contexts.TestContext()
    (context :> AvatarJob.GetContext).avatarJobSource := Spies.Source(calledAvatarGetJob, None)
    let actual = 
        World.GetAvatarJob
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledAvatarGetJob.Value)


