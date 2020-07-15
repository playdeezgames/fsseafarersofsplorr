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
    let input = avatar
    let expectedPosition = (1.0,0.0)
    let actual =
        input
        |> Avatar.Move
    Assert.AreEqual(expectedPosition, actual.Position)

[<Test>]
let ``Move.It removes a ration when the given avatar has rations and full satiety and full health.`` () =
    let input = 
        {avatar with 
            Inventory = Map.empty |> Map.add 1UL 2u}
    let expectedSatiety = input.Satiety.CurrentValue
    let expectedHealth = input.Health.CurrentValue
    let expectedInventory = Map.empty |> Map.add 1u 1u
    let actual =
        input
        |> Avatar.Move
    Assert.AreEqual(expectedInventory, actual.Inventory)
    Assert.AreEqual(expectedSatiety, actual.Satiety.CurrentValue)
    Assert.AreEqual(expectedHealth, actual.Health.CurrentValue)

[<Test>]
let ``Move.It removes a ration and increases satiety when the given avatar has rations and less than full satiety.`` () =
    let input = 
        {avatar with 
            Inventory = Map.empty |> Map.add 1UL 2u
            Satiety = {avatar.Satiety with CurrentValue=0.0}}
    let expectedSatiety = input.Satiety.CurrentValue + 1.0
    let expectedHealth = input.Health.CurrentValue
    let expectedInventory = Map.empty |> Map.add 1u 1u
    let actual =
        input
        |> Avatar.Move
    Assert.AreEqual(expectedInventory, actual.Inventory)
    Assert.AreEqual(expectedSatiety, actual.Satiety.CurrentValue)
    Assert.AreEqual(expectedHealth, actual.Health.CurrentValue)

[<Test>]
let ``Move.It lowers the avatar's satiety but not health when the given avatar has no rations.`` () =
    let input = {avatar with Inventory = Map.empty}
    let expectedSatiety = input.Satiety.CurrentValue - 1.0
    let expectedHealth = input.Health.CurrentValue
    let actual =
        input
        |> Avatar.Move
    Assert.AreEqual(expectedSatiety, actual.Satiety.CurrentValue)
    Assert.AreEqual(expectedHealth, actual.Health.CurrentValue)

[<Test>]
let ``Move.It lowers the avatar's health when the given avatar has no rations and minimum satiety.`` () =
    let input = {avatar with Inventory = Map.empty; Satiety = {avatar.Satiety with CurrentValue=0.0}}
    let expectedSatiety = 0.0
    let expectedHealth = input.Health.CurrentValue - 1.0
    let actual =
        input
        |> Avatar.Move
    Assert.AreEqual(expectedSatiety, actual.Satiety.CurrentValue)
    Assert.AreEqual(expectedHealth, actual.Health.CurrentValue)

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
    let expected = 
        {input with 
            Job=None
            Reputation = input.Reputation - 1.0
            Metrics = input.Metrics |> Map.add Metric.AbandonedJob 1u}
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
let ``CompleteJob.It sets job to None, adds reward money, adds reputation and metrics when the given avatar has a job.`` () =
    let input = employedAvatar
    let inputJob = employedAvatar.Job.Value
    let expected = 
        {input with 
            Job = None
            Money = input.Money + inputJob.Reward
            Reputation = input.Reputation + 1.0
            Metrics = input.Metrics |> Map.add Metric.CompletedJob 1u}
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
    let inputItem = 1UL
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
    let inputItem = 1UL
    let expected = 0u
    let actual =
        input
        |> Avatar.GetItemCount inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetItemCount.It returns the item count when the given avatar has an entry for the given item.`` () =
    let input = rationedAvatar
    let inputItem = 1UL
    let expected = rationedAvatar.Inventory.[inputItem]
    let actual =
        input
        |> Avatar.GetItemCount inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``RemoveInventory.It does nothing.When given a quantity of 0 items to remove or the given avatar has no items.`` ([<Values(0u, 1u)>]inputQuantity:uint32) =
    let input = avatar
    let inputItem = 1UL
    let expected =
        input
    let actual = 
        input
        |> Avatar.RemoveInventory inputItem inputQuantity
    Assert.AreEqual(expected, actual)

