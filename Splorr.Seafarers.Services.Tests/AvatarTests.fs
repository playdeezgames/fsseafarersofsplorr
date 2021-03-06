module AvatarTests

open System
open NUnit.Framework
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open CommonTestFixtures
open AvatarTestFixtures
open Tarot

let private inputAvatarId = "avatar"

type TestAvatarSetPrimaryStatisticContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarSetPrimaryStatisticContext
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarAbandonJobContext 
        (avatarJobSink,
        avatarJobSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource) =
    interface AvatarAbandonJobContext with
        member this.avatarJobSink: AvatarJobSink = avatarJobSink
        member this.avatarJobSource: AvatarJobSource = avatarJobSource
    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarAddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarCompleteJobContext 
        (avatarJobSink,
        avatarJobSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource) =
    interface AvatarCompleteJobContext with
        member _.avatarJobSink : AvatarJobSink = avatarJobSink
        member _.avatarJobSource : AvatarJobSource = avatarJobSource

    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface AvatarAddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
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

type TestAvatarMoveContext
        (avatarInventorySink,
        avatarInventorySource,
        avatarShipmateSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateRationItemSource, 
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource, 
        vesselSingleStatisticSink, 
        vesselSingleStatisticSource) =
    interface AvatarEatContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
        
    interface AvatarGetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        
    interface AvatarTransformShipmatesContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
        
    interface VesselBefoulContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        
    interface AvatarMoveContext with
        member _.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        
    interface AvatarAddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
        
    interface AvatarSetPositionContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        
    interface AvatarGetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
        
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

type TestAvatarCleanHullContext
        (avatarShipmateSource,
        avatarSingleMetricSink,
        avatarSingleMetricSource,
        shipmateSingleStatisticSink, 
        shipmateSingleStatisticSource, 
        vesselSingleStatisticSink, 
        vesselSingleStatisticSource) =
    interface AvatarCleanHullContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarTransformShipmatesContext with
        member this.avatarShipmateSource: AvatarShipmateSource = avatarShipmateSource
    interface AvatarAddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestShipmateGetStatusContext(shipmateSingleStatisticSource) =
    interface ShipmateGetStatusContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarEarnMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarEarnMoneyContext
    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarSpendMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarSpendMoneyContext
    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestShipmateTransformStatisticContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface ShipmateTransformStatisticContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarGetPrimaryStatisticContext(shipmateSingleStatisticSource) = 
    interface AvatarGetPrimaryStatisticContext with
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface AvatarSetSpeedContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarSetHeadingContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface AvatarSetHeadingContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarSetPositionContext(vesselSingleStatisticSink, vesselSingleStatisticSource) =
    interface AvatarSetPositionContext with
        member this.vesselSingleStatisticSink: VesselSingleStatisticSink = vesselSingleStatisticSink
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarAddInventoryContext(avatarInventorySink, avatarInventorySource) =
    interface AvatarGetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource
    interface AvatarAddInventoryContext with
        member _.avatarInventorySink   : AvatarInventorySink = avatarInventorySink
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource

type TestAvatarGetItemCountContext(avatarInventorySource) =
    interface AvatarGetItemCountContext with
        member _.avatarInventorySource : AvatarInventorySource = avatarInventorySource

type TestAvatarRemoveInventoryContext(avatarInventorySink, avatarInventorySource) =
    interface AvatarRemoveInventoryContext with
        member this.avatarInventorySink: AvatarInventorySink = avatarInventorySink
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource

type TestAvatarAddMessagesContext(avatarMessageSink) =
    interface AvatarAddMessagesContext with
        member this.avatarMessageSink: AvatarMessageSink = avatarMessageSink

type TestAvatarAddMetricContext
        (avatarSingleMetricSink,
        avatarSingleMetricSource) =
    interface AvatarAddMetricContext with
        member this.avatarSingleMetricSink: AvatarSingleMetricSink = avatarSingleMetricSink
        member this.avatarSingleMetricSource: AvatarSingleMetricSource = avatarSingleMetricSource

type TestAvatarGetUsedTonnageContext(avatarInventorySource) =
    interface AvatarGetUsedTonnageContext with
        member this.avatarInventorySource: AvatarInventorySource = avatarInventorySource

type TestAvatarGetEffectiveSpeedContext(vesselSingleStatisticSource) =
    interface AvatarGetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarGetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
    interface AvatarGetEffectiveSpeedContext

