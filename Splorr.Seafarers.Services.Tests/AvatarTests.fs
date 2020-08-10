module AvatarTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

open CommonTestFixtures
open AvatarTestFixtures

[<Test>]
let ``GetReputation.It retrieves the reputation of the primary shipmate.`` () =
    let inputReputation = 100.0
    let input =
        avatar
        |> Avatar.TransformShipmate 
            (Shipmate.TransformStatistic 
                ShipmateStatisticIdentifier.Reputation 
                (Statistic.SetCurrentValue inputReputation >> Some)) Primary
    let expected = inputReputation
    let actual =
        input
        |> Avatar.GetReputation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetMoney.It retrieves the money of the primary shipmate.`` () =
    let inputMoney = 100.0
    let input =
        avatar
        |> Avatar.TransformShipmate 
            (Shipmate.TransformStatistic 
                ShipmateStatisticIdentifier.Money 
                (Statistic.SetCurrentValue inputMoney >> Some)) Primary
    let expected = inputMoney
    let actual =
        input
        |> Avatar.GetMoney
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetMoney.It assigns the amount of money of the primary shipmate.`` () =
    let inputMoney = 100.0
    let input =
        avatar
    let expected = inputMoney
    let actual =
        input
        |> Avatar.SetMoney inputMoney
        |> Avatar.GetMoney
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetReputation.It assigns the amount of reputation of the primary shipmate.`` () =
    let inputReputation = 100.0
    let input =
        avatar
    let expected = inputReputation
    let actual =
        input
        |> Avatar.SetReputation inputReputation
        |> Avatar.GetReputation
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Create.It creates an avatar.`` () =
    let actual =
        avatar
    Assert.AreEqual(1, actual.Shipmates.Count)

[<Test>]
let ``SetSpeed.It sets all stop when given less than zero.`` () =
    let input = avatarId
    let inputSpeed = -1.0
    let originalSpeed = 0.5
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    input
    |> Avatar.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed

[<Test>]
let ``SetSpeed.It sets full speed when gives more than one.`` () =
    let input = avatarId
    let inputSpeed = 2.0
    let originalSpeed = 0.5
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    input
    |> Avatar.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed

[<Test>]
let ``SetSpeed.It sets half speed when given half speed.`` () =
    let input = avatarId
    let inputSpeed = 0.5
    let originalSpeed = 1.0
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 0.5
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    input
    |> Avatar.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed


[<Test>]
let ``SetSpeed.It sets full speed when given one.`` () =    
    let input = avatarId
    let inputSpeed = 1.0
    let originalSpeed = 0.5
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 1.0
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    input
    |> Avatar.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed


[<Test>]
let ``SetSpeed.It sets all stop when given zero.`` () =
    let input = avatarId
    let inputSpeed = 0.0
    let originalSpeed = 0.5
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=originalSpeed} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedSpeed = 0.0
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Speed,identifier)
        Assert.AreEqual(expectedSpeed, statistic.CurrentValue)
    input
    |> Avatar.SetSpeed vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed

[<Test>]
let ``SetHeading.It sets a given heading.`` () =
    let input = avatarId
    let inputHeading = 2.5
    let originalHeading = 0.0
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=originalHeading} |> Some
        | _ -> 
            raise (System.NotImplementedException "Dont call me.")
            None
    let expectedHeading = inputHeading |> Angle.ToRadians
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        Assert.AreEqual(VesselStatisticIdentifier.Heading,identifier)
        Assert.AreEqual(expectedHeading, statistic.CurrentValue)
    input
    |> Avatar.SetHeading vesselSingleStatisticSource vesselSingleStatisticSink inputHeading

