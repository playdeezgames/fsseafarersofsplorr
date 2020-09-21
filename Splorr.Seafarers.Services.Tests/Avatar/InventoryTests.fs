module AvatarInventoryTests

open Splorr.Seafarers.Services
open NUnit.Framework
open Splorr.Seafarers.Models

type TestAvatarAddInventoryContext(avatarInventorySink, avatarInventorySource) =
    interface Avatar.GetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface Avatar.AddInventoryContext with
        member _.avatarInventorySink   : AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource


type TestAvatarRemoveInventoryContext(avatarInventorySink, avatarInventorySource) =
    interface Avatar.RemoveInventoryContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource

type TestAvatarGetItemCountContext(avatarInventorySource) =
    interface Avatar.GetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource

type TestAvatarGetUsedTonnageContext(avatarInventorySource) =
    interface Avatar.GetUsedTonnageContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource




[<Test>]
let ``AddInventory.It adds a given number of given items to the given avatar's inventory.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputItem = 1UL
    let inputQuantity = 2UL
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(1, inventory.Count)
        Assert.AreEqual(inputQuantity, inventory.[inputItem])
    let context = TestAvatarAddInventoryContext(avatarInventorySink, avatarInventorySource) :> Avatar.AddInventoryContext
    input
    |> Avatar.AddInventory 
        context
        inputItem 
        inputQuantity

[<Test>]
let ``RemoveInventory.It does nothing.When given a quantity of 0 items to remove or the given avatar has no items.`` 
        ([<Values(0UL, 1UL)>]inputQuantity:uint64) =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputItem = 1UL
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let context = TestAvatarRemoveInventoryContext(avatarInventorySink, avatarInventorySource) :> Avatar.RemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
        inputItem 
        inputQuantity

[<Test>]
let ``RemoveInventory.It reduces the given avatars inventory to 0 when the given number of items exceed the avatar's inventory.``() =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputItem = 1UL
    let inputQuantity = 2UL
    let originalQuantity = inputQuantity - 1UL
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add inputItem originalQuantity
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(0, inventory.Count)
    let context = TestAvatarRemoveInventoryContext(avatarInventorySink, avatarInventorySource) :> Avatar.RemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
        inputItem 
        inputQuantity

[<Test>]
let ``RemoveInventory.It reduces the given avatar's inventory by the given amount.``() =
    let input = Fixtures.Common.Dummy.AvatarId 
    let inputItem = 1UL
    let inputQuantity = 20UL
    let originalQuantity = inputQuantity + inputQuantity
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add inputItem originalQuantity
    let expectedQuantity = originalQuantity - inputQuantity
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(1, inventory.Count)
        Assert.AreEqual(expectedQuantity, inventory.[inputItem])
    let context = TestAvatarRemoveInventoryContext(avatarInventorySink, avatarInventorySource) :> Avatar.RemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
        inputItem 
        inputQuantity

[<Test>]
let ``GetItemCount.It returns zero when the given avatar has no entry for the given item.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputItem = 1UL
    let expected = 0u
    let avatarInventorySource (_) =
        Map.empty
    let context = TestAvatarGetItemCountContext(avatarInventorySource) :> Avatar.GetItemCountContext
    let actual =
        input
        |> Avatar.GetItemCount 
            context
            inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetItemCount.It returns the item count when the given avatar has an entry for the given item.`` () =
    let input = Fixtures.Common.Dummy.AvatarId
    let inputItem = 1UL
    let expected = 2UL
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add inputItem expected
    let context = TestAvatarGetItemCountContext(avatarInventorySource) :> Avatar.GetItemCountContext
    let actual =
        input
        |> Avatar.GetItemCount 
            context
            inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetUsedTonnage.It calculates the used tonnage based on inventory and item descriptors.`` () =
    let input = 
        Fixtures.Common.Dummy.AvatarId
    let inputItems =
        Map.empty
        |> Map.add 1UL {
                        ItemName =""
                        Commodities =Map.empty
                        Occurrence  =0.0
                        Tonnage     =1.0
                        }
        |> Map.add 2UL {
                        ItemName =""
                        Commodities =Map.empty
                        Occurrence  =0.0
                        Tonnage     =3.0
                        }
    let expected = 15.0
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add 1UL 3UL
        |> Map.add 2UL 4UL
    let context = TestAvatarGetUsedTonnageContext(avatarInventorySource) :> Avatar.GetUsedTonnageContext
    let actual =
        input
        |> Avatar.GetUsedTonnage 
            context
            inputItems
    Assert.AreEqual(expected, actual)
