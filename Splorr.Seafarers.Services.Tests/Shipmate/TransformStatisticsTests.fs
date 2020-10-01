module TransformStatisticsTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestShipmateTransformStatisticContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface ShipmateStatistic.PutContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
    interface ShipmateStatistic.GetContext with
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
    let context = TestShipmateTransformStatisticContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> ServiceContext
    ShipmateStatistic.Transform 
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
    let context = TestShipmateTransformStatisticContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> ServiceContext
    ShipmateStatistic.Transform 
        context
        ShipmateStatisticIdentifier.Health 
        (fun _ -> (inputHealth |> Some))
        Fixtures.Common.Dummy.AvatarId
        Primary

type TestShipmateGetStatisticContext
        (shipmateSingleStatisticSource) =
    interface ServiceContext
    interface ShipmateStatistic.GetContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
[<Test>]
let ``GetStatistic.It calls ShipmateSingleStatisticSource in the context.`` () =
    let mutable called = false
    let shipmateSingleStatisticSource (_) (_) (_) =
        called <- true
        None
    let context = TestShipmateGetStatisticContext(shipmateSingleStatisticSource) :> ServiceContext
    let expected = None
    let actual = 
        ShipmateStatistic.Get
            context
            Fixtures.Common.Dummy.AvatarId
            ShipmateIdentifier.Primary
            ShipmateStatisticIdentifier.Health
    Assert.AreEqual(expected, actual)
    Assert.IsTrue(called)

        