[<Test>]
let ``SetPosition.It sets a given position.`` () =
    let input = avatarId
    let inputPosition = (10.0, 11.0)
    let originalPosition = (8.0, 9.0)
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=originalPosition |> fst} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=originalPosition |> snd} |> Some
        | _ -> 
            raise (System.NotImplementedException "Source - Dont call me.")
            None
    let expectedPosition = inputPosition
    let vesselSingleStatisticSink (_) (identifier: VesselStatisticIdentifier, statistic:Statistic) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            Assert.AreEqual(expectedPosition |> fst, statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionY ->
            Assert.AreEqual(expectedPosition |> snd, statistic.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Sink - Dont call me.")
    input
    |> Avatar.SetPosition vesselSingleStatisticSource vesselSingleStatisticSink inputPosition

let private inputAvatarId = "avatar"

[<Test>]
let ``Move.It moves the avatar.`` () =
    let input = avatar
    let expectedPosition = (51.0,50.0)
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier, statistic:Statistic) =
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            Assert.AreEqual(expectedPosition |> fst,statistic.CurrentValue)
        | VesselStatisticIdentifier.PositionY ->
            Assert.AreEqual(expectedPosition |> snd,statistic.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Kaboom set")
    input
    |> Avatar.Move vesselSingleStatisticSource vesselSingleStatisticSink shipmateRationItemSourceStub inputAvatarId
    |> ignore

[<Test>]
let ``Move.It removes a ration when the given avatar has rations and full satiety and full health.`` () =
    let input = 
        {avatar with 
            Inventory = Map.empty |> Map.add 1UL 2u
            Shipmates =
                avatar.Shipmates}
    let expectedSatiety = input.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue
    let expectedHealth = input.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue
    let expectedInventory = Map.empty |> Map.add 1u 1u
    let shipmateRationItemSource (_) (_) = [0UL; 1UL]
    let actual =
        input
        |> Avatar.Move vesselSingleStatisticSource vesselSingleStatisticSink shipmateRationItemSource inputAvatarId
    Assert.AreEqual(expectedInventory, actual.Inventory)
    Assert.AreEqual(expectedSatiety, actual.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue)
    Assert.AreEqual(expectedHealth, actual.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue)

[<Test>]
let ``Move.It removes a ration and increases satiety when the given avatar has rations and less than full satiety.`` () =
    let input = 
        {avatar with 
            Inventory = Map.empty |> Map.add 1UL 2u}
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Satiety (fun x-> {x with CurrentValue=0.0} |> Some)) Primary
    let expectedSatiety = input.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue + 1.0
    let expectedHealth = input.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue
    let expectedInventory = Map.empty |> Map.add 1u 1u
    let actual =
        input
        |> Avatar.Move vesselSingleStatisticSource vesselSingleStatisticSink shipmateRationItemSourceStub inputAvatarId
    Assert.AreEqual(expectedInventory, actual.Inventory)
    Assert.AreEqual(expectedSatiety, actual.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue)
    Assert.AreEqual(expectedHealth, actual.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue)

[<Test>]
let ``Move.It lowers the avatar's satiety but not health when the given avatar has no rations.`` () =
    let input = {avatar with Inventory = Map.empty}
    let expectedSatiety = input.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue - 1.0
    let expectedHealth = input.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue
    let actual =
        input
        |> Avatar.Move vesselSingleStatisticSource vesselSingleStatisticSink shipmateRationItemSourceStub inputAvatarId
    Assert.AreEqual(expectedSatiety, actual.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue)
    Assert.AreEqual(expectedHealth, actual.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Health].CurrentValue)

[<Test>]
let ``Move.It lowers the avatar's maximum turn when the given avatar has no rations and minimum satiety.`` () =
    let input = 
        {avatar with 
            Inventory = Map.empty}
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Satiety (fun x -> {x with CurrentValue=0.0} |> Some)) Primary
    let expectedSatiety = 0.0
    let expectedTurnMaximum = input.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Turn].MaximumValue - 1.0
    let actual =
        input
        |> Avatar.Move vesselSingleStatisticSource vesselSingleStatisticSink shipmateRationItemSourceStub inputAvatarId
    Assert.AreEqual(expectedSatiety, actual.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Satiety].CurrentValue)
    Assert.AreEqual(expectedTurnMaximum, actual.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Turn].MaximumValue)

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
            Metrics = input.Metrics |> Map.add Metric.AbandonedJob 1u}
        |> Avatar.SetReputation ((input |> Avatar.GetReputation)-1.0)
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
            Metrics = input.Metrics |> Map.add Metric.CompletedJob 1u}
        |> Avatar.EarnMoney inputJob.Reward
        |> Avatar.SetReputation ((input |> Avatar.GetReputation)+1.0)
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
    let inputAmount = (input |> Avatar.GetMoney) + 1.0
    let expected = 
        input |> Avatar.SetMoney 0.0
    let actual =
        input
        |> Avatar.SpendMoney inputAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SpendMoney.It updates the avatars money when the given amount is less than the given avatar's money.`` () =
    let input = employedAvatar
    let inputAmount = 1.0
    let expected = 
        input |> Avatar.SetMoney ((input |> Avatar.GetMoney) - inputAmount)
    let actual =
        input
        |> Avatar.SpendMoney inputAmount
    Assert.AreEqual(expected, actual)

