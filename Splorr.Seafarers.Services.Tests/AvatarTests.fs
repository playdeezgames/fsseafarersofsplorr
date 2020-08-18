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
        avatarId
    let expected = inputReputation
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (inputReputation, inputReputation) inputReputation
            |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom Get")
            None
    let actual =
        input
        |> Avatar.GetReputation shipmateSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetMoney.It retrieves the money of the primary shipmate.`` () =
    let inputMoney = 100.0
    let input =
        avatarId
    let expected = inputMoney
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (inputMoney, inputMoney) inputMoney
            |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom Get")
            None
    let actual =
        input
        |> Avatar.GetMoney shipmateSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetMoney.It assigns the amount of money of the primary shipmate.`` () =
    let inputMoney = 100.0
    let input =
        avatarId
    let expected = inputMoney
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) = 
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 1000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier,statistic:Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(inputMoney, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Kaboom set")
    input
    |> Avatar.SetMoney shipmateSingleStatisticSource shipmateSingleStatisticSink inputMoney

[<Test>]
let ``SetReputation.It assigns the amount of reputation of the primary shipmate.`` () =
    let inputReputation = 100.0
    let input =
        avatarId
    let expected = inputReputation
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) = 
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (-1000.0, 1000.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom get")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(inputReputation, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Kaboom set")
    input
    |> Avatar.SetReputation shipmateSingleStatisticSource shipmateSingleStatisticSink inputReputation

[<Test>]
let ``Create.It creates an avatar.`` () =
    let actual =
        avatar
    Assert.AreEqual(None, actual.Job)

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
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    Avatar.Move 
        avatarInventorySink
        avatarInventorySource
        avatarShipmateSourceStub
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 0UL)])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSinkStub 
        shipmateSingleStatisticSourceStub
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputAvatarId
    |> ignore

[<Test>]
let ``Move.It removes a ration when the given avatar has rations and full satiety.`` () =
    let input = 
        avatar
    let originalInventory = Map.empty |> Map.add 1UL 2UL
    let expectedInventory = Map.empty |> Map.add 1UL 1UL
    let shipmateRationItemSource (_) (_) = [0UL; 1UL]
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 100.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
            Assert.AreEqual(100.0, statistic.Value.MaximumValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(100.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        originalInventory
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(expectedInventory, inventory)
    Avatar.Move 
        avatarInventorySink
        avatarInventorySource
        avatarShipmateSource
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 1UL)])
        avatarSingleMetricSourceStub
        shipmateRationItemSource 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputAvatarId

[<Test>]
let ``Move.It removes a ration and increases satiety when the given avatar has rations and less than full satiety.`` () =
    let input = 
        avatar
    let originalInventory = Map.empty |> Map.add 1UL 2UL
    let expectedInventory = Map.empty |> Map.add 1UL 1UL
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
            Assert.AreEqual(100.0, statistic.Value.MaximumValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(51.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        originalInventory
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(expectedInventory, inventory)
    Avatar.Move 
        avatarInventorySink
        avatarInventorySource
        avatarShipmateSource
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 1UL)])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputAvatarId
    


[<Test>]
let ``Move.It lowers the avatar's satiety but does not affect turns when the given avatar has no rations.`` () =
    let input = avatar
    let expectedAvatar = 
        input
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with 
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
            Assert.AreEqual(100.0, statistic.Value.MaximumValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    Avatar.Move 
        avatarInventorySink
        avatarInventorySource
        avatarShipmateSource
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 0UL)])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputAvatarId


