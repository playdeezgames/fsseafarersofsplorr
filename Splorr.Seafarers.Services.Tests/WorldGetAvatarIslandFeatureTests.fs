module WorldGetAvatarIslandFeatureTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarIslandFeature.It gets the avatar island feature.`` () =
    let calledGetFeature = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource :=
        Spies.Source(calledGetFeature, None)
    let actual =
        World.GetAvatarIslandFeature
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(None, actual)
    Assert.IsTrue(calledGetFeature.Value)
