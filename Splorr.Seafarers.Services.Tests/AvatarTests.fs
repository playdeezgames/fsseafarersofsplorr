module AvatarTests

open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures
open AvatarTestFixtures

let private inputAvatarId = "avatar"

type TestAvatarSetPrimaryStatisticContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarSetPrimaryStatisticContext
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarAbandonJobContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarAbandonJobContext
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarCompleteJobContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarCompleteJobContext
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarCreateContext
        (avatarJobSink,
        rationItemSource,
        shipmateRationItemSink,
        shipmateSingleStatisticSink,
        shipmateStatisticTemplateSource,
        vesselStatisticSink, 
        vesselStatisticTemplateSource) =
    interface AvatarCreateContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.vesselStatisticSink: VesselStatisticSink = vesselStatisticSink
        member _.vesselStatisticTemplateSource: VesselStatisticTemplateSource = vesselStatisticTemplateSource
    interface ShipmateCreateContext with
        member _.rationItemSource: RationItemSource = rationItemSource
        member _.shipmateRationItemSink: ShipmateRationItemSink = shipmateRationItemSink
        member _.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member _.shipmateStatisticTemplateSource: ShipmateStatisticTemplateSource = shipmateStatisticTemplateSource

type TestAvatarMoveContext(shipmateRationItemSource, shipmateSingleStatisticSink, shipmateSingleStatisticSource, vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface AvatarMoveContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarGetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface ShipmateEatContext with
        member this.shipmateRationItemSource: ShipmateRationItemSource = shipmateRationItemSource
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface VesselTransformFoulingContext with
        member _.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarCleanHullContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource, vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface AvatarCleanHullContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestShipmateGetStatusContext(shipmateSingleStatisticSource) =
    interface ShipmateGetStatusContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarEarnMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarEarnMoneyContext
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarSpendMoneyContext
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestShipmateTransformStatisticContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarGetPrimaryStatisticContext() = 
    interface AvatarGetPrimaryStatisticContext

type TestAvatarSetSpeedContext() =
    interface AvatarSetSpeedContext

type TestAvatarSetHeadingContext() =
    interface AvatarSetHeadingContext

type TestAvatarSetPositionContext() =
    interface AvatarSetPositionContext

type TestAvatarAddInventoryContext() =
    interface AvatarAddInventoryContext

type TestAvatarGetItemCountContext() =
    interface AvatarGetItemCountContext

type TestAvatarRemoveInventoryContext() =
    interface AvatarRemoveInventoryContext

type TestAvatarAddMessagesContext() =
    interface AvatarAddMessagesContext

type TestAvatarAddMetricContext() =
    interface AvatarAddMetricContext

type TestAvatarGetUsedTonnageContext() =
    interface AvatarGetUsedTonnageContext

type TestAvatarGetEffectiveSpeedContext() =
    interface AvatarGetEffectiveSpeedContext

type TestAvatarGetCurrentFoulingContext() = 
    interface AvatarGetCurrentFoulingContext

type TestAvatarGetMaximumFoulingContext() =
    interface AvatarGetMaximumFoulingContext

type TestAvatarGetSpeedContext() =
    interface AvatarGetSpeedContext

type TestAvatarGetHeadingContext() = 
    interface AvatarGetHeadingContext

type TestAvatarGetPositionContext(vesselSingleStatisticSource) =
    interface AvatarGetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
            

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
    let context = TestAvatarGetPrimaryStatisticContext() :> AvatarGetPrimaryStatisticContext
    let actual =
        input
        |> Avatar.GetReputation context shipmateSingleStatisticSource
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
    let context = TestAvatarGetPrimaryStatisticContext() :> AvatarGetPrimaryStatisticContext
    let actual =
        input
        |> Avatar.GetMoney context shipmateSingleStatisticSource
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
    let context = TestAvatarSetPrimaryStatisticContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSetPrimaryStatisticContext
    input
    |> Avatar.SetMoney 
        context
        shipmateSingleStatisticSource 
        shipmateSingleStatisticSink 
        inputMoney

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
    let context = TestAvatarSetPrimaryStatisticContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSetPrimaryStatisticContext
    input
    |> Avatar.SetReputation 
        context
        shipmateSingleStatisticSource 
        shipmateSingleStatisticSink 
        inputReputation

[<Test>]
let ``Create.It creates an avatar.`` () =
    let avatarJobSink (_) (actual) = 
        Assert.AreEqual(None, actual)
    let rationItemSource () = 
        []
    let shipmateRationItemSink (_) (_) (actual) = 
        Assert.AreEqual([], actual)
    let shipmateSingleStatisticSink (_) (_) (_) = 
        Assert.Fail("shipmateSingleStatisticSink")
    let shipmateStatisticTemplateSource () = 
        Map.empty
    let vesselStatisticSink (_) (actual) = 
        Assert.AreEqual(Map.empty, actual)
    let vesselStatisticTemplateSource () = 
        Map.empty
    let context = 
        TestAvatarCreateContext
            (avatarJobSink,
            rationItemSource,
            shipmateRationItemSink,
            shipmateSingleStatisticSink,
            shipmateStatisticTemplateSource,
            vesselStatisticSink, 
            vesselStatisticTemplateSource)
    Avatar.Create
        context
        avatarId                        

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
    let context = TestAvatarSetSpeedContext() :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed

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
    let context = TestAvatarSetSpeedContext() :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed

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
    let context = TestAvatarSetSpeedContext() :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed


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
    let context = TestAvatarSetSpeedContext() :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed


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
    let context = TestAvatarSetSpeedContext() :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context vesselSingleStatisticSource vesselSingleStatisticSink inputSpeed

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
    let context = TestAvatarSetHeadingContext() :> AvatarSetHeadingContext
    input
    |> Avatar.SetHeading context vesselSingleStatisticSource vesselSingleStatisticSink inputHeading

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
    let context = TestAvatarSetPositionContext() :> AvatarSetPositionContext
    input
    |> Avatar.SetPosition context vesselSingleStatisticSource vesselSingleStatisticSink inputPosition

[<Test>]
let ``Move.It moves the avatar.`` () =
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
    let context = TestAvatarMoveContext(shipmateRationItemSourceStub, shipmateSingleStatisticSinkStub, shipmateSingleStatisticSourceStub, vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
        avatarInventorySink
        avatarInventorySource
        avatarShipmateSourceStub
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL)])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSinkStub 
        shipmateSingleStatisticSourceStub
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputAvatarId

