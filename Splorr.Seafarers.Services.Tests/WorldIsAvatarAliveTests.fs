module WorldIsAvatarAliveTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures

[<Test>]
let ``IsAvatarAlive.It returns a true when given a world with an avatar with above minimum health.`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    if avatarId |> World.IsAvatarAlive shipmateSingleStatisticSource then
        ()
    else
        Assert.Fail("It detected that the avatar is not alive")

[<Test>]
let ``IsAvatarAlive.It returns a false when given a world with an avatar minimum health (zero).`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    if avatarId |> World.IsAvatarAlive shipmateSingleStatisticSource |> not then
        ()
    else
        Assert.Fail("It detected that the avatar is not dead")