type TestAvatarGetCurrentFoulingContext(vesselSingleStatisticSource) = 
    interface AvatarGetCurrentFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetMaximumFoulingContext(vesselSingleStatisticSource) =
    interface AvatarGetMaximumFoulingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetSpeedContext(vesselSingleStatisticSource) =
    interface AvatarGetSpeedContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetHeadingContext(vesselSingleStatisticSource) = 
    interface AvatarGetHeadingContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource

type TestAvatarGetPositionContext(vesselSingleStatisticSource) =
    interface AvatarGetPositionContext with
        member this.vesselSingleStatisticSource: VesselSingleStatisticSource = vesselSingleStatisticSource
     
type TestAvatarSetMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarSetMoneyContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarSetReputationContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) =
    interface AvatarSetReputationContext with
        member this.shipmateSingleStatisticSink: ShipmateSingleStatisticSink = shipmateSingleStatisticSink
        member this.shipmateSingleStatisticSource: ShipmateSingleStatisticSource = shipmateSingleStatisticSource

type TestAvatarGetGamblingHandContext (avatarGamblingHandSource) =
    interface AvatarGetGamblingHandContext with
        member _.avatarGamblingHandSource : AvatarGamblingHandSource = avatarGamblingHandSource

type TestAvatarDealGamblingHandContext(avatarGamblingHandSink, random) =
    interface AvatarDealGamblingHandContext with
        member _.avatarGamblingHandSink : AvatarGamblingHandSink = avatarGamblingHandSink
        member _.random : Random = random

type TestAvatarEnterIslandFeatureContext() =
    interface AvatarEnterIslandFeatureContext

(*HERE BE THE END OF THE TEST CONTEXTS!*)
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
    let context = TestAvatarGetPrimaryStatisticContext(shipmateSingleStatisticSource) :> AvatarGetPrimaryStatisticContext
    let actual =
        input
        |> Avatar.GetReputation context
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
    let context = TestAvatarGetPrimaryStatisticContext(shipmateSingleStatisticSource) :> AvatarGetPrimaryStatisticContext
    let actual =
        input
        |> Avatar.GetMoney context
    Assert.AreEqual(expected, actual)

[<Test>]
let ``SetMoney.It assigns the amount of money of the primary shipmate.`` () =
    let inputMoney = 100.0
    let input =
        avatarId
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
    let context = TestAvatarSetMoneyContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSetMoneyContext
    input
    |> Avatar.SetMoney 
        context
        inputMoney

[<Test>]
let ``SetReputation.It assigns the amount of reputation of the primary shipmate.`` () =
    let inputReputation = 100.0
    let input =
        avatarId
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
    let context = TestAvatarSetReputationContext(shipmateSingleStatisticSink, shipmateSingleStatisticSource) :> AvatarSetReputationContext
    input
    |> Avatar.SetReputation 
        context
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
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context inputSpeed

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
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context inputSpeed

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
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context inputSpeed


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
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context inputSpeed


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
    let context = TestAvatarSetSpeedContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarSetSpeedContext
    input
    |> Avatar.SetSpeed context inputSpeed

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
    let context = TestAvatarSetHeadingContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarSetHeadingContext
    input
    |> Avatar.SetHeading 
        context 
        inputHeading

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
    let context = TestAvatarSetPositionContext(vesselSingleStatisticSink, vesselSingleStatisticSource) :> AvatarSetPositionContext
    input
    |> Avatar.SetPosition context inputPosition

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
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSourceStub,
            (assertAvatarSingleMetricSink [(Metric.Moved, 1UL)]),
            avatarSingleMetricSourceStub,
            shipmateRationItemSourceStub, 
            shipmateSingleStatisticSinkStub, 
            shipmateSingleStatisticSourceStub, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
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
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 1UL)]),
            avatarSingleMetricSourceStub,
            shipmateRationItemSource, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
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
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Ate, 1UL)]),
            avatarSingleMetricSourceStub,
            shipmateRationItemSourceStub, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
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
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (assertAvatarSingleMetricSink [(Metric.Moved, 1UL)]),
            avatarSingleMetricSourceStub,
            shipmateRationItemSourceStub, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
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
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (assertAvatarSingleMetricSink [(Metric.Moved, 1UL);(Metric.Starved, 1UL)]),
            avatarSingleMetricSourceStub,
            shipmateRationItemSourceStub, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
        context
        inputAvatarId

