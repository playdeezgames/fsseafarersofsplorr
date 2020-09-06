module WorldUndockTests

open NUnit.Framework
open Splorr.Seafarers.Services
open CommonTestFixtures
open Splorr.Seafarers.Models

type TestWorldUndockContext
        (avatarIslandFeatureSink,
        avatarMessageSink) =
    interface WorldUndockContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink
        member _.avatarIslandFeatureSink : AvatarIslandFeatureSink = avatarIslandFeatureSink

[<Test>]
let ``Undock.It removes the feature for the avatar and adds a message.`` () =
    let avatarIslandFeatureSink (feature : AvatarIslandFeature option,_) =
        Assert.AreEqual(None, feature)
    let context : WorldUndockContext = 
        TestWorldUndockContext
            (avatarIslandFeatureSink,
            avatarExpectedMessageSink "You undock.") 
        :> WorldUndockContext
    World.Undock
        context
        avatarId
    