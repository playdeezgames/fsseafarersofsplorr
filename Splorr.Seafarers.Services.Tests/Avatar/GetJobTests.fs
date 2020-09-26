module AvatarGetJobTests

open Splorr.Seafarers.Services
open NUnit.Framework

type TestAvatarGetJobContext
        (avatarJobSource) =
    interface ServiceContext
    interface AvatarJob.GetContext with
        member this.avatarJobSource: AvatarJobSource = avatarJobSource

[<Test>]
let ``GetJob.It calls the AvatarJobSource in the operating context.`` () =
    let mutable called = false
    let avatarJobSource (_) =
        called <- true
        None
    let context = TestAvatarGetJobContext(avatarJobSource) :> ServiceContext
    let expected = None
    let actual =
        AvatarJob.Get context Fixtures.Common.Dummy.AvatarId
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)
    