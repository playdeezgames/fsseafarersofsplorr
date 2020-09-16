module GambleTests

open NUnit.Framework
open System
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type private WorldCanPlaceBetContext(shipmateSingleStatisticSource) =
    interface AvatarGetPrimaryStatisticContext with
        member _.shipmateSingleStatisticSource : ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface World.CanPlaceBetContext

module private Mock =
    let ShipmateSingleStatisticSource (mockValue:Statistic option) (_) (_) (_) =
        mockValue

[<Test>]
let ``CanPlaceBet.It returns false when the avatar does not have enough money.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenAmount = 1.0
    let givenStatistic = Statistic.Create (0.0, 1000000.0) 0.0 |> Some
    let expected = false
    let context = 
        WorldCanPlaceBetContext
            (Mock.ShipmateSingleStatisticSource givenStatistic) :> World.CanPlaceBetContext 
    let actual =
        World.CanPlaceBet context givenAvatarId givenAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CanPlaceBet.It returns true when the avatar has enough money.`` () =
    let givenAvatarId = Fixtures.Common.Dummy.AvatarId
    let givenAmount = 1.0
    let givenStatistic = Statistic.Create (0.0, 1000000.0) 1.0 |> Some
    let expected = true
    let context = 
        WorldCanPlaceBetContext
            (Mock.ShipmateSingleStatisticSource givenStatistic) :> World.CanPlaceBetContext 
    let actual =
        World.CanPlaceBet context givenAvatarId givenAmount
    Assert.AreEqual(expected, actual)