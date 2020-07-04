module AvatarTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

open AvatarTestFixtures

[<Test>]
let ``Create.It creates an avatar.`` () =
    let actual =
        avatar
    Assert.AreEqual((0.0,0.0), actual.Position)
    Assert.AreEqual(1.0, actual.Speed)
    Assert.AreEqual(0.0, actual.Heading)


[<Test>]
let ``SetSpeed.It sets all stop when given less than zero.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (-1.0)
    Assert.AreEqual(0.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets full speed when gives more than one.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (2.0)
    Assert.AreEqual(1.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets half speed when given half speed.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (0.5)
    Assert.AreEqual(0.5, actual.Speed)

[<Test>]
let ``SetSpeed.It sets full speed when given one.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (1.0)
    Assert.AreEqual(1.0, actual.Speed)

[<Test>]
let ``SetSpeed.It sets all stop when given zero.`` () =
    let actual =
        avatar
        |> Avatar.SetSpeed (0.0)
    Assert.AreEqual(0.0, actual.Speed)

[<Test>]
let ``SetHeading.It sets a given heading.`` () =
    let heading = 
        {
            Degrees = 1
            Minutes = 2
            Seconds = 3.0
        }
    let actual =
        avatar
        |> Avatar.SetHeading heading
    Assert.AreEqual(heading |> Dms.ToFloat, actual.Heading)

[<Test>]
let ``Move.It moves the avatar.`` () =
    let actual =
        avatar
        |> Avatar.Move
    Assert.AreEqual((1.0,0.0), actual.Position)

[<Test>]
let ``SetJob.It sets the job of the given avatar.`` () =
    let actual =
        avatar
        |> Avatar.SetJob job
    Assert.AreEqual(job |> Some, actual.Job)

[<Test>]
let ``AbandonJob.It does nothing when the given avatar has no job.`` () =
    let input = avatar
    let expected = input
    let actual =
        input
        |> Avatar.AbandonJob
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AbandonJob.It set job to None when the given avatar has a job.`` () =
    let input = employedAvatar
    let expected = {input with Job=None; Reputation = input.Reputation - 1.0}
    let actual =
        input
        |> Avatar.AbandonJob
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``CompleteJob.It does nothing when the given avatar has no job.`` () =
    let input = avatar
    let expected = input
    let actual = 
        input
        |> Avatar.CompleteJob
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CompleteJob.It sets job to None, adds reward money, adds reputation when the given avatar has a job.`` () =
    let input = employedAvatar
    let inputJob = employedAvatar.Job.Value
    let expected = {input with Job = None; Money = input.Money + inputJob.Reward; Reputation = input.Reputation + 1.0}
    let actual = 
        input
        |> Avatar.CompleteJob
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SpendMoney.It has no effect when given a negative amount to spend.`` () =
    let input = employedAvatar
    let inputAmount = -1.0
    let expected = input
    let actual =
        input
        |> Avatar.SpendMoney inputAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``EarnMoney.It has no effect when given a negative amount to earn.`` () =
    let input = employedAvatar
    let inputAmount = -1.0
    let expected = input
    let actual =
        input
        |> Avatar.EarnMoney inputAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SpendMoney.It has no effect when the given avatar has no money.`` () =
    let input = avatar
    let inputAmount = 1.0
    let expected = input
    let actual =
        input
        |> Avatar.SpendMoney inputAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SpendMoney.It reduces the avatar's money to zero when the given amount exceeds the given avatar's money.`` () =
    let input = employedAvatar
    let inputAmount = input.Money + 1.0
    let expected = 
        {input with Money = 0.0}
    let actual =
        input
        |> Avatar.SpendMoney inputAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SpendMoney.It updates the avatars money when the given amount is less than the given avatar's money.`` () =
    let input = employedAvatar
    let inputAmount = 1.0
    let expected = 
        {input with Money = input.Money - inputAmount}
    let actual =
        input
        |> Avatar.SpendMoney inputAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``EarnMoney.It updates the avatars money by adding the given amount.`` () =
    let input = employedAvatar
    let inputAmount = 1.0
    let expected = 
        {input with Money = input.Money + inputAmount}
    let actual =
        input
        |> Avatar.EarnMoney inputAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AddInventory.It adds a given number of given items to the given avatar's inventory.`` () =
    let input = employedAvatar
    let inputItem = Ration
    let inputQuantity = 2u
    let expected =
        {input with Inventory = input.Inventory |> Map.add inputItem inputQuantity}
    let actual =
        input
        |> Avatar.AddInventory inputItem inputQuantity
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetItemCount.It returns zero when the given avatar has no entry for the given item.`` () =
    let input = avatar
    let inputItem = Ration
    let expected = 0u
    let actual =
        input
        |> Avatar.GetItemCount inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetItemCount.It returns the item count when the given avatar has an entry for the given item.`` () =
    let input = rationedAvatar
    let inputItem = Ration
    let expected = rationedAvatar.Inventory.[inputItem]
    let actual =
        input
        |> Avatar.GetItemCount inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``RemoveInventory.It does nothing.When given a quantity of 0 items to remove or the given avatar has no items.`` ([<Values(0u, 1u)>]inputQuantity:uint32) =
    let input = avatar
    let inputItem = Ration
    let expected =
        input
    let actual = 
        input
        |> Avatar.RemoveInventory inputItem inputQuantity
    Assert.AreEqual(expected, actual)

[<Test>]
let ``RemoveInventory.It reduces the given avatars inventory to 0 when the given number of items exceed the avatar's inventory.``() =
    let input = rationedAvatar 
    let inputItem = Ration
    let inputQuantity = 2u
    let expected =
        {input with Inventory = Map.empty}
    let actual = 
        input
        |> Avatar.RemoveInventory inputItem inputQuantity
    Assert.AreEqual(expected, actual)

[<Test>]
let ``RemoveInventory.It reduces the given avatar's inventory by the given amount.``() =
    let input = hoarderAvatar 
    let inputItem = Ration
    let inputQuantity = 20u
    let expected =
        {input with Inventory = input.Inventory |> Map.add Ration 80u}
    let actual = 
        input
        |> Avatar.RemoveInventory inputItem inputQuantity
    Assert.AreEqual(expected, actual)
