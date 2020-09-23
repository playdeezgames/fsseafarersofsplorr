module AvatarGetJobTests

open Splorr.Seafarers.Services
open NUnit.Framework

type TestAvatarGetJobContext
        (avatarJobSource) =
    interface OperatingContext
    interface Avatar.GetJobContext with
        member this.avatarJobSource: AvatarJobSource = avatarJobSource

[<Test>]
let ``GetJob.It calls the AvatarJobSource in the operating context.`` () =
    let mutable called = false
    let avatarJobSource (_) =
        called <- true
        None
    let context = TestAvatarGetJobContext(avatarJobSource) :> OperatingContext
    let expected = None
    let actual =
        Avatar.GetJob context Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
    