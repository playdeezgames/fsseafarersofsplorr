module WorldIsAvatarAliveTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``IsAvatarAlive.It returns false when the primary shipmate has zero health.`` () =
    let callsForGetShipmateStatistic = ref 0UL
    let context = Contexts.TestContext()
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource :=
        Spies.SourceTable(callsForGetShipmateStatistic, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Health) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=0.0})
            |> Map.add (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Turn) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0}))
    let actual = 
        World.IsAvatarAlive
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(false, actual)
    Assert.AreEqual(2UL, callsForGetShipmateStatistic.Value)

[<Test>]
let ``IsAvatarAlive.It returns false when the primary shipmate has aged out.`` () =
    let callsForGetShipmateStatistic = ref 0UL
    let context = Contexts.TestContext()
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource :=
        Spies.SourceTable(callsForGetShipmateStatistic, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Health) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
            |> Map.add (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Turn) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=100.0}))
    let actual = 
        World.IsAvatarAlive
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(false, actual)
    Assert.AreEqual(2UL, callsForGetShipmateStatistic.Value)


[<Test>]
let ``IsAvatarAlive.It returns true when the primary shipmate has health and has not aged out.`` () =
    let callsForGetShipmateStatistic = ref 0UL
    let context = Contexts.TestContext()
    (context :> ShipmateStatistic.GetContext).shipmateSingleStatisticSource :=
        Spies.SourceTable(callsForGetShipmateStatistic, 
            Map.empty
            |> Map.add (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Health) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0})
            |> Map.add (Dummies.ValidAvatarId, Primary, ShipmateStatisticIdentifier.Turn) (Some {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0}))
    let actual = 
        World.IsAvatarAlive
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(true, actual)
    Assert.AreEqual(2UL, callsForGetShipmateStatistic.Value)

