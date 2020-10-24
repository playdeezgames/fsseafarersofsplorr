module WorldHasDarkAlleyMinimumStakesTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``HasDarkAlleyMinimumStakes.It returns false when the avatar is not in a dark alley.`` () = 
    let calledGetFeatureContext = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := Spies.Source(calledGetFeatureContext, None)
    let actual = 
        World.HasDarkAlleyMinimumStakes
            context
            Dummies.ValidAvatarId
    Assert.IsFalse(actual)
    Assert.IsTrue(calledGetFeatureContext.Value)

[<Test>]
let ``HasDarkAlleyMinimumStakes.It returns false when the avatar is in a dark alley but has insufficient funds.`` () = 
    let calledGetFeatureContext = ref false
    let calledGetIslandStatistic = ref false
    let calledGetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := 
        Spies.Source(calledGetFeatureContext, Some {featureId = IslandFeatureIdentifier.DarkAlley; location=Dummies.ValidIslandLocation})
    (context :> Island.GetStatisticContext).islandSingleStatisticSource :=
        Spies.Source(calledGetIslandStatistic, Some {MaximumValue=5.0; MinimumValue=5.0; CurrentValue=5.0})
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource :=
        Spies.Source(calledGetShipmateStatistic, Some {MaximumValue=0.0; MinimumValue=0.0; CurrentValue=0.0})
    let actual = 
        World.HasDarkAlleyMinimumStakes
            context
            Dummies.ValidAvatarId
    Assert.IsFalse(actual)
    Assert.IsTrue(calledGetFeatureContext.Value)
    Assert.IsTrue(calledGetIslandStatistic.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)


[<Test>]
let ``HasDarkAlleyMinimumStakes.It returns true when the avatar is in a dark alley and has sufficient funds.`` () = 
    let calledGetFeatureContext = ref false
    let calledGetIslandStatistic = ref false
    let calledGetShipmateStatistic = ref false
    let context = Contexts.TestContext()
    (context :> AvatarIslandFeature.GetFeatureContext).avatarIslandFeatureSource := 
        Spies.Source(calledGetFeatureContext, Some {featureId = IslandFeatureIdentifier.DarkAlley; location=Dummies.ValidIslandLocation})
    (context :> Island.GetStatisticContext).islandSingleStatisticSource :=
        Spies.Source(calledGetIslandStatistic, Some {MaximumValue=5.0; MinimumValue=5.0; CurrentValue=5.0})
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource :=
        Spies.Source(calledGetShipmateStatistic, Some {MaximumValue=5.0; MinimumValue=5.0; CurrentValue=5.0})
    let actual = 
        World.HasDarkAlleyMinimumStakes
            context
            Dummies.ValidAvatarId
    Assert.IsTrue(actual)
    Assert.IsTrue(calledGetFeatureContext.Value)
    Assert.IsTrue(calledGetIslandStatistic.Value)
    Assert.IsTrue(calledGetShipmateStatistic.Value)