[<Test>]
let ``RemoveInventory.It reduces the given avatars inventory to 0 when the given number of items exceed the avatar's inventory.``() =
    let input = rationedAvatar 
    let inputItem = 1UL
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
    let inputItem = 1UL
    let inputQuantity = 20u
    let expected =
        {input with Inventory = input.Inventory |> Map.add 1UL 80u}
    let actual = 
        input
        |> Avatar.RemoveInventory inputItem inputQuantity
    Assert.AreEqual(expected, actual)


[<Test>]
let ``ALIVE/DEAD.It returns a ALIVE when given an avatar with above minimum health.`` () =
    match avatar with
    | Avatar.ALIVE -> Assert.Pass("It detected that the avatar is alive")
    | _ -> Assert.Fail("It detected that the avatar is not alive")

[<Test>]
let ``ALIVE/DEAD.It returns a DEAD when given an avatar minimum health (zero).`` () =
    match deadAvatar with
    | Avatar.DEAD -> Assert.Pass("It detected that the avatar is dead")
    | _ -> Assert.Fail("It detected that the avatar is not dead")

[<Test>]
let ``ClearMessages.It removes all of the messages from a given avatar.`` () =
    let input =
        {avatar with Messages=["Please clear me!"]}
    let expected =
        {input with Messages=[]}
    let actual =
        input
        |> Avatar.ClearMessages
    Assert.AreEqual(expected.Messages, actual.Messages)

[<Test>]
let ``AddMessages.It adds messages to a given avatar.`` () =
    let input = {avatar with Messages=["This is a previous message!"]}
    let inputMessages = ["Here's a message!";"And another one!"]
    let expectedMessages = List.append input.Messages inputMessages
    let actual =
        input
        |> Avatar.AddMessages inputMessages
    Assert.AreEqual(expectedMessages, actual.Messages)

[<Test>]
let ``AddMetric.It creates a metric value when there is no previously existing metric value in the avatar's table.`` () = 
    let input = avatar
    let inputMetric = Metric.Moved
    let inputValue = 1u
    let expected = {input with Metrics = input.Metrics |> Map.add inputMetric inputValue}
    let actual =
        input
        |> Avatar.AddMetric inputMetric inputValue
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AddMetric.It adds to a metric value when there is a previously existing metric value in the avatar's table.`` () = 
    let input = {avatar with Metrics = Map.empty |> Map.add Metric.Moved 1u}
    let inputMetric = Metric.Moved
    let inputValue = 1u
    let expectedValue = 2u
    let expected = {input with Metrics = input.Metrics |> Map.add inputMetric expectedValue}
    let actual =
        input
        |> Avatar.AddMetric inputMetric inputValue
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetUsedTonnage.It calculates the used tonnage based on inventory and item descriptors.`` () =
    let input = 
        {avatar with
            Inventory =
                Map.empty
                |> Map.add 1UL 2u
                |> Map.add 2UL 3u}
    let inputItems =
        Map.empty
        |> Map.add 1UL {
                        ItemId = 1UL
                        ItemName =""
                        Commodities =Map.empty
                        Occurrence  =0.0
                        Tonnage     =1.0
                        }
        |> Map.add 2UL {
                        ItemId = 1UL
                        ItemName =""
                        Commodities =Map.empty
                        Occurrence  =0.0
                        Tonnage     =3.0
                        }
    let expected = 11.0
    let actual =
        input
        |> Avatar.GetUsedTonnage inputItems
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetEffectiveSpeed.It returns full speed when there is no fouling.`` () =
    let input = 
        avatar
    let expected = 1.0
    let actual =
        input
        |> Avatar.GetEffectiveSpeed
    Assert.AreEqual(expected, actual)

let private fouledAvatar = 
    {avatar with 
        Vessel = 
            {avatar.Vessel with
                Fouling = avatar.Vessel.Fouling |> Statistic.ChangeBy 0.5}}

[<Test>]
let ``GetEffectiveSpeed.It returns proportionally reduced speed when there is fouling.`` () =
    let input = fouledAvatar
    let expected = 0.5
    let actual =
        input
        |> Avatar.GetEffectiveSpeed
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CleanHull.It cleans the hull of the given avatar.`` () =
    let input = fouledAvatar
    let expected =
        {input with 
            Turn = input.Turn |> Statistic.ChangeBy 1.0
            Vessel = 
                {input.Vessel with Fouling = input.Vessel.Fouling |> Statistic.ChangeBy -0.5}}
    let actual =
        input
        |> Avatar.CleanHull
    Assert.AreEqual(expected, actual)