[<Test>]
let ``Move.It lowers the avatar's maximum turn when the given avatar has no rations and minimum satiety.`` () =
    let input = 
        avatar
    let avatarShipmateSource (_) = 
        [ Primary ]
    let mutable sinkCalls = 0u
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create 
                (0.0, 100.0) 
                (if sinkCalls>0u then 1.0 else 0.0) 
            |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic : Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            match sinkCalls with
            | 0u ->
                Assert.AreEqual(1.0, statistic.Value.CurrentValue)
                Assert.AreEqual(100.0, statistic.Value.MaximumValue)
            | 1u ->
                Assert.AreEqual(1.0, statistic.Value.CurrentValue)
                Assert.AreEqual(99.0, statistic.Value.MaximumValue)
            | _ ->
                raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink - bad number of sink calls")
            sinkCalls <- sinkCalls + 1u
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    Avatar.Move 
        avatarInventorySink
        avatarInventorySource
        avatarShipmateSource
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 0UL)])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputAvatarId


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
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            shipmateSingleStatisticSinkStub
            shipmateSingleStatisticSourceStub
            avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``AbandonJob.It set job to None when the given avatar has a job.`` () =
    let input = employedAvatar
    let expected = 
        {input with 
            Job=None}
    let shipmateSingleStatisticSource (_) (_) (identifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (-100.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier,statistic:Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(-1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let actual =
        input
        |> Avatar.AbandonJob
            (assertAvatarSingleMetricSink [(Metric.AbandonedJob, 1UL)])
            avatarSingleMetricSourceStub
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            avatarId
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``CompleteJob.It does nothing when the given avatar has no job.`` () =
    let input = avatar
    let expected = input
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let actual = 
        input
        |> Avatar.CompleteJob
            avatarSingleMetricSinkExplode
            avatarSingleMetricSourceStub
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CompleteJob.It sets job to None, adds reward money, adds reputation and metrics when the given avatar has a job.`` () =
    let input = employedAvatar
    let inputJob = employedAvatar.Job.Value
    let expected = 
        {input with 
            Job = None}
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) (0.0) |> Some
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (-100.0, 100.0) (0.0) |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(inputJob.Reward, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let actual = 
        input
        |> Avatar.CompleteJob
            (assertAvatarSingleMetricSink [(Metric.CompletedJob, 1UL)])
            avatarSingleMetricSourceStub
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            avatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SpendMoney.It has no effect when given a negative amount to spend.`` () =
    let input = avatarId
    let inputAmount = -1.0
    let expected = input
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    input
    |> Avatar.SpendMoney 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        inputAmount


[<Test>]
let ``EarnMoney.It has no effect when given a negative amount to earn.`` () =
    let input = avatarId
    let inputAmount = -1.0
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    input
    |> Avatar.EarnMoney 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        inputAmount


[<Test>]
let ``SpendMoney.It has no effect when the given avatar has no money.`` () =
    let input = avatarId
    let inputAmount = 1.0
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(0.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    input
    |> Avatar.SpendMoney 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        inputAmount

[<Test>]
let ``SpendMoney.It reduces the avatar's money to zero when the given amount exceeds the given avatar's money.`` () =
    let input = avatarId
    let inputAmount = 101.0
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic:Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(0.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    input
    |> Avatar.SpendMoney 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        inputAmount

[<Test>]
let ``SpendMoney.It updates the avatars money when the given amount is less than the given avatar's money.`` () =
    let input = avatarId
    let inputAmount = 1.0
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic:Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    input
    |> Avatar.SpendMoney 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        inputAmount

[<Test>]
let ``EarnMoney.It updates the avatars money by adding the given amount.`` () =
    let input = avatarId
    let inputAmount = 1.0
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(inputAmount, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    input
    |> Avatar.EarnMoney 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        inputAmount

[<Test>]
let ``AddInventory.It adds a given number of given items to the given avatar's inventory.`` () =
    let input = avatarId
    let inputItem = 1UL
    let inputQuantity = 2UL
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(1, inventory.Count)
        Assert.AreEqual(inputQuantity, inventory.[inputItem])
    input
    |> Avatar.AddInventory 
        avatarInventorySink
        avatarInventorySource
        inputItem 
        inputQuantity


[<Test>]
let ``GetItemCount.It returns zero when the given avatar has no entry for the given item.`` () =
    let input = avatarId
    let inputItem = 1UL
    let expected = 0u
    let avatarInventorySource (_) =
        Map.empty
    let actual =
        input
        |> Avatar.GetItemCount 
            avatarInventorySource
            inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetItemCount.It returns the item count when the given avatar has an entry for the given item.`` () =
    let input = avatarId
    let inputItem = 1UL
    let expected = 2UL
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add inputItem expected
    let actual =
        input
        |> Avatar.GetItemCount 
            avatarInventorySource
            inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``RemoveInventory.It does nothing.When given a quantity of 0 items to remove or the given avatar has no items.`` 
        ([<Values(0UL, 1UL)>]inputQuantity:uint64) =
    let input = avatarId
    let inputItem = 1UL
    let expected =
        input
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    input
    |> Avatar.RemoveInventory 
        avatarInventorySource
        avatarInventorySink
        inputItem 
        inputQuantity

[<Test>]
let ``RemoveInventory.It reduces the given avatars inventory to 0 when the given number of items exceed the avatar's inventory.``() =
    let input = avatarId
    let inputItem = 1UL
    let inputQuantity = 2UL
    let originalQuantity = inputQuantity - 1UL
    let avatarInventorySource (_) =
        Map.empty
        |> Map.add inputItem originalQuantity
    let expectedQuantity = 0UL
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(0, inventory.Count)
    input
    |> Avatar.RemoveInventory 
        avatarInventorySource
        avatarInventorySink
        inputItem 
        inputQuantity

[<Test>]
let ``RemoveInventory.It reduces the given avatar's inventory by the given amount.``() =
    let input = avatarId 
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
    input
    |> Avatar.RemoveInventory 
        avatarInventorySource
        avatarInventorySink
        inputItem 
        inputQuantity


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
    let inputAvatarId = avatarId
    let inputShipmateId = Primary
    match Shipmate.GetStatus shipmateSingleStatisticSource inputAvatarId inputShipmateId with
    | Alive -> Assert.Pass("It detected that the avatar is alive")
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
    let inputAvatarId = avatarId
    let inputShipmateId = Primary
    match Shipmate.GetStatus shipmateSingleStatisticSource inputAvatarId inputShipmateId with
    | Dead ZeroHealth -> Assert.Pass("It detected that the avatar is dead of zero health")
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
    let inputAvatarId = avatarId
    let inputShipmateId = Primary
    match Shipmate.GetStatus shipmateSingleStatisticSource inputAvatarId inputShipmateId with
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
    let input = avatarId
    let inputMetric = Metric.Moved
    let inputValue = 1UL
    input
    |> Avatar.AddMetric
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL)])
        avatarSingleMetricSourceStub
        inputMetric 
        inputValue

[<Test>]
let ``AddMetric.It adds to a metric value when there is a previously existing metric value in the avatar's table.`` () = 
    let input = avatarId
    let inputMetric = Metric.Moved
    let inputValue = 1UL
    let expectedValue = 2UL
    input
    |> Avatar.AddMetric 
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL)])
        avatarSingleMetricSourceStub
        inputMetric 
        inputValue