[<Test>]
let ``EarnMoney.It updates the avatars money by adding the given amount.`` () =
    let input = employedAvatar
    let inputAmount = 1.0
    let expected = 
        input |> Avatar.SetMoney ((input |> Avatar.GetMoney) + inputAmount)
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


[<Test>]//TODO - bad name
let ``ALIVE/ZERO_HEALTH/OLD_AGE.It returns a ALIVE when given an avatar with above minimum health and not end of life.`` () =
    match avatar.Shipmates.[Primary] |> Shipmate.GetStatus with
    | Alive -> Assert.Pass("It detected that the avatar is alive")
    | _ -> Assert.Fail("It detected that the avatar is not alive")

[<Test>]//TODO - bad name
let ``ALIVE/ZERO_HEALTH/OLD_AGE.It returns a ZERO_HEALTH when given an avatar at minimum health (zero).`` () =
    match deadAvatar.Shipmates.[Primary] |> Shipmate.GetStatus with
    | Dead ZeroHealth -> Assert.Pass("It detected that the avatar is dead of zero health")
    | _ -> Assert.Fail("It detected that the avatar is not dead")

[<Test>]//TODO - bad name
let ``ALIVE/ZERO_HEALTH/OLD_AGE.It returns a OLD_AGE when given an avatar at maximum turn.`` () =
    match oldAvatar.Shipmates.[Primary] |> Shipmate.GetStatus with
    | Dead OldAge -> Assert.Pass("It detected that the avatar is dead of old age")
    | _ -> Assert.Fail("It detected that the avatar is not dead")

[<Test>]
let ``AddMessages.It adds messages to a given avatar.`` () =
    let input = avatarId
    let firstMessage = "Here's a message!"
    let secondMessage = "And another one!"
    let inputMessages = [firstMessage; secondMessage]
    let avatarMessageSink (_) (message:string) =
        match message with
        | x when x = firstMessage || x = secondMessage ->
            Assert.Pass("Got an expected message.")
        | _ ->
            Assert.Fail("Got an unexpected message.")
    input
    |> Avatar.AddMessages avatarMessageSink inputMessages


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
    let expected = 1.0
    let actual =
        Avatar.GetEffectiveSpeed vesselSingleStatisticSource inputAvatarId
    Assert.AreEqual(expected, actual)

let private fouledAvatar = avatar

[<Test>]
let ``GetEffectiveSpeed.It returns proportionally reduced speed when there is fouling.`` () =
    let expected = 0.125
    let vesselSingleStatisticSource (_) (_) = {MinimumValue=0.0;MaximumValue=0.25;CurrentValue=0.25} |> Some
    let actual =
        Avatar.GetEffectiveSpeed vesselSingleStatisticSource inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CleanHull.It cleans the hull of the given avatar.`` () =
    let input = fouledAvatar
    let inputSide = Port
    let vesselSingleStatisticSource (_) (_) = {MinimumValue=0.0;MaximumValue=0.5;CurrentValue=0.5} |> Some
    let vesselSingleStatisticSink (_) (_:VesselStatisticIdentifier, statistic:Statistic) : unit =
        Assert.AreEqual(statistic.MinimumValue, statistic.CurrentValue)
    let expected =
        input
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Turn (Statistic.ChangeCurrentBy 1.0 >> Some)) Primary
        |> Avatar.AddMetric Metric.CleanedHull 1u
    let actual =
        input
        |> Avatar.CleanHull vesselSingleStatisticSource vesselSingleStatisticSink inputAvatarId inputSide
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetStatistic.It adds a statistic to the avatar when the statistic is not already present.`` () =
    let input = avatarNoStats
    let inputHealth = Statistic.Create (0.0,100.0) 0.0
    let expectedHealth = inputHealth
    let expected =
        {input with 
            Shipmates =
                input.Shipmates
                |> Map.map (fun _ x -> {x with Statistics = x.Statistics |> Map.add ShipmateStatisticIdentifier.Health expectedHealth})}
    let actual =
        input
        |> Avatar.TransformShipmate (Shipmate.SetStatistic ShipmateStatisticIdentifier.Health (inputHealth |> Some)) Primary
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetStatistic.It replaces a statistic on the avatar when the statistic is already present.`` () =
    let input = avatar
    let inputHealth = Statistic.Create (5.0,10.0) 5.0
    let expectedHealth = inputHealth
    let expected =
        {input with 
            Shipmates =
                input.Shipmates
                |> Map.map (fun _ x -> {x with Statistics = x.Statistics |> Map.add ShipmateStatisticIdentifier.Health expectedHealth})
            }
    let actual =
        input
        |> Avatar.TransformShipmate (Shipmate.SetStatistic ShipmateStatisticIdentifier.Health (inputHealth |> Some)) Primary
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetStatistic.It removes a statistic on the avatar when the statistic is present and new value is None.`` () =
    let input = avatar
    let inputHealth = None
    let expected =
        {input with 
            Shipmates =
                input.Shipmates
                |> Map.map (fun _ x -> {x with Statistics = x.Statistics |> Map.remove ShipmateStatisticIdentifier.Health})}
    let actual =
        input
        |> Avatar.TransformShipmate (Shipmate.SetStatistic ShipmateStatisticIdentifier.Health inputHealth) Primary
    Assert.AreEqual(expected, actual)


