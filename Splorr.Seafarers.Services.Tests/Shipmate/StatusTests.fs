module ShipmateStatusTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestShipmateGetStatusContext(shipmateSingleStatisticSource) =
    interface ShipmateStatistic.GetContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

[<Test>]//TODO - bad name
let ``ALIVE/ZERO_HEALTH/OLD_AGE.It returns a ALIVE when given an avatar with above minimum health and not end of life.`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0} |> Some
        | ShipmateStatisticIdentifier.Health ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=100.0} |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let inputAvatarId = Fixtures.Common.Dummy.AvatarId
    let inputShipmateId = Primary
    let context = TestShipmateGetStatusContext(shipmateSingleStatisticSource) :> ServiceContext
    match Shipmate.GetStatus 
        context
        inputAvatarId 
        inputShipmateId with
    | Alive -> ()
    | _ -> Assert.Fail("It detected that the avatar is not alive")

[<Test>]//TODO - bad name
let ``ALIVE/ZERO_HEALTH/OLD_AGE.It returns a ZERO_HEALTH when given an avatar at minimum health (zero).`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0} |> Some
        | ShipmateStatisticIdentifier.Health ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0} |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let inputAvatarId = Fixtures.Common.Dummy.AvatarId
    let inputShipmateId = Primary
    let context = TestShipmateGetStatusContext(shipmateSingleStatisticSource) :> ServiceContext
    match Shipmate.GetStatus 
        context
        inputAvatarId 
        inputShipmateId with
    | Dead ZeroHealth -> ()
    | _ -> Assert.Fail("It detected that the avatar is not dead")

[<Test>]//TODO - bad name
let ``ALIVE/ZERO_HEALTH/OLD_AGE.It returns a OLD_AGE when given an avatar at maximum turn.`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=100.0} |> Some
        | ShipmateStatisticIdentifier.Health ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=100.0} |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let inputAvatarId = Fixtures.Common.Dummy.AvatarId
    let inputShipmateId = Primary
    let context = TestShipmateGetStatusContext(shipmateSingleStatisticSource) :> ServiceContext
    match Shipmate.GetStatus 
        context
        inputAvatarId 
        inputShipmateId with
    | Dead OldAge -> ()
    | _ -> Assert.Fail("It detected that the avatar is not dead")


