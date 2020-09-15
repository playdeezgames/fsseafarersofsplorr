module WorldUndockTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type TestWorldUndockContext
        (avatarIslandFeatureSink,
        avatarMessageSink) =
    interface WorldUndockContext with
        member _.avatarMessageSink : AvatarMessageSink = avatarMessageSink
        member _.avatarIslandFeatureSink : AvatarIslandFeatureSink = avatarIslandFeatureSink

    interface AvatarAddMessagesContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink
        
    interface WorldAddMessagesContext with
        member _.avatarMessageSink: AvatarMessageSink = avatarMessageSink

[<Test>]
let ``Undock.It removes the feature for the avatar and adds a message.`` () =
    let avatarIslandFeatureSink (feature : AvatarIslandFeature option,_) =
        Assert.AreEqual(None, feature)
    let context : WorldUndockContext = 
        TestWorldUndockContext
            (avatarIslandFeatureSink,
            Fixtures.Common.Mock.AvatarMessageSink "You undock.") 
        :> WorldUndockContext
    World.Undock
        context
        Fixtures.Common.Dummy.AvatarId
    