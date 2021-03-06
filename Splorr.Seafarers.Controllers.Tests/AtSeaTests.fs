module AtSeaTests

open NUnit.Framework
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Controllers
open CommonTestFixtures
open AtSeaTestFixtures

let private functionUnderTest
        (avatarIslandFeatureSink        : AvatarIslandFeatureSink)
        (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
        (avatarJobSink                  : AvatarJobSink)
        (avatarJobSource                : AvatarJobSource)
        (avatarMessageSink              : AvatarMessageSink)
        (avatarSingleMetricSink         : AvatarSingleMetricSink)
        (gameDataSink                   : GameDataSink)
        (islandLocationByNameSource     : IslandLocationByNameSource)
        (islandSingleNameSource         : IslandSingleNameSource)
        (islandSingleStatisticSource    : IslandSingleStatisticSource)
        (shipmateSingleStatisticSource  : ShipmateSingleStatisticSource)
        (vesselSingleStatisticSource    : VesselSingleStatisticSource) 
        = 
    let context : ServiceContext =
        TestAtSeaRunContext
            (avatarInventorySinkStub,
            avatarInventorySourceStub,
            avatarIslandFeatureSink,
            avatarIslandSingleMetricSinkStub,
            avatarIslandSingleMetricSource,
            avatarJobSink,
            avatarJobSource,
            avatarMessagePurgerStub,
            avatarMessageSink,
            avatarMessageSourceDummy,
            avatarShipmateSourceStub,
            avatarSingleMetricSink,
            avatarSingleMetricSourceStub,
            atSeaCommoditySource ,
            (fun () -> System.DateTimeOffset.Now.ToUnixTimeSeconds() |> uint64),
            gameDataSink,
            atSeaIslandItemSink ,
            atSeaIslandItemSource, 
            islandJobSinkStub,
            islandJobSourceStub,
            islandLocationByNameSource,
            atSeaIslandMarketSink ,
            atSeaIslandMarketSource, 
            islandSingleNameSource,
            islandSingleStatisticSource,
            islandSourceStub ,
            atSeaItemSource,
            shipmateRationItemSourceStub,
            shipmateSingleStatisticSinkStub,
            shipmateSingleStatisticSource,
            Fixtures.Common.Fake.TermListSource,
            vesselSingleStatisticSinkStub,
            vesselSingleStatisticSource,
            worldSingleStatisticSourceStub) :> ServiceContext
    AtSea.Run 
        context

let private functionUsuallyUnderTest
        (gameDataSink:GameDataSink)
        (islandSingleNameSource: IslandSingleNameSource)= 
    functionUnderTest 
        avatarIslandFeatureSinkDummy
        avatarIslandSingleMetricSourceStub
        avatarMessageSinkStub
        avatarJobSourceStub
        avatarJobSinkStub
        avatarSingleMetricSinkExplode
        gameDataSink
        islandLocationByNameSourceStub
        islandSingleNameSource
        islandSingleStatisticSourceStub
        shipmateSingleStatisticSourceStub
        vesselSingleStatisticSourceStub 

[<Test>]
let ``Run.It returns GameOver when the given world's avatar is dead.`` () =
    let input = deadWorld   
    let inputSource() : Command option =
        Assert.Fail("It will not reach for user input because the avatar is dead.")
        None
    let expectedMessages = []
    let expected =
        expectedMessages
        |> Gamestate.GameOver
        |> Some
    let shipmateSingleStatisticSource (_) (_) (identifier:ShipmateStatisticIdentifier) =
        match identifier with
        | ShipmateStatisticIdentifier.Health ->
            Statistic.Create (0.0, 100.0) 0.0 |> Some
        | _ ->
            raise (System.NotImplementedException (identifier.ToString() |> sprintf "shipmateSingleStatisticSource - %s"))
    let actual =
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            avatarMessageSinkStub 
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSourceStub
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSource
            vesselSingleStatisticSourceStub 
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns ConfirmQuit when given Quit command.`` () =
    let input = world
    let inputSource = 
        Command.Quit 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Gamestate.ConfirmQuit 
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual = 
        input
        |> functionUsuallyUnderTest
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns InvalidInput when given invalid command.`` () =
    let input = world
    let inputSource = 
        None 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        ("Maybe try 'help'?",input
        |> Gamestate.InPlay)
        |> Gamestate.ErrorMessage
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea with new speed when given Set Speed command.`` () =
    let newSpeed = 0.5
    let input = world
    let inputSource = 
        newSpeed 
        |> SetCommand.Speed 
        |> Command.Set 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["You set your speed to 1.00."]//note - the statistic sink does not actually track speed, so this value is "wrong" but the behavior is correct
    let expected = 
        input
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) = "i am a rock" |> Some
    let actual =
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub 
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea with new heading when given Set Heading command.`` () =
    let inputHeading = 1.5
    let input = world
    let inputSource = 
        inputHeading 
        |> SetCommand.Heading 
        |> Command.Set 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = 
        [
            "You set your heading to 0.00\u00b0." //note - because of stub function the actual heading is not stored, just testing that a message is added
        ]
    let expected = 
        input
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) = "i am a rock" |> Some
    let actual =
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It moves the avatar when given Move command.`` () =
    let input = world
    let inputSource = 
        1u 
        |> Command.Move 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["Steady as she goes."]
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Some
    let avatarSingleMetricSink (_) (metric: Metric, value: uint64) : unit =
        match metric with
        | Metric.Moved ->
            Assert.AreEqual(1UL, value)
        | _ ->
            Assert.Fail(metric.ToString() |> sprintf "avatarSingleMetricSink - %s")
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSink
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea Help when given the Help command.`` () =
    let input = world
    let inputSource = 
        Command.Help 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Gamestate.Help 
        |> Some
    let islandSingleNameSource (_) = "i am a rock" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest 
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea Metrics when given the Metrics command.`` () =
    let input = world
    let inputSource = 
        Command.Metrics 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Gamestate.Metrics 
        |> Some
    let islandSingleNameSource (_) = "i am a rock" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest 
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns At Sea Inventory when given the Inventory command.`` () =
    let input = world
    let inputSource = 
        Command.Inventory 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Gamestate.Inventory 
        |> Some
    let islandSingleNameSource (_) = "i am a rock" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest 
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Main Menu when given the Menu command.`` () =
    let input = world
    let inputSource = 
        Command.Menu 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input 
        |> Some 
        |> Gamestate.MainMenu 
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest 
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Island List when given the Islands command.`` () =
    let input = world
    let inputSource = 
        0u 
        |> Command.Islands 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        (0u, input |> Gamestate.InPlay) 
        |> Gamestate.IslandList 
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest 
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns AtSea when given the Dock command and there is no sufficiently close island.`` () =
    let input = emptyWorld
    let inputSource = 
        Command.Dock 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["There is no place to dock."]
    let expected = 
        input
        |>Gamestate.InPlay
        |>Some
    let vesselSingleStatisticSource (a) (identifier: VesselStatisticIdentifier) = 
        match identifier with 
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=10.0} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=10.0} |> Some
        | _ ->
            vesselSingleStatisticSourceStub a identifier
    let actual =
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSourceStub
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Docked (at Dock) when given the Dock command and there is a near enough island.`` () =
    let input = dockWorld
    let inputSource = Command.Dock |> Some |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["You dock."]
    let expectedWorld = 
        input
    let expected = 
        expectedWorld
        |>Gamestate.InPlay
        |>Some
    let avatarSingleMetricSink (_) (metric: Metric, value: uint64) : unit =
        match metric with
        | Metric.VisitedIsland ->
            Assert.AreEqual(0UL, value)
        | _ ->
            Assert.Fail(metric.ToString() |> sprintf "avatarSingleMetricSink - %s")
    let avatarIslandFeatureSink (feature:AvatarIslandFeature option, _) =
        Assert.AreEqual(IslandFeatureIdentifier.Dock, feature.Value.featureId)
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUnderTest
            avatarIslandFeatureSink
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSink
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given a Head For command and the given island does not exist.`` () =
    let input = headForWorldUnvisited
    let inputSource = 
        "foo" 
        |> Command.HeadFor 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["I don't know how to get to `foo`."]
    let expected = 
        input
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual = 
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given a Head For command and the given island exists but is not known.`` () =
    let input = headForWorldUnvisited
    let inputSource = 
        "yermom" 
        |> Command.HeadFor 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["I don't know how to get to `yermom`."]
    let expected = 
        input
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual = 
        input
        |> functionUnderTest
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and changes heading when given a Head For command and the given island exists and is known.`` () =
    let input = headForWorldVisited
    let inputSource = "yermom" |> Command.HeadFor |> Some |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = 
        [
            "You set your heading to 0.00\u00b0." //heading not actually set because of stub functions, so really just test that a message is added
            "You head for `yermom`."
        ]
    let expected = 
        input
        |> Gamestate.InPlay
        |> Some
    let avatarIslandSingleMetricSource (_) (_) (identifier: AvatarIslandMetricIdentifier) =
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            1UL |> Some
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandLocationByNameSource (_) =
        (0.0, 0.0)
        |> Some
    let islandSingleNameSource (_) =
        "yermom" |> Some
    let actual = 
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSource
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSource
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Chart when given the command Chart.`` () =
    let input = world
    let inputChartName = "chartname"
    let inputSource = 
        inputChartName
        |> Command.Chart 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        (inputChartName, input)
        |> Gamestate.Chart 
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest 
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Status when given the command Status.`` () =
    let input = world
    let inputSource = 
        Command.Status 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay
        |> Gamestate.Status 
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest 
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message when given the command Abandon Job and the avatar has no current job.`` () =
    let input = dockWorld
    let inputSource = 
        Job 
        |> Command.Abandon 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["You have no job to abandon."]
    let expected = 
        input
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and abandons the job when given the command Abandon Job and the avatar has a current job.`` () =
    let input = abandonJobWorld
    let inputSource = 
        Job 
        |> Command.Abandon 
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["You abandon your job."]
    let expected = 
        input
        |> Gamestate.InPlay
        |> Some
    let avatarJobSource (_) =
        {
            FlavorText  = ""
            Reward      = 0.0
            Destination = (0.0, 0.0)
        }
        |> Some
    let avatarSingleMetricSink (_) (metric: Metric, value: uint64) : unit =
        match metric with
        | Metric.AbandonedJob ->
            Assert.AreEqual(1UL, value)
        | _ ->
            Assert.Fail(metric.ToString() |> sprintf "avatarSingleMetricSink - %s")
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSource
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSink
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It gives a message and returns AtSea when the avatar is too far away from an island to careen.`` () =
    let inputPosition = (10.0, 10.0)
    let input = 
        dockWorld
    let inputSource = 
        Port
        |> Command.Careen
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = ["You cannot careen here."]
    let expected =
        input
        |> Gamestate.InPlay
        |> Some
    let vesselSingleStatisticSource (_) (identifier) =
        match identifier with
        | VesselStatisticIdentifier.PortFouling
        | VesselStatisticIdentifier.StarboardFouling ->
            None
        | VesselStatisticIdentifier.PositionX ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=inputPosition |> fst} |> Some
        | VesselStatisticIdentifier.PositionY ->
            {MinimumValue=0.0; MaximumValue=100.0; CurrentValue=inputPosition |> snd} |> Some
        | VesselStatisticIdentifier.Speed ->
            {MinimumValue=0.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.DockDistance ->
            {MinimumValue=1.0; MaximumValue=1.0; CurrentValue=1.0} |> Some
        | VesselStatisticIdentifier.ViewDistance ->
            {MinimumValue=10.0; MaximumValue=10.0; CurrentValue=10.0} |> Some
        | VesselStatisticIdentifier.Heading ->
            {MinimumValue=0.0; MaximumValue=6.3; CurrentValue=0.0} |> Some
        | _ ->
            Assert.Fail("Kaboom get")
            None
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUnderTest 
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It returns Careen Port when given the careen port command and the avatar is sufficiently close to an island to careen.`` () =
    let input = dockWorld
    let inputSource = 
        Port
        |> Command.Careen
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected =
        (Port, input)
        |> Gamestate.Careened
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUsuallyUnderTest 
            gameDataSinkFake
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)
    