[<Test>]
let ``Move.It removes a ration when the given avatar has rations and full satiety.`` () =
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
    let context = TestAvatarMoveContext(shipmateRationItemSource, shipmateSingleStatisticSink, shipmateSingleStatisticSource, vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
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
    let context = TestAvatarMoveContext(shipmateRationItemSourceStub, shipmateSingleStatisticSink, shipmateSingleStatisticSource, vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
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
    let context = TestAvatarMoveContext(shipmateRationItemSourceStub, shipmateSingleStatisticSink, shipmateSingleStatisticSource, vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
        avatarInventorySink
        avatarInventorySource
        avatarShipmateSource
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL)])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputAvatarId


[<Test>]
let ``Move.It lowers the avatar's maximum turn and updates the starvation metric when the given avatar has no rations and minimum satiety.`` () =
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
    let context = TestAvatarMoveContext(shipmateRationItemSourceStub, shipmateSingleStatisticSink, shipmateSingleStatisticSource, vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
        avatarInventorySink
        avatarInventorySource
        avatarShipmateSource
        (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Starved, 1UL)])
        avatarSingleMetricSourceStub
        shipmateRationItemSourceStub 
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        vesselSingleStatisticSink 
        vesselSingleStatisticSource 
        inputAvatarId

[<Test>]
let ``AbandonJob.It does nothing when the given avatar has no job.`` () =
    let input = avatarId
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let context = TestAvatarAbandonJobContext (shipmateSingleStatisticSinkStub, shipmateSingleStatisticSourceStub) :> AvatarAbandonJobContext
    input
    |> Avatar.AbandonJob
        context
        avatarJobSink
        avatarJobSource
        avatarSingleMetricSinkExplode
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSinkStub
        shipmateSingleStatisticSourceStub

[<Test>]
let ``AbandonJob.It set job to None when the given avatar has a job.`` () =
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
    let avatarJobSink (_) (job:Job option) =
        let expected : Job option = None
        Assert.AreEqual(expected, job)
    let avatarJobSource (_) =
        {
            FlavorText  = ""
            Reward      = 0.0
            Destination = (0.0, 0.0)
        }
        |> Some
    let context = TestAvatarAbandonJobContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarAbandonJobContext
    Avatar.AbandonJob
        context
        avatarJobSink
        avatarJobSource
        (assertAvatarSingleMetricSink [(Metric.AbandonedJob, 1UL)])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        avatarId
    
    
[<Test>]
let ``CompleteJob.It does nothing when the given avatar has no job.`` () =
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let context = TestAvatarCompleteJobContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarCompleteJobContext
    Avatar.CompleteJob
        context
        avatarJobSink
        avatarJobSource
        avatarSingleMetricSinkExplode
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        avatarId

