module WorldUndockTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``Undock.It undocks the avatar when docked.`` () =
    let calledAddAvatarMessage = ref false
    let calledSetAvatarFeature = ref false
    let calledGetAvatarIslandFeature = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := 
        Spies.Source(calledGetAvatarIslandFeature, 
            Some {featureId = IslandFeatureIdentifier.Dock; location = Dummies.InvalidIslandLocation})
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessage)
    (context :> AvatarIslandFeature.SetFeatureContext).avatarIslandFeatureSink := Spies.Expect(calledSetAvatarFeature, (None, Dummies.ValidAvatarId))
    World.Undock
        context
        Dummies.ValidAvatarId
    Assert.IsTrue(calledAddAvatarMessage.Value)
    Assert.IsTrue(calledSetAvatarFeature.Value)
    Assert.IsTrue(calledGetAvatarIslandFeature.Value)

[<Test>]
let ``Undock.It gives a message the avatar when the avatar is on an island but not at the dock.`` () =
    let calledAddAvatarMessage = ref false
    let calledGetAvatarIslandFeature = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := 
        Spies.Source(calledGetAvatarIslandFeature, 
            Some {featureId = IslandFeatureIdentifier.DarkAlley; location = Dummies.InvalidIslandLocation})
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessage)
    World.Undock
        context
        Dummies.ValidAvatarId
    Assert.IsTrue(calledAddAvatarMessage.Value)
    Assert.IsTrue(calledGetAvatarIslandFeature.Value)

[<Test>]
let ``Undock.It gives a message when the avatar when at sea.`` () =
    let calledAddAvatarMessage = ref false
    let calledGetAvatarIslandFeature = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := 
        Spies.Source(calledGetAvatarIslandFeature, None)
    (context :> AvatarMessages.AddContext).avatarMessageSink := Spies.Sink(calledAddAvatarMessage)
    World.Undock
        context
        Dummies.ValidAvatarId
    Assert.IsTrue(calledAddAvatarMessage.Value)
    Assert.IsTrue(calledGetAvatarIslandFeature.Value)

