module AvatarMoneyReputationTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

let avatarId = ""

type TestAvatarGetPrimaryStatisticContext(shipmateSingleStatisticSource) = 
    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarSetMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarSetPrimaryStatisticContext
    interface AvatarSetMoneyContext 
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarSetReputationContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarSetPrimaryStatisticContext
    interface AvatarSetReputationContext
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``GetReputation.It retrieves the reputation of the primary shipmate.`` () =
    let inputReputation = 100.0
    let input =
        avatarId
    let expected = inputReputation
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (inputReputation, inputReputation) inputReputation
            |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom Get")
            None
    let context = TestAvatarGetPrimaryStatisticContext(shipmateSingleStatisticSource) :> AvatarGetPrimaryStatisticContext
    let actual =
        input
        |> Avatar.GetReputation context
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetMoney.It retrieves the money of the primary shipmate.`` () =
    let inputMoney = 100.0
    let input =
        avatarId
    let expected = inputMoney
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (inputMoney, inputMoney) inputMoney
            |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom Get")
            None
    let context = TestAvatarGetPrimaryStatisticContext(shipmateSingleStatisticSource) :> AvatarGetPrimaryStatisticContext
    let actual =
        input
        |> Avatar.GetMoney context
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetMoney.It assigns the amount of money of the primary shipmate.`` () =
    let inputMoney = 100.0
    let input =
        avatarId
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) = 
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier,statistic:Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(inputMoney, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Kaboom set")
    let context = TestAvatarSetMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSetMoneyContext
    input
    |> Avatar.SetMoney 
        context
        inputMoney

[<Test>]
let ``SetReputation.It assigns the amount of reputation of the primary shipmate.`` () =
    let inputReputation = 100.0
    let input =
        avatarId
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) = 
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (-1000.0, 1000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(inputReputation, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Kaboom set")
    let context = TestAvatarSetReputationContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSetReputationContext
    input
    |> Avatar.SetReputation 
        context
        inputReputation