[<Test>]
let ``GetUsedTonnage.It calculates the used tonnage based on inventory and item descriptors.`` () =
    let input = 
        avatarId
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
    let actual =
        input
        |> Avatar.GetUsedTonnage 
            avatarInventorySource
            inputItems
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
    let input = avatarId
    let inputSide = Port
    let vesselSingleStatisticSource (_) (_) = {MinimumValue=0.0;MaximumValue=0.5;CurrentValue=0.5} |> Some
    let vesselSingleStatisticSink (_) (_:VesselStatisticIdentifier, statistic:Statistic) : unit =
        Assert.AreEqual(statistic.MinimumValue, statistic.CurrentValue)
    let avatarShipmateSource (_) =
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier: ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException "Kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier: ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "Kaboom shipmateSingleStatisticSink")
    Avatar.CleanHull
        avatarShipmateSource
        (assertAvatarSingleMetricSink [(Metric.CleanedHull, 1UL)])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputSide
        inputAvatarId 

[<Test>]
let ``TransformStatistic.It replaces the statistic when that statistic is originally present in the avatar.`` () =
    let input = avatar
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
    Shipmate.TransformStatistic 
        shipmateSingleStatisticSource
        shipmateSingleStatisticSink
        ShipmateStatisticIdentifier.Health 
        (fun _ -> (inputHealth |> Some))
        avatarId
        Primary

[<Test>]
let ``TransformStatistic.It does nothing when the given statistic is absent from the avatar.`` () =
    let inputHealth = Statistic.Create (5.0,10.0) 5.0
    let shipmateSingleStatisticSource (_) (_) (_) =
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        Assert.Fail("Dont call me.")
    Shipmate.TransformStatistic 
        shipmateSingleStatisticSource
        shipmateSingleStatisticSink
        ShipmateStatisticIdentifier.Health 
        (fun _ -> (inputHealth |> Some))
        avatarId
        Primary

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