[<Test>]
let ``CompleteJob.It sets job to None, adds reward money, adds reputation and metrics when the given avatar has a job.`` () =
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Statistic.Create (0.0, 100.0) (0.0) |> Some
        | ShipmateStatisticIdentifier.Reputation ->
            Statistic.Create (-100.0, 100.0) (0.0) |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let inputJob = 
        {
            Reward = 10.0
            FlavorText=""
            Destination=(0.0, 0.0)
        }
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Money ->
            Assert.AreEqual(inputJob.Reward, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Reputation ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarJobSink (_) (job: Job option) =
        let expected : Job option = None
        Assert.AreEqual(expected, job)
    let avatarJobSource (_) =
        inputJob 
        |> Some
    let context = TestAvatarCompleteJobContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarCompleteJobContext
    Avatar.CompleteJob
        context
        avatarJobSink
        avatarJobSource
        (assertAvatarSingleMetricSink [(Metric.CompletedJob, 1UL)])
        avatarSingleMetricSourceStub
        shipmateSingleStatisticSink
        shipmateSingleStatisticSource
        avatarId

[<Test>]
let ``SpendMoney.It has no effect when given a negative amount to spend.`` () =
    let input = avatarId
    let inputAmount = -1.0
    let shipmateSingleStatisticSource (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
        None
    let shipmateSingleStatisticSink (_) (_) (_) =
        raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let context = TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSpendMoneyContext
    input
    |> Avatar.SpendMoney 
        context
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
    let context = TestAvatarEarnMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarEarnMoneyContext
    input
    |> Avatar.EarnMoney 
        context
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
    let context = TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSpendMoneyContext
    input
    |> Avatar.SpendMoney 
        context
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
    let context = TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSpendMoneyContext
    input
    |> Avatar.SpendMoney 
        context
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
    let context = TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSpendMoneyContext
    input
    |> Avatar.SpendMoney 
        context
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
    let context = TestAvatarEarnMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarEarnMoneyContext
    input
    |> Avatar.EarnMoney 
        context
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
    let context = TestAvatarAddInventoryContext() :> AvatarAddInventoryContext
    input
    |> Avatar.AddInventory 
        context
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
    let context = TestAvatarGetItemCountContext() :> AvatarGetItemCountContext
    let actual =
        input
        |> Avatar.GetItemCount 
            context
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
    let context = TestAvatarGetItemCountContext() :> AvatarGetItemCountContext
    let actual =
        input
        |> Avatar.GetItemCount 
            context
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
    let context = TestAvatarRemoveInventoryContext() :> AvatarRemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
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
    let context = TestAvatarRemoveInventoryContext() :> AvatarRemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
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
    let context = TestAvatarRemoveInventoryContext() :> AvatarRemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
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
    let context = TestShipmateGetStatusContext(shipmateSingleStatisticSource) :> ShipmateGetStatusContext
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
    let inputAvatarId = avatarId
    let inputShipmateId = Primary
    let context = TestShipmateGetStatusContext(shipmateSingleStatisticSource) :> ShipmateGetStatusContext
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
    let inputAvatarId = avatarId
    let inputShipmateId = Primary
    let context = TestShipmateGetStatusContext(shipmateSingleStatisticSource) :> ShipmateGetStatusContext
    match Shipmate.GetStatus 
        context
        inputAvatarId 
        inputShipmateId with
    | Dead OldAge -> ()
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
            ()
        | _ ->
            Assert.Fail("Got an unexpected message.")
    let context = TestAvatarAddMessagesContext() :> AvatarAddMessagesContext
    input
    |> Avatar.AddMessages context avatarMessageSink inputMessages


[<Test>]
let ``AddMetric.It creates a metric value when there is no previously existing metric value in the avatar's table.`` () = 
    let input = avatarId
    let inputMetric = Metric.Moved
    let inputValue = 1UL
    let context = TestAvatarAddMetricContext() :> AvatarAddMetricContext
    input
    |> Avatar.AddMetric
        context
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
    let context = TestAvatarAddMetricContext() :> AvatarAddMetricContext
    input
    |> Avatar.AddMetric 
        context
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
    let context = TestAvatarGetUsedTonnageContext() :> AvatarGetUsedTonnageContext
    let actual =
        input
        |> Avatar.GetUsedTonnage 
            context
            avatarInventorySource
            inputItems
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetEffectiveSpeed.It returns full speed when there is no fouling.`` () =
    let expected = 1.0
    let context = TestAvatarGetEffectiveSpeedContext() :> AvatarGetEffectiveSpeedContext
    let actual =
        Avatar.GetEffectiveSpeed context vesselSingleStatisticSource inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetEffectiveSpeed.It returns proportionally reduced speed when there is fouling.`` () =
    let expected = 0.125
    let vesselSingleStatisticSource (_) (_) = {MinimumValue=0.0;MaximumValue=0.25;CurrentValue=0.25} |> Some
    let context = TestAvatarGetEffectiveSpeedContext() :> AvatarGetEffectiveSpeedContext
    let actual =
        Avatar.GetEffectiveSpeed context vesselSingleStatisticSource inputAvatarId
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
    let context = TestAvatarCleanHullContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource, vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarCleanHullContext
    Avatar.CleanHull
        context
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
    let context = TestShipmateTransformStatisticContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> ShipmateTransformStatisticContext
    Shipmate.TransformStatistic 
        context
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
    let context = TestShipmateTransformStatisticContext (shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> ShipmateTransformStatisticContext
    Shipmate.TransformStatistic 
        context
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
    let context = TestAvatarGetCurrentFoulingContext() :> AvatarGetCurrentFoulingContext
    let actual = 
        Avatar.GetCurrentFouling context vesselSingleStatisticSource inputAvatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``GetMaximumFouling.It returns the maximum fouling for the Avatar.`` () =
    let vesselSingleStatisticSource (_) (_) =
        {MaximumValue=0.5; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let inputAvatarId = "avatar"
    let expected = 1.0
    let context = TestAvatarGetMaximumFoulingContext() :> AvatarGetMaximumFoulingContext
    let actual = 
        Avatar.GetMaximumFouling context vesselSingleStatisticSource inputAvatarId
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
    let context = TestAvatarGetSpeedContext() :> AvatarGetSpeedContext
    let actual =
        inputAvatarId
        |> Avatar.GetSpeed context vesselSingleStatisticSource
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
    let context = TestAvatarGetPositionContext(vesselSingleStatisticSource) :> AvatarGetPositionContext
    let actual =
        inputAvatarId
        |> Avatar.GetPosition context vesselSingleStatisticSource
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
    let context = TestAvatarGetHeadingContext() :> AvatarGetHeadingContext
    let actual =
        inputAvatarId
        |> Avatar.GetHeading context vesselSingleStatisticSource
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Move.It transforms the avatar within the given world.`` () =
    let vesselSingleStatisticSource (_) (identifier) = 
        match identifier with
        | VesselStatisticIdentifier.PositionX
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=50.0} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.5; CurrentValue=0.0} |> Some
        | VesselStatisticIdentifier.FoulRate ->
            {MinimumValue=0.01; MaximumValue=0.01; CurrentValue=0.01} |> Some
        | _ ->
            None
    let mutable xPositionCalled = 0
    let mutable yPositionCalled = 0
    let vesselSingleStatisticSink (_) (identifier:VesselStatisticIdentifier,_) =
        match identifier with
        | VesselStatisticIdentifier.PositionX ->
            xPositionCalled <- xPositionCalled + 1
        | VesselStatisticIdentifier.PositionY ->
            yPositionCalled <- yPositionCalled + 1
        | _ ->
            raise (System.NotImplementedException "Kaboom set")
    let avatarShipmateSource (_) = 
        [ Primary ]
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Statistic.Create (0.0, 50000.0) 0.0 |> Some
        | ShipmateStatisticIdentifier.Satiety ->
            Statistic.Create (0.0, 100.0) 50.0 |> Some
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSource")
            None
    let shipmateSingleStatisticSink (_) (_) (identifier:ShipmateStatisticIdentifier, statistic: Statistic option) =
        match identifier with
        | ShipmateStatisticIdentifier.Turn ->
            Assert.AreEqual(1.0, statistic.Value.CurrentValue)
        | ShipmateStatisticIdentifier.Satiety ->
            Assert.AreEqual(49.0, statistic.Value.CurrentValue)
        | _ ->
            raise (System.NotImplementedException "kaboom shipmateSingleStatisticSink")
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let context = TestAvatarMoveContext(shipmateRationItemSourceStub, shipmateSingleStatisticSink, shipmateSingleStatisticSource, vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
            context
            avatarInventorySink
            avatarInventorySource
            avatarShipmateSource
            (assertAvatarSingleMetricSink [Metric.Moved, 1UL; Metric.Ate, 0UL])
            avatarSingleMetricSourceStub
            shipmateRationItemSourceStub 
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSink 
            vesselSingleStatisticSource 
            avatarId
    Assert.AreEqual(1, xPositionCalled)
    Assert.AreEqual(1, yPositionCalled)