[<Test>]
let ``Run.It adds a message when given a Distance To command with an island name that does not exist.`` () =
    let input = dockWorld
    let inputName = "$$$$$$"
    let inputSource = 
        inputName
        |> Command.DistanceTo
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = [inputName |> sprintf "I don't know how to get to `%s`."]
    let expectedWorld =
        input
    let expected =
        expectedWorld
        |> Gamestate.InPlay
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUnderTest
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSourceStub
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource
            sinkDummy
    Assert.AreEqual(expected, actual)

[<Test>]
let ``Run.It adds a message when given a Distance To command with an island name that does exist.`` () =
    let input = 
        dockWorld
    let inputIslandName = "yermom"
    let inputSource = 
        inputIslandName
        |> Command.DistanceTo
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expectedMessages = [inputIslandName |> sprintf "I don't know how to get to `%s`."]
    let expectedWorld =
        input
    let expected =
        expectedWorld
        |> Gamestate.InPlay
        |> Some
    let avatarIslandSingleMetricSource (_) (_) (identifier: AvatarIslandMetricIdentifier) =
        match identifier with
        | AvatarIslandMetricIdentifier.VisitCount ->
            None
        | _ ->
            Assert.Fail(identifier.ToString() |> sprintf "avatarIslandSingleMetricSource - %s")
            None
    let islandSingleNameSource (_) = "island name" |> Some
    let actual =
        input
        |> functionUnderTest
            avatarIslandFeatureSinkDummy
            avatarIslandSingleMetricSource
            avatarJobSinkStub
            avatarJobSourceStub
            (avatarMessagesSinkFake expectedMessages)
            avatarSingleMetricSinkExplode
            gameDataSinkFake
            islandLocationByNameSourceStub
            islandSingleNameSource
            islandSingleStatisticSourceStub
            shipmateSingleStatisticSourceStub
            vesselSingleStatisticSourceStub
            inputSource
            sinkDummy
    Assert.AreEqual(expected, actual)


[<Test>]
let ``Run.It returns InPlay when given Save command.`` () =
    let input = world
    let inputSource =
        None
        |> Command.Save  
        |> Some 
        |> Fixtures.Common.Mock.CommandSource
    let expected = 
        input
        |> Gamestate.InPlay 
        |> Some
    let islandSingleNameSource (_) = "island name" |> Some
    let mutable called = false
    let gameDataSink (_) =
        called <- true
        None
    let actual = 
        input
        |> functionUsuallyUnderTest
            gameDataSink
            islandSingleNameSource
            inputSource 
            sinkDummy
    Assert.AreEqual(expected, actual)
    Assert.True(called)