[<Test>]
let ``GetStatistic.It returns the statistic when it exists in the avatar.`` () =
    let input = avatar
    let expected = input.Shipmates.[Primary].Statistics.[ShipmateStatisticIdentifier.Health] |> Some
    let actual =
        input.Shipmates.[Primary]
        |> Shipmate.GetStatistic ShipmateStatisticIdentifier.Health
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetStatistic.It returns None when the statistic is absent from the avatar.`` () =
    let input = avatarNoStats
    let expected = None
    let actual =
        input.Shipmates.[Primary]
        |> Shipmate.GetStatistic ShipmateStatisticIdentifier.Health
    Assert.AreEqual(expected, actual)

[<Test>]
let ``TransformStatistic.It replaces the statistic when that statistic is originally present in the avatar.`` () =
    let input = avatar
    let inputHealth = Statistic.Create (5.0,10.0) 5.0
    let expectedHealth = inputHealth
    let expected =
        {input with 
            Shipmates =
                input.Shipmates
                |> Map.map (fun _ x -> {x with Statistics = x.Statistics |> Map.add ShipmateStatisticIdentifier.Health expectedHealth})}
    let actual =
        input
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Health (fun _ -> (inputHealth |> Some))) Primary
    Assert.AreEqual(expected, actual)

[<Test>]
let ``TransformStatistic.It does nothing when the given statistic is absent from the avatar.`` () =
    let input = avatarNoStats
    let inputHealth = Statistic.Create (5.0,10.0) 5.0
    let expected =
        input
    let actual =
        input
        |> Avatar.TransformShipmate (Shipmate.TransformStatistic ShipmateStatisticIdentifier.Health (fun _ -> (inputHealth |> Some))) Primary
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetCurrentFouling.It returns the current fouling for the Avatar.`` () =
    let vesselSingleStatisticSource (_) (_) =
        {MaximumValue=0.5; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let inputAvatarId = "avatar"
    let expected = 0.5
    let actual = 
        Avatar.GetCurrentFouling vesselSingleStatisticSource inputAvatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``GetMaximumFouling.It returns the maximum fouling for the Avatar.`` () =
    let vesselSingleStatisticSource (_) (_) =
        {MaximumValue=0.5; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let inputAvatarId = "avatar"
    let expected = 1.0
    let actual = 
        Avatar.GetMaximumFouling vesselSingleStatisticSource inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetSpeed.It gets the speed of an avatar.`` () =
    let actualSpeed = 
        {
            MinimumValue = 0.0
            MaximumValue = 1.0
            CurrentValue = 0.5
        }
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Speed -> actualSpeed |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let inputAvatarId="avatar"
    let expected = 0.5 |> Some
    let actual =
        inputAvatarId
        |> Avatar.GetSpeed vesselSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetPosition.It gets the position of an avatar.`` () =
    let actualX = 
        {
            MinimumValue = 0.0
            MaximumValue = 50.0
            CurrentValue = 25.0
        }
    let actualY = 
        {
            MinimumValue = 50.0
            MaximumValue = 100.0
            CurrentValue = 75.0
        }
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PositionX -> actualX |> Some
        | VesselStatisticIdentifier.PositionY -> actualY |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let inputAvatarId="avatar"
    let expected = 
        (actualX.CurrentValue, actualY.CurrentValue) 
        |> Some
    let actual =
        inputAvatarId
        |> Avatar.GetPosition vesselSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetHeading.It gets the heading of an avatar.`` () =
    let actualSpeed = 
        {
            MinimumValue = 0.0
            MaximumValue = 6.3
            CurrentValue = 0.0
        }
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.Heading -> actualSpeed |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let inputAvatarId="avatar"
    let expected = 0.0 |> Some
    let actual =
        inputAvatarId
        |> Avatar.GetHeading vesselSingleStatisticSource
    Assert.AreEqual(expected, actual)