[<Test>]
let ``AbandonJob.It does nothing when the given avatar has no job.`` () =
    let input = avatarId
    let avatarJobSink (_) (_) =
        Assert.Fail("avatarJobSink")
    let avatarJobSource (_) =
        None
    let context = 
        TestAvatarAbandonJobContext 
            (avatarJobSink,
            avatarJobSource,
            avatarSingleMetricSinkExplode,
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSinkStub, 
            shipmateSingleStatisticSourceStub) :> AvatarAbandonJobContext
    input
    |> Avatar.AbandonJob
        context

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
    let context = 
        TestAvatarAbandonJobContext
            (avatarJobSink,
            avatarJobSource,
            (assertAvatarSingleMetricSink [(Metric.AbandonedJob, 1UL)]),
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> AvatarAbandonJobContext
    Avatar.AbandonJob
        context
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
    let context = 
        TestAvatarCompleteJobContext 
            (avatarJobSink,
            avatarJobSource,
            avatarSingleMetricSinkExplode,
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> AvatarCompleteJobContext
    Avatar.CompleteJob
        context
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
    let context = 
        TestAvatarCompleteJobContext
            (avatarJobSink,
            avatarJobSource,
            (assertAvatarSingleMetricSink [(Metric.CompletedJob, 1UL)]),
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource) :> AvatarCompleteJobContext
    Avatar.CompleteJob
        context
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
    let context = TestAvatarAddInventoryContext(avatarInventorySink, avatarInventorySource) :> AvatarAddInventoryContext
    input
    |> Avatar.AddInventory 
        context
        inputItem 
        inputQuantity


[<Test>]
let ``GetItemCount.It returns zero when the given avatar has no entry for the given item.`` () =
    let input = avatarId
    let inputItem = 1UL
    let expected = 0u
    let avatarInventorySource (_) =
        Map.empty
    let context = TestAvatarGetItemCountContext(avatarInventorySource) :> AvatarGetItemCountContext
    let actual =
        input
        |> Avatar.GetItemCount 
            context
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
    let context = TestAvatarGetItemCountContext(avatarInventorySource) :> AvatarGetItemCountContext
    let actual =
        input
        |> Avatar.GetItemCount 
            context
            inputItem
    Assert.AreEqual(expected, actual)

[<Test>]
let ``RemoveInventory.It does nothing.When given a quantity of 0 items to remove or the given avatar has no items.`` 
        ([<Values(0UL, 1UL)>]inputQuantity:uint64) =
    let input = avatarId
    let inputItem = 1UL
    let avatarInventorySource (_) =
        Map.empty
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(Map.empty, inventory)
    let context = TestAvatarRemoveInventoryContext(avatarInventorySink, avatarInventorySource) :> AvatarRemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
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
    let avatarInventorySink (_) (inventory:Map<uint64, uint64>) =
        Assert.AreEqual(0, inventory.Count)
    let context = TestAvatarRemoveInventoryContext(avatarInventorySink, avatarInventorySource) :> AvatarRemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
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
    let context = TestAvatarRemoveInventoryContext(avatarInventorySink, avatarInventorySource) :> AvatarRemoveInventoryContext
    input
    |> Avatar.RemoveInventory 
        context
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
    let context = TestAvatarAddMessagesContext(avatarMessageSink) :> AvatarAddMessagesContext
    input
    |> Avatar.AddMessages context inputMessages


[<Test>]
let ``AddMetric.It creates a metric value when there is no previously existing metric value in the avatar's table.`` () = 
    let input = avatarId
    let inputMetric = Metric.Moved
    let inputValue = 1UL
    let context = 
        TestAvatarAddMetricContext
            ((assertAvatarSingleMetricSink [(Metric.Moved, 1UL)]),
            avatarSingleMetricSourceStub) :> AvatarAddMetricContext
    input
    |> Avatar.AddMetric
        context
        inputMetric 
        inputValue

[<Test>]
let ``AddMetric.It adds to a metric value when there is a previously existing metric value in the avatar's table.`` () = 
    let input = avatarId
    let inputMetric = Metric.Moved
    let inputValue = 1UL
    let context = 
        TestAvatarAddMetricContext
            ((assertAvatarSingleMetricSink [(Metric.Moved, 1UL)]),
            avatarSingleMetricSourceStub) :> AvatarAddMetricContext
    input
    |> Avatar.AddMetric 
        context
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
    let context = TestAvatarGetUsedTonnageContext(avatarInventorySource) :> AvatarGetUsedTonnageContext
    let actual =
        input
        |> Avatar.GetUsedTonnage 
            context
            inputItems
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetEffectiveSpeed.It returns full speed when there is no fouling.`` () =
    let expected = 1.0
    let context = TestAvatarGetEffectiveSpeedContext(vesselSingleStatisticSource) :> AvatarGetEffectiveSpeedContext
    let actual =
        Avatar.GetEffectiveSpeed context inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``GetEffectiveSpeed.It returns proportionally reduced speed when there is fouling.`` () =
    let expected = 0.125
    let vesselSingleStatisticSource (_) (_) = {MinimumValue=0.0;MaximumValue=0.25;CurrentValue=0.25} |> Some
    let context = TestAvatarGetEffectiveSpeedContext(vesselSingleStatisticSource) :> AvatarGetEffectiveSpeedContext
    let actual =
        Avatar.GetEffectiveSpeed context inputAvatarId
    Assert.AreEqual(expected, actual)

[<Test>]
let ``CleanHull.It cleans the hull of the given avatar.`` () =
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
    let context = 
        TestAvatarCleanHullContext
            (avatarShipmateSource,
            (assertAvatarSingleMetricSink [(Metric.CleanedHull, 1UL)]),
            avatarSingleMetricSourceStub,
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> AvatarCleanHullContext
    Avatar.CleanHull
        context
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
    let context = TestAvatarGetCurrentFoulingContext(vesselSingleStatisticSource) :> AvatarGetCurrentFoulingContext
    let actual = 
        Avatar.GetCurrentFouling context inputAvatarId
    Assert.AreEqual(expected, actual)


[<Test>]
let ``GetMaximumFouling.It returns the maximum fouling for the Avatar.`` () =
    let vesselSingleStatisticSource (_) (_) =
        {MaximumValue=0.5; MinimumValue=0.0; CurrentValue=0.25} |> Some
    let inputAvatarId = "avatar"
    let expected = 1.0
    let context = TestAvatarGetMaximumFoulingContext(vesselSingleStatisticSource) :> AvatarGetMaximumFoulingContext
    let actual = 
        Avatar.GetMaximumFouling context inputAvatarId
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
    let context = TestAvatarGetSpeedContext(vesselSingleStatisticSource) :> AvatarGetSpeedContext
    let actual =
        inputAvatarId
        |> Avatar.GetSpeed context
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
        |> Avatar.GetPosition context
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
    let context = TestAvatarGetHeadingContext(vesselSingleStatisticSource) :> AvatarGetHeadingContext
    let actual =
        inputAvatarId
        |> Avatar.GetHeading 
            context
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
    let context = 
        TestAvatarMoveContext
            (avatarInventorySink,
            avatarInventorySource,
            avatarShipmateSource,
            (assertAvatarSingleMetricSink [Metric.Moved, 1UL; Metric.Ate, 0UL]),
            avatarSingleMetricSourceStub,
            shipmateRationItemSourceStub, 
            shipmateSingleStatisticSink, 
            shipmateSingleStatisticSource, 
            vesselSingleStatisticSink, 
            vesselSingleStatisticSource) :> AvatarMoveContext
    Avatar.Move 
            context
            avatarId
    Assert.AreEqual(1, xPositionCalled)
    Assert.AreEqual(1, yPositionCalled)

[<Test>]
let ``GetGamblingHand.It retrieves a gambling hand for a given avatar.`` () =
    let expected =
        (Minor (Wands, Rank.Ace),Minor (Wands, Rank.Deuce),Minor (Wands, Rank.Three)) |> Some
    let mutable called : bool = false
    let avatarGamblingHandSource (_) =
        called <- true
        expected
    let context = TestAvatarGetGamblingHandContext (avatarGamblingHandSource) :> AvatarGetGamblingHandContext
    let actual =
        Avatar.GetGamblingHand
            context
            avatarId
    Assert.AreEqual(expected, actual)
    Assert.True(called)
    
[<Test>]
let ``DealGamblingHand.It deals a new hand to the given avatar.`` () =
    let expectedHand : AvatarGamblingHand option =
        (Major Arcana.Empress,Major Arcana.WheelOfFortune,Minor (Pentacles, Rank.Four)) |> Some
    let mutable called : bool = false
    let avatarGamblingHandSink (_) (hand:AvatarGamblingHand option) =
        called <- true
        Assert.AreEqual(expectedHand, hand)
    let random : Random = Random(1000)
    let context = TestAvatarDealGamblingHandContext(avatarGamblingHandSink, random) :> AvatarDealGamblingHandContext 
    Avatar.DealGamblingHand
        context
        avatarId
    Assert.True(called)

[<Test>]
let ``EnterIslandFeature.It does not enter the dark alley when one is not present.`` () =
    let inputLocation = (0.0, 0.0)
    let context = TestAvatarEnterIslandFeatureContext() :> AvatarEnterIslandFeatureContext
    Avatar.EnterIslandFeature
        context
        avatarId
        inputLocation
        IslandFeatureIdentifier.DarkAlley

    