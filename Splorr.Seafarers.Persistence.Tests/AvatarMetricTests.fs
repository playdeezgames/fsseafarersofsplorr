module AvatarMetricTests

open NUnit.Framework
open CommonTestFixtures
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

[<Test>]
let ``GetForAvatar.It returns a list of metrics for an avatar that exists.`` () =
    use connection = SetupConnection()
    try
        let avatarId = ExistingAvatarId
        match AvatarMetric.GetForAvatar connection avatarId with
        | Ok actual ->
            let expectedCount = System.Enum.GetValues(typedefof<Metric>).Length
            Assert.AreEqual(expectedCount, actual.Count)
        | Error message -> Assert.Fail message
    finally
        connection.Close()


[<Test>]
let ``GetForAvatar.It returns a empty list for an avatar that does not exist.`` () =
    use connection = SetupConnection()
    try
        let avatarId = NewAvatarId
        match AvatarMetric.GetForAvatar connection avatarId with
        | Ok actual ->
            let expectedCount = 0
            Assert.AreEqual(expectedCount, actual.Count)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``GetMetricForAvatar.It returns 0 when the metric does not exist for the given avatar.`` () =
    use connection = SetupConnection()
    try
        let avatarId = NewAvatarId
        match AvatarMetric.GetMetricForAvatar connection avatarId Metric.Moved with
        | Ok actual ->
            let expectedValue = 0UL
            Assert.AreEqual(expectedValue, actual)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``GetMetricForAvatar.It returns the metric when the metric exists for the given avatar.`` () =
    use connection = SetupConnection()
    try
        let avatarId = ExistingAvatarId
        match AvatarMetric.GetMetricForAvatar connection avatarId Metric.Moved with
        | Ok actual ->
            let expectedValue = 1UL
            Assert.AreEqual(expectedValue, actual)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetMetricForAvatar.It sets the metric when the metric does not exist for the given avatar.`` () =
    use connection = SetupConnection()
    try
        let avatarId = NewAvatarId
        let inputMetric = Metric.Moved
        let inputValue = 1UL
        let expectedInitial : Result<uint64,string> = 0UL |> Ok
        Assert.AreEqual(expectedInitial, AvatarMetric.GetMetricForAvatar connection avatarId inputMetric)
        match AvatarMetric.SetMetricForAvatar connection avatarId (inputMetric, inputValue) with
        | Ok () ->
            let expectedValue : Result<uint64,string> = inputValue |> Ok
            let actualValue = AvatarMetric.GetMetricForAvatar connection avatarId inputMetric
            Assert.AreEqual(expectedValue, actualValue)
        | Error message -> Assert.Fail message
    finally
        connection.Close()

[<Test>]
let ``SetMetricForAvatar.It removes the metric when the given metric is 0 for the given avatar.`` () =
    use connection = SetupConnection()
    try
        let avatarId = ExistingAvatarId
        let inputMetric = Metric.Moved
        let inputValue = 0UL
        match AvatarMetric.GetForAvatar connection avatarId with
        | Ok x ->
            Assert.AreEqual(7, x.Count)
        | _ ->
            Assert.Fail()
        match AvatarMetric.SetMetricForAvatar connection avatarId (inputMetric, inputValue) with
        | Ok () ->
            match AvatarMetric.GetForAvatar connection avatarId with
            | Ok x ->
                Assert.AreEqual(6, x.Count)
            | _ ->
                Assert.Fail()
        | Error message -> Assert.Fail message
    finally
        connection.Close()
    