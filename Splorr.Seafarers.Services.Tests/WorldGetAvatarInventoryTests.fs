module WorldGetAvatarInventoryTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetAvatarInventory.It gets the avatar's inventory.`` () =
    let calledGetAvatarInventory = ref false
    let context = Contexts.TestContext()
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetAvatarInventory, Map.empty)
    let actual = 
        World.GetAvatarInventory
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(Map.empty, actual)
    Assert.IsTrue(calledGetAvatarInventory.Value)

