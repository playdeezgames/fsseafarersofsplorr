module WorldGetIslandDisplayNameTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common
open System

[<Test>]
let ``GetIslandDisplayName.It raises an exception when the given island doesn't exist.`` () =
    let calledGetIslandName = ref false
    let context = Contexts.TestContext()
    (context :> IslandName.GetNameContext).islandSingleNameSource := Spies.Source(calledGetIslandName, None)
    Assert.Throws<NotImplementedException>(
        fun () ->
            World.GetIslandDisplayName
                context
                Dummies.ValidAvatarId
                Dummies.InvalidIslandLocation
            |> ignore)
    |> ignore
    Assert.IsTrue(calledGetIslandName.Value)

[<Test>]
let ``GetIslandDisplayName.It returns "(unknown)" then the island exists but the avatar doesn't know about it.`` () =
    let calledGetIslandName = ref false
    let calledGetAvatarIslandMetric = ref false
    let context = Contexts.TestContext()
    (context :> IslandName.GetNameContext).islandSingleNameSource := Spies.Source(calledGetIslandName, Some "Island Name")
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource := Spies.Source(calledGetAvatarIslandMetric, None)
    let actual = 
        World.GetIslandDisplayName
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
    Assert.AreEqual("(unknown)", actual)
    Assert.IsTrue(calledGetIslandName.Value)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)

[<Test>]
let ``GetIslandDisplayName.It returns the island name when the island exists and the avatar knows about it.`` () =
    let calledGetIslandName = ref false
    let calledGetAvatarIslandMetric = ref false
    let context = Contexts.TestContext()
    (context :> IslandName.GetNameContext).islandSingleNameSource := Spies.Source(calledGetIslandName, Some "Island Name")
    (context :> AvatarIslandMetric.GetContext).avatarIslandSingleMetricSource := Spies.Source(calledGetAvatarIslandMetric, Some 0UL)
    let actual = 
        World.GetIslandDisplayName
            context
            Dummies.ValidAvatarId
            Dummies.ValidIslandLocation
    Assert.AreEqual("Island Name", actual)
    Assert.IsTrue(calledGetIslandName.Value)
    Assert.IsTrue(calledGetAvatarIslandMetric.Value)


