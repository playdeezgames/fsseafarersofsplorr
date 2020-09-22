module TransformStatisticsTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestShipmateTransformStatisticContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface Shipmate.TransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]
let ``TransformStatistic.It replaces the statistic when that statistic is originally present in the avatar.`` () =
    let inputHealth = Statistic.Create (5.0,10.0) 5.0
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create(0.0, 100.0) 100.0 |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic:Statistic option) =
        match identifier with 
        | ShipmateStatisticIdentifier.Health ->
            Assert.AreEqual(inputHealth, statistic.Value)
        | _ ->
            raise (System.NotImplementedException "Kaboom shipmateSingleStatisticSink")
    let context = TestShipmateTransformStatisticContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> Shipmate.TransformStatisticContext
    Shipmate.TransformStatistic 
        context
        ShipmateStatisticIdentifier.Health 
        (fun _ -> (inputHealth |> Some))
        Fixtures.Common.Dummy.AvatarId
        Primary

[<Test>]
let ``TransformStatistic.It does nothing when the given statistic is absent from the avatar.`` () =
    let inputHealth = Statistic.Create (5.0,10.0) 5.0
    let shipmateSingleStatisticSource (_) (_) (_) =
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        Assert.Fail("Dont call me.")
    let context = TestShipmateTransformStatisticContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> Shipmate.TransformStatisticContext
    Shipmate.TransformStatistic 
        context
        ShipmateStatisticIdentifier.Health 
        (fun _ -> (inputHealth |> Some))
        Fixtures.Common.Dummy.AvatarId
        Primary


