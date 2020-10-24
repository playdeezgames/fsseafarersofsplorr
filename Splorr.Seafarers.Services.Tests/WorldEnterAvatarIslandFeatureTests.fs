module WorldEnterAvatarIslandFeatureTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``EnterAvatarIslandFeature.It does nothing when the given island does not have the given feature.`` () =
    let calledHasFeature = ref false
    let context = Contexts.TestContext()
    (context :> Island.HasFeatureContext).islandSingleFeatureSource := Spies.Source(calledHasFeature, false)
    World.EnterAvatarIslandFeature
        context
        Dummies.ValidAvatarId
        Dummies.ValidIslandLocation
        IslandFeatureIdentifier.Dock
    Assert.IsTrue(calledHasFeature.Value)


[<Test>]
let ``EnterAvatarIslandFeature.It enters the feature when the given island has the given feature.`` () =
    let calledHasFeature = ref false
    let calledSetFeature = ref false
    let context = Contexts.TestContext()
    (context :> Island.HasFeatureContext).islandSingleFeatureSource := Spies.Source(calledHasFeature, true)
    (context :> AvatarIslandFeature.SetFeatureContext).avatarIslandFeatureSink := 
        Spies.Expect(calledSetFeature, (Some {featureId=IslandFeatureIdentifier.Dock; location=Dummies.ValidIslandLocation}, Dummies.ValidAvatarId))
    World.EnterAvatarIslandFeature
        context
        Dummies.ValidAvatarId
        Dummies.ValidIslandLocation
        IslandFeatureIdentifier.Dock
    Assert.IsTrue(calledHasFeature.Value)
    Assert.IsTrue(calledSetFeature.Value)

