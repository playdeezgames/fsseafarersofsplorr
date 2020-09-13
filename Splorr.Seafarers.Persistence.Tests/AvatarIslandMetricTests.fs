module AvatarIslandMetricTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

[<Test>]
let ``GetMetricForAvatarIsland.It retrieves a metric for a given avatar and a given island.`` () =
    use connection = SetupConnection()
    try
        let avatarId = ExistingAvatarId
        let islandPosition = VisitedIslandLocation
        let metric = AvatarIslandMetricIdentifier.VisitCount
        match AvatarIslandMetric.GetMetricForAvatarIsland connection avatarId islandPosition metric with
        | Ok actual ->
            Assert.AreEqual(1UL |> Some, actual)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``GetMetricForAvatarIsland.It retrieves none for a given metric for a given avatar and a given island that does not exist.`` () =
    use connection = SetupConnection()
    try
        let avatarId = NewAvatarId
        let islandPosition = VisitedIslandLocation
        let metric = AvatarIslandMetricIdentifier.VisitCount
        match AvatarIslandMetric.GetMetricForAvatarIsland connection avatarId islandPosition metric with
        | Ok actual ->
            Assert.AreEqual(None, actual)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetMetricForAvatarIsland.It sets a value for a given metric of a given avatar and given island.`` () =
    use connection = SetupConnection()
    try
        let avatarId = NewAvatarId
        let islandPosition = VisitedIslandLocation
        let metric = AvatarIslandMetricIdentifier.VisitCount
        let value = 2UL |> Some
        match AvatarIslandMetric.GetMetricForAvatarIsland connection avatarId islandPosition metric with
        | Ok actual ->
            Assert.AreEqual(None, actual)
        | Error message -> Assert.Fail message
        match AvatarIslandMetric.SetMetricForAvatarIsland connection avatarId islandPosition metric value with
        | Error _ ->
            Assert.Fail("AvatarIslandMetric.SetMetricForAvatarIsland failed")
        | _ -> ()
        match AvatarIslandMetric.GetMetricForAvatarIsland connection avatarId islandPosition metric with
        | Ok actual ->
            Assert.AreEqual(value, actual)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetMetricForAvatarIsland.It remove a value when passed None for a given metric of a given avatar and given island.`` () =
    use connection = SetupConnection()
    try
        let avatarId = ExistingAvatarId
        let islandPosition = VisitedIslandLocation
        let metric = AvatarIslandMetricIdentifier.VisitCount
        let value = None
        match AvatarIslandMetric.GetMetricForAvatarIsland connection avatarId islandPosition metric with
        | Ok actual ->
            Assert.AreEqual(1UL |> Some, actual)
        | Error message -> Assert.Fail message
        match AvatarIslandMetric.SetMetricForAvatarIsland connection avatarId islandPosition metric value with
        | Error _ ->
            Assert.Fail("AvatarIslandMetric.SetMetricForAvatarIsland failed")
        | _ -> ()
        match AvatarIslandMetric.GetMetricForAvatarIsland connection avatarId islandPosition metric with
        | Ok actual ->
            Assert.AreEqual(value, actual)
        | Error message -> Assert.Fail message
    finally
        connection.Close()
