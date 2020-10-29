module WorldGetVesselUsedTonnageTests


open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Tests.Common

[<Test>]
let ``GetVesselUsedTonnage.It calculates the vessel's used tonnage for the given avatar.`` () =
    let calledGetItemList = ref false
    let calledGetAvatarInventory = ref false
    let context = Contexts.TestContext()
    (context :> Item.GetListContext).itemSource :=
        Spies.Source(calledGetItemList, Dummies.ValidItemTable)
    (context :> AvatarInventory.GetInventoryContext).avatarInventorySource := Spies.Source(calledGetAvatarInventory, Map.empty |> Map.add Dummies.ValidItemId 1UL)
        
    let actual = 
        World.GetVesselUsedTonnage
            context
            Dummies.ValidAvatarId
    Assert.AreEqual(0.10000000000000001, actual)
    Assert.IsTrue(calledGetItemList.Value)
    Assert.IsTrue(calledGetAvatarInventory.Value)
