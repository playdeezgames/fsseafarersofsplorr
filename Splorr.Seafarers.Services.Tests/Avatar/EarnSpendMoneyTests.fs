module AvatarEarnSpendMoneyTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestAvatarEarnMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface Avatar.GetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource


[<Test>]
let ``EarnMoney.It has no effect when given a negative amount to earn.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputAmount = -1.0
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let context = TestAvatarEarnMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> OperatingContext
    input
    |> Avatar.EarnMoney 
        context
        inputAmount

[<Test>]
let ``EarnMoney.It updates the avatars money by adding the given amount.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputAmount = 1.0
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(inputAmount, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let context = TestAvatarEarnMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> OperatingContext
    input
    |> Avatar.EarnMoney 
        context
        inputAmount

[<Test>]
let ``SpendMoney.It has no effect when given a negative amount to spend.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputAmount = -1.0
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let context = TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> OperatingContext
    input
    |> Avatar.SpendMoney 
        context
        inputAmount

[<Test>]
let ``SpendMoney.It has no effect when the given avatar has no money.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputAmount = 1.0
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(0.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let context = TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> OperatingContext
    input
    |> Avatar.SpendMoney 
        context
        inputAmount

[<Test>]
let ``SpendMoney.It reduces the avatar's money to zero when the given amount exceeds the given avatar's money.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputAmount = 101.0
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic:Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(0.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let context = TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> OperatingContext
    input
    |> Avatar.SpendMoney 
        context
        inputAmount

[<Test>]
let ``SpendMoney.It updates the avatars money when the given amount is less than the given avatar's money.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputAmount = 1.0
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic:Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let context = TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> OperatingContext
    input
    |> Avatar.SpendMoney 
        context
        inputAmount

